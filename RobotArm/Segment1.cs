using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text;

namespace Motion_and_vision
{
    public class Segment
    {
        private double angle;
        private float length;

        public Segment(PointF position, float angle, float length)
        {
            this.Position = position;
            this.angle = angle;
            this.length = length;
            this.Position = position;
        }

        public Segment(PointF position, float angle, float length, Segment child)
        {
            this.Position = position;
            this.angle = angle;
            this.length = length;
            this.Position = position;
            Child = child;
        }
        public Segment Child { get; set; }
        public PointF Position { get; set; }

        public PointF UpdateIK(PointF target)
        {
            var localTarget = Rotate(Translate(target, -Position.X, -Position.Y), -angle);
            PointF endpoint;
            if(Child != null)
            {
                endpoint = Child.UpdateIK(localTarget);
            }
            else
            {
                //base case, the end point is the end of the current segment
                endpoint = new PointF(length, 0);
            }
            angle = Angle(localTarget) - Angle(endpoint);
            var normal = Translate(Rotate(endpoint, angle), Position.X, Position.Y);
            return normal;
        }

        /// <summary> Rotates the line </summary>
        /// <param name="angle"> The angle in degrees</param>
        private static PointF Rotate(PointF point, double angle)
        {
            var cos = Math.Cos(angle);
            var sin = Math.Sin(angle);
            return new PointF
            {
                X = (float)((point.X * cos) - (point.Y * sin)),
                Y = (float)((point.X * sin) + (point.Y * cos))
            };
        }
        private static PointF Translate(PointF point, float horizontal, float vertical)
        {
            return new PointF(point.X + horizontal, point.Y + vertical);
        }

        private static double Angle(PointF point)
        {
            return Math.Atan2(point.X, point.Y);
        }
    }
}