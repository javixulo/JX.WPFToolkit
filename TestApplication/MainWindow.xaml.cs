using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using JXWPFToolkit.Controls;
using JXWPFToolkit.Controls.NotificationPanel.NotificationCards;
using JXWPFToolkit.Windows;

namespace TestApplication
{
	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void InputValuesWindowClick(object sender, RoutedEventArgs e)
		{
			var items = new List<InputValuesControl.InputItem>
			{
				// normal controls
				new InputValuesControl.InputItem("Normal int")
				{
					DataType = typeof (int),
					Label = "Normal int: ",
					Value = 1
				},
				new InputValuesControl.InputItem("Normal string")
				{
					DataType = typeof (string),
					Label = "Normal string: ",
					Value = "some text"
				},
				new InputValuesControl.InputItem("Normal DateTime")
				{
					DataType = typeof (DateTime),
					Label = "Normal DateTime: ",
					Value = DateTime.Now
				},
				new InputValuesControl.InputItem("Normal bool")
				{
					DataType = typeof (bool),
					Label = "Normal bool: ",
					Value = true
				},
				new InputValuesControl.InputItem("Normal double")
				{
					DataType = typeof (double),
					Label = "Normal double: ",
					Value = 1.5
				},
				// required
				new InputValuesControl.InputItem("Normal string")
				{
					DataType = typeof (string),
					Label = "Required string: ",
					Required = true
				},
				// readonly
				new InputValuesControl.InputItem("Readonly string")
				{
					DataType = typeof (string),
					Label = "Readonly string: ",
					ReadOnly = true
				},
				new InputValuesControl.InputItem("Readonly int")
				{
					DataType = typeof (int),
					Label = "Readonly int: ",
					ReadOnly = true,
					Value = 1
				},
				new InputValuesControl.InputItem("Formated date")
				{
					DataType = typeof (DateTime),
					Label = "Formated date: ",
					Value = DateTime.Now,
					Format = "dd/MM/yyyy"
				},
				new InputValuesControl.InputItem("Formated int")
				{
					DataType = typeof (int),
					Label = "Formated int: ",
					Value = 1,
					Format = "#.00"
				}
			};

			InputValuesWindow window = new InputValuesWindow(items, WindowSize.Medium, Orientation.Vertical, 2);
			window.InputFinished += OnInputFinished;
			window.Show();
		}

		private void OnInputFinished(object sender, InputValuesWindow.InputFinishedEventArgs args)
		{
			StringBuilder builder = new StringBuilder();
			foreach (InputValuesControl.InputItem item in args.Items)
			{
				builder.AppendLine(string.Format("Control: {0}, new value: {1}", item.Id, item.Value));
			}

			MessageBox.Show(builder.ToString(), "Result", MessageBoxButton.OK, MessageBoxImage.Information);
		}

		private int _cardNumber = 1;

		private void OnAddCardClick(object sender, RoutedEventArgs e)
		{
			var type = (NotificationCardBase.CardType)(_cardNumber % 4);
			if (_cardNumber % 5 == 0)
				NotificationPanel.AddTextCard("This is the card with a veeery long text\nI hope you like it and I hope it looks correct: " + _cardNumber++, type);
			else
				NotificationPanel.AddTextCard("Card " + _cardNumber++, type);
		}

		private void OnAddTemporaryCardClick(object sender, RoutedEventArgs e)
		{
			var type = (NotificationCardBase.CardType)(_cardNumber % 4);
			NotificationPanel.AddTextCard("Card " + _cardNumber++, type, 2000);
		}

		private void OnAddButtonCardClick(object sender, RoutedEventArgs e)
		{
			var type = (NotificationCardBase.CardType)(_cardNumber % 4);
			NotificationPanel.AddButtonCard("Card " + _cardNumber++, type);
		}
	}
}