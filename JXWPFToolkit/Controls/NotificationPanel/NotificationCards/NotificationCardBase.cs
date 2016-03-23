using System.ComponentModel;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace JX.WPFToolkit.Controls.NotificationPanel.NotificationCards
{
	public abstract class NotificationCardBase : UserControl, INotifyPropertyChanged
	{
		private CardType _type;

		public CardType Type
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
				NotifyPropertyChanged("Type");
			}
		}

		protected Timer Timer;

		public static readonly DependencyProperty TimeoutProperty = DependencyProperty.Register("Timeout", typeof (double), typeof (NotificationCard));

		public double Timeout
		{
			get
			{
				return (double)GetValue(TimeoutProperty);
			}
			set
			{
				SetValue(TimeoutProperty, value);

				Timer.Stop();

				if (value < 0)
					return;

				Timer.Interval = value;
				Timer.Start();
			}
		}

		public event RoutedEventHandler CardDeleted;

		protected virtual void OnCardDeleted(object sender, RoutedEventArgs args)
		{
			if (CardDeleted != null)
				CardDeleted(this, args);
		}

		protected NotificationCardBase()
		{
			Timer = new Timer();
			Timer.Elapsed += OnTimerElapsed;

			Height = 45;
			MaxWidth = 200;
			Width = 200;
			Background = Brushes.White;
			BorderBrush = Brushes.DimGray;
			BorderThickness = new Thickness(1);
		}

		private void OnTimerElapsed(object sender, ElapsedEventArgs args)
		{
			Timer.Stop();
			Timer.Dispose();
			Timer = null;

			if (CardDeleted != null)
				CardDeleted(this, new RoutedEventArgs());
		}

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		protected void NotifyPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion

		public enum CardType
		{
			None,
			Info,
			Warning,
			Error
		}
	}
}
