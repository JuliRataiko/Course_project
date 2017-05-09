using kursach.Controls;
using kursach.ImageProcessing;
using System;
using System.Collections.Generic;
using System.Drawing;
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

namespace kursach
{
	/// <summary>
	/// Логика взаимодействия для NewWindow.xaml
	/// </summary>
	public partial class NewWindow : Window
	{
		private BitmapImage originalImage;
		private BitmapSource currentCanvasImage;
		private BitmapSource tempImage;
		const double ScaleRate = 1.1;

		public NewWindow(BitmapImage image)
		{
			InitializeComponent();
			this.originalImage = image;
			currentCanvasImage = image;
			MainCanvas.Background = new ImageBrush { ImageSource = currentCanvasImage };
			//MainCanvas.Width = image.Width;
			//MainCanvas.Height = image.Height;
		}

		//private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
		//{
		//	bool handle = (Keyboard.Modifiers & ModifierKeys.Control) > 0;
		//	if (!handle)
		//		return;

		//	if (e.Delta > 0)
		//	{
		//		CanvasScaleTransform.ScaleX *= ScaleRate;
		//		CanvasScaleTransform.ScaleY *= ScaleRate;
		//	}
		//	else
		//	{
		//		CanvasScaleTransform.ScaleX /= ScaleRate;
		//		CanvasScaleTransform.ScaleY /= ScaleRate;
		//	}
		//}

		private void PopupBox_OnOpened(object sender, RoutedEventArgs e)
		{
			Console.WriteLine("Just making sure the popup has opened.");
		}

		private void PopupBox_OnClosed(object sender, RoutedEventArgs e)
		{
			Console.WriteLine("Just making sure the popup has closed.");
		}

		private void CanvasScroll_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			e.Handled = true;
			bool handle = (Keyboard.Modifiers & ModifierKeys.Control) > 0;
			if (!handle)
				return;

			if (e.Delta > 0)
			{
				CanvasScaleTransform.ScaleX *= ScaleRate;
				CanvasScaleTransform.ScaleY *= ScaleRate;
			}
			else
			{
				CanvasScaleTransform.ScaleX /= ScaleRate;
				CanvasScaleTransform.ScaleY /= ScaleRate;
			}
		}

		private void Black_wight_Click(object sender, RoutedEventArgs e)
		{
			currentCanvasImage = currentCanvasImage.ConvertToGrayscale();
			MainCanvas.Background = new ImageBrush { ImageSource = currentCanvasImage };
		}

		private void Discard_item_Click(object sender, RoutedEventArgs e)
		{
			currentCanvasImage = originalImage;
			MainCanvas.Background = new ImageBrush { ImageSource = currentCanvasImage };
		}

		private void Inversion_Click(object sender, RoutedEventArgs e)
		{
			var tmp = currentCanvasImage.ConvertToBitmap();
			currentCanvasImage = tmp.ReverseImage();
			MainCanvas.Background = new ImageBrush { ImageSource = currentCanvasImage };
		}

		private void Sepia_Click(object sender, RoutedEventArgs e)
		{
			currentCanvasImage = currentCanvasImage.ChangeSepia();
			MainCanvas.Background = new ImageBrush { ImageSource = currentCanvasImage };
		}

		private void Contrast_Click(object sender, RoutedEventArgs e)
		{
			BrightnessContrastWindow controlPane = new BrightnessContrastWindow(this);
			controlPane.ResizeMode = ResizeMode.NoResize;
			controlPane.ShowDialog();
		}

		private void Color_balance_Click(object sender, RoutedEventArgs e)
		{
			ColorBalanceControl controlPane = new ColorBalanceControl(this);
			controlPane.ResizeMode = ResizeMode.NoResize;
			controlPane.ShowDialog();
		}

		private void Saturation_Click(object sender, RoutedEventArgs e)
		{
			SaturationControl controlPane = new SaturationControl();
			controlPane.ResizeMode = ResizeMode.NoResize;
			controlPane.ShowDialog();
		}

		public void ChangeBrightness(int brightnessValue)
		{
			tempImage = currentCanvasImage.ChangeBrightness(brightnessValue);
			MainCanvas.Background = new ImageBrush { ImageSource = tempImage };
		}

		public void ChangeContrast(int contrastValue)
		{
			tempImage = currentCanvasImage.ChangeContrast(contrastValue);
			MainCanvas.Background = new ImageBrush { ImageSource = tempImage };
		}

		public void UpdateCanvasAfterFiltering()
		{
			MainCanvas.Background = new ImageBrush { ImageSource = tempImage };
		}

		public void ChangeColorBalance(int red, int green, int blue)
		{
			tempImage = currentCanvasImage.ChangeColorBalance(red, green, blue);
			MainCanvas.Background = new ImageBrush { ImageSource = tempImage };
		}

		private void In_item_Click(object sender, RoutedEventArgs e)
		{
			EncodeTextWindow controlPane = new EncodeTextWindow(this);
			controlPane.ResizeMode = ResizeMode.NoResize;
			controlPane.ShowDialog();
		}

		public void EncodeText(string text)
		{
			currentCanvasImage.TextToImage(text);
			MainCanvas.Background = new ImageBrush { ImageSource = currentCanvasImage };
		}

		public string DecodeText()
		{
			return currentCanvasImage.GetTextFromImage();
		}

		private void Out_item_Click(object sender, RoutedEventArgs e)
		{
			DecodeTextWindow controlPane = new DecodeTextWindow(this);
			controlPane.ResizeMode = ResizeMode.NoResize;
			controlPane.ShowDialog();
		}
	}
}
