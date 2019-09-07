using System;
using System.Diagnostics;
using System.Drawing;

namespace RobotArm
{
    public class Segment
    {
        private readonly double length;

        public Segment(PointF position, double length)
        {
            this.length = length;
            Position = position;
        }

        public Segment(PointF position, double length, Segment child)
        {
            this.length = length;
            Position = position;
            Position = position;
            Child = child;
        }

        public Segment Child { get; set; }
        public PointF Position { get; private set; }

        public PointF UpdateIk(PointF joint, PointF target)
        {
            // Get current position
            var initialPos = Position;

            // Get the difference in The angle
            var deltaAngle = GetAngle(joint ,target) - GetAngle(joint, Position);

            // Rotate the current Joint
            Rotate(joint ,deltaAngle);
            var deltaX = initialPos.X - Position.X;
            var deltaY = initialPos.Y - Position.Y;
            Debug.WriteLine($"Delta X = {deltaX} \n Delta y = {deltaY}");

            var endpoint = Child?.UpdateIk(Position, target) ?? Position;
            return endpoint;
        }

        /// <summary> Rotates the line around a point </summary>
        /// <param name="center">The point to rotate </param>
        /// <param name="rotation"> The angle in degrees</param>
        private void  Rotate(PointF center, double rotation)
        {
            var previousAngle = Math.Atan((Position.Y - center.Y) / (Position.X - center.X));
            Debug.WriteLine($"Previous angle: {previousAngle}");

            var newAngle = previousAngle + rotation;
            Debug.WriteLine($"New angle: {newAngle}");

            var rotatedX = center.X + (length * Math.Cos(newAngle));
            var rotatedY = center.Y + (length * Math.Sin(newAngle));
            
            Position = new PointF((float) rotatedX, (float) rotatedY);
        }

        /// <summary> Gets the angle to a designated point </summary>
        /// <param name="joint">The joint to rotate around</param>
        /// <param name="target"> The designated point</param>
        /// <returns> The angle </returns>
        private static double GetAngle(PointF joint, PointF target)
        {
            var deltaX = target.X - joint.X;
            var deltaY = target.Y - joint.Y;
            var thetaRadians = Math.Atan2(deltaY, deltaX);
            return thetaRadians;
        }

        
    }
}