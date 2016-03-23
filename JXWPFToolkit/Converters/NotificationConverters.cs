using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using JXWPFToolkit.Controls.NotificationPanel.NotificationCards;

namespace JXWPFToolkit.Converters
{
	public class TypeToImageConverter : IValueConverter
	{
		private ResourceDictionary _resources;

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (_resources == null)
				_resources = new ResourceDictionary { Source = new Uri("/JXWPFToolkit;component/Dictionaries/NotificationPanelDictionary.xaml", UriKind.RelativeOrAbsolute) };

			NotificationCardBase.CardType type;
			Enum.TryParse(value.ToString(), true, out type);

			switch (type)
			{
				case NotificationCardBase.CardType.None:
					break;

				case NotificationCardBase.CardType.Info:
					return (BitmapImage)_resources["ImageInfo"];

				case NotificationCardBase.CardType.Warning:
					return (BitmapImage)_resources["ImageWarning"];

				case NotificationCardBase.CardType.Error:
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
			NotificationCardBase.CardType type;
			Enum.TryParse(value.ToString(), true, out type);

			return type == NotificationCardBase.CardType.None ? Visibility.Collapsed : Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
