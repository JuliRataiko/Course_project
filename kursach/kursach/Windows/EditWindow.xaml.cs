using kursach.Controls;
using kursach.ImageProcessing;
using kursach.Core;
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
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;

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
		private ImageDetails originalImageDetails;
		const double ScaleRate = 1.1;
		private CanvasController CanvasController { get; set; }
		
		private static BusyWindow busyWindow;
		private Thread busyWindowThread;

		public NewWindow(ImageDetails originalImageDetails)
		{
			InitializeComponent();
			this.originalImageDetails = originalImageDetails;
			var image = new BitmapImage(new Uri(originalImageDetails.Path));
			this.originalImage = image;
			currentCanvasImage = image;
			MainCanvas.Width = image.Width;
			MainCanvas.Height = image.Height;
			CanvasController = new CanvasController(MainCanvas);
			CanvasController.UpdateCanvas(currentCanvasImage);
			InitBusyThread();
		}

		private void InitBusyThread()
		{
			busyWindowThread = new Thread(new ThreadStart(() =>
			{
				Dispatcher.Run();
			}));

			busyWindowThread.SetApartmentState(ApartmentState.STA);
			busyWindowThread.IsBackground = true;
			busyWindowThread.Start();
		}

		private void ShowBusyWindow()
		{
			Dispatcher.FromThread(busyWindowThread).Invoke(() =>
			{
				busyWindow = new BusyWindow();
				busyWindow.Show();
			});
		}

		private void CloseBusyWindow()
		{
			Dispatcher.FromThread(busyWindowThread).Invoke(() =>
			{
				if (busyWindow != null)
				{
					busyWindow.Close();
				}
			});
		}

		private void InvokeActionWithBusyIndicator(Action action)
		{
			ShowBusyWindow();
			action.Invoke();
			CloseBusyWindow();
		}

		/// <summary>
		/// Canvas accessor.
		/// </summary>
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

		private void Black_wight_Click(object sender, RoutedEventArgs e)
		{
			InvokeActionWithBusyIndicator(() =>
			{
				currentCanvasImage = Utils.GetBitmapFromCanvas(MainCanvas).ConvertToGrayscale();
				MainCanvas.Children.Clear();
				CanvasController.UpdateCanvas(currentCanvasImage);
			});
		}

		private void DiscardChangesItem_Click(object sender, RoutedEventArgs e)
		{
			InvokeActionWithBusyIndicator(() =>
			{
				currentCanvasImage = originalImage;
				MainCanvas.Children.Clear();
				MainCanvas.Width = originalImage.Width;
				MainCanvas.Height = originalImage.Height;
				CanvasController.UndoAllChanges(originalImage);
			});
		}

		private void Inversion_Click(object sender, RoutedEventArgs e)
		{
			InvokeActionWithBusyIndicator(() =>
			{
				currentCanvasImage = Utils.GetBitmapFromCanvas(MainCanvas).ReverseImage();
				MainCanvas.Children.Clear();
				CanvasController.UpdateCanvas(currentCanvasImage);
			});
		}

		private void Sepia_Click(object sender, RoutedEventArgs e)
		{
			InvokeActionWithBusyIndicator(() =>
			{
				currentCanvasImage = Utils.GetBitmapFromCanvas(MainCanvas).ChangeSepia();
				MainCanvas.Children.Clear();
				CanvasController.UpdateCanvas(currentCanvasImage);
			});
		}

		private void Contrast_Click(object sender, RoutedEventArgs e)
		{
			BrightnessContrastWindow controlPane = new BrightnessContrastWindow(this);
			tempImage = Utils.GetBitmapFromCanvas(MainCanvas).ConvertToGrayscale();
			controlPane.ResizeMode = ResizeMode.NoResize;
			controlPane.ShowDialog();
		}

		private void Illumination_item_Click(object sender, RoutedEventArgs e)
		{
			InvokeActionWithBusyIndicator(() =>
			{
				currentCanvasImage = Utils.GetBitmapFromCanvas(MainCanvas).NormalizeIllumination();
				MainCanvas.Children.Clear();
				CanvasController.UpdateCanvas(currentCanvasImage);
			});
		}

		private void Color_balance_Click(object sender, RoutedEventArgs e)
		{
			ColorBalanceControl controlPane = new ColorBalanceControl(this);
			controlPane.ResizeMode = ResizeMode.NoResize;
			controlPane.ShowDialog();
		}

		public void PrepareCanvasForFiltering()
		{
			InvokeActionWithBusyIndicator(() =>
			{
				currentCanvasImage = Utils.GetBitmapFromCanvas(MainCanvas).ToBitmapImage();
				MainCanvas.Children.Clear();
				CanvasController.UpdateCanvas(currentCanvasImage);
			});
		}

		public void ChangeBrightnessAndContrast(int brightnessValue, int contrastValue)
		{
			tempImage = currentCanvasImage.ChangeContrast(contrastValue).ChangeBrightness(brightnessValue);
			MainCanvas.Background = new ImageBrush { ImageSource = tempImage };
		}

		public void UpdateCanvasAfterFiltering()
		{
			InvokeActionWithBusyIndicator(() =>
			{
				currentCanvasImage = tempImage;
				CanvasController.UpdateCanvas(currentCanvasImage);
			});
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

		#region Stegonography

		public void EncodeText(string text)
		{
			currentCanvasImage = Utils.GetBitmapFromCanvas(MainCanvas).EncodeText(text);
			CanvasController.UpdateCanvas(currentCanvasImage);
		}

		public string DecodeText()
		{
			return currentCanvasImage.DecodeText();
		}

		#endregion

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
			if (Utils.Tool != Utils.Tools.Arrow)
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
						Utils.executor.DrawWithPencil(CanvasController, e.GetPosition(MainCanvas));
						break;

					case Utils.Tools.Eraser:
						Utils.executor.Erase(CanvasController, e.GetPosition(MainCanvas));
						break;

					case Utils.Tools.ColorPicker:
						ColorPicker.SelectedColor = Utils.GetPixelColor(e.GetPosition(MainCanvas), ref MainCanvas);
						break;

					case Utils.Tools.Line:
						Utils.executor.DrawWithLine(CanvasController, e.GetPosition(MainCanvas));
						break;

					case Utils.Tools.Rectangle:
						Utils.executor.DrawWithRectangle(CanvasController, e.GetPosition(MainCanvas));
						break;
					case Utils.Tools.FilledRectangle:
						Utils.executor.DrawWithRectangle(CanvasController, e.GetPosition(MainCanvas), true);
						break;
					case Utils.Tools.FilledEllipse:
						Utils.executor.DrawWithEllipse(CanvasController, e.GetPosition(MainCanvas), true);
						break;
					case Utils.Tools.Ellipse:
						Utils.executor.DrawWithEllipse(CanvasController, e.GetPosition(MainCanvas));
						break;

					case Utils.Tools.Fill:
						InvokeActionWithBusyIndicator(() =>
						{
							Utils.executor.MakeFloodFill(CanvasController, e.GetPosition(MainCanvas));
						});
						break;
					case Utils.Tools.Text:
						Utils.executor.DrawText(sender, e.GetPosition(MainCanvas), CanvasController);
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

					case Utils.Tools.FilledRectangle:
						{
							if (e.LeftButton == MouseButtonState.Pressed)
							{
								Utils.executor.HoldPressedRecatngleAndMove(e.GetPosition(MainCanvas));
							}
						}
						break;

					case Utils.Tools.FilledEllipse:
						{
							if (e.LeftButton == MouseButtonState.Pressed)
							{
								Utils.executor.HoldPressedEllipseAndMove(e.GetPosition(MainCanvas));
							}
						}
						break;

					default:
						break;
				}
			}
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

		private void FilledSquareButton_Click(object sender, RoutedEventArgs e)
		{
			Utils.Tool = Utils.Tools.FilledRectangle;
		}

		private void FilledEllipseButton_Click(object sender, RoutedEventArgs e)
		{
			Utils.Tool = Utils.Tools.FilledEllipse;
		}

		private void TextButton_Click(object sender, RoutedEventArgs e)
		{
			Utils.Tool = Utils.Tools.Text;
			var window = new SettingsWindow();
			window.ShowDialog();
		}

		private void ArrowButton_Click(object sender, RoutedEventArgs e)
		{
			Utils.Tool = Utils.Tools.Arrow;
		}

		#endregion

		private void RedoItem_Click(object sender, RoutedEventArgs e)
		{
			InvokeActionWithBusyIndicator(() =>
			{
				CanvasController.RedoChanges();
				currentCanvasImage = Utils.GetBitmapFromCanvas(MainCanvas).ToBitmapImage();
			});
		}

		private void UndoItem_Click(object sender, RoutedEventArgs e)
		{
			InvokeActionWithBusyIndicator(() =>
			{
				CanvasController.UndoChanges();
				currentCanvasImage = Utils.GetBitmapFromCanvas(MainCanvas).ToBitmapImage();
			});
		}

		private void Save_as_item_Click(object sender, RoutedEventArgs e)
		{
			SaveFile();
		}

		private bool SaveFile()
		{
			Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
			dlg.FileName = "Untitled";
			dlg.DefaultExt = ".jpg";
			dlg.Filter = dlg.Filter = "Bitmap Image (.bmp)|*.bmp|Gif Image (.gif)|*.gif|JPEG Image (.jpeg)|*.jpeg|Png Image (.png)|*.png|Tiff Image (.tiff)|*.tiff";

			if (dlg.ShowDialog().GetValueOrDefault())
			{
				using (FileStream fs = new FileStream(dlg.FileName, FileMode.Create))
				{
					try
					{
						Utils.GetBitmapFromCanvas(MainCanvas).Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
						MessageBox.Show("Сохранено.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
						CanvasController.UndoAllChanges(new BitmapImage(new Uri(originalImageDetails.Path)));
					}
					catch (Exception)
					{
						MessageBox.Show("Не удалось сохранить файл, попробуйте еще раз.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
					}
				}
			}
			else
			{
				return false;
			}

			return true;
		}

		private void RotateToLeftItem_Click(object sender, RoutedEventArgs e)
		{
			InvokeActionWithBusyIndicator(() =>
			{
				var canvas = Utils.GetBitmapFromCanvas(MainCanvas);
				canvas.RotateFlip(RotateFlipType.Rotate270FlipNone);
				currentCanvasImage = canvas.ToBitmapImage();
				MainCanvas.Width = canvas.Width;
				MainCanvas.Height = canvas.Height;
				CanvasController.UpdateCanvas(currentCanvasImage);
			});
		}

		private void RotateToRightItem_Click(object sender, RoutedEventArgs e)
		{
			InvokeActionWithBusyIndicator(() =>
			{
				var canvas = Utils.GetBitmapFromCanvas(MainCanvas);
				canvas.RotateFlip(RotateFlipType.Rotate90FlipNone);
				currentCanvasImage = canvas.ToBitmapImage();
				MainCanvas.Width = canvas.Width;
				MainCanvas.Height = canvas.Height;
				CanvasController.UpdateCanvas(currentCanvasImage);
			});
		}

		private void BlurItem_Click(object sender, RoutedEventArgs e)
		{
			InvokeActionWithBusyIndicator(() =>
			{
				currentCanvasImage = Utils.GetBitmapFromCanvas(MainCanvas).ApplyBlur();
				MainCanvas.Children.Clear();
				CanvasController.UpdateCanvas(currentCanvasImage);
			});
		}

		private void Quit_item_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (CanvasController.GetNumberOfChanges() > 1)
			{
				MessageBoxResult messageBoxResult = MessageBox.Show("Есть несохраненные изменения. Сохранить изменения?", "Изменения", MessageBoxButton.YesNoCancel);
				if (messageBoxResult == MessageBoxResult.Yes)
				{
					e.Cancel = !SaveFile();
				}
				if (messageBoxResult == MessageBoxResult.Cancel)
				{
					e.Cancel = true;
				}
			}
		}

		private void SharpenItem_Click(object sender, RoutedEventArgs e)
		{
			InvokeActionWithBusyIndicator(() =>
			{
				currentCanvasImage = Utils.GetBitmapFromCanvas(MainCanvas).Sharpen();
				MainCanvas.Children.Clear();
				CanvasController.UpdateCanvas(currentCanvasImage);
			});
		}
	}
}
