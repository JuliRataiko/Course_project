using kursach.Controls;
using kursach.ImageProcessing;
using Maestro.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
		private CanvasController CanvasController { get; set; }
		private LinkedList<Canvas> CanvasStates { get; set; }
		private BitmapSource ImageBeforeFiltering { get; set; }

		public NewWindow(BitmapImage image)
		{
			InitializeComponent();
			this.originalImage = image;
			currentCanvasImage = image;

			MainCanvas.Width = image.Width;
			MainCanvas.Height = image.Height;

			//CanvasController.UpdateCanvas(currentCanvasImage);

			CanvasController = new CanvasController(this);
			CanvasController.UpdateCanvas(currentCanvasImage);
		}

		public Canvas Canvas
		{
			get
			{
				return MainCanvas;
			}
			set
			{
				MainCanvas = value;
			}
		}



		//private void CanvasScroll_MouseWheel(object sender, MouseWheelEventArgs e)
		//{
		//	e.Handled = true;
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

		private void Black_wight_Click(object sender, RoutedEventArgs e)
		{
			currentCanvasImage = Utils.GetBitmapFromCanvas(ref MainCanvas).ConvertToGrayscale();
			MainCanvas.Children.Clear();
			CanvasController.UpdateCanvas(currentCanvasImage);
		}

		private void DiscardChangesItem_Click(object sender, RoutedEventArgs e)
		{
			currentCanvasImage = originalImage;
			MainCanvas.Children.Clear();

			//MainCanvas.Width = originalImage.Width;
			//MainCanvas.Height = originalImage.Height;

			CanvasController.UpdateCanvas(currentCanvasImage);
		}

		private void Inversion_Click(object sender, RoutedEventArgs e)
		{
			currentCanvasImage = Utils.GetBitmapFromCanvas(ref MainCanvas).ReverseImage();
			MainCanvas.Children.Clear();
			CanvasController.UpdateCanvas(currentCanvasImage);
		}

		private void Sepia_Click(object sender, RoutedEventArgs e)
		{
			currentCanvasImage = Utils.GetBitmapFromCanvas(ref MainCanvas).ChangeSepia();
			MainCanvas.Children.Clear();
			CanvasController.UpdateCanvas(currentCanvasImage);
		}

		private void Contrast_Click(object sender, RoutedEventArgs e)
		{
			BrightnessContrastWindow controlPane = new BrightnessContrastWindow(this);
			tempImage = Utils.GetBitmapFromCanvas(ref MainCanvas).ConvertToGrayscale();
			controlPane.ResizeMode = ResizeMode.NoResize;
			controlPane.ShowDialog();
		}

		private void Illumination_item_Click(object sender, RoutedEventArgs e)
		{
			currentCanvasImage = Utils.GetBitmapFromCanvas(ref MainCanvas).NormalizeIllumination();
			MainCanvas.Children.Clear();
			CanvasController.UpdateCanvas(currentCanvasImage);
		}

		private void Color_balance_Click(object sender, RoutedEventArgs e)
		{
			ColorBalanceControl controlPane = new ColorBalanceControl(this);
			controlPane.ResizeMode = ResizeMode.NoResize;
			controlPane.ShowDialog();
		}

		//public void ChangeBrightness(int brightnessValue)
		//{
		//	tempImage = currentCanvasImage.ChangeBrightness(brightnessValue);
		//	CanvasController.UpdateCanvas(tempImage);
		//}

		//public void ChangeContrast(int contrastValue)
		//{
		//	tempImage = currentCanvasImage.ChangeContrast(contrastValue);
		//	CanvasController.UpdateCanvas(tempImage);
		//}

		public void ChangeBrightnessAndContrast(int brightnessValue, int contrastValue)
		{
			tempImage = currentCanvasImage.ChangeContrast(contrastValue).ChangeBrightness(brightnessValue);
			MainCanvas.Background = new ImageBrush { ImageSource = tempImage };
		}

		public void UpdateCanvasAfterFiltering()
		{
			currentCanvasImage = tempImage;
			CanvasController.UpdateCanvas(currentCanvasImage);
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
			currentCanvasImage = Utils.GetBitmapFromCanvas(ref MainCanvas).EncodeText(text);
			CanvasController.UpdateCanvas(currentCanvasImage);
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

		#region Canvas events

		private void canvas_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			//Info_panel.CanvasSize = Utils.ComposeCanvaSizeLabelContent(MainCanvas.Width, MainCanvas.Height);
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
			//Info_panel.Position = Utils.ComposePositionLabelContent(e.GetPosition(MainCanvas));
		}

		private void canvas_MouseLeave(object sender, MouseEventArgs e)
		{
			Mouse.OverrideCursor = Cursors.Arrow;
			//Info_panel.Position = "Позиция курсора: ";
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
						Utils.executor.DrawText(sender, e.GetPosition(MainCanvas), ref MainCanvas);
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

			//Info_panel.Position = (Utils.ComposePositionLabelContent(e.GetPosition(MainCanvas)));
		}

		#endregion

		#region Toolbar events

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

		private void ArrowButton_Click(object sender, RoutedEventArgs e)
		{
			Utils.Tool = Utils.Tools.Arrow;
		}

		#endregion
		
		private void RedoItem_Click(object sender, RoutedEventArgs e)
		{
			CanvasController.RedoChanges();
			currentCanvasImage = Utils.GetBitmapFromCanvas(ref MainCanvas).ToBitmapImage();
		}

		private void UndoItem_Click(object sender, RoutedEventArgs e)
		{
			CanvasController.UndoChanges();
			currentCanvasImage = Utils.GetBitmapFromCanvas(ref MainCanvas).ToBitmapImage();
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

		private void RotateToLeftItem_Click(object sender, RoutedEventArgs e)
		{
			var canvas = Utils.GetBitmapFromCanvas(ref MainCanvas);
			canvas.RotateFlip(RotateFlipType.Rotate270FlipNone);
			currentCanvasImage = canvas.ToBitmapImage();
			MainCanvas.Width = canvas.Width;
			MainCanvas.Height = canvas.Height;
			CanvasController.UpdateCanvas(currentCanvasImage);
		}

		private void RotateToRightItem_Click(object sender, RoutedEventArgs e)
		{
			var canvas = Utils.GetBitmapFromCanvas(ref MainCanvas);
			canvas.RotateFlip(RotateFlipType.Rotate90FlipNone);
			currentCanvasImage = canvas.ToBitmapImage();
			MainCanvas.Width = canvas.Width;
			MainCanvas.Height = canvas.Height;
			CanvasController.UpdateCanvas(currentCanvasImage);
		}

		private void SettingsItem_Click(object sender, RoutedEventArgs e)
		{
			var window = new SettingsWindow();
			window.ShowDialog();
		}
	}
}
