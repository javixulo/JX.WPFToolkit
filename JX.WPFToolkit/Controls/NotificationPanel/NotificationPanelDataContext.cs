using System;
using System.Windows;
using JX.WPFToolkit.Helpers;

namespace JX.WPFToolkit.Controls.NotificationPanel
{
	public class NotificationPanelDataContext : ViewModelBase
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
				RaisePropertyChanged("Visibility");
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
	}
}
