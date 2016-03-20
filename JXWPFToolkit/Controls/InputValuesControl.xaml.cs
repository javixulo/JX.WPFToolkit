using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using JXWPFToolkit.Helpers;
using JXWPFToolkit.Validations;
using Xceed.Wpf.Toolkit;

namespace JXWPFToolkit.Controls
{
	public partial class InputValuesControl : Grid
	{
		public InputValuesControl()
		{
			InitializeComponent();
		}

		public void SetItems(IEnumerable<InputItem> items, uint itemsPerRow)
		{
			for (int i = 0; i < itemsPerRow; i++)
			{
				ItemsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
				ItemsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
			}

			itemsPerRow *= 2;

			int row = -1;
			int column = 0;

			foreach (InputItem item in items)
			{
				if (column % itemsPerRow == 0)
				{
					ItemsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
					row++;
					column = 0;
				}

				AddItemToGrid(item, row, column);

				column += 2;
			}
		}

		public IEnumerable<InputItem> GetItems()
		{
			List<InputItem> items = new List<InputItem>();
			foreach (Control control in ItemsGrid.Descendants<Control>().Where(x => x.Tag is InputItem))
			{
				InputItem item = (InputItem)control.Tag;
				item.Value = GetValue(control);
				items.Add(item);
			}

			return items;
		}

		private void AddItemToGrid(InputItem item, int row, int column)
		{
			Label label = new Label { Content = item.Label, HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Center };

			SetRow(label, row);
			SetColumn(label, column);
			ItemsGrid.Children.Add(label);

			Control control = GetControl(item);

			SetRow(control, row);
			SetColumn(control, column + 1);
			ItemsGrid.Children.Add(control);
		}

		private Control GetControl(InputItem item)
		{
			Control control;

			if (item.DataType == typeof (bool))
			{
				control = new CheckBox { IsChecked = (bool)item.Value };
				((CheckBox)control).Checked += OnValueChanged;
				((CheckBox)control).Unchecked += OnValueChanged;
			}
			else if (item.DataType == typeof(Int32) || item.DataType == typeof(Int16))
			{
				control = new IntegerUpDown { Value = item.Value == null ? 0 : Convert.ToInt32(item.Value), FormatString = item.Format };
				((IntegerUpDown)control).ValueChanged += OnValueChanged;
				
				if (item.Required)
				{
					Binding binding = new Binding("Value");
					binding.Mode = BindingMode.OneWayToSource;
					binding.RelativeSource = new RelativeSource(RelativeSourceMode.Self);
					binding.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
					binding.NotifyOnValidationError = true;
					binding.NotifyOnSourceUpdated = true;
					binding.NotifyOnTargetUpdated = true;
					binding.ValidationRules.Add(new RequiredFieldValidationRule());
				}

			}
			else if (item.DataType == typeof(Int64))
			{
				control = new LongUpDown { Value = item.Value == null ? 0 : Convert.ToInt64(item.Value), FormatString = item.Format };
				((LongUpDown)control).ValueChanged += OnValueChanged;

				if (item.Required)
				{
					Binding binding = new Binding("Value");
					binding.Mode = BindingMode.OneWayToSource;
					binding.RelativeSource = new RelativeSource(RelativeSourceMode.Self);
					binding.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
					binding.NotifyOnValidationError = true;
					binding.NotifyOnSourceUpdated = true;
					binding.NotifyOnTargetUpdated = true;
					binding.ValidationRules.Add(new RequiredFieldValidationRule());
				}
			}
			else if (item.DataType == typeof(double))
			{
				control = new DoubleUpDown { Value = item.Value == null ? 0 : (double)item.Value };
				((DoubleUpDown)control).ValueChanged += OnValueChanged;

				if (item.Required)
				{
					Binding binding = new Binding("Value");
					binding.Mode = BindingMode.OneWayToSource;
					binding.RelativeSource = new RelativeSource(RelativeSourceMode.Self);
					binding.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
					binding.NotifyOnValidationError = true;
					binding.NotifyOnSourceUpdated = true;
					binding.NotifyOnTargetUpdated = true;
					binding.ValidationRules.Add(new RequiredFieldValidationRule());
				}
			}
				//else if (item.DataType == typeof (decimal))
				//{
				//    control = new DecimalUpDown { Value = item.Value == null ? 0 : (decimal)item.Value, FormatString = item.Format };
				//    ((DecimalUpDown)control).ValueChanged += OnValueChanged;
				//}
			else if (item.DataType == typeof(DateTime))
			{
				control = new DateTimePicker { Value = item.Value == null ? DateTime.Today : (DateTime)item.Value, Format = DateTimeFormat.Custom, TimeFormatString = item.Format, FormatString = item.Format };
				((DateTimePicker)control).ValueChanged += OnValueChanged;

				if (item.Required)
				{
					Binding binding = new Binding("Value");
					binding.Mode = BindingMode.OneWayToSource;
					binding.RelativeSource = new RelativeSource(RelativeSourceMode.Self);
					binding.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
					binding.NotifyOnValidationError = true;
					binding.NotifyOnSourceUpdated = true;
					binding.NotifyOnTargetUpdated = true;
					binding.ValidationRules.Add(new RequiredFieldValidationRule());
				}

			}
			else if (item.DataType == typeof (IEnumerable<ComboBoxItem>))
			{
				IEnumerable<ComboBoxItem> items = (IEnumerable<ComboBoxItem>)item.Value;
				int selectedIndex  = items.FindIndex(x => x.IsSelected);
				control = new ComboBox { ItemsSource = items, SelectedIndex = selectedIndex };
				((ComboBox)control).SelectionChanged += OnValueChanged;
			}
			else
			{
				control = new TextBox { Tag = item.Id, Text = item.Value == null ? string.Empty : item.Value.ToString() };
				control.LostFocus += OnValueChanged;
				if (item.Required)
				{
					control.Style = (Style)Resources["RequiredStyle"];

					Binding binding = new Binding("Text");
					binding.Mode = BindingMode.OneWayToSource;
					binding.RelativeSource = new RelativeSource(RelativeSourceMode.Self);
					binding.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
					binding.NotifyOnValidationError = true;
					binding.NotifyOnSourceUpdated = true;
					binding.NotifyOnTargetUpdated= true;
					binding.ValidationRules.Add(new RequiredFieldValidationRule());

					control.SetBinding(TextBox.TextProperty, binding);
				}
			}

			control.VerticalAlignment = VerticalAlignment.Center;
			control.Tag = item;
			control.Name = string.Format("{0}", item.Id);
			control.IsEnabled = !item.ReadOnly;

			RegisterName(control.Name, control);

			//if (item.IsEnabledBinding != null)
			//	control.SetBinding(IsEnabledProperty, item.IsEnabledBinding);

			return control;
		}

		#region events

		/// <summary>
		/// This event gets fired when a value in an item its changed
		/// </summary>
		public event ItemValueChangedEventHandler ItemValueChanged;

		private void OnValueChanged(object sender, RoutedEventArgs routedEventArgs)
		{
			if (ItemValueChanged == null)
				return;

			var control = sender as Control;
			var args = new InputItemValueChangedEventArgs();
			args.Item = (InputItem)control.Tag;
			args.OldValue = args.Item.Value;
			args.NewValue = GetValue(control);

			args.Item.Value = args.NewValue;

			if (!args.NewValue.Equals(args.OldValue))
				ItemValueChanged(sender, args);
		}

		#endregion

		private static object GetValue(Control control)
		{
			if (control is CheckBox)
				return (control as CheckBox).IsChecked;

			if (control is IntegerUpDown)
				return (control as IntegerUpDown).Value;

			if (control is LongUpDown)
				return (control as LongUpDown).Value;

			if (control is DoubleUpDown)
				return (control as DoubleUpDown).Value;

			//if (control is DecimalUpDown)
			//	return (control as DecimalUpDown).Value;

			if (control is DateTimePicker)
				return (control as DateTimePicker).Value;

			if (control is ComboBox)
				return (control as ComboBox).SelectedValue;

			//if (control is TextBox)
			//    return (control as TextBox).Text;

			InputItem item = (InputItem)control.Tag;
			if (!(control is TextBox))
				return null;

			string text = (control as TextBox).Text;

			if (item.DataType == typeof (int))
				return string.IsNullOrEmpty(text) ? (int)0 : Convert.ToInt32(text);
			if (item.DataType == typeof(Int64))
				return string.IsNullOrEmpty(text) ? (Int64)0 : Convert.ToInt64(text);
			if (item.DataType == typeof (double))
				return string.IsNullOrEmpty(text) ? (double)0 : Convert.ToDouble(text);
			if (item.DataType == typeof (decimal))
				return string.IsNullOrEmpty(text) ? (decimal)0 : Convert.ToDecimal(text);
			if (item.DataType == typeof (DateTime))
				return DateTime.ParseExact(text, item.Format, null);


			return text;
		}

		public delegate void ItemValueChangedEventHandler(object sender, InputItemValueChangedEventArgs e);

		public class InputItemValueChangedEventArgs : EventArgs
		{
			public InputItem Item;
			public object OldValue;
			public object NewValue;
		}

		public class InputItem
		{
			/// <summary>
			/// Unique ID for the item
			/// </summary>
			public readonly string Id;

			/// <summary>
			/// Label that will be displayed next to the item
			/// </summary>
			public string Label;

			/// <summary>
			/// Order to display the items (not used by the moment...)
			/// </summary>
			public uint Order;

			/// <summary>
			/// Format to apply to numbers or dates (not used by the moment...)
			/// </summary>
			public string Format;

			/// <summary>
			/// Put anything you like in here!
			/// </summary>
			public object Tag;

			/// <summary>
			/// Data type of the item. Please note that the data type will change the control that is being used to display the value.
			/// </summary>
			public Type DataType;

			/// <summary>
			/// Set to true to add a <see cref="RequiredFieldValidationRule"/>
			/// </summary>
			public bool Required;

			public bool ReadOnly;

			public object Value;

			// IDEAS: ReadOnly, IsEnabledBinding (one item may be enabled just if another is), AllowDelete, AllowEmpty, bind Validation...

			// public Binding IsEnabledBinding;

			public InputItem(string id)
			{
				Id = GetValidId(id);
				DataType = typeof (string);
			}

			private static string GetValidId(string id)
			{
				return id.Replace('#', '_');
			}

			public InputItem(string id, Type type)
			{
				Id = id;
				DataType = type;
			}
		}
	}
}
