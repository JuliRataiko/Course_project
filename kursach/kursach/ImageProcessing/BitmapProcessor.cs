using System;
using System.Collections;
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

		private static int ClampColor(double value)
		{
			return value > 255.0 ? 255 : value < 0.0 ? 0 : (int)value;
		}

		private static int ClampColor(int value)
		{
			return value > 255 ? 255 : value < 0 ? 0 : value;
		}

		private static double ChangePixel(byte data, double c)
		{
			const float oneOver255 = 1.0f / 255.0f;
			return ((data * oneOver255 - 0.5) * c + 0.5) * 255;
		}


		public static BitmapSource ConvertToGrayscale(this BitmapSource source)
		{
			var newFormatedBitmapSource = new FormatConvertedBitmap();

			newFormatedBitmapSource.BeginInit();
			newFormatedBitmapSource.Source = source;
			newFormatedBitmapSource.DestinationFormat = PixelFormats.Gray32Float;
			newFormatedBitmapSource.EndInit();

			return newFormatedBitmapSource;
		}

		public static unsafe BitmapImage ReverseImage(this Bitmap source)
		{
			for (var y = 0; y <= source.Height - 1; y++)
			{
				for (var x = 0; x <= source.Width - 1; x++)
				{
					var inv = source.GetPixel(x, y);
					inv = Color.FromArgb(255, 255 - inv.R, 255 - inv.G, 255 - inv.B);
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
			using (var outStream = new MemoryStream())
			{
				BitmapEncoder enc = new BmpBitmapEncoder();
				enc.Frames.Add(BitmapFrame.Create(image));
				enc.Save(outStream);
				var bitmap = new Bitmap(outStream);

				return new Bitmap(bitmap);
			}
		}

		public static Bitmap ToBitmap(this BitmapSource source)
		{
			var bmp = new Bitmap(source.PixelWidth, source.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
			var data = bmp.LockBits(
				new Rectangle(0, 0, bmp.Width, bmp.Height),
				ImageLockMode.ReadWrite,
				bmp.PixelFormat);

			source.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
			bmp.UnlockBits(data);

			return bmp;
		}

		public static BitmapImage ToBitmapImage(this Bitmap bitmap)
		{
			var bitmapImage = new BitmapImage();
			using (var memory = new MemoryStream())
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
			using (var memoryStream = new MemoryStream())
			{
				var encoder = new JpegBitmapEncoder();
				encoder.Frames.Add(BitmapFrame.Create(source));
				encoder.Save(memoryStream);

				memoryStream.Position = 0;
				var bImg = new BitmapImage();
				bImg.BeginInit();
				bImg.StreamSource = memoryStream;
				bImg.EndInit();

				memoryStream.Close();

				return bImg;
			}
		}

		public static unsafe BitmapImage ChangeSepia(this BitmapSource source)
		{
			var bmp = source.ToBitmap();
			var bData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
			var scan0 = (byte*)bData.Scan0.ToPointer();

			for (var i = 0; i < bData.Height; ++i)
				for (var j = 0; j < bData.Width; ++j)
				{
					var data = scan0 + i * bData.Stride + j * 3;
					var tone = (byte)(0.299 * (*(data + 2)) + 0.587 * (*(data + 1)) + 0.114 * (*data));
					*(data + 2) = (byte)((tone > 206) ? 255 : tone + 49);
					*(data + 1) = (byte)((tone < 14) ? 0 : tone - 14);
					*(data + 0) = (byte)((tone < 56) ? 0 : tone - 56);
				}
			bmp.UnlockBits(bData);

			return new Bitmap(bmp).ToBitmapImage();
		}

		public static unsafe BitmapImage ChangeBrightness(this BitmapSource source, int newBrightness)
		{
			var bmp = source.ToBitmap();
			var bData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
			var scan0 = (byte*)bData.Scan0.ToPointer();
			for (var i = 0; i < bData.Height; ++i)
				for (var j = 0; j < bData.Width; ++j)
				{
					var data = scan0 + i * bData.Stride + j * 4;

					*data = (byte)ClampColor(*data + newBrightness);
					*(data + 1) = (byte)ClampColor(*(data + 1) + newBrightness);
					*(data + 2) = (byte)ClampColor(*(data + 2) + newBrightness);
				}
			bmp.UnlockBits(bData);

			return bmp.ToBitmapImage();
		}

		public static unsafe BitmapImage ChangeContrast(this BitmapSource source, int contrastValue)
		{
			var bmp = source.ToBitmap();

			if (contrastValue < -100 || contrastValue > 100) return source.ConvertToBitmapImage();

			var bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
			var scan0 = (byte*)bmpData.Scan0.ToPointer();
			var contrast = (100.0 + contrastValue) / 100.0;
			contrast *= contrast;
			for (var i = 0; i < bmpData.Height; ++i)
				for (var j = 0; j < bmpData.Width; ++j)
				{
					var data = scan0 + i * bmpData.Stride + j * 3;
					data[0] = (byte)ClampColor(ChangePixel(data[0], contrast));
					data[1] = (byte)ClampColor(ChangePixel(data[1], contrast));
					data[2] = (byte)ClampColor(ChangePixel(data[2], contrast));
				}
			bmp.UnlockBits(bmpData);

			return bmp.ToBitmapImage();
		}

		public static unsafe BitmapImage ChangeColorBalance(this BitmapSource source, int red, int green, int blue)
		{

			var bmp = source.ToBitmap();
			var bData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
			var scan0 = (byte*)bData.Scan0.ToPointer();
			for (var i = 0; i < bData.Height; ++i)
				for (var j = 0; j < bData.Width; ++j)
				{
					var data = scan0 + i * bData.Stride + j * 4;

					*data = (byte)ClampColor(*data + blue);
					*(data + 1) = (byte)ClampColor(*(data + 1) + green);
					*(data + 2) = (byte)ClampColor(*(data + 2) + red);
				}

			bmp.UnlockBits(bData);

			return bmp.ToBitmapImage();
		}

		public static BitmapSource TextToImage(this BitmapSource source, string text)
		{
			var bmp = source.ToBitmap();
			if (text.Length * 4 > bmp.Height * bmp.Width)
			{
				return source;
			}

			var index = 0;
			bmp.SetPixel(0, 0, Color.FromArgb(text.Length));

			for (var i = 0; i < bmp.Width; i++)
				for (var j = 0; j < bmp.Height; j++)
				{
					var pixel = bmp.GetPixel(i, j);
					byte red = pixel.R,
						 green = pixel.G,
						 blue = pixel.B,
						 alpha = pixel.A;
					var symbol = text[index++];
					red &= (byte)Convert.ToInt32("11111100", 2); green &= (byte)Convert.ToInt32("11111100", 2); blue &= (byte)Convert.ToInt32("11111100", 2); alpha &= (byte)Convert.ToInt32("11111100", 2);
					byte newRed = (byte)((symbol & (byte)Convert.ToInt32("11000000", 2)) >> 6), newGreen = (byte)(symbol & (byte)Convert.ToInt32("00110000", 2) >> 4), newBlue = (byte)(symbol & (byte)Convert.ToInt32("00001100") >> 2), newAlpha = (byte)(symbol & (byte)Convert.ToInt32("00000011", 2));
					bmp.SetPixel(i, j, Color.FromArgb(alpha | newAlpha, red | newRed, green | newGreen, blue | newBlue));

					if (index >= text.Length)
					{
						return bmp.ToBitmapImage();
					}
				}

			return bmp.ToBitmapImage();
		}

		public static string GetTextFromImage(this BitmapSource source)
		{
			var bmp = source.ToBitmap();
			var recognizedText = string.Empty;
			var imageLength = bmp.GetPixel(0, 0).ToArgb();

			for (var i = 0; i < bmp.Width; i++)
				for (var j = 0; j < bmp.Height; j++)
				{
					var pixel = bmp.GetPixel(i, j);
					byte r = (byte)((pixel.R & (byte)Convert.ToInt32("11", 2)) << 6),
					 g = (byte)(pixel.G & (byte)Convert.ToInt32("11", 2) << 4),
					 b = (byte)(pixel.B & (byte)Convert.ToInt32("11", 2) << 2),
					 a = (byte)(pixel.A & (byte)Convert.ToInt32("11", 2));
					recognizedText += (char)(r | g | b | a);

					if (recognizedText.Length == imageLength)
					{
						return recognizedText;
					}
				}

			return recognizedText;
		}

		public static BitmapSource EncodeText(this BitmapSource source, string text)
		{
			var steganography = new Steganography();
			var bPic = source.ToBitmap();
			byte[] bytes = Encoding.ASCII.GetBytes(text);
			int CountText = bytes.Length;

			if (CountText > ((bPic.Width * bPic.Height)) - 4)
			{
				return source;
			}

			if (steganography.isEncryption(bPic))
			{
				return source;
			}

			byte[] Symbol = Encoding.GetEncoding(1251).GetBytes("/");
			BitArray ArrBeginSymbol = steganography.ByteToBit(Symbol[0]);
			Color curColor = bPic.GetPixel(0, 0);
			BitArray tempArray = steganography.ByteToBit(curColor.R);
			tempArray[0] = ArrBeginSymbol[0];
			tempArray[1] = ArrBeginSymbol[1];
			byte nR = steganography.BitToByte(tempArray);

			tempArray = steganography.ByteToBit(curColor.G);
			tempArray[0] = ArrBeginSymbol[2];
			tempArray[1] = ArrBeginSymbol[3];
			tempArray[2] = ArrBeginSymbol[4];
			byte nG = steganography.BitToByte(tempArray);

			tempArray = steganography.ByteToBit(curColor.B);
			tempArray[0] = ArrBeginSymbol[5];
			tempArray[1] = ArrBeginSymbol[6];
			tempArray[2] = ArrBeginSymbol[7];
			byte nB = steganography.BitToByte(tempArray);

			Color nColor = Color.FromArgb(nR, nG, nB);
			bPic.SetPixel(0, 0, nColor);

			steganography.WriteCountText(CountText, bPic);

			int index = 0;
			bool st = false;
			for (int i = 4; i < bPic.Width; i++)
			{
				for (int j = 0; j < bPic.Height; j++)
				{
					Color pixelColor = bPic.GetPixel(i, j);
					if (index == bytes.Length)
					{
						st = true;
						break;
					}
					BitArray colorArray = steganography.ByteToBit(pixelColor.R);
					BitArray messageArray = steganography.ByteToBit(bytes[index]);
					colorArray[0] = messageArray[0];
					colorArray[1] = messageArray[1];
					byte newR = steganography.BitToByte(colorArray);

					colorArray = steganography.ByteToBit(pixelColor.G);
					colorArray[0] = messageArray[2];
					colorArray[1] = messageArray[3];
					colorArray[2] = messageArray[4];
					byte newG = steganography.BitToByte(colorArray);

					colorArray = steganography.ByteToBit(pixelColor.B);
					colorArray[0] = messageArray[5];
					colorArray[1] = messageArray[6];
					colorArray[2] = messageArray[7];
					byte newB = steganography.BitToByte(colorArray);

					Color newColor = Color.FromArgb(newR, newG, newB);
					bPic.SetPixel(i, j, newColor);
					index++;
				}
				if (st)
				{
					break;
				}
			}

			return bPic.ToBitmapImage();
		}

		public static string DecodeText(this BitmapSource source)
		{
			var bPic = source.ToBitmap();
			var steganography = new Steganography();

			if (!steganography.isEncryption(bPic))
			{
				return string.Empty;
			}

			int countSymbol = steganography.ReadCountText(bPic);
			byte[] message = new byte[countSymbol];
			int index = 0;
			bool st = false;
			for (int i = 4; i < bPic.Width; i++)
			{
				for (int j = 0; j < bPic.Height; j++)
				{
					Color pixelColor = bPic.GetPixel(i, j);
					if (index == message.Length)
					{
						st = true;
						break;
					}
					BitArray colorArray = steganography.ByteToBit(pixelColor.R);
					BitArray messageArray = steganography.ByteToBit(pixelColor.R); ;
					messageArray[0] = colorArray[0];
					messageArray[1] = colorArray[1];

					colorArray = steganography.ByteToBit(pixelColor.G);
					messageArray[2] = colorArray[0];
					messageArray[3] = colorArray[1];
					messageArray[4] = colorArray[2];

					colorArray = steganography.ByteToBit(pixelColor.B);
					messageArray[5] = colorArray[0];
					messageArray[6] = colorArray[1];
					messageArray[7] = colorArray[2];
					message[index] = steganography.BitToByte(messageArray);
					index++;
				}
				if (st)
				{
					break;
				}
			}

			return Encoding.GetEncoding(1251).GetString(message);
		}

		public static unsafe BitmapImage NormalizeIllumination(this BitmapSource source)
		{
			var bmp = source.ToBitmap();

			BitmapData bData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
			var scan0 = (byte*)bData.Scan0.ToPointer(); //Pointer to first byte of image
			int r = 0, g = 0, b = 0, avg = 0;
			for (int i = 0; i < bData.Height; ++i)
				for (int j = 0; j < bData.Width; ++j)
				{
					byte* data = scan0 + i * bData.Stride + j * 3;
					r += *(data + 2);
					g += *(data + 1);
					b += *data;
				}
			r /= bData.Height * bData.Width;
			g /= bData.Height * bData.Width;
			b /= bData.Height * bData.Width;
			avg = ((r + g + b) / 3);
			for (int i = 0; i < bData.Height; ++i)
				for (int j = 0; j < bData.Width; ++j)
				{
					byte* data = scan0 + i * bData.Stride + j * 3;
					*(data + 2) = (byte)(*(data + 2) * avg / r);
					*(data + 1) = (byte)(*(data + 1) * avg / g);
					*data = (byte)(*(data) * avg / b);
				}

			bmp.UnlockBits(bData);

			return bmp.ToBitmapImage();
		}
	}
}
