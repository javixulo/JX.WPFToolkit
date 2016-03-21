using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using JXWPFToolkit.Controls.NotificationPanel;

namespace JXWPFToolkit.Converters
{
	public class TypeToImageConverter : IValueConverter
	{
		private ResourceDictionary _resources;
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (_resources == null)
				_resources = new ResourceDictionary { Source = new Uri("/JXWPFToolkit;component/Dictionaries/NotificationPanelDictionary.xaml", UriKind.RelativeOrAbsolute) };

			NotificationCard.CardType type;
			Enum.TryParse(value.ToString(), true, out type);

			switch (type)
			{
				case NotificationCard.CardType.None:
					break;

				case NotificationCard.CardType.Info:
					return (BitmapImage)_resources["ImageInfo"];

				case NotificationCard.CardType.Warning:
					return (BitmapImage)_resources["ImageWarning"];

				case NotificationCard.CardType.Error:
					return _resources["ImageError"];
				default:
					throw new ArgumentOutOfRangeException();
			}

			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class TypeToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{

			NotificationCard.CardType type;
			Enum.TryParse(value.ToString(), true, out type);

			return type == NotificationCard.CardType.None ? Visibility.Collapsed : Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
