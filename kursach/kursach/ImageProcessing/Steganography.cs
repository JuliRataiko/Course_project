using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kursach.ImageProcessing
{
	public class Steganography
	{
		public BitArray ByteToBit(byte src)
		{
			BitArray bitArray = new BitArray(8);
			bool st = false;
			for (int i = 0; i < 8; i++)
			{
				if ((src >> i & 1) == 1)
				{
					st = true;
				}
				else st = false;
				bitArray[i] = st;
			}
			return bitArray;
		}

		public byte BitToByte(BitArray scr)
		{
			byte num = 0;
			for (int i = 0; i < scr.Count; i++)
				if (scr[i] == true)
					num += (byte)Math.Pow(2, i);
			return num;
		}

		public bool isEncryption(Bitmap scr)
		{
			byte[] rez = new byte[1];
			Color color = scr.GetPixel(0, 0);
			BitArray colorArray = ByteToBit(color.R); //получаем байт цвета и преобразуем в массив бит
			BitArray messageArray = ByteToBit(color.R); ;//инициализируем результирующий массив бит
			messageArray[0] = colorArray[0];
			messageArray[1] = colorArray[1];

			colorArray = ByteToBit(color.G);//получаем байт цвета и преобразуем в массив бит
			messageArray[2] = colorArray[0];
			messageArray[3] = colorArray[1];
			messageArray[4] = colorArray[2];

			colorArray = ByteToBit(color.B);//получаем байт цвета и преобразуем в массив бит
			messageArray[5] = colorArray[0];
			messageArray[6] = colorArray[1];
			messageArray[7] = colorArray[2];
			rez[0] = BitToByte(messageArray); //получаем байт символа, записанного в 1 пикселе
			string m = Encoding.GetEncoding(1251).GetString(rez);
			if (m == "/")
			{
				return true;
			}
			else return false;
		}

		public void WriteCountText(int count, Bitmap src)
		{
			byte[] CountSymbols = Encoding.GetEncoding(1251).GetBytes(count.ToString());
			for (int i = 0; i < CountSymbols.Length; i++)
			{
				BitArray bitCount = ByteToBit(CountSymbols[i]); //биты количества символов
				Color pColor = src.GetPixel(0, i + 1); //1, 2, 3 пикселы
				BitArray bitsCurColor = ByteToBit(pColor.R); //бит цветов текущего пикселя
				bitsCurColor[0] = bitCount[0];
				bitsCurColor[1] = bitCount[1];
				byte nR = BitToByte(bitsCurColor); //новый бит цвета пиксея

				bitsCurColor = ByteToBit(pColor.G);//бит бит цветов текущего пикселя
				bitsCurColor[0] = bitCount[2];
				bitsCurColor[1] = bitCount[3];
				bitsCurColor[2] = bitCount[4];
				byte nG = BitToByte(bitsCurColor);//новый цвет пиксея

				bitsCurColor = ByteToBit(pColor.B);//бит бит цветов текущего пикселя
				bitsCurColor[0] = bitCount[5];
				bitsCurColor[1] = bitCount[6];
				bitsCurColor[2] = bitCount[7];
				byte nB = BitToByte(bitsCurColor);//новый цвет пиксея

				Color nColor = Color.FromArgb(nR, nG, nB); //новый цвет из полученных битов
				src.SetPixel(0, i + 1, nColor); //записали полученный цвет в картинку
			}
		}

		public int ReadCountText(Bitmap src)
		{
			byte[] rez = new byte[3]; //массив на 3 элемента, т.е. максимум 999 символов шифруется
			for (int i = 0; i < 3; i++)
			{
				Color color = src.GetPixel(0, i + 1); //цвет 1, 2, 3 пикселей 
				BitArray colorArray = ByteToBit(color.R); //биты цвета
				BitArray bitCount = ByteToBit(color.R); ; //инициализация результирующего массива бит
				bitCount[0] = colorArray[0];
				bitCount[1] = colorArray[1];

				colorArray = ByteToBit(color.G);
				bitCount[2] = colorArray[0];
				bitCount[3] = colorArray[1];
				bitCount[4] = colorArray[2];

				colorArray = ByteToBit(color.B);
				bitCount[5] = colorArray[0];
				bitCount[6] = colorArray[1];
				bitCount[7] = colorArray[2];
				rez[i] = BitToByte(bitCount);
			}
			string m = Encoding.GetEncoding(1251).GetString(rez);
			return Convert.ToInt32(m, 10);
		}
	}
}
