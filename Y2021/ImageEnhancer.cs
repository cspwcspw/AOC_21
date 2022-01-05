using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Y2021
{
    public class ImageEnhancer
    {
        string replaceMap;
        List<string> image;

        int width, height;

        int voidBit;

        int enhanceCount = 0;

        public ImageEnhancer(string [] lines)
        {
            replaceMap = lines[0];
            Debug.Assert(replaceMap.Length == 512);
            image = new List<string>();
            voidBit = 0;
            for (int i =2; i < lines.Length; i++)
            {
                image.Add(lines[i]);    
            }
        }

        public void padImage()
        {
            char pad = voidBit == 1 ? '#' : '.';

            for (int i = 0; i < image.Count; i++)
            {
                image[i] = $"{pad}{pad}{pad}{image[i]}{pad}{pad}{pad}";
            }

            StringBuilder sb = new StringBuilder();
            for (int i = 0;i < image[0].Length; i++)
            {
                sb.Append(pad);
            }
            string pads = sb.ToString();
            for (int i = 0; i < 3; i++)
            {
                image.Add(pads);
            }
            for (int i = 0; i < 3; i++)
            {
                image.Insert(0,pads);
            }

        }

        void trimImage()
        {
            while (isEmptyRow(image[image.Count-1]))
            {
                image.RemoveAt(image.Count-1);
            }
            while (isEmptyRow(image[0]))
            {
                image.RemoveAt(0);
            }

            int n = 0;
            while (isEmptyCol(n))
            {
                n++;
            }
            int z = image[0].Length - 1;
            while (isEmptyCol(z))
            {
                z--;
            }
            for (int row=0; row < image.Count ; row++)
            {
                image[row] = image[row].Substring(n, z - n + 1);
            }
        }

        private bool isEmptyCol(int z)
        {
           for (int row=0; row < image.Count; row++)
            {
                if (image[row][z] == '#')
                {
                    return false;
                }
            }
            return true;

        }

        private bool isEmptyRow(string v)
        {
            int indx = v.IndexOf('#');
            return indx < 0;
        }

        public void Enhance()
        {
            padImage();
            enhanceCount++;
          //  Console.Write($" eh={enhanceCount}");
          //  Show();
            width = image[0].Length;
            height = image.Count;
            List<string> newImage = new List<string>();
            for (int row = 0; row < image.Count; row++)
            {
                StringBuilder sb = new StringBuilder();
                for (int col = 0; col < image[row].Length; col++)
                {
                    int indx = buildIndx(row, col);
                    sb.Append(replaceMap[indx]);
                }
                newImage.Add(sb.ToString());
            }

            image = newImage;
            // Show();
            if (replaceMap[0] == '#')
            {
                voidBit = voidBit == 1 ? 0 : 1;
            }
            if (enhanceCount % 2 == 0)
            {
                trimImage();
            }
        }

        private int buildIndx(int row, int col)
        {
            
            int bits = 0;
            for (int i = -1; i < 2; i++)
            {   int ri = row + i;
                for (int j = -1; j < 2; j++)
                {
                    int cj = col + j;
                    if (ri < 0 || ri >= height || cj < 0 || cj >= width)
                    {
                        bits = (bits << 1) + voidBit;
                    }
                    else
                    {
                        int b = image[ri][cj] == '#' ? 1 : 0;
                        bits = (bits << 1) + b;

                    }
                }
            }
            return bits;
        }

        public long litPixels()
        {
            long count = 0;
            int width = image[0].Length;
            for (int row = 0; row < image.Count; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (image[row][col] == '#') count++;
                }
            }
            return count;
        }

        public void Show()
        {
           
            for (int row = 0; row < image.Count; row++)
            {
                Console.WriteLine(image[row]);
            }
            Console.WriteLine();
        }
    }
}
