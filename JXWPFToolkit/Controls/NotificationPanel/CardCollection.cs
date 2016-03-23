using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using JXWPFToolkit.Controls.NotificationPanel.NotificationCards;

namespace JXWPFToolkit.Controls.NotificationPanel
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
