using kursach.Core;
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
			switch (Utils.executor.FontFamily)
			{
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

			switch (Utils.executor.FontSize)
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

			TextBox.Text = Utils.executor.Text;
			BoldItem.IsSelected = Utils.executor.boldText;
			ItalianItem.IsSelected = Utils.executor.italianText;
			UnderlineItem.IsSelected = Utils.executor.underlinedText;
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			Utils.executor.FontSize = (TextSizePicker.SelectedValue as ComboBoxItem).Content as string;
			Utils.executor.FontFamily = (FontPicker.SelectedValue as ComboBoxItem).Content as string;
			Utils.executor.Text = TextBox.Text;
			Utils.executor.boldText = BoldItem.IsSelected;
			Utils.executor.italianText = ItalianItem.IsSelected;
			Utils.executor.underlinedText = UnderlineItem.IsSelected;
		}

		private void ApplyButton_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void ResetButton_Click(object sender, RoutedEventArgs e)
		{
			FontPicker.SelectedIndex = 0;
			TextSizePicker.SelectedIndex = 0;
			TextBox.Text = "Пример текста АБВ";
			BoldItem.IsSelected = false;
			ItalianItem.IsSelected = false;
			UnderlineItem.IsSelected = false;
		}
	}
}
