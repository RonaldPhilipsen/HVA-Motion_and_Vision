using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Motion_and_vision
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

            segments.Add(new Segment(new PointF(150, 0), 0, 100));
            AddSegment();
            AddSegment();

            AddSegmentButton.Click += delegate { AddSegment(); };
            RemoveSegmentButton.Click += delegate
            {
                if (segments.Count > 0)
                    segments.RemoveAt(segments.Count - 1);
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
            segments[0].UpdateIK(new PointF((float)pos.X, (float)pos.Y));
            Draw();
        }

        /// <summary> Adds a segment </summary>
        private void AddSegment()
        {
            var lastSegment = segments[^1];
            var newSegment = new Segment(new PointF(lastSegment.Position.X, lastSegment.Position.X + 100), 0, 0);
            lastSegment.Child = newSegment;
            segments.Add(newSegment);
            Draw();
        }
        #endregion



        /// <summary> Draws the robot </summary>
        private void Draw()
        {
            var lines = new List<Line>();

            RobotCanvas.Children.Clear();
            for (var i = 0; i < segments.Count; i++)
            {
                var joint = new Ellipse { Width = 15, Height = 15, Stroke = Brushes.Red };

                var line = lines.Count > 0
                    ? new Line
                    {
                        X1 = lines[^1].X2,
                        Y1 = lines[^1].Y2,
                        X2 = lines[^1].X2 + segments[i].Position.X,
                        Y2 = lines[^1].Y2 + segments[i].Position.Y,
                        Stroke = Brushes.Black,
                        StrokeThickness = 2
                    }
                    : new Line
                    {
                        X1 = segments[i].Position.X,
                        Y1 = segments[i].Position.Y,
                        X2 = segments[i].Position.X,
                        Y2 = segments[i].Position.Y,
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

