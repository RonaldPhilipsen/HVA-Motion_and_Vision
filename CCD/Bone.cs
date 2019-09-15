namespace CCD
{

    /// <summary>
    /// This class is used internally by the to represent a bone in both world and relative space
    /// </summary>
    public class Bone
    {
        public double AbsX;        // x position in world space
        public double AbsY;        // y position in world space
        public double AbsAngle;    // angle in world space
        public double CosAbsAngle; // sine of angle
        public double SinAbsAngle; // cosine of angle

        public double X;
        public double Y;
        public double Angle;
        public double Minlimiter;
        public double Maxlimiter;
    };

}
