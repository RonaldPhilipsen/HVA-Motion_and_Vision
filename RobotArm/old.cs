using System;
using System.Drawing;
using System.Numerics;

namespace Motion_and_vision
{
    public class old
    {

        private readonly int length;
        private readonly Point anchor;

        public float Angle { get; set; }
        public float MinAngle { get; }
        public float MaxAngle { get; }

        public old(Point anchor)
        {
            this.anchor = anchor;
            End = Anchor;
        }

        public old(old previousSegment, int length)
        {
            anchor =  previousSegment.End;
            PreviousSegment = previousSegment;
            PreviousSegment.NextSegment = this;
            End = new Point(previousSegment.End.X, PreviousSegment.End.Y + 100);
            this.length = length;
        }


        public Point Anchor { get => PreviousSegment?.End ?? anchor; }
        public Point End { get; private set; }
        /// <summary> The rotation with respect to the anchor point </summary>
        public double Rotation { get; private set; }

        public old PreviousSegment { get; }
        public old NextSegment { get; set; }

        /// <summary> Rotates the line </summary>
        /// <param name="angle"> The angle in degrees</param>
        private void Rotate( double angle)
        {
            var cos = Math.Cos(angle);
            var sin = Math.Sin(angle);
            End = new Point
                      {
                          X = anchor.X - (int)(cos * length),
                          Y = anchor.Y - (int)(sin * length)
                      };

        }
        
        public void PointAt(Point pos)
        {
            Rotation = PreviousSegment == null 
                           ? CalculateAngle(Anchor, pos)
                           : CalculateAngle(PreviousSegment.End, pos);

            Rotate(Rotation);

            UpdateChildren(pos);
        }

        /// <summary> Calculate the angle in radians </summary>
        /// <param name="pos1"> The first position </param>
        /// <param name="pos2"> The second position</param>
        /// <returns></returns>
        private static double CalculateAngle(Point pos1, Point pos2)
        {
            float xDiff = pos1.X - pos2.X;
            float yDiff = pos1.Y - pos2.Y;
            var rads =  Math.Atan2(yDiff, xDiff);
            return rads;
        }

        private  void UpdateChildren(Point pos)
        {
            if (NextSegment == null)
            {
                return;
            }
            
            NextSegment.PointAt(pos);
            NextSegment.UpdateChildren(pos);
        }
    }
}
