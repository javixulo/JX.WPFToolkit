using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using JX.WPFToolkit.Controls;
using JX.WPFToolkit.Helpers;

namespace JX.WPFToolkit.Windows
{
	/// <summary>
	/// Generic window used for asking the user to input some values
	/// </summary>
	public partial class InputValuesWindow
	{
		public InputValuesWindow(IEnumerable<InputValuesControl.InputItem> items, WindowSize size = WindowSize.Small, Orientation orientation = Orientation.Vertical, uint itemsPerRow = 1)
		{
			InitializeComponent();

			AdjustWindowSize(size, orientation);

			InputValues.SetItems(items, itemsPerRow);
		}

		#region events

		/// <summary>
		/// This event gets fired when the user has ended introducing values. It is sent when the window is closed.
		/// </summary>
		public event InputFinishedEventHandler InputFinished;


		public delegate void InputFinishedEventHandler(object sender, InputFinishedEventArgs e);

		public class InputFinishedEventArgs : EventArgs
		{
			public IEnumerable<InputValuesControl.InputItem> Items;
		}

		private void OnCloseClicked(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void OnSaveClicked(object sender, EventArgs e)
		{
			if (!InputValues.IsValid())
			{
				MessageBox.Show("Hay errores de validación. Soluciónelos para poder guardar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			if (InputFinished == null)
			{
				return;
			}

			var args = new InputFinishedEventArgs();
			args.Items = InputValues.GetItems();

			InputFinished(this, args);
		}

		#endregion

		private void AdjustWindowSize(WindowSize size, Orientation orientation)
		{
			switch (size)
			{
				case WindowSize.Small:
					Height = 350;
					Width = 300;
					break;
				case WindowSize.Medium:
					Height = 450;
					Width = 400;
					break;
				case WindowSize.Large:
					Height = 700;
					Width = 600;
					break;
				default:
					throw new ArgumentOutOfRangeException("size");
			}

			MinWidth = 300;
			MinHeight = 300;

			if (orientation == Orientation.Horizontal)
			{
				double aux = Height;
				Height = Width;
				Width = aux;
			}
		}
	}

	public enum WindowSize
	{
		Small,
		Medium,
		Large
	}
}
