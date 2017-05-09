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
	/// Interaction logic for ColorBalanceControl.xaml
	/// </summary>
	public partial class ColorBalanceControl : Window
	{
		private NewWindow ControlledWindow { get; set; }

		public ColorBalanceControl(NewWindow controlledWindow)
		{
			ControlledWindow = controlledWindow;
			InitializeComponent();
		}

		private void RedSlider_changed(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			ControlledWindow.ChangeColorBalance((int)RedSlider.Value, (int)GreenSlider.Value, (int)BlueSlider.Value);
		}

		private void GreenSlider_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			ControlledWindow.ChangeColorBalance((int)RedSlider.Value, (int)GreenSlider.Value, (int)BlueSlider.Value);
		}

		private void BlueSlider_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			ControlledWindow.ChangeColorBalance((int)RedSlider.Value, (int)GreenSlider.Value, (int)BlueSlider.Value);
		}
	}
}
