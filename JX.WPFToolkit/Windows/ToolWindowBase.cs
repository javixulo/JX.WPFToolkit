using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace JX.WPFToolkit.Windows
{
	public class ToolWindowBase : BaseWindow
	{

		public ToolWindowBase()
		{
			WindowStartupLocation = WindowStartupLocation.CenterScreen;
			Topmost = true;
			ShowInTaskbar = false;
		}

		public new bool? Show()
		{
			return ShowDialog();
		}

		public override FrameworkElementFactory GetWindowButtons()
		{
			var stackPanel = new FrameworkElementFactory(typeof(StackPanel));
			stackPanel.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
			stackPanel.SetValue(MarginProperty, new Thickness(5, 2, 5, 2));

			var closeButton = new FrameworkElementFactory(typeof(Button));
			closeButton.SetValue(StyleProperty, ResourceDictionary["StyleButtonWindowTitle"]);
			closeButton.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(OnCloseButtonClick));

			var buttonImage = new FrameworkElementFactory(typeof(Image));
			buttonImage.SetValue(Image.SourceProperty, ResourceDictionary["ImageCloseWindow"]);
			closeButton.AppendChild(buttonImage);
			stackPanel.AppendChild(closeButton);

			return stackPanel;
		}
	}
}
