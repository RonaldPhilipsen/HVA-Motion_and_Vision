using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace CCD
{
    class Kinematics
	{
        /// <summary>
        /// This function will convert an angle to the equivalent rotation in the range [-pi,pi]
        /// </summary>
        /// <param name="angle">The angle in radians</param>
        /// <returns>The simplified angle</returns>
        private static double SimplifyAngle(double angle)
		{
			angle %= (2.0 * Math.PI);
			if( angle < -Math.PI )
				angle += (2.0 * Math.PI);
			else if( angle > Math.PI )
				angle -= (2.0 * Math.PI);
			return angle;
		}

        /// <summary>
        /// This enum represents the resulting state of a CCD iteration.
        /// </summary>
        public enum CcdResult
		{
			Success,    // the target was reached
			Processing, // still trying to reach the target
			Failure,    // failed to reach the target
		}


        /// <summary>		
        /// Given a bone chain located at the origin, this function will perform a single cyclic
        /// coordinate descent (CCD) iteration. This finds a solution of bone angles that places
        /// the final bone in the given chain at a target position. The supplied bone angles are
        /// used to prime the CCD iteration. If a valid solution does not exist, the angles will
        /// move as close to the target as possible. The user should resupply the updated angles 
        /// until a valid solution is found (or until an iteration limit is met).
        /// </summary>
        /// <param name="bones">  Bone values to update</param>
        /// <param name="target"> Target position for the end effector</param>
        /// <param name="arrivalDist"> What to consider in-range</param>
        /// <returns> CCDResult.Success when a valid solution was found.
        ///          CCDResult.Processing when still searching for a valid solution.
        ///          CCDResult.Failure when it can get no closer to the target.
        /// </returns>
        public static CcdResult Inverse (ref List<Bone> bones, PointF target, double arrivalDist)
		{
			// Set an epsilon value to prevent division by small numbers.
			const double epsilon = 0.0001; 

			// Set max arc length a bone can move the end effector an be considered no motion
			// so that we can detect a failure state.
			const double trivialArcLength = 0.00001; 


			var numBones = bones.Count;
			Debug.Assert(numBones > 0);

			var arrivalDistSqr = arrivalDist*arrivalDist;

			//===
			// Generate the world space bone data.
			var worldBones = new List<Bone>();

            // Start with the root bone.
            var rootWorldBone = new Bone
            {
                AbsX = bones[0].X,
                AbsY = bones[0].Y,
                AbsAngle = bones[0].Angle
            };
            rootWorldBone.CosAbsAngle = Math.Cos( rootWorldBone.AbsAngle );
			rootWorldBone.SinAbsAngle = Math.Sin( rootWorldBone.AbsAngle );
			worldBones.Add( rootWorldBone );

			
			// Convert child bones to world space.
			for( var boneIdx = 1; boneIdx < numBones; ++boneIdx )
			{
				var prevWorldBone	= worldBones[boneIdx-1];
				var curLocalBone = bones[boneIdx];

                var newWorldBone = new Bone
                {
                    AbsX = prevWorldBone.AbsX + prevWorldBone.CosAbsAngle * curLocalBone.X 
                           - prevWorldBone.SinAbsAngle * curLocalBone.Y,
                    AbsY = prevWorldBone.AbsY + prevWorldBone.SinAbsAngle * curLocalBone.X
                           + prevWorldBone.CosAbsAngle * curLocalBone.Y,
                    AbsAngle = prevWorldBone.AbsAngle + curLocalBone.Angle
                };
                newWorldBone.CosAbsAngle = Math.Cos( newWorldBone.AbsAngle );
				newWorldBone.SinAbsAngle = Math.Sin( newWorldBone.AbsAngle );
				worldBones.Add(newWorldBone);
			}

            //===
            // Track the end effector position (the final bone)

			var endX = worldBones[numBones-1].AbsX;
			var endY = worldBones[numBones-1].AbsY;

			//===
			// Perform CCD on the bones by optimizing each bone in a loop 
			// from the final bone to the root bone
			var modifiedBones = false;
			for( var boneIdx = numBones-2; boneIdx >= 0; --boneIdx )
			{
                // Get the vector from the current bone to the end effector position.

				var curToEndX = endX - worldBones[boneIdx].AbsX;
				var curToEndY = endY - worldBones[boneIdx].AbsY;
				var curToEndMag = Math.Sqrt( curToEndX*curToEndX + curToEndY*curToEndY );

				// Get the vector from the current bone to the target position.
				var curToTargetX = target.X - worldBones[boneIdx].AbsX;
				var curToTargetY = target.Y - worldBones[boneIdx].AbsY;
				var curToTargetMag = Math.Sqrt(   curToTargetX*curToTargetX
				                                   + curToTargetY*curToTargetY );

				// Get rotation to place the end effector on the line from the current
				// joint position to the target postion.
				double cosRotAng;
				double sinRotAng;
				var endTargetMag = (curToEndMag*curToTargetMag);
				if( endTargetMag <= epsilon )
				{
					cosRotAng = 1;
					sinRotAng = 0;
				}
				else
				{
					cosRotAng = (curToEndX*curToTargetX + curToEndY*curToTargetY) / endTargetMag;
					sinRotAng = (curToEndX*curToTargetY - curToEndY*curToTargetX) / endTargetMag;
				}

				// Clamp the cosine into range when computing the angle (might be out of range
				// due to floating point error).
				var rotAng = Math.Acos( Math.Max(-1, Math.Min(1,cosRotAng) ) );
				if( sinRotAng < 0.0 )
					rotAng = -rotAng;
				
				// Rotate the end effector position.
				endX = worldBones[boneIdx].AbsX + cosRotAng*curToEndX - sinRotAng*curToEndY;
				endY = worldBones[boneIdx].AbsY + sinRotAng*curToEndX + cosRotAng*curToEndY;

				// Rotate the current bone in local space (this value is output to the user)
				bones[boneIdx].Angle = SimplifyAngle( bones[boneIdx].Angle + rotAng );
                if (bones[boneIdx].Angle != 0)
                {
                    if (bones[boneIdx].Angle > bones[boneIdx].Maxlimiter)
                        bones[boneIdx].Angle = bones[boneIdx].Maxlimiter;
                    if (bones[boneIdx].Angle < bones[boneIdx].Minlimiter)
                        bones[boneIdx].Angle = bones[boneIdx].Minlimiter;
                }
                // Check for termination
				var endToTargetX = (target.X-endX);
				var endToTargetY = (target.Y-endY);
				if( endToTargetX*endToTargetX + endToTargetY*endToTargetY <= arrivalDistSqr )
				{
					// We found a valid solution.
					return CcdResult.Success;
				}

				// Track if the arc length that we moved the end effector was
				// a nontrivial distance.
				if( !modifiedBones && Math.Abs(rotAng)*curToEndMag > trivialArcLength )
				{
					modifiedBones = true;
				}
			}

			// We failed to find a valid solution during this iteration.
			return modifiedBones ? CcdResult.Processing : CcdResult.Failure;
		}
	
	}
}
