using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace RobotArm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private const float DistanceThreshold = 0.05f;
        private readonly List<Segment> segments;

        public MainWindow()
        {
            segments = new List<Segment>();
            InitializeComponent();

            segments.Add(new Segment(new PointF(150, 100),100));
            AddSegment();
            AddSegment();

            AddSegmentButton.Click += delegate { AddSegment(); };
            RemoveSegmentButton.Click += delegate
            {
                if (segments.Count <= 0)
                {
                    return;
                }

                segments.RemoveAt(segments.Count - 1);
                Draw();
            };
            Draw();

        }

        #region ui handlers
        /// <summary> Makes the robot point to the mouse </summary>
        /// <param name="sender"> The sender</param>
        /// <param name="e"> The robot event args</param>
        public void RobotCanvasOnClick(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(RobotCanvas);
            UpdateIk(new PointF((float)pos.X, (float)pos.Y));
            Draw();
        }

        private void UpdateIk(PointF target)
        {
            if (segments.Count <= 0)
            {
                return;
            }

            if (GetDistance(segments[^1].Position, target) < DistanceThreshold) return;


            for (var i = 0; i < 20; i++)
            {
                segments[0].UpdateIk(new PointF(150f, 0f), target);
                if (GetDistance(segments[^1].Position, target) < DistanceThreshold) return;
            }

        }

        /// <summary> Gets the distance between two arbitrary points </summary>
        /// <param name="pointA">The first point</param>
        /// <param name="pointB">The second point</param>
        /// <returns></returns>
        private static double GetDistance(PointF pointA, PointF pointB) => ((pointA.X - pointB.X) * (pointA.X - pointB.X)) + 
                                                                           ((pointA.Y - pointB.Y) * (pointA.Y - pointB.Y));

        /// <summary> Adds a segment </summary>
        private void AddSegment()
        {
            Segment newSegment;
            if (segments.Count > 0)
            {
                var lastSegment = segments[^1];
                newSegment = new Segment(new PointF(lastSegment.Position.X, lastSegment.Position.Y + 100), 100);
                lastSegment.Child = newSegment;
            }
            else
            {
                newSegment = new Segment(new PointF(150, 100),100);
            }

            segments.Add(newSegment);
            Draw();
        }
        #endregion

        /// <summary> Draws the robot </summary>
        private void Draw()
        {
            var lines = new List<Line>();

            RobotCanvas.Children.Clear();
            foreach (var t in segments)
            {
                var joint = new Ellipse { Width = 15, Height = 15, Stroke = Brushes.Red };

                var line = lines.Count > 0
                               ? new Line
                                 {
                                     X1 = lines[^1].X2,
                                     Y1 = lines[^1].Y2,
                                     X2 = t.Position.X,
                                     Y2 = t.Position.Y,
                                     Stroke = Brushes.Black,
                                     StrokeThickness = 2
                                 }
                               : new Line
                                 {
                                     X1 = 150,
                                     Y1 = 0,
                                     X2 = t.Position.X,
                                     Y2 = t.Position.Y,
                                     Stroke = Brushes.Black,
                                     StrokeThickness = 2
                                 };
                lines.Add(line);

                RobotCanvas.Children.Add(line);
                RobotCanvas.Children.Add(joint);

                Canvas.SetLeft(joint, line.X2 - joint.Width / 2);
                Canvas.SetTop(joint, line.Y2 - joint.Height / 2);
            }

        }
    }

}

