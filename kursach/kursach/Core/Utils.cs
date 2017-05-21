using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;

namespace Maestro.UI
{
    public static class Utils
    {
        public enum Tools { Pencil, Eraser, Magnifier, ColorPicker, Fill, Text, Line, Rectangle, Ellipse, Arrow }
        public static Tools Tool { get; set; }

        public static Executor executor = new Executor();

		public static bool PressLeftButton { get; set; }

		public static event EventHandler ChangeInstrument;
		public static event EventHandler ChangeColor;

		// Используется для изменения порядка слоев 
		public static int LayersCount { get; set; }

		// Испольузется для именования новых слоев
		public static int LayersIndexes { get; set; }

		//private static Brush _color = Brushes.Black;
		//public static Brush Color
		//{
		//	get
		//	{
		//		return _color;
		//	}
		//	set
		//	{
		//		_color = value;
		//		ChangeColor(value, null);
		//	}
		//}

		//private static Instruments _currentTool = Instruments.Arrow;
		//public static Instruments CurrentTool
		//{
		//	get
		//	{
		//		return _currentTool;
		//	}
		//	set
		//	{
		//		_currentTool = value;
		//		ChangeInstrument(value, null);
		//	}
		//}

		private static Size _brushSize;
		public static Size BrushSize
		{
			get { return _brushSize; }
			set { _brushSize = value; }
		}

		public static BitmapImage BitmapToImageSource(System.Drawing.Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        public static void BooleanTrigger(ref bool variable)
        {
            variable = !variable;
        }

        public static System.Drawing.Bitmap GetBitmapFromCanvas(ref Canvas canvas)
        {
            Transform transform = canvas.LayoutTransform;
            canvas.LayoutTransform = null;
            Size size = new Size(canvas.Width, canvas.Height);
            canvas.Measure(size);
            canvas.Arrange(new Rect(size));
            RenderTargetBitmap renderBitmap =
              new RenderTargetBitmap(
                (int)size.Width,
                (int)size.Height,
                96d,
                96d,
                PixelFormats.Pbgra32);
            renderBitmap.Render(canvas);
            System.Drawing.Bitmap result;
            using (MemoryStream outStream = new MemoryStream())
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                encoder.Save(outStream);
                result = new System.Drawing.Bitmap(outStream);
            }
            canvas.LayoutTransform = transform;
            return result;
        }

        public static Color GetPixelColor(Point point, ref Canvas canvas)
        {
            var color = GetBitmapFromCanvas(ref canvas).GetPixel((int)point.X, (int)point.Y);
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static double GetDistance(double X1, double Y1, double X2, double Y2)
        {
            double a = (double)(X2 - X1);
            double b = (double)(Y2 - Y1);

            return Math.Sqrt(a * a + b * b);
        }

        public static System.Drawing.Color GetPixelDrawingColor(ref Canvas canvas, Point point)
        {
            return Utils.GetBitmapFromCanvas(ref canvas).GetPixel((int)point.X, (int)point.Y);
        }

        public static string ComposePositionLabelContent(Point position)
        {
            return "Позиция курсора: x:" + Math.Round(position.X,2) + ", y:" + Math.Round(position.Y, 2);
        }

        public static string ComposeCanvaSizeLabelContent(double canvasWidth, double canvasHeight)
        {
            return "Размер изображения: " + canvasWidth + "x" + canvasHeight;
        }

		public static Canvas Clone(this Canvas source)
		{
			var newCanvas = new Canvas();
			newCanvas.Width = source.Width;
			newCanvas.Height = source.Height;
			newCanvas.Background = source.Background;
			newCanvas.Children.Clear();
			foreach (var child in source.Children)
			{
				newCanvas.Children.Add((UIElement)child);
			}

			return newCanvas;
		}
	}
}
