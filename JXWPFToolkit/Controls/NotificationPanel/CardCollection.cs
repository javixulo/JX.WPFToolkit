using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace JXWPFToolkit.Controls.NotificationPanel
{
	public class CardCollection : ObservableCollection<NotificationCard>
	{
		public event EventHandler OnAdd;

		public new void Add(NotificationCard item)
		{
			item.CardDeleted += OnCardDeletedByUser;
			if (OnAdd != null)
				OnAdd(this, null);

			base.Add(item);
		}

		private void OnCardDeletedByUser(object sender, RoutedEventArgs routedEventArgs)
		{
			NotificationCard card = (NotificationCard)sender;

			Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => Remove(card)));

			card.CardDeleted -= OnCardDeletedByUser;
		}
	}
}
