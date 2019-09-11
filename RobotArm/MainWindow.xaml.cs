using System.Collections.Generic;
using System.Numerics;
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
        public const int XOffSet = 0;
        public const int YOffSet = 0;
        private const int NumSegments = 3;
        private const float Epsilon = 0.05f;
        private readonly List<ArmSegment> segments = new List<ArmSegment>();
        
        public MainWindow()
        {
            InitializeComponent();

            for (var i = 0; i < NumSegments; i++)
            {
                AddSegment();
            }

            AddSegmentButton.Click += delegate 
                                      {
                                          AddSegment();
                                          Draw();
                                      };
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
        private void RobotCanvasOnClick(object sender, MouseEventArgs e)
        {
            foreach (var armSegment in segments)
            {
                armSegment.Angle = 0;
            }

            var pos = e.GetPosition(RobotCanvas);
            var relativePos = new Vector3((float) pos.X, (float) pos.Y, 0); 
            if (!Kinematics.Inverse(segments, relativePos, Epsilon, 20))
            {
                 //MessageBox.Show("Failed to set IK");
            }
            Draw();
        }

        /// <summary> Adds a segment </summary>
        private void AddSegment()
        {
            var newSegment = segments.Count > 0 ? new ArmSegment(segments[^1], 100, -180,180,45)
                                                : new ArmSegment(null, 0, -180,180,0);
            segments.Add(newSegment);
        }
        #endregion

        /// <summary> Draws the robot </summary>
        private void Draw()
        {
            RobotCanvas.Children.Clear();
            foreach (var t in segments)
            {
                var pos = Kinematics.Forward(t);
                var parentPos = t.Parent != null ? (Vector3?) Kinematics.Forward(t.Parent) : null;

                var joint = new Ellipse { Width = 15, Height = 15, Stroke = Brushes.Red };

                var line = new Line
                {
                    X1 = (parentPos?.X ?? 0), 
                    Y1 = (parentPos?.Y ?? 0), 
                    X2 = pos.X,
                    Y2 = pos.Y,
                    Stroke = Brushes.Black,
                    StrokeThickness = 2
                };

                RobotCanvas.Children.Add(line);
                RobotCanvas.Children.Add(joint);

                Canvas.SetLeft(joint, line.X2 - joint.Width / 2);
                Canvas.SetTop(joint, line.Y2 - joint.Height / 2);
            }

        }


    }

}

