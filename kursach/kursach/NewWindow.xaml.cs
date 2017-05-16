using kursach.Controls;
using kursach.ImageProcessing;
using Maestro.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
			Utils.executor.ClearCanvas(ref MainCanvas, ref CanvasScaleTransform);
			MainCanvas.Background = new ImageBrush { ImageSource = currentCanvasImage };
		}

		private void Inversion_Click(object sender, RoutedEventArgs e)
		{
			var tmp = currentCanvasImage.ToBitmap();
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
			currentCanvasImage = currentCanvasImage.EncodeText(text);
			MainCanvas.Background = new ImageBrush { ImageSource = currentCanvasImage };
		}

		public string DecodeText()
		{
			return currentCanvasImage.DecodeText();
		}

		private void Out_item_Click(object sender, RoutedEventArgs e)
		{
			DecodeTextWindow controlPane = new DecodeTextWindow(this);
			controlPane.ResizeMode = ResizeMode.NoResize;
			controlPane.ShowDialog();
		}

		private void text_bold_button_Click(object sender, RoutedEventArgs e)
		{
			Utils.BooleanTrigger(ref Utils.executor.boldText);
		}

		private void text_italic_button_Click(object sender, RoutedEventArgs e)
		{
			Utils.BooleanTrigger(ref Utils.executor.italicText);
		}

		private void text_underline_button_Click(object sender, RoutedEventArgs e)
		{
			Utils.BooleanTrigger(ref Utils.executor.underlinedText);
		}

		private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
		{
			Utils.executor.Color = ColorPicker.SelectedColor.GetValueOrDefault();
		}

		private void ThicknessChooser_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			string selectedItemText = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content as string;
			try
			{
				Utils.executor.Thickness = Double.Parse(selectedItemText.Substring(0, 1));
			}
			catch (Exception)
			{
				Utils.executor.Thickness = 1;
			}
		}

		private void canvas_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			Info_panel.CanvasSize = Utils.ComposeCanvaSizeLabelContent(MainCanvas.Width, MainCanvas.Height);
		}

		private void canvas_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.W)
			{
				Utils.executor.ZoomIn(ref CanvasScaleTransform, ref MainCanvas);
			}
			if (e.Key == Key.Q)
			{
				Utils.executor.ZoomOut(ref CanvasScaleTransform, ref MainCanvas);
			}
		}


		private void canvas_MouseEnter(object sender, MouseEventArgs e)
		{
			Mouse.OverrideCursor = Cursors.Cross;
			MainCanvas.Focus();
			Info_panel.Position = Utils.ComposePositionLabelContent(e.GetPosition(MainCanvas));
		}

		private void canvas_MouseLeave(object sender, MouseEventArgs e)
		{
			Mouse.OverrideCursor = Cursors.Arrow;
			Info_panel.Position = "Позиция курсора: ";
		}


		private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
		{
			Utils.executor.CleanShapes();
		}

		private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ButtonState == MouseButtonState.Pressed)
			{
				switch (Utils.Tool)
				{
					case Utils.Tools.Pencil:
						Utils.executor.DrawWithPencil(ref MainCanvas, e.GetPosition(MainCanvas));
						break;

					case Utils.Tools.Eraser:
						Utils.executor.Erase(ref MainCanvas, e.GetPosition(MainCanvas));
						break;

					case Utils.Tools.ColorPicker:
						ColorPicker.SelectedColor = Utils.GetPixelColor(e.GetPosition(MainCanvas), ref MainCanvas);
						break;

					case Utils.Tools.Line:
						Utils.executor.DrawWithLine(ref MainCanvas, e.GetPosition(MainCanvas));
						break;

					case Utils.Tools.Rectangle:
						Utils.executor.DrawWithRectangle(ref MainCanvas, e.GetPosition(MainCanvas));
						break;

					case Utils.Tools.Ellipse:
						Utils.executor.DrawWithEllipse(ref MainCanvas, e.GetPosition(MainCanvas));
						break;

					case Utils.Tools.Fill:
						Utils.executor.MakeFloodFill(ref MainCanvas, e.GetPosition(MainCanvas));
						break;
					case Utils.Tools.Text:
						Utils.executor.DrawText(sender, e.GetPosition(MainCanvas), ref MainCanvas, ref textBox,
							(FontPicker.SelectedItem as ComboBoxItem).Content as string,
							(TextSizePicker.SelectedItem as ComboBoxItem).Content as string);
						break;
				}
				Utils.executor.CurrentPoint = e.GetPosition(MainCanvas);
			}
		}

		private void canvas_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				switch (Utils.Tool)
				{
					case Utils.Tools.Pencil:
						{
							if (e.LeftButton == MouseButtonState.Pressed)
							{
								Utils.executor.HoldPressedPencilAndMove(e.GetPosition(MainCanvas));
							}
						}
						break;

					case Utils.Tools.Line:
						{
							if (e.LeftButton == MouseButtonState.Pressed)
							{
								Utils.executor.HoldPressedLineAndMove(e.GetPosition(MainCanvas));
							}
						}
						break;

					case Utils.Tools.Rectangle:
						{
							if (e.LeftButton == MouseButtonState.Pressed)
							{
								Utils.executor.HoldPressedRecatngleAndMove(e.GetPosition(MainCanvas));
							}
						}
						break;

					case Utils.Tools.Ellipse:
						{
							if (e.LeftButton == MouseButtonState.Pressed)
							{
								Utils.executor.HoldPressedEllipseAndMove(e.GetPosition(MainCanvas));
							}
						}
						break;

					case Utils.Tools.Eraser:
						{
							if (e.LeftButton == MouseButtonState.Pressed)
							{
								Utils.executor.HoldPressedEraserAndMove(e.GetPosition(MainCanvas));
							}
						}
						break;

					default:
						break;
				}
			}
			Info_panel.Position = (Utils.ComposePositionLabelContent(e.GetPosition(MainCanvas)));
		}

		private void PencilButton_Click(object sender, RoutedEventArgs e)
		{
			Utils.Tool = Utils.Tools.Pencil;
		}

		private void EraserButton_Click(object sender, RoutedEventArgs e)
		{
			Utils.Tool = Utils.Tools.Eraser;
		}

		private void FillButton_Click(object sender, RoutedEventArgs e)
		{
			Utils.Tool = Utils.Tools.Fill;
		}

		private void LineButton_Click(object sender, RoutedEventArgs e)
		{
			Utils.Tool = Utils.Tools.Line;
		}

		private void EllipseButton_Click(object sender, RoutedEventArgs e)
		{
			Utils.Tool = Utils.Tools.Ellipse;
		}

		private void SquareButton_Click(object sender, RoutedEventArgs e)
		{
			Utils.Tool = Utils.Tools.Rectangle;
		}

		private void TextButton_Click(object sender, RoutedEventArgs e)
		{
			Utils.Tool = Utils.Tools.Text;
		}

		private void Redo_item_Click(object sender, RoutedEventArgs e)
		{

		}

		private void Undo_item_Click(object sender, RoutedEventArgs e)
		{
			Utils.executor.Undo(ref MainCanvas);
		}

		private void Save_as_item_Click(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
			dlg.FileName = "Untitled";
			dlg.DefaultExt = ".jpg";
			dlg.Filter = "Image (.jpg)|*.jpg";

			if (dlg.ShowDialog().GetValueOrDefault())
			{
				using (FileStream fs = new FileStream(dlg.FileName, FileMode.Create))
				{
					Utils.GetBitmapFromCanvas(ref MainCanvas).Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
				}
			}
		}
	}
}
