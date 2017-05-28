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
	/// Interaction logic for DecodeTextWindow.xaml
	/// </summary>
	public partial class DecodeTextWindow : Window
	{
		public DecodeTextWindow(NewWindow controlledWindow)
		{
			InitializeComponent();
			ResultText.Text = controlledWindow.DecodeText();
		}
	}
}
