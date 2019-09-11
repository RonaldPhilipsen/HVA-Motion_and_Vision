using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;

namespace RobotArm
{
    public static class Kinematics
    {
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

        public static bool Inverse(List<ArmSegment> segments, Vector3 targetPosition, double epsilon, int maxTries)
        {
            //targetPosition -= new Vector3(MainWindow.XOffSet, MainWindow.YOffSet, 0);
            uint tries = 0;
            var numSegments = segments.Count - 1;
            //get the last segment
            var a = segments[^1];
            var endPos = Forward(a) ;

            while (tries++ < maxTries)
            {
                var currentPos = Forward(a);

                if (targetPosition.CalculateDistance(endPos) < epsilon) 
                    return true;

                var center = a.Parent != null
                                 ? Forward(a.Parent)
                                 : new Vector3(0, 0, 0); 

                var angle = AngleBetweenVectors(center, currentPos, targetPosition);
                a.Rotate(angle);

                if (--numSegments > 0)
                {
                    a = segments[numSegments];
                    continue;
                }
                a = segments[^1];
            }

            return false;
        }

        public static double AngleBetweenVectors(Vector3 center,
                                                 Vector3 currentVector,
                                                 Vector3 targetPosition)
        {
            var cv = currentVector - center;
            var tv = targetPosition - center;

            cv = Vector3.Normalize(cv);
            tv = Vector3.Normalize(tv);

            double radians = Vector3.Dot(cv, tv);
            var direction = Vector3.Cross(cv, tv);


            if (direction.Z > 0)
            {
                radians = -radians;
            }
            return radians;
        }

        private static double CalculateDistance(this Vector3 segment, Vector3 target) => (segment - target).Length();
    }

}
