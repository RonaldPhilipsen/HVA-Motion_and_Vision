namespace RobotArm
{
    using System;

    public class ArmSegment
    {
        private readonly double maxAngle;
        private readonly double minAngle;

        public ArmSegment(ArmSegment parent, double length, double minAngle, double maxAngle, double angle)
        {
            this.maxAngle = maxAngle;
            this.minAngle = minAngle;
            Parent = parent;
            Length = length;
            Angle = angle * (Math.PI/ 180 );
        }
        
        public ArmSegment Parent { get; }
        public double Length { get; }
        public double Angle { get; set; }
        public double Damping { get; set; }
        

        public void Rotate(double angle)
        { 
            if (Parent == null) return;

            /*
            if (angle < minAngle)
            {
                angle = minAngle;
            } else if (angle > maxAngle)
            {
                angle = maxAngle;
            }
            */

            Angle = angle;
            //Angle -= angle;
        }
    }
}
