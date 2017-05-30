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

namespace kursach.Core
{
	public static class Utils
	{
		public enum Tools { Pencil, Eraser, Magnifier, ColorPicker, Fill, Text, Line, Rectangle, FilledRectangle, Ellipse, FilledEllipse, Arrow }
		public static Tools Tool { get; set; }

		public static Executor executor = new Executor();

		public static bool PressLeftButton { get; set; }

		public static event EventHandler ChangeInstrument;
		public static event EventHandler ChangeColor;

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

		public static System.Drawing.Bitmap GetBitmapFromCanvas(Canvas canvas)
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
			var color = GetBitmapFromCanvas(canvas).GetPixel((int)point.X, (int)point.Y);
			return Color.FromArgb(color.A, color.R, color.G, color.B);
		}

		public static double GetDistance(double X1, double Y1, double X2, double Y2)
		{
			double a = X2 - X1;
			double b = Y2 - Y1;

			return Math.Sqrt(a * a + b * b);
		}

		public static System.Drawing.Color GetPixelDrawingColor(Canvas canvas, Point point)
		{
			return Utils.GetBitmapFromCanvas(canvas).GetPixel((int)point.X, (int)point.Y);
		}

		public static Canvas Clone(this Canvas source)
		{
			var newCanvas = new Canvas();
			newCanvas.Width = source.Width;
			newCanvas.Height = source.Height;
			newCanvas.Background = source.Background;
			newCanvas.Children.Clear();
			for (int i = 0; i < source.Children.Count; i++)
			{
				newCanvas.Children.Add(source.Children[i].Clone());
			}

			return newCanvas;
		}

		public static T Clone<T>(this T original)
		{
			if (original == null)
			{
				return (default(T));
			}

			string s = XamlWriter.Save(original);
			StringReader stringReader = new StringReader(s);
			XmlReader xmlReader = XmlTextReader.Create(stringReader, new XmlReaderSettings());

			return (T)XamlReader.Load(xmlReader);
		}

		public static List<T> CloneCollection<T>(this IList<T> original)
		{
			var result = new List<T>();
			foreach (T obj in original)
				result.Add(obj.Clone());

			return result;
		}
	}
}
