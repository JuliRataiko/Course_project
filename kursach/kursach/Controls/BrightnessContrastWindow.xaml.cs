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
	/// Interaction logic for BrightnessContrastWindow.xaml
	/// </summary>
	public partial class BrightnessContrastWindow : Window
	{
		private NewWindow ControlledWindow { get; set; }

		public BrightnessContrastWindow(NewWindow controlledWindow)
		{
			ControlledWindow = controlledWindow;
			InitializeComponent();
		}

		private void BrightnessSlider_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			ControlledWindow.ChangeBrightness((int)BrightnessSlider.Value);
		}

		private void WindowClosed(object sender, EventArgs e)
		{
			ControlledWindow.UpdateCanvasAfterFiltering();
		}

		private void ContrastSlider_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			ControlledWindow.ChangeContrast((int)ContrastSlider.Value);
		}
	}
}
