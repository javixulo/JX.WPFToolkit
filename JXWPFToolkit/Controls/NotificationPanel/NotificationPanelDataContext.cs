using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace JXWPFToolkit.Controls.NotificationPanel
{
	public class NotificationPanelDataContext : INotifyPropertyChanged
	{
		private Visibility _visibility;
		public CardCollection Cards { get; set; }

		public Visibility Visibility
		{
			get
			{
				return _visibility;
			}
			set
			{
				_visibility = value;
				NotifyPropertyChanged("Visibility");
			}
		}

		public NotificationPanelDataContext()
		{
			Cards = new CardCollection();
			Cards.OnAdd += OnCardAdded;
			Visibility = Visibility.Collapsed;
		}

		private void OnCardAdded(object sender, EventArgs eventArgs)
		{
			Visibility = Visibility.Visible;
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
