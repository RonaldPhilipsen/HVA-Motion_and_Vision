using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;

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
            return new Vector3(pos.X + (float)(segment.Length * System.Math.Sin(segment.Parent.Angle + segment.Angle)),
                               pos.Y + (float)(segment.Length * System.Math.Cos(segment.Parent.Angle + segment.Angle)),
                               0);
        }

        public static bool Inverse(List<ArmSegment> Joints, Vector3 targetPos, double epsilon, int maxTries)
        {
            //targetPosition -= new Vector3(MainWindow.XOffSet, MainWindow.YOffSet, 0);
            var numSegments = Joints.Count - 1;
            //get the last segment
            var a = Joints[numSegments];
            var endPos = Forward(a) ;

            for (var tries = 0; tries < maxTries; tries ++)
            {
                var currentPos = Forward(a);

                if (targetPos.CalculateDistance(endPos) < epsilon) 
                    return true;

                //Get the absolute position of the parent 
                var center = a.Parent != null
                                 ? Forward(a.Parent)
                                 : new Vector3(0, 0, 0);

                var cv = GetOffsetVector(center, currentPos);
                var tv = GetOffsetVector(center, targetPos);

                var angle = AngleBetweenVectors(cv,tv );
                a.Rotate(angle);
/*
                if (--numSegments > 1)
                {
                    a = Joints[numSegments];
                    continue;
                }

                numSegments = Joints.Count -1;
                a = Joints[^1];*/
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
