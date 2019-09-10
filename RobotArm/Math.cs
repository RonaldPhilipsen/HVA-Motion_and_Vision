using System.Drawing;

namespace RobotArm
{
    public static class Math
    {
        public const double DegToRads = (System.Math.PI / 180);
        public const double RadsToDeg = (180 / System.Math.PI);

        /// <summary> function to convert an angle to its simplest form
        /// (between -pi to pi radians)
        /// </summary>
        public static double SimpleAngle(double theta)
        {
            theta %= (2.0 * System.Math.PI);
            if (theta < -System.Math.PI)
                theta += 2.0 * System.Math.PI;
            else if (theta > System.Math.PI)
                theta -= 2.0 * System.Math.PI;
            return theta;
        }

        /// <summary> Gets the distance between two arbitrary points </summary>
        /// <param name="pointA">The first point</param>
        /// <param name="pointB">The second point</param>
        /// <returns></returns>
        private static double GetDistance(PointF pointA,
                                          PointF pointB) => ((pointA.X - pointB.X) * (pointA.X - pointB.X)) +
                                                            ((pointA.Y - pointB.Y) * (pointA.Y - pointB.Y));

    }
}
