using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
