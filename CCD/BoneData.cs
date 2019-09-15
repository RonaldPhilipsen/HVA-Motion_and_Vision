using System;
using System.ComponentModel;

namespace CCD
{
        // this class represents a bone in it's parent space
        public class BoneData : INotifyPropertyChanged
		{
			private double length;
			private double angle;

            // joint limiters
            private double min = -3.14;
            private double max = 3.14;

			#region INotifyPropertyChanged interface
			// event used by the user interface to bind to our properties
			public event PropertyChangedEventHandler PropertyChanged;

            // helper function to notify PropertyChanged subscribers
            protected void NotifyPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            #endregion

            public double Length
			{
				get { return length; }
				set { length = value; NotifyPropertyChanged(nameof(Length)); }
			}
			public double Radians
			{ 
				get { return angle; }
				set { angle = value; NotifyPropertyChanged(nameof(Radians)); NotifyPropertyChanged(nameof(Degrees)); }
			}

			public double Degrees
			{ 
				get { return angle * 180.0 / Math.PI; }
				set { angle = value * Math.PI / 180.0; NotifyPropertyChanged(nameof(Radians)); NotifyPropertyChanged(nameof(Degrees)); }
			}

            public double MinLimiter
            {
                get { return min; }
                set { min = value * Math.PI / 180.0; NotifyPropertyChanged(nameof(MinLimiter)); }
            }
            public double MinLimiterConv
            {
                get { return min * 180.0 / Math.PI; }
                set { min = value * Math.PI / 180.0; NotifyPropertyChanged(nameof(MinLimiter)); }
            }

            public double MaxLimiter
            {
                get { return max; }
                set { max = value * Math.PI / 180.0; NotifyPropertyChanged("Maxlimiter"); }
            }

            public double MaxLimiterConv
            {
                get { return max * 180.0 / Math.PI; }
                set { max = value * Math.PI / 180.0; NotifyPropertyChanged("Maxlimiter"); }
            }
		}

	}

