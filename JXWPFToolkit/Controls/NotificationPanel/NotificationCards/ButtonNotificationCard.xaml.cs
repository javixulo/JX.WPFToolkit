using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace JXWPFToolkit.Controls.NotificationPanel.NotificationCards
{
	public partial class ButtonNotificationCard : NotificationCardBase
	{
		public static readonly DependencyProperty CardContentProperty = DependencyProperty.Register("CardContent", typeof(string), typeof(ButtonNotificationCard));

		public string CardContent
		{
			get { return (string)GetValue(CardContentProperty); }
			set { SetValue(CardContentProperty, value); }
		}

		public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(ButtonNotificationCard));

		public ICommand Command
		{
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}

		public ButtonNotificationCard()
		{
			InitializeComponent();
		}
	}
}
