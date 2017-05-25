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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Ink;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Win32;
using kursach;

namespace kursach.Core
{
	public class Executor
	{
		private const double MAX_SCALE = 4;
		private const double MIN_SCALE = 0.25;
		public Point CurrentPoint { get; set; }
		private Point PreviousPoint { get; set; }
		private Point StartPoint { get; set; }
		public Color Color { get; set; }
		public double Thickness { get; set; }
		private Rectangle rectangle;
		private Ellipse ellipse;
		private Line line;
		double X2, Y2, X1, Y1;
		private Polyline eraser;
		private Polyline pencil;
		private const double ScaleRate = 1.1;
		public bool boldText = false;
		public bool italianText = false;
		public bool underlinedText = false;
		public string fontFamily = "Tahoma";
		public string fontSize = "8";
		public string text = "Multiline. Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. The quick brown fox jumps over the lazy dog. War and peace. Keep going. Go on. For how long? Not long. Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";

		//private Stack<UIElement> lastAddedUiEls = new Stack<UIElement>();

		public void CleanShapes()
		{
			rectangle = null;
			ellipse = null;
			line = null;
			eraser = null;
			pencil = null;
		}

		public void DrawWithPencil(CanvasController canvasController, Point mouseCursorPoint)
		{
			this.StartPoint = mouseCursorPoint;
			this.pencil = new Polyline();
			this.pencil.Stroke = new SolidColorBrush(this.Color);
			this.pencil.StrokeThickness = this.Thickness;
			//canvasController.GetActualCanvas().Children.Add(this.pencil);
			canvasController.UpdateCanvas(this.pencil);
		}

		public void Erase(CanvasController canvasController, Point mouseCursorPoint)
		{
			this.StartPoint = mouseCursorPoint;
			this.eraser = new Polyline();
			this.eraser.Stroke = new SolidColorBrush(Colors.White);
			this.eraser.StrokeThickness = 10;
			canvasController.UpdateCanvas(this.eraser);
		}

		public void DrawWithLine(CanvasController canvasController, Point mouseCursorPoint)
		{
			line = new Line();
			line.X1 = mouseCursorPoint.X;
			line.Y1 = mouseCursorPoint.Y;
			line.X2 = line.X1 + 1;
			line.Y2 = line.Y1 + 1;
			line.StrokeThickness = this.Thickness;
			line.Stroke = new SolidColorBrush(this.Color);
			canvasController.UpdateCanvas(line);
		}

		public void DrawWithRectangle(CanvasController canvasController, Point mouseCursorPoint, bool toFill = false)
		{
			rectangle = new Rectangle();
			X1 = mouseCursorPoint.X;
			Y1 = mouseCursorPoint.Y;
			X2 = X1;
			Y2 = Y1;
			rectangle.Width = X2 - X1;
			rectangle.Height = Y2 - Y1;
			rectangle.StrokeThickness = this.Thickness;
			rectangle.Stroke = new SolidColorBrush(this.Color);
			if (toFill)
			{
				rectangle.Fill = new SolidColorBrush(this.Color);
			}
			canvasController.UpdateCanvas(rectangle);
		}

		public void DrawWithEllipse(CanvasController canvasController, Point mouseCursorPoint, bool toFill = false)
		{
			ellipse = new Ellipse();
			X1 = mouseCursorPoint.X;
			Y1 = mouseCursorPoint.Y;
			X2 = X1;
			Y2 = Y1;
			ellipse.Width = X2 - X1;
			ellipse.Height = Y2 - Y1;
			ellipse.StrokeThickness = this.Thickness;
			ellipse.Stroke = new SolidColorBrush(this.Color);
			if(toFill)
			{
				ellipse.Fill = new SolidColorBrush(this.Color);
			}
			canvasController.UpdateCanvas(ellipse);
		}

		public void HoldPressedPencilAndMove(Point mouseCursorPoint)
		{
			if (this.StartPoint != mouseCursorPoint && mouseCursorPoint != null && pencil != null)
			{
				this.pencil.Points.Add(mouseCursorPoint);
			}
		}

		public void HoldPressedLineAndMove(Point mouseCursorPoint)
		{
			if (line != null)
			{
				line.X2 = mouseCursorPoint.X;
				line.Y2 = mouseCursorPoint.Y;
			}
		}

		public void HoldPressedRecatngleAndMove(Point mouseCursorPoint)
		{
			if (rectangle != null)
			{
				X2 = mouseCursorPoint.X;
				Y2 = mouseCursorPoint.Y;
				if ((X2 - X1) > 0 && (Y2 - Y1) > 0)
					rectangle.Margin = new Thickness(X1, Y1, X2, Y2);
				else if ((X2 - X1) < 0)
					rectangle.Margin = new Thickness(X2, Y1, X1, Y2);
				else if ((Y2 - Y1) < 0)
					rectangle.Margin = new Thickness(X1, Y2, X2, Y1);
				else if ((X2 - X1) < 0 && (Y2 - Y1) < 0)
					rectangle.Margin = new Thickness(X2, Y1, X1, Y2);
				rectangle.Width = Math.Abs(X2 - X1);
				rectangle.Height = Math.Abs(Y2 - Y1);
			}
		}

		public void HoldPressedEllipseAndMove(Point mouseCursorPoint)
		{
			if (ellipse != null)
			{
				X2 = mouseCursorPoint.X;
				Y2 = mouseCursorPoint.Y;
				if ((X2 - X1) > 0 && (Y2 - Y1) > 0)
					ellipse.Margin = new Thickness(X1, Y1, X2, Y2);
				else if ((X2 - X1) < 0)
					ellipse.Margin = new Thickness(X2, Y1, X1, Y2);
				else if ((Y2 - Y1) < 0)
					ellipse.Margin = new Thickness(X1, Y2, X2, Y1);
				else if ((X2 - X1) < 0 && (Y2 - Y1) < 0)
					ellipse.Margin = new Thickness(X2, Y1, X1, Y2);
				ellipse.Width = Math.Abs(X2 - X1);
				ellipse.Height = Math.Abs(Y2 - Y1);
			}
		}

		public void HoldPressedEraserAndMove(Point mouseCursorPoint)
		{
			if (eraser != null)
			{
				if (this.StartPoint != mouseCursorPoint && mouseCursorPoint != null && this.eraser != null)
				{
					this.eraser.Points.Add(mouseCursorPoint);
				}
			}
		}

		public void ZoomIn(ref ScaleTransform canvasScale, ref Canvas canvas)
		{
			if (canvasScale.ScaleX * ScaleRate < MAX_SCALE && canvasScale.ScaleY * ScaleRate < MAX_SCALE)
			{
				canvasScale.ScaleX *= ScaleRate;
				canvasScale.ScaleY *= ScaleRate;
			}
		}

		public void ZoomOut(ref ScaleTransform canvasScale, ref Canvas canvas)
		{
			if (canvasScale.ScaleX / ScaleRate > MIN_SCALE && canvasScale.ScaleY / ScaleRate > MIN_SCALE)
			{
				canvasScale.ScaleX /= ScaleRate;
				canvasScale.ScaleY /= ScaleRate;
			}
		}

		public void DrawText(object sender, Point point, CanvasController canvasController)
		{
			TextBlock textBlock = new TextBlock();
			textBlock.Text = text;
			try
			{
				textBlock.FontSize = Double.Parse(fontSize);
			}
			catch (Exception)
			{
				textBlock.FontSize = 8;
			}
			if (boldText)
			{
				textBlock.FontWeight = FontWeights.Bold;
			}
			if (italianText)
			{
				textBlock.FontStyle = FontStyles.Italic;
			}
			if (underlinedText)
			{
				textBlock.TextDecorations = TextDecorations.Underline;
			}
			textBlock.Foreground = new SolidColorBrush(this.Color);
			Canvas.SetLeft(textBlock, point.X);
			Canvas.SetTop(textBlock, point.Y);
			canvasController.UpdateCanvas(textBlock);
		}

		public void MakeFloodFill(CanvasController canvasController, Point canvasMousePoint)
		{
			System.Drawing.Bitmap image = Utils.GetBitmapFromCanvas(canvasController.GetActualCanvas());
			int width = image.Width;
			int height = image.Height;
			System.Drawing.Color replacementColor = System.Drawing.Color.FromArgb(this.Color.A, this.Color.R, this.Color.G, this.Color.B);
			Point node = canvasMousePoint;
			System.Drawing.Color targetColor = Utils.GetPixelDrawingColor(canvasController.GetActualCanvas(), canvasMousePoint);
			int target = targetColor.ToArgb();
			if (targetColor != replacementColor)
			{
				Queue<Point> queue = new Queue<Point>();
				bool noMorePixelsLeft = false;
				do
				{
					int x = (int)node.X;
					int y = (int)node.Y;
					while (x > 0 && image.GetPixel(x - 1, y).ToArgb() == target)
					{
						x--;
					}
					bool spanUp = false;
					bool spanDown = false;
					while (x < width && image.GetPixel(x, y).ToArgb() == target)
					{
						image.SetPixel(x, y, replacementColor);
						if (!spanUp && y > 0 && image.GetPixel(x, y - 1).ToArgb() == target)
						{
							queue.Enqueue(new Point(x, y - 1));
							spanUp = true;
						}
						else if (spanUp && y > 0 && image.GetPixel(x, y - 1).ToArgb() != target)
						{
							spanUp = false;
						}
						if (!spanDown && y < height - 1 && image.GetPixel(x, y + 1).ToArgb() == target)
						{
							queue.Enqueue(new Point(x, y + 1));
							spanDown = true;
						}
						else if (spanDown && y < height - 1 && image.GetPixel(x, y + 1).ToArgb() != target)
						{
							spanDown = false;
						}
						x++;
					}
					noMorePixelsLeft = false;
					if (queue.Count > 0)
					{
						node = queue.Dequeue();
						noMorePixelsLeft = true;
					}
					else noMorePixelsLeft = false;
				} while (noMorePixelsLeft);
				Image img = new Image();
				img.Source = Utils.BitmapToImageSource(image);
				canvasController.UpdateCanvas(img);
			}
		}
	}
}
