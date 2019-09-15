using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CCD
{
    /// <summary>
    /// 2D cyclic coordinate descent based inverse kinematics
    /// </summary>
    public partial class CalcIk2DCcd : INotifyPropertyChanged
	{

#region Internal types
		
		#endregion

		#region Private data

		private static readonly Brush[] BoneColours = 
		{
			Brushes.Black,
			Brushes.GreenYellow,
			Brushes.DarkBlue,
			Brushes.DarkRed,
			Brushes.MintCream,
		};

		// number of CCD iterations to perform on each update
		private int iterationsPerUpdate;

        // lines used to draw the bones
        private readonly List<Line> boneLines = new List<Line>();
		
		// target position to reach for
		private Point targetPos = new Point(0,0);

		// max distance from end effector to target for a valid solution
		private double arrivalDist = 1.0;

		// result of current IK calculation
		private string ccdResult = "";

		private readonly DispatcherTimer updateTimer;

		#endregion

		#region INotifyPropertyChanged interface
		// event used by the user interface to bind to our properties
		public event PropertyChangedEventHandler PropertyChanged;

		// helper function to notify PropertyChanged subscribers
		protected void NotifyPropertyChanged(string propertyName)
		{
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Public properties

        public ObservableCollection<BoneData> Bones { get; } = new ObservableCollection<BoneData>();

        public int IterationsPerUpdate
		{
			get { return iterationsPerUpdate; }
			set
			{
				iterationsPerUpdate = Math.Max(1,value);
				NotifyPropertyChanged(nameof(IterationsPerUpdate)); // update the bound UI
			}
		}

		public string CcdResult
		{
			get { return ccdResult; }
			set
			{
				ccdResult = value;
				NotifyPropertyChanged(nameof(CcdResult)); // update the bound UI
			}
		}
			
		public double TargetPosX
		{
			get { return targetPos.X; }
			set
			{
				targetPos.X = value;
				NotifyPropertyChanged(nameof(TargetPosX)); // update the bound UI
				UpdateDisplay(); // redraw
			}
		}

		public double TargetPosY
		{
			get { return targetPos.Y; }
			set
			{
				targetPos.Y = value;
				NotifyPropertyChanged(nameof(TargetPosY)); // update the bound UI
				UpdateDisplay(); // redraw
			}
		}

		public double ArrivalDist
		{
			get { return arrivalDist; }
			set
			{
				arrivalDist = value;
				NotifyPropertyChanged(nameof(ArrivalDist)); // update the bound UI
				UpdateDisplay(); // redraw
			}
		}

		#endregion

		#region Lifespan functions
		public CalcIk2DCcd()
		{
			InitializeComponent();
			
			// set the iteration number
			IterationsPerUpdate = 1;

			// create the timer
			updateTimer = new DispatcherTimer(DispatcherPriority.Normal);
			updateTimer.Tick += UpdateTimer_Tick;
			updateTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);

			// add the initial bones
			AddBone();
			AddBone();
            AddBone();
            AddBone();


			TargetPosX = 100;
			TargetPosY = 100;

			// update the display
			UpdateDisplay();
		}
		#endregion
		
		#region Coordinate Conversion

		// compute the logical origin in _viewport coordinated
		private double ViewportWidth { get { return ViewportColumn.ActualWidth; } }
		private double ViewportHieght { get { return MainGrid.ActualHeight; } }
		private double ViewportCenterX { get { return ViewportWidth / 2; } }
		private double ViewportCenterY { get { return ViewportHieght / 2; } }

		// convert logical coordinates to _viewport coordinates
		private double LogicalToViewportX(double logicalX) { return logicalX + ViewportCenterX; }
		private double LogicalToViewportY(double logicalY) { return -logicalY + ViewportCenterY; }
	
		// convert _viewport coordinates to logical coordinates
		private double ViewportToLogicalX(double viewportX) { return viewportX - ViewportCenterX; }
		private double ViewportToLogicalY(double viewportY) { return -viewportY + ViewportCenterY; }
		
		#endregion

		#region Logic Functions

		/// <summary>
		/// Add a new bone to the chain at the selected location or at the end if no location is selected.
		/// </summary>
		void AddBone()
		{
            var newBone = new BoneData
            {
                Length = 50
            };

            newBone.PropertyChanged += BonePropertyChanged;

			// insert at the end if no bone is selected
			if( BoneList.SelectedIndex == -1 )
				Bones.Add( newBone );
			else
				Bones.Insert( BoneList.SelectedIndex, newBone );
		}

		/// <summary>
		/// Remove a new bone from the chain at the selected location or from the end if no location is selected.
		/// </summary>
		private void RemoveBone()
		{
			if( Bones.Count == 0 )
				return;

			// remove the end bone if no bone is selected
			var removeId = BoneList.SelectedIndex;
			if( removeId == -1 )
				removeId = (Bones.Count - 1);


			Bones[removeId].PropertyChanged -= BonePropertyChanged;
			Bones.RemoveAt( removeId );
		}

		/// <summary>
		/// Perform an iteration of IK
		/// </summary>
		private void UpdateIk()
		{
			var numBones = Bones.Count;
			
			if( numBones == 0 )
				return;

			// calculate the bone angles
			var ccdBones = new List< Bone>();
			for( var boneIdx = 0; boneIdx <= numBones; ++boneIdx )
			{
                var newCcdBone = new Bone
                {
                    Angle = (boneIdx < numBones) ? Bones[boneIdx].Radians : 0,
                    X = (boneIdx > 0) ? Bones[boneIdx - 1].Length : 0,
                    Y = 0,
                    Minlimiter = (boneIdx < numBones) ? Bones[boneIdx].MinLimiter : 0,
                    Maxlimiter = (boneIdx < numBones) ? Bones[boneIdx].MaxLimiter : 0
                };
                ccdBones.Add( newCcdBone );
			}

			// iterate CCD until limit is reached or we find a valid solution
			for( var itrCount = 0; itrCount < IterationsPerUpdate; ++itrCount )
			{
				var result = Kinematics.Inverse( ref ccdBones, new System.Drawing.PointF((float)TargetPosX, (float)TargetPosY), ArrivalDist );
				if( result == Kinematics.CcdResult.Processing )
				{
					CcdResult = "Processing";
				}
				else if( result == Kinematics.CcdResult.Success )
				{
					CcdResult = "Done";
					break;
				}
				else if( result == Kinematics.CcdResult.Failure )
				{
					CcdResult = "Failure";
					break;
				}
				else
				{
					CcdResult = "[UNKNOWN]";
                    Debug.Assert(false);
                    break;
				}
			}

			// extract the new bone data from the results
			for( var boneIdx = 0; boneIdx < numBones; ++boneIdx )
			{
				Bones[boneIdx].Radians = ccdBones[boneIdx].Angle;
			}
		}

		/// <summary>
		/// Update the scene displayed in the viewport
		/// </summary>
		private void UpdateDisplay()
		{
			var numBones = Bones.Count;
			
			// resize the number of bone lines
			while( boneLines.Count > numBones )
			{
				Viewport.Children.Remove( boneLines[boneLines.Count-1] );
				boneLines.RemoveAt( boneLines.Count-1 );
			}

			while( boneLines.Count < numBones )
			{
                var newBoneLine = new Line
                {
                    Stroke = BoneColours[boneLines.Count % BoneColours.Length],
                    StrokeThickness = 3
                };
                newBoneLine.SetValue( Panel.ZIndexProperty, 100 );

				boneLines.Add( newBoneLine );
				Viewport.Children.Add( newBoneLine );
			}

			// compute the orientations of the bone lines in logical space
			double curAngle = 0;
			for( var boneIdx = 0; boneIdx < numBones; ++boneIdx )
			{
				var curBone = Bones[boneIdx];

				curAngle += curBone.Radians;
				var cosAngle = Math.Cos( curAngle );
				var sinAngle = Math.Sin( curAngle );

				if( boneIdx > 0 )
				{
					boneLines[boneIdx].X1 = boneLines[boneIdx-1].X2;
					boneLines[boneIdx].Y1 = boneLines[boneIdx-1].Y2;
				}
				else
				{
					boneLines[boneIdx].X1 = 0;
					boneLines[boneIdx].Y1 = 0;
				}

				boneLines[boneIdx].X2 = boneLines[boneIdx].X1 + cosAngle*curBone.Length;
				boneLines[boneIdx].Y2 = boneLines[boneIdx].Y1 + sinAngle*curBone.Length;
			}

			// convert the bone positions to viewport space
			foreach( var curLine in boneLines )
			{
				curLine.X1 = LogicalToViewportX(curLine.X1);
				curLine.Y1 = LogicalToViewportY(curLine.Y1);

				curLine.X2 = LogicalToViewportX(curLine.X2);
				curLine.Y2 = LogicalToViewportY(curLine.Y2);
			}

			// draw the arrival distance
			Canvas.SetLeft( ArrivalEllipse, LogicalToViewportX(TargetPosX - ArrivalDist) );
			Canvas.SetTop( ArrivalEllipse, LogicalToViewportY(TargetPosY + ArrivalDist) );
			ArrivalEllipse.Width = 2.0 * ArrivalDist;
			ArrivalEllipse.Height = 2.0 * ArrivalDist;

			// draw the target
			Canvas.SetLeft( TargetEllipse, LogicalToViewportX(TargetPosX - TargetEllipse.Width/2) );
			Canvas.SetTop( TargetEllipse, LogicalToViewportY(TargetPosY + TargetEllipse.Height/2) );
			
			// draw the axes
			XAxisLine.X1 = 0;
			XAxisLine.Y1 = ViewportCenterY;
			XAxisLine.X2 = ViewportWidth;
			XAxisLine.Y2 = ViewportCenterY;

			YAxisLine.X1 = ViewportCenterX;
			YAxisLine.Y1 = 0;
			YAxisLine.X2 = ViewportCenterX;
			YAxisLine.Y2 = ViewportHieght;
		}

		/// <summary>
		/// Update logic at a set interval
		/// </summary>
		private void UpdateTimer_Tick(object sender, EventArgs e)
        {
			UpdateIk();
			UpdateDisplay();
		}

		#endregion
		
		#region Event Handlers

		private void BonePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case "Radians":
					UpdateDisplay();
					break;
				case "Length":
					UpdateDisplay();
					break;
			}
		}

		private void Viewport_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			// capture the mouse to keep grabing MouseMove events if the user drags the
			// mouse outside of the _viewport bounds
			if (!Viewport.IsMouseCaptured)
            {
                Viewport.CaptureMouse();
			}

			// update the target position
			var viewportPos = e.GetPosition(Viewport);
			TargetPosX = ViewportToLogicalX( viewportPos.X );
			TargetPosY = ViewportToLogicalY( viewportPos.Y );
		}

		private void Viewport_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			// release the captured mouse
			if (Viewport.IsMouseCaptured)
            {
                Viewport.ReleaseMouseCapture();
            }
		}

		private void Viewport_MouseMove(object sender, MouseEventArgs e)
		{
			// update the target position if we are still in a captured state
			// (i.e. the user has not released the mouse button)
			if (Viewport.IsMouseCaptured)
            {
				var viewportPos = e.GetPosition(Viewport);
				TargetPosX = ViewportToLogicalX( viewportPos.X );
				TargetPosY = ViewportToLogicalY( viewportPos.Y );
            }
		}

		private void ThisWindow_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			// update the display shapes based on the new window size
			UpdateDisplay();
		}

        private void ThisWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{			
			if( updateTimer != null )
			{
				if( IsVisible )
				{
					if( PlayRadioButton.IsChecked == true )
						updateTimer.Start();
				}
				else
				{
					updateTimer.Stop();
				}
			}
		}

		private void AddBoneButton_Click(object sender, RoutedEventArgs e)
		{
			AddBone();
			UpdateDisplay();
		}

		private void RemoveBoneButton_Click(object sender, RoutedEventArgs e)
		{
			RemoveBone();
			UpdateDisplay();
		}	

		private void PlayRadioButton_Checked(object sender, RoutedEventArgs e)
		{
			if( updateTimer != null )
				updateTimer.Start();
		}

		private void PauseRadioButton_Checked(object sender, RoutedEventArgs e)
		{
			if( updateTimer != null )
				updateTimer.Stop();
		}

		private void SingleUpdateButton_Click(object sender, RoutedEventArgs e)
		{
			PauseRadioButton.IsChecked = true;
			UpdateIk();
			UpdateDisplay();
		}

		#endregion
	}
}
