using System;
using System.Collections.Generic;
using JXWPFToolkit.Controls;

namespace TestApplication
{
	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();


			var items = new List<InputValuesControl.InputItem>
			{
				// normal controls
				new InputValuesControl.InputItem("id1")
				{
					DataType = typeof (int),
					Label = "Normal int: ",
					Value = 1
				},
				new InputValuesControl.InputItem("id2")
				{
					DataType = typeof (string),
					Label = "Normal string: ",
					Value = "some text"
				},
				new InputValuesControl.InputItem("id3")
				{
					DataType = typeof (DateTime),
					Label = "Normal DateTime: ",
					Value = DateTime.Now
				},
				new InputValuesControl.InputItem("id4")
				{
					DataType = typeof (bool),
					Label = "Normal bool: ",
					Value = true
				},
				new InputValuesControl.InputItem("id5")
				{
					DataType = typeof (double),
					Label = "Normal double: ",
					Value = 1.5
				},
				// required
				new InputValuesControl.InputItem("id6")
				{
					DataType = typeof (string),
					Label = "Required string: ",
					Required = true
				},
				// readonly
				new InputValuesControl.InputItem("id7")
				{
					DataType = typeof (string),
					Label = "Readonly string: ",
					ReadOnly = true
				},
				new InputValuesControl.InputItem("id8")
				{
					DataType = typeof (int),
					Label = "Readonly int: ",
					ReadOnly = true,
					Value = 1
				},
				new InputValuesControl.InputItem("id9")
				{
					DataType = typeof (DateTime),
					Label = "Formated date: ",
					Value = DateTime.Now,
					Format = "dd/MM/yyyy"
				},
				new InputValuesControl.InputItem("id10")
				{
					DataType = typeof (int),
					Label = "Formated int: ",
					Value = 1,
					Format = "#.00"
				}
			};


			InputControl.SetItems(items, 2);
		}
	}
}