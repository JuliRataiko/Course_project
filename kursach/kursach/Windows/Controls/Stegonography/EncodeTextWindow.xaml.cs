using System.Windows;
using System.Windows.Controls;

namespace kursach.Controls
{
	/// <summary>
	/// Interaction logic for EncodeTextWindow.xaml
	/// </summary>
	public partial class EncodeTextWindow : Window
	{
		private NewWindow ControlledWindow { get; set; }

		public EncodeTextWindow(NewWindow controlledWindow)
		{
			ControlledWindow = controlledWindow;
			InitializeComponent();
		}

		private void Encode_Click(object sender, RoutedEventArgs e)
		{
			if (TextBox.Text == string.Empty) return;

			ControlledWindow.EncodeText(TextBox.Text);

			this.Close();
		}
	}
}
