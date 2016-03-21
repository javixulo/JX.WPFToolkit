using System.ComponentModel;
using System.Linq;
using System.Timers;
using System.Windows;

namespace JXWPFToolkit.Controls.NotificationPanel
{
	public partial class NotificationCard : INotifyPropertyChanged
	{
		Timer _timer;
    
		public static readonly DependencyProperty TimeoutProperty = DependencyProperty.Register("Timeout", typeof(double), typeof(NotificationCard));

		public double Timeout
		{
			get
			{
				return (double)GetValue(TimeoutProperty);
			}
			set
			{
				SetValue(TimeoutProperty, value);

				_timer.Stop();

				if (value < 0)
					return;

				_timer.Interval = value;
				_timer.Start();

			}
		}

		public static readonly DependencyProperty CardContentProperty = DependencyProperty.Register("CardContent", typeof(string), typeof(NotificationCard));

		public string CardContent
		{
			get { return (string)GetValue(CardContentProperty); }
			set { SetValue(CardContentProperty, value); }
		}

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

		public NotificationCard()
		{
			_timer = new Timer();
			_timer.Elapsed += OnTimerElapsed;

			InitializeComponent();
		}

		public event RoutedEventHandler CardDeleted;

		private void OnDeleteButtonClick(object sender, RoutedEventArgs e)
		{
			if (CardDeleted != null)
				CardDeleted(this, new RoutedEventArgs());
		}

		private void OnTimerElapsed(object sender, ElapsedEventArgs args)
		{
			_timer.Stop();
			_timer.Dispose();
			_timer = null;

			if (CardDeleted != null)
				CardDeleted(this, new RoutedEventArgs());
		}

		public enum CardType
		{
			None,
			Info,
			Warning,
			Error
		}

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion
	}
}
