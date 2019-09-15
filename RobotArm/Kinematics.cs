using System;
using System.Collections.Generic;
using System.Numerics;

namespace RobotArm
{
    public static class Kinematics
    {
        /// <summary> Get the absolute position of a given arm segment </summary>
        /// <param name="segment"> The segment </param>
        /// <returns> a vector3 pointing at the absolute position </returns>
        public static Vector3 Forward(ArmSegment segment)
        {
            if (segment.Parent == null)
            {
                return new Vector3((float)(segment.Length * System.Math.Sin(segment.Angle)),
                                   (float)(segment.Length * System.Math.Cos(segment.Angle)),
                                   0);
            }

            var pos = Forward(segment.Parent);
            return new Vector3(pos.X + (float)(segment.Length * System.Math.Sin(segment.Angle)),
                               pos.Y + (float)(segment.Length * System.Math.Cos(segment.Angle)),
                               0);
        }

        public static bool Inverse(List<ArmSegment> joints, Vector3 targetPos, double epsilon, int maxTries)
        {
            //targetPosition -= new Vector3(MainWindow.XOffSet, MainWindow.YOffSet, 0);
            var numSegments = joints.Count - 1;
            //get the last segment
            var a = joints[numSegments];
           
            for (var tries = 0; tries < maxTries; tries ++)
            {
                var endPos = Forward(joints[numSegments]);

                //var currentPos = Forward(a);

                if (targetPos.CalculateDistance(endPos) < epsilon) 
                    return true;

                //Get the absolute position of the parent 
                var center = a.Parent != null
                                 ? Forward(a.Parent)
                                 : new Vector3(0, 0, 0);

                var cv = GetOffsetVector(center, endPos);
                var tv = GetOffsetVector(center, targetPos);

                var angle = AngleBetweenVectors(cv,tv );
                a.Rotate(angle);

                if (--numSegments > 0)
                {
                    a = joints[numSegments];
                    continue;
                }

                numSegments = joints.Count -1;
                a = joints[^1];
            }

            return false;
        }

        public static double AngleBetweenVectors(Vector3 currentVector,
                                          Vector3 targetPosition)
        {
            var cv = Vector3.Normalize(currentVector);
            var tv = Vector3.Normalize(targetPosition);

            double angle = Vector3.Dot(cv, tv);
            var direction = Vector3.Cross(cv, tv);

            if (angle > 1)
            {
                return 0;
            }

            angle = Math.Acos(angle);

            if (direction.Z > 0)
            {
                angle = -angle;
            }
            return angle;
        }

        private static double CalculateDistance(this Vector3 segment, Vector3 target) => (segment - target).Length();

        private static Vector3 GetOffsetVector(Vector3 center, Vector3 target) {
            var res = target;
            if (center.X > 0)
            {
                res.X -= center.X;
            }
            else if (center.X < 0)
            {
                res.X += center.X;
            }

            if (center.Y > 0)
            {
                res.Y -= center.Y;
            }
            else if (center.Y < 0)
            {
                res.Y += center.Y;
            }

            if (center.Z > 0)
            {
                res.Z -= center.Z;
            }
            else if (center.Z < 0)
            {
                res.Z += center.Z;
            }


            return res;
        }
    }

}
