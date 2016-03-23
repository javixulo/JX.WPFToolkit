using System.Linq;
using System.Windows;
using System.Windows.Input;
using JXWPFToolkit.Controls.NotificationPanel.NotificationCards;
using JXWPFToolkit.Helpers;

namespace JXWPFToolkit.Controls.NotificationPanel
{
	public partial class NotificationPanel
	{
		private NotificationPanelDataContext Context
		{
			get
			{
				return (NotificationPanelDataContext)DataContext;
			}
		}

		public NotificationPanel()
		{
			InitializeComponent();

			SayHelloCommand = new RelayCommand(x=>SayHello());
		}

		public void AddTextCard(string text, NotificationCardBase.CardType type, double timeout = -1)
		{
			NotificationCard card = new NotificationCard();
			card.CardContent = text;
			card.Type = type;
			card.Timeout = timeout;
			Context.Cards.Add(card);
		}

		public RelayCommand SayHelloCommand { get; set; }

		public void AddButtonCard(string text, NotificationCardBase.CardType type, double timeout = -1)
		{
			
			ButtonNotificationCard card = new ButtonNotificationCard();
			card.CardContent = text;
			card.Type = type;
			card.Timeout = timeout;
			card.Command = SayHelloCommand;
			Context.Cards.Add(card);
		}

		private void SayHello()
		{
			MessageBox.Show("Hi!");
		}

		private void OnCloseclick(object sender, RoutedEventArgs e)
		{
			((NotificationPanelDataContext)DataContext).Visibility = Visibility.Collapsed;
		}
	}
}
