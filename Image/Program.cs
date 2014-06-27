using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Image
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 2)
            {
                float motionThreshold = 0.1f;

                float mcomp = motionDetect(args[0], args[1], 10, 0, 90);

                Console.WriteLine(mcomp > motionThreshold ? "true" : "false");
            }
            else
            {
                Console.WriteLine("Guru meditation, args?");
            }
        }

        static float motionDetect( string filename1, string filename2, int tolerance, int xmin = 0, int ymin = 0, int xmax = -1, int ymax = -1 )
        {
            Bitmap inputBitmap1 = new Bitmap(filename1);
            toGreyScale(inputBitmap1);
            toPixelate(inputBitmap1, 8);

            Bitmap inputBitmap2 = new Bitmap(filename2);
            toGreyScale(inputBitmap2);
            toPixelate(inputBitmap2, 8);

            float delta = compareBitmap(inputBitmap1, inputBitmap2, tolerance, xmin, ymin, xmax, ymax);

            inputBitmap1.Save("out.jpg");

            return delta;
        }

        static float compareBitmap( Bitmap b1, Bitmap b2, int tolerance, int xmin, int ymin, int xmax, int ymax)
        {
            int xmaxActual = xmax == -1 ? b1.Width : xmax;
            int ymaxActual = ymax == -1 ? b1.Height : ymax;

            long pixelCount = 0;
            long pixelDeltaCount = 0;

            for (int x = xmin; x < xmaxActual; x++)
            {
                for (int y = ymin; y < ymaxActual; y++)
                {
                    Color c1 = b1.GetPixel(x, y);
                    Color c2 = b2.GetPixel(x, y);

                    int difference = Math.Abs( c1.R - c2.R );

                    if ( difference > tolerance )
                    {
                        b1.SetPixel(x, y, Color.Red);
                        pixelDeltaCount++;
                    }

                    pixelCount++;
                }
            }

            if (pixelDeltaCount == 0)
                return 0;
            else
                return (float)pixelDeltaCount / (float)pixelCount;
        }

        static void toPixelate(Bitmap b, int blockSize, int numGreyLevels = -1)
        {
            int realColourScale = 256 / numGreyLevels;

            for (int x = 0; x < b.Width; x += blockSize)
            {
                for (int y = 0; y < b.Height; y += blockSize)
                {
                    int greyScale = 0;

                    for (int xx = 0; xx < blockSize; ++xx)
                    {
                        for (int yy = 0; yy < blockSize; ++yy)
                        {
                            if (x + xx >= b.Width || y + yy >= b.Height)
                            {
                                continue;
                            }

                            var color = b.GetPixel(x + xx, y + yy);
                            greyScale += color.R;
                        }
                    }

                    // average blockcolor
                    greyScale = greyScale / (blockSize * blockSize);

                    if ( numGreyLevels > 0 )
                        greyScale = (greyScale / realColourScale) * realColourScale;

                    // new colour
                    Color newScale = Color.FromArgb(greyScale, greyScale, greyScale);

                    // set pixels
                    for (int xx = 0; xx < blockSize; ++xx)
                    {
                        for (int yy = 0; yy < blockSize; ++yy)
                        {
                            if (x + xx >= b.Width || y + yy >= b.Height)
                            {
                                continue;
                            }
                            b.SetPixel(x + xx, y + yy, newScale);                            
                        }
                    }

                }
            }
        }

        static void toGreyScale(Bitmap b)
        {
            int x, y;

            for (x = 0; x < b.Width; x++)
            {
                for (y = 0; y < b.Height; y++)
                {
                    Color pixelColor = b.GetPixel(x, y);

                    int greyScale = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;

                    Color newColor = Color.FromArgb(greyScale, greyScale, greyScale);
                    b.SetPixel(x, y, newColor); // Now greyscale
                }
            }
        }
    }
}
