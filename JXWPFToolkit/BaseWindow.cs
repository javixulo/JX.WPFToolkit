using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Binding = System.Windows.Data.Binding;
using Button = System.Windows.Controls.Button;
using ButtonBase = System.Windows.Controls.Primitives.ButtonBase;
using Cursors = System.Windows.Input.Cursors;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using Orientation = System.Windows.Controls.Orientation;
using Panel = System.Windows.Controls.Panel;

namespace JXWPFToolkit
{
	public class BaseWindow : Window
	{
		private Button _maxRestoreButton;
		private ResourceDictionary _resources;

		public static readonly DependencyProperty ButtonAreaProperty = DependencyProperty.Register("ButtonArea", typeof(object), typeof(ContentControl));

		public object ButtonArea
		{
			get
			{
				return GetValue(ButtonAreaProperty);
			}
			set
			{
				SetValue(ButtonAreaProperty, value);
			}
		}

		public double CalculatedHeight
		{
			get
			{
				return Screen.PrimaryScreen.Bounds.Height * 0.75;
			}
		}

		public double CalculatedWidth
		{
			get
			{
				return Screen.PrimaryScreen.Bounds.Width * 0.75;
			}
		}

		public BaseWindow()
		{
			InitializeComponent();

			WindowStyle = WindowStyle.None;

			AllowsTransparency = true;
			Background = Brushes.Transparent;
			ResizeMode = ResizeMode.CanResizeWithGrip;
			StateChanged += OnStateChanged;

			SourceInitialized += OnSourceInitialized;
		}

		#region events

		/// <summary>
		/// TitleBar_MouseDown - Drag if single-click, resize if double-click
		/// </summary>
		private void OnTitleBarMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
				if (e.ClickCount == 2)
				{
					AdjustWindowSize();
				}
				else
				{
					DragMove();
				}
		}

		private void OnCloseButtonClick(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void OnMaximizeRestoreButtonClick(object sender, RoutedEventArgs e)
		{
			AdjustWindowSize();
		}

		private void OnMinimizeButtonClick(object sender, RoutedEventArgs e)
		{
			WindowState = WindowState.Minimized;
		}

		private void OnMaximizeRestoreButtonLoaded(object sender, RoutedEventArgs e)
		{
			_maxRestoreButton = sender as Button;
		}

		private void OnStateChanged(object sender, EventArgs e)
		{
			switch (WindowState)
			{
				case WindowState.Normal:
					((Image)_maxRestoreButton.Content).Source = (BitmapImage)_resources["ImageMaximizeWindow"];
					break;
				case WindowState.Maximized:
					((Image)_maxRestoreButton.Content).Source = (BitmapImage)_resources["ImageRestoreWindow"];
					break;
			}
		}

		#endregion

		#region System menu - icon

		private const int WmSysCommand = 0x112;
		private const uint TpmLeftAlign = 0x0000;
		private const uint TpmReturnCmd = 0x0100;
		private const UInt32 MfEnabled = 0x00000000;
		private const UInt32 MfGrayed = 0x00000001;
		private const UInt32 ScMaximize = 0xF030;
		private const UInt32 ScRestore = 0xF120;

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

		[DllImport("user32.dll")]
		private static extern int TrackPopupMenuEx(IntPtr hmenu, uint fuFlags, int x, int y, IntPtr hwnd, IntPtr lptpm);

		[DllImport("user32.dll")]
		public static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		private static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

		private void OnIconClick(object sender, MouseButtonEventArgs e)
		{
			Image icon = sender as Image;

			// get location of the icon to display the menu there

			Point locationFromScreen = icon.PointToScreen(new Point(0, 0));

			// Transform screen point to WPF device independent point

			PresentationSource source = PresentationSource.FromVisual(this);
			Point point = source.CompositionTarget.TransformFromDevice.Transform(locationFromScreen);

			WindowInteropHelper helper = new WindowInteropHelper(this);
			IntPtr callingWindow = helper.Handle;
			IntPtr wMenu = GetSystemMenu(callingWindow, false);

			// Display the menu

			if (WindowState == WindowState.Maximized)
				EnableMenuItem(wMenu, ScMaximize, MfGrayed);
			else
				EnableMenuItem(wMenu, ScMaximize, MfEnabled);

			int command = TrackPopupMenuEx(wMenu, TpmLeftAlign | TpmReturnCmd, (int)point.X, (int)point.Y, callingWindow, IntPtr.Zero);
			if (command == 0)
				return;

			PostMessage(callingWindow, WmSysCommand, new IntPtr(command), IntPtr.Zero);
		}

		#endregion

		#region Set max window size

		private void OnSourceInitialized(object sender, EventArgs eventArgs)
		{
			IntPtr handle = (new WindowInteropHelper(this)).Handle;
			HwndSource.FromHwnd(handle).AddHook(new System.Windows.Interop.HwndSourceHook(WindowProc));
		}

		private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			switch (msg)
			{
				case 0x0024: /* WM_GETMINMAXINFO */
					WmGetMinMaxInfo(hwnd, lParam);
					handled = true;
					break;
			}

			return (IntPtr)0;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct POINT
		{
			/// <summary>
			/// x coordinate of point.
			/// </summary>
			public int x;

			/// <summary>
			/// y coordinate of point.
			/// </summary>
			public int y;

			/// <summary>
			/// Construct a point of coordinates (x,y).
			/// </summary>
			public POINT(int x, int y)
			{
				this.x = x;
				this.y = y;
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct MINMAXINFO
		{
			public POINT ptReserved;
			public POINT ptMaxSize;
			public POINT ptMaxPosition;
			public POINT ptMinTrackSize;
			public POINT ptMaxTrackSize;
		};

		[DllImport("user32")]
		internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

		[DllImport("User32")]
		internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

		private const int MONITOR_DEFAULTTONEAREST = 0x00000002;

		private void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
		{
			MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

			// Adjust the maximized size and position to fit the work area of the correct monitor
			IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

			if (monitor != IntPtr.Zero)
			{
				MONITORINFO monitorInfo = new MONITORINFO();
				GetMonitorInfo(monitor, monitorInfo);
				RECT rcWorkArea = monitorInfo.rcWork;
				RECT rcMonitorArea = monitorInfo.rcMonitor;
				mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
				mmi.ptMinTrackSize.x = (int)MinWidth;
				mmi.ptMinTrackSize.y = (int)MinHeight;
				mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
				mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
				mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
			}

			Marshal.StructureToPtr(mmi, lParam, true);
		}

		#endregion

		#region private helpers

		private void AdjustWindowSize()
		{
			if (WindowState == WindowState.Maximized)
				WindowState = WindowState.Normal;
			else
				WindowState = WindowState.Maximized;
		}

		private void InitializeComponent()
		{
			_resources = new ResourceDictionary { Source = new Uri("/JXWPFToolkit;component/Dictionaries/BaseDictionary.xaml", UriKind.RelativeOrAbsolute) };

			var template = new ControlTemplate();

			var border = new FrameworkElementFactory(typeof(Border));
			border.SetValue(Border.BorderThicknessProperty, new Thickness(2));
			border.SetValue(Border.BorderBrushProperty, _resources["MainColorBrush"]);
			border.SetValue(Border.CornerRadiusProperty, new CornerRadius(5));

			var outerGrid = GetOuterGrid();
			var innerGrid = GetInnerGrid();

			var adorner = new FrameworkElementFactory(typeof(AdornerDecorator));
			adorner.SetValue(DockPanel.DockProperty, Dock.Bottom);
			adorner.SetValue(Grid.RowProperty, 1);
			var contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
			contentPresenter.SetBinding(ContentPresenter.ContentProperty, new Binding("Content") { RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent) });


			var resizeGrip = new FrameworkElementFactory(typeof(ResizeGrip));
			resizeGrip.SetValue(Grid.RowProperty, 2);
			resizeGrip.SetValue(CursorProperty, Cursors.SizeWE);
			resizeGrip.SetValue(BackgroundProperty, Brushes.White);

			template.VisualTree = border;
			border.AppendChild(outerGrid);
			outerGrid.AppendChild(innerGrid);
			outerGrid.AppendChild(adorner);
			adorner.AppendChild(contentPresenter);
			outerGrid.AppendChild(resizeGrip);

			Template = template;
		}

		private static FrameworkElementFactory GetOuterGrid()
		{
			var outerGrid = new FrameworkElementFactory(typeof(Grid));

			// Title bar
			var rowDef = new FrameworkElementFactory(typeof(RowDefinition));
			rowDef.SetValue(RowDefinition.HeightProperty, new GridLength(38));
			outerGrid.AppendChild(rowDef);

			// Content
			rowDef = new FrameworkElementFactory(typeof(RowDefinition));
			rowDef.SetValue(RowDefinition.HeightProperty, new GridLength(1, GridUnitType.Star));
			outerGrid.AppendChild(rowDef);

			// ResizeGrip
			rowDef = new FrameworkElementFactory(typeof(RowDefinition));
			rowDef.SetValue(RowDefinition.HeightProperty, GridLength.Auto);
			outerGrid.AppendChild(rowDef);
			return outerGrid;
		}

		private FrameworkElementFactory GetInnerGrid()
		{
			var innerGrid = new FrameworkElementFactory(typeof(Grid));
			innerGrid.SetValue(Grid.RowProperty, 0);
			innerGrid.SetValue(Panel.BackgroundProperty, _resources["MainColorBrush"]);

			int columnIndex = 0;

			// Icon

			var columnDef = new FrameworkElementFactory(typeof(ColumnDefinition));
			columnDef.SetValue(ColumnDefinition.WidthProperty, GridLength.Auto);
			innerGrid.AppendChild(columnDef);

			var icon = new FrameworkElementFactory(typeof(Image));
			icon.SetValue(Grid.ColumnProperty, columnIndex);
			icon.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
			icon.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
			icon.SetValue(WidthProperty, (double)16);
			icon.SetValue(MarginProperty, new Thickness(5, 0, 5, 0));
			icon.SetBinding(Image.SourceProperty, new Binding("Icon") { RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent) });
			icon.AddHandler(MouseDownEvent, new MouseButtonEventHandler(OnIconClick));

			// Window title

			columnIndex++;

			columnDef = new FrameworkElementFactory(typeof(ColumnDefinition));
			columnDef.SetValue(ColumnDefinition.WidthProperty, new GridLength(1, GridUnitType.Star));
			innerGrid.AppendChild(columnDef);

			var title = new FrameworkElementFactory(typeof(TextBlock));
			title.SetValue(Grid.ColumnProperty, columnIndex);
			title.SetValue(TextBlock.TextAlignmentProperty, TextAlignment.Center);
			title.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
			title.SetValue(MarginProperty, new Thickness(3));
			title.SetValue(ForegroundProperty, new SolidColorBrush(Colors.White));
			title.SetValue(FontSizeProperty, (double)11);

			title.SetBinding(TextBlock.TextProperty, new Binding("Title") { RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent) });
			title.AddHandler(MouseDownEvent, new MouseButtonEventHandler(OnTitleBarMouseDown));

			// ButtonArea

			columnIndex++;

			columnDef = new FrameworkElementFactory(typeof(ColumnDefinition));
			columnDef.SetValue(ColumnDefinition.WidthProperty, GridLength.Auto);
			innerGrid.AppendChild(columnDef);

			var buttonArea = new FrameworkElementFactory(typeof(ContentControl));
			buttonArea.SetValue(Grid.ColumnProperty, columnIndex);
			buttonArea.SetBinding(ContentControl.ContentProperty, new Binding("ButtonArea") { RelativeSource = new RelativeSource { AncestorType = typeof(BaseWindow) } });

			// Buttons

			columnIndex++;

			columnDef = new FrameworkElementFactory(typeof(ColumnDefinition));
			columnDef.SetValue(ColumnDefinition.WidthProperty, GridLength.Auto);
			innerGrid.AppendChild(columnDef);

			var stackPanel = new FrameworkElementFactory(typeof(StackPanel));
			stackPanel.SetValue(Grid.ColumnProperty, columnIndex);
			stackPanel.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
			stackPanel.SetValue(MarginProperty, new Thickness(5, 2, 5, 2));

			var minButton = new FrameworkElementFactory(typeof(Button));

			minButton.SetValue(StyleProperty, _resources["StyleButtonWindowTitle"]);
			minButton.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(OnMinimizeButtonClick));

			var buttonImage = new FrameworkElementFactory(typeof(Image));
			buttonImage.SetValue(Image.SourceProperty, _resources["ImageMinimizeWindow"]);
			minButton.AppendChild(buttonImage);

			stackPanel.AppendChild(minButton);

			var maxButton = new FrameworkElementFactory(typeof(Button));
			maxButton.SetValue(StyleProperty, _resources["StyleButtonWindowTitle"]);
			maxButton.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(OnMaximizeRestoreButtonClick));
			maxButton.AddHandler(LoadedEvent, new RoutedEventHandler(OnMaximizeRestoreButtonLoaded));

			buttonImage = new FrameworkElementFactory(typeof(Image));
			buttonImage.SetValue(Image.SourceProperty, _resources["ImageMaximizeWindow"]);
			maxButton.AppendChild(buttonImage);
			stackPanel.AppendChild(maxButton);

			var closeButton = new FrameworkElementFactory(typeof(Button));
			closeButton.SetValue(StyleProperty, _resources["StyleButtonWindowTitle"]);
			closeButton.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(OnCloseButtonClick));

			buttonImage = new FrameworkElementFactory(typeof(Image));
			buttonImage.SetValue(Image.SourceProperty, _resources["ImageCloseWindow"]);
			closeButton.AppendChild(buttonImage);
			stackPanel.AppendChild(closeButton);

			// Add all elements in order

			innerGrid.AppendChild(icon);
			innerGrid.AppendChild(title);
			innerGrid.AppendChild(buttonArea);
			innerGrid.AppendChild(stackPanel);

			return innerGrid;
		}

		#endregion
	}

	#region Set max window size

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	public class MONITORINFO
	{
		public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));

		public RECT rcMonitor = new RECT();

		public RECT rcWork = new RECT();

		public int dwFlags = 0;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 0)]
	public struct RECT
	{
		public int left;

		public int top;

		public int right;

		public int bottom;

		public static readonly RECT Empty = new RECT();

		public int Width
		{
			get
			{
				return Math.Abs(right - left);
			} // Abs needed for BIDI OS
		}

		public int Height
		{
			get
			{
				return bottom - top;
			}
		}

		public RECT(int left, int top, int right, int bottom)
		{
			this.left = left;
			this.top = top;
			this.right = right;
			this.bottom = bottom;
		}

		public RECT(RECT rcSrc)
		{
			left = rcSrc.left;
			top = rcSrc.top;
			right = rcSrc.right;
			bottom = rcSrc.bottom;
		}

		public bool IsEmpty
		{
			get
			{
				// BUGBUG : On Bidi OS (hebrew arabic) left > right
				return left >= right || top >= bottom;
			}
		}

		public override string ToString()
		{
			if (this == RECT.Empty)
			{
				return "RECT {Empty}";
			}
			return "RECT { left : " + left + " / top : " + top + " / right : " + right + " / bottom : " + bottom + " }";
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Rect))
			{
				return false;
			}
			return (this == (RECT)obj);
		}

		public override int GetHashCode()
		{
			return left.GetHashCode() + top.GetHashCode() + right.GetHashCode() + bottom.GetHashCode();
		}

		public static bool operator ==(RECT rect1, RECT rect2)
		{
			return (rect1.left == rect2.left && rect1.top == rect2.top && rect1.right == rect2.right && rect1.bottom == rect2.bottom);
		}

		public static bool operator !=(RECT rect1, RECT rect2)
		{
			return !(rect1 == rect2);
		}
	}

	#endregion
}
