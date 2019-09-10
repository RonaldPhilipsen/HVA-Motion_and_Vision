using System.Numerics;

namespace RobotArm
{
    public class ArmSegment
    {
        public ArmSegment(ArmSegment parent, double length, double angle)
        {
            Parent = parent;
            Length = length;
            Angle = angle * Math.DegToRads;
        }
        
        public ArmSegment Parent { get; }
        public double Length { get; }
        public double Angle { get; private set; }

        public Vector3 GetForwardKinematics()
        {
            if (Parent == null)
            {
                return new Vector3(200 + (float) (Length * System.Math.Sin(Angle)),
                                   200 + (float) (Length * System.Math.Cos(Angle)),
                                   0);
            }

            var pos = Parent.GetForwardKinematics();
            return new Vector3(pos.X + (float) (Length * System.Math.Sin(Parent.Angle + Angle)),
                               pos.Y + (float) (Length * System.Math.Cos(Parent.Angle + Angle)),
                               0);
        }

        public void Rotate(Vector3 axis,double angle)
        { 
            if (Parent == null) return;
            Angle += angle;
        }
    }
}
