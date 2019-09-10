using System.Collections.Generic;
using System.Diagnostics;
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
        private const int NumSegments = 3;
        private const int MaxRetries = 20;
        private const float Epsilon = 0.05f;
        private readonly List<ArmSegment> segments = new List<ArmSegment>();
        
        // Array of angles to rotate by (for each joint);
        private float[] theta = new float[NumSegments];
        // the sine component for each joint
        private float[] sin = new float[NumSegments];
        // The cosine component for each joint
        private float[] cos = new float[NumSegments];

        public MainWindow()
        {
            InitializeComponent();

            for (var i = 0; i < NumSegments; i++)
            {
                AddSegment();
            }

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
        private void RobotCanvasOnClick(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(RobotCanvas);

            if (!UpdateIk(new Vector3((float) pos.X, (float) pos.Y, 0)))
            {
               // MessageBox.Show("Failed to set IK");
            }
            Draw();
        }

        private bool UpdateIk(Vector3 target)
        {
            var tries = 0;
            while (tries < MaxRetries)
            {
                for (var i = segments.Count - 1; i >= 0; i--)
                {
                    var currentPos = segments[i].GetForwardKinematics();
                    Debug.WriteLine($"start pos: {currentPos}");

                    // Vector from the ith joint to the end effector
                    var r1 = segments[^1].GetForwardKinematics() - currentPos;
                    
                    // Vector from the i'Th joint to the target
                    var r2 = target - currentPos;

                    if (r1.Length() * r2.Length() <= 0.001f)
                    {
                        // cos component will be 1 and sin will be 0
                        cos[i] = 1;
                        sin[i] = 0;
                    }
                    else
                    {
                        // find the components using dot and cross product
                        cos[i] = Vector3.Dot(r1, r2) / (r1.Length() * r2.Length());
                        sin[i] = Vector3.Cross(r1, r2).Length() / (r1.Length() * r2.Length());
                    }

                    // The axis of rotation is basically the 
                    // unit vector along the cross product 
                    var axis = Vector3.Cross(r1, r2);

                    // find the angle between r1 and r2 (and clamp values of cos to avoid errors)
                    theta[i] = (float) System.Math.Acos(System.Math.Max(-1, System.Math.Min(1f, cos[i])));
                    // invert angle if sin component is negative
                    if (sin[i] < 0.0f)
                    {
                        theta[i] = -theta[i];
                    }

                    // obtain an angle value between -pi and pi, and then convert to degrees
                    theta[i] = (float)(Math.SimpleAngle(theta[i]) * Math.RadsToDeg);

                    // rotate the ith joint along the axis by theta degrees in the world space.
                    segments[i].Rotate(axis, theta[i]);
                    Debug.WriteLine($"new  pos: {segments[i].GetForwardKinematics()}");

                }

                tries++;
            }

            var delta = segments[^1].GetForwardKinematics() - target;
            //If target is in reach, return true, else return false;
            return System.Math.Abs(delta.X) < Epsilon &&
                   System.Math.Abs(delta.Y) < Epsilon &&
                   System.Math.Abs(delta.Z) < Epsilon;
        }

        /// <summary> Adds a segment </summary>
        private void AddSegment()
        {
            var newSegment = segments.Count > 0 ? new ArmSegment(segments[^1], 90, 45) 
                                                : new ArmSegment(null, 0, 0);
            segments.Add(newSegment);
            Draw();
        }
        #endregion

        /// <summary> Draws the robot </summary>
        private void Draw()
        {
            RobotCanvas.Children.Clear();
            foreach (var t in segments)
            {
                var pos = t.GetForwardKinematics();
                var parentPos = t.Parent?.GetForwardKinematics();

                var joint = new Ellipse { Width = 15, Height = 15, Stroke = Brushes.Red };

                var line = new Line
                           {
                               X1 = parentPos?.X ?? 200,
                               Y1 = parentPos?.Y ?? 200,
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

