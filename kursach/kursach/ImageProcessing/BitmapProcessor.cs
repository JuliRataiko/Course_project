using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;

namespace kursach.ImageProcessing
{
	public static class BitmapProcessor
	{
		public static BitmapSource ConvertToGrayscale(this BitmapSource source)
		{
			FormatConvertedBitmap newFormatedBitmapSource = new FormatConvertedBitmap();

			newFormatedBitmapSource.BeginInit();
			newFormatedBitmapSource.Source = source;
			newFormatedBitmapSource.DestinationFormat = PixelFormats.Gray32Float;
			newFormatedBitmapSource.EndInit();

			return newFormatedBitmapSource;
		}

		public static unsafe BitmapImage ReverseImage(this Bitmap source)
		{
			for (int y = 0; (y <= (source.Height - 1)); y++)
			{
				for (int x = 0; (x <= (source.Width - 1)); x++)
				{
					System.Drawing.Color inv = source.GetPixel(x, y);
					inv = System.Drawing.Color.FromArgb(255, (255 - inv.R), (255 - inv.G), (255 - inv.B));
					source.SetPixel(x, y, inv);
				}
			}

			//Bitmap tmp = (Bitmap)source.Clone();
			//BitmapData data = tmp.LockBits(
			//	new Rectangle(0, 0, tmp.Width, tmp.Height),
			//	ImageLockMode.ReadWrite,
			//	tmp.PixelFormat);
			//int Height = tmp.Height;
			//int Width = tmp.Width;


			//int* bytes = (int*)data.Scan0;
			//for (int i = Width * Height - 1; i >= 0; i--)
			//	bytes[i] = ~bytes[i];
			//source.UnlockBits(data);

			return source.ToBitmapImage();
		}

		public static Bitmap ConvertToBitmap(this BitmapImage image)
		{
			using (MemoryStream outStream = new MemoryStream())
			{
				BitmapEncoder enc = new BmpBitmapEncoder();
				enc.Frames.Add(BitmapFrame.Create(image));
				enc.Save(outStream);
				Bitmap bitmap = new Bitmap(outStream);

				return new Bitmap(bitmap);
			}
		}
		public static Bitmap ConvertToBitmap(this BitmapSource source)
		{
			Bitmap bmp = new Bitmap(source.PixelWidth, source.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
			BitmapData data = bmp.LockBits(
				new Rectangle(0, 0, bmp.Width, bmp.Height),
				ImageLockMode.ReadWrite,
				bmp.PixelFormat);
			int Height = bmp.Height;
			int Width = bmp.Width;

			source.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
			bmp.UnlockBits(data);

			return bmp;
		}

		public static BitmapImage ToBitmapImage(this Bitmap bitmap)
		{
			BitmapImage bitmapImage = new BitmapImage();
			using (MemoryStream memory = new MemoryStream())
			{
				bitmap.Save(memory, ImageFormat.Png);
				memory.Position = 0;
				bitmapImage.BeginInit();
				bitmapImage.StreamSource = memory;
				bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
				bitmapImage.EndInit();
			}

			return bitmapImage;
		}

		public static BitmapImage ConvertToBitmapImage(this BitmapSource source)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				JpegBitmapEncoder encoder = new JpegBitmapEncoder();
				encoder.Frames.Add(BitmapFrame.Create(source));
				encoder.Save(memoryStream);

				memoryStream.Position = 0;
				BitmapImage bImg = new BitmapImage();
				bImg.BeginInit();
				bImg.StreamSource = memoryStream;
				bImg.EndInit();

				memoryStream.Close();

				return bImg;
			}
		}

		public static unsafe BitmapImage ChangeSepia(this BitmapSource source)
		{
			Bitmap bmp = source.ConvertToBitmap();
			BitmapData bData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
			byte* scan0 = (byte*)bData.Scan0.ToPointer(); //Pointer to first byte of image
			byte Tone;
			for (int i = 0; i < bData.Height; ++i)
				for (int j = 0; j < bData.Width; ++j)
				{
					byte* data = scan0 + i * bData.Stride + j * 3;
					Tone = (byte)(0.299 * (*(data + 2)) + 0.587 * (*(data + 1)) + 0.114 * (*data));
					*(data + 2) = (byte)((Tone > 206) ? 255 : Tone + 49); //R  
					*(data + 1) = (byte)((Tone < 14) ? 0 : Tone - 14); //G  
					*(data + 0) = (byte)((Tone < 56) ? 0 : Tone - 56); //B  
				}
			bmp.UnlockBits(bData);

			return new Bitmap(bmp).ToBitmapImage();
		}

		public static unsafe BitmapImage ChangeBrightness(this BitmapSource source, int newBrightness)
		{
			Bitmap bmp = source.ConvertToBitmap();
			int clamp_color(int value) => value > 255 ? 255 : value < 0 ? 0 : value;

			BitmapData bData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
			byte* scan0 = (byte*)bData.Scan0.ToPointer(); //Pointer to first byte of image
			for (int i = 0; i < bData.Height; ++i)
				for (int j = 0; j < bData.Width; ++j)
				{
					byte* data = scan0 + i * bData.Stride + j * 4;

					*data = (byte)clamp_color(*data + newBrightness); // For R
					*(data + 1) = (byte)clamp_color(*(data + 1) + newBrightness); // For G
					*(data + 2) = (byte)clamp_color(*(data + 2) + newBrightness); // For B
				}
			bmp.UnlockBits(bData);

			return bmp.ToBitmapImage();
		}

		public static unsafe BitmapImage ChangeContrast(this BitmapSource source, int contrastValue)
		{
			Bitmap bmp = source.ConvertToBitmap();

			const float OneOver255 = 1.0f / 255.0f;

			int clamp_color(double value) => value > 255.0 ? 255 : value < 0.0 ? 0 : (int)value;
			double change_pixel(byte data, double c) => ((data * OneOver255 - 0.5) * c + 0.5) * 255;

			if (contrastValue < -100 || contrastValue > 100) return source.ConvertToBitmapImage();

			BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
			byte* scan0 = (byte*)bmpData.Scan0.ToPointer();
			//int stopAddress = (int)ptr + bmpData.Stride * bmpData.Height;
			double contrast = (100.0 + contrastValue) / 100.0;
			contrast *= contrast;
			for (int i = 0; i < bmpData.Height; ++i)
				for (int j = 0; j < bmpData.Width; ++j)
				{
					byte* data = scan0 + i * bmpData.Stride + j * 3;
					data[0] = (byte)clamp_color(change_pixel(data[0], contrast));
					data[1] = (byte)clamp_color(change_pixel(data[1], contrast));
					data[2] = (byte)clamp_color(change_pixel(data[2], contrast));
				}
			bmp.UnlockBits(bmpData);

			return bmp.ToBitmapImage();
		}

		public static unsafe BitmapImage ChangeColorBalance(this BitmapSource source, int red, int green, int blue)
		{
			int clamp_color(int value) => value > 255 ? 255 : value < 0 ? 0 : value;
			Bitmap bmp = source.ConvertToBitmap();

			BitmapData bData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
			byte* scan0 = (byte*)bData.Scan0.ToPointer();
			for (int i = 0; i < bData.Height; ++i)
				for (int j = 0; j < bData.Width; ++j)
				{
					byte* data = scan0 + i * bData.Stride + j * 4;

					*data = (byte)clamp_color(*data + blue);
					*(data + 1) = (byte)clamp_color(*(data + 1) + green);
					*(data + 2) = (byte)clamp_color(*(data + 2) + red);
				}

			bmp.UnlockBits(bData);

			return bmp.ToBitmapImage();
		}

		public static unsafe void TextToImage(this BitmapSource source, string text)
		{
			Bitmap bmp = source.ConvertToBitmap();
			if (text.Length * 4 > bmp.Height * bmp.Width) return;
			int i_for_string = 0;
			bmp.SetPixel(0, 0, Color.FromArgb(text.Length));
			for (int i = 0; i < bmp.Height; i++)
				for (int j = 1; j < bmp.Width; j++)
				{
					Color c = bmp.GetPixel(i, j);
					byte r = c.R,
						 g = c.G,
						 b = c.B,
						 a = c.A;
					char symbol = text[i_for_string++];
					r &= 0b1111_1100; g &= 0b1111_1100; b &= 0b1111_1100; a &= 0b1111_1100;
					byte new_r = (byte)((symbol & 0b1100_0000) >> 6),
						new_g = (byte)((symbol & 0b0011_0000) >> 4),
						new_b = (byte)((symbol & 0b0000_1100) >> 2),
						new_a = (byte)(symbol & 0b0000_0011);
					bmp.SetPixel(i, j, Color.FromArgb(a | new_a, r | new_r, g | new_g, b | new_b));
					if (i_for_string >= text.Length) return;
				}

			source = bmp.ToBitmapImage();
		}

		public static string GetTextFromImage(this BitmapSource source)
		{
			Bitmap bmp = source.ConvertToBitmap();
			string rc = "";
			int length = bmp.GetPixel(0, 0).ToArgb();
			for (int i = 0; i < bmp.Height; i++)
				for (int j = 1; j < bmp.Width; j++)
				{
					Color c = bmp.GetPixel(i, j);
					byte r = (byte)((c.R & 0b11) << 6),
						 g = (byte)((c.G & 0b11) << 4),
						 b = (byte)((c.B & 0b11) << 2),
						 a = (byte)(c.A & 0b11);
					rc += (char)(r | g | b | a);
					if (rc.Length == length) return rc;

				}

			return rc;
		}
	}
}
