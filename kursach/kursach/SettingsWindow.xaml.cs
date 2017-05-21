using Maestro.UI;
using System;
using System.Windows;
using System.Windows.Controls;

namespace kursach
{
	/// <summary>
	/// Interaction logic for SettingsWindow.xaml
	/// </summary>
	public partial class SettingsWindow : Window
	{
		public SettingsWindow()
		{
			InitializeComponent();
			switch (Utils.executor.fontFamily){
				case "Tahoma":
					FontPicker.SelectedIndex = 0;
					break;
				case "Calibri":
					FontPicker.SelectedIndex = 1;
					break;
				case "Times New Roman":
					FontPicker.SelectedIndex = 2;
					break;
			}

			switch (Utils.executor.fontSize)
			{
				case "8":
					TextSizePicker.SelectedIndex = 0;
					break;
				case "12":
					TextSizePicker.SelectedIndex = 1;
					break;
				case "24":
					TextSizePicker.SelectedIndex = 2;
					break;
			}

			TextBox.Text = Utils.executor.text;
		}

		private void BoldTextButton_Click(object sender, RoutedEventArgs e)
		{
			Utils.BooleanTrigger(ref Utils.executor.boldText);
		}

		private void ItalicTextButton_Click(object sender, RoutedEventArgs e)
		{
			Utils.BooleanTrigger(ref Utils.executor.italicText);
		}

		private void UnderlineTextButton_Click(object sender, RoutedEventArgs e)
		{
			Utils.BooleanTrigger(ref Utils.executor.underlinedText);
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			Utils.executor.fontSize = (TextSizePicker.SelectedValue as ComboBoxItem).Content as string;
			Utils.executor.fontFamily = (FontPicker.SelectedValue as ComboBoxItem).Content as string;
			Utils.executor.text = TextBox.Text;
		}
	}
}
