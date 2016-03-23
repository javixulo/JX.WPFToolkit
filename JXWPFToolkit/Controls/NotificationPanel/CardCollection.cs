using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using JX.WPFToolkit.Controls.NotificationPanel.NotificationCards;

namespace JX.WPFToolkit.Controls.NotificationPanel
{
	public class CardCollection : ObservableCollection<NotificationCardBase>
	{
		public event EventHandler OnAdd;

		public new void Add(NotificationCardBase item)
		{
			item.CardDeleted += OnCardDeletedByUser;
			if (OnAdd != null)
				OnAdd(this, null);

			base.Add(item);
		}

		private void OnCardDeletedByUser(object sender, RoutedEventArgs routedEventArgs)
		{
			NotificationCardBase card = (NotificationCardBase)sender;

			Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => Remove(card)));

			card.CardDeleted -= OnCardDeletedByUser;
		}
	}
}
