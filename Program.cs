using System;
using System.Drawing;
using System.IO;

namespace centerCropResize
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            int size;
            if (args.Length != 2 || !int.TryParse(args[1], out size))
            {
                Console.WriteLine("Usage centerCropResize <filename> <size>");
                return;
            }

            string filename = args[0];
            if (!File.Exists(filename))
            {
                Console.WriteLine(filename + " not found.");
                return;
            }

            Bitmap sourceImage = new Bitmap(filename);

            int xMin = sourceImage.Width;
            int yMin = sourceImage.Height;
            int xMax = 0;
            int yMax = 0;

            //Iterate all pixels and determine the bounding box
            //for non-transparent pixels only.
            for (var y = 0; y < sourceImage.Height; y++)
            {
                for (var x = 0; x < sourceImage.Width; x++)
                {
                    if (sourceImage.GetPixel(x, y).A != 0)
                    {
                        xMin = Math.Min(x, xMin);
                        yMin = Math.Min(y, yMin);
                        xMax = Math.Max(x, xMax);
                        yMax = Math.Max(y, yMax);
                    }
                }
            }
            Rectangle sourceRect = new Rectangle(xMin, yMin, xMax - xMin, yMax - yMin);

            //Create a new bitmap from the source image that only contains the non-transparent pixels.
            using (Bitmap cropped = sourceImage.Clone(sourceRect, sourceImage.PixelFormat))
            {
                //Determine the size of the scaled image.
                float floatWidth = cropped.Width;
                float floatHeight = cropped.Height;
                if (floatWidth > size)
                {
                    floatHeight = floatHeight * (size / floatWidth);
                    floatWidth = size;
                }
                if (floatHeight > size)
                {
                    floatWidth = floatWidth * (size / floatHeight);
                    floatHeight = size;
                }

                int drawWidth = (int)Math.Ceiling(floatWidth);
                int drawHeight = (int)Math.Ceiling(floatHeight);
                if (drawHeight != size && drawHeight % 2 != 0)
                {
                    drawHeight++;
                }
                if (drawWidth != size && drawWidth % 2 != 0)
                {
                    drawWidth++;
                }

                using (Bitmap output = new Bitmap(size, size, sourceImage.PixelFormat))
                {
                    //Finally draw and scale the
                    using (Graphics g = Graphics.FromImage(output))
                    {
                        g.DrawImage(cropped, new Rectangle((size - drawWidth) / 2, (size - drawHeight) / 2, drawWidth, drawHeight));
                    }

                    //Dispose the source image so we can overwrite it.
                    sourceImage.Dispose();

                    //Save the result
                    output.Save(filename);
                }
            }
        }
    }
}