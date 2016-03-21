using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace JXWPFToolkit.Controls.NotificationPanel
{
	public partial class NotificationPanel
	{
		private NotificationPanelDataContext Context
		{
			get { return (NotificationPanelDataContext)DataContext; }
		}

		public NotificationPanel()
		{
			InitializeComponent();
		}

		public void AddTextCard(string text, NotificationCard.CardType type, double timeout = -1)
		{
			NotificationCard card = new NotificationCard();
			card.CardContent = text;
			card.Type = type;
			card.Timeout = timeout;
			Context.Cards.Add(card);
		}

		private void OnCloseclick(object sender, RoutedEventArgs e)
		{
			((NotificationPanelDataContext)DataContext).Visibility = Visibility.Collapsed;
		}
	}
}
