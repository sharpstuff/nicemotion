using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace nicecuppa.graphics
{
    public abstract class Motion
    {
        protected int tolerance = 40;        

        protected bool dumpOutput = false;

        protected bool debug = false;

        #region Detection routines
        
        public abstract float MotionDetect(string filename1, string filename2, int xmin = 0, int ymin = 0, int xmax = -1, int ymax = -1);

        /// <summary>
        /// Clumsy motion detection using changes in grey-scales between two images
        /// </summary>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <param name="greyScaleTolerance"></param>
        /// <param name="xmin"></param>
        /// <param name="ymin"></param>
        /// <param name="xmax"></param>
        /// <param name="ymax"></param>
        /// <returns></returns>
        public virtual float CompareBitmap(Bitmap b1, Bitmap b2, int xmin, int ymin, int xmax, int ymax)
        {
            if ( debug )
                Console.WriteLine("Comparing bitmap...");
            
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

                    int difference = Math.Abs(c1.R - c2.R);

                    if (difference > tolerance)
                    {
                        b1.SetPixel(x, y, Color.Green);
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

        protected void pixelate(Bitmap b, int blockSize, int numGreyLevels = -1)
        {
            if ( debug )
                Console.WriteLine("Pixelating...");
            
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

                    if (numGreyLevels > 0)
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

        protected void greyScale(Bitmap b)
        {
            if ( debug )
                Console.WriteLine("Generating grey-scales...");
            
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
        #endregion

        public int Tolerance
        {
            get
            {
                return tolerance;
            }
            set
            {
                tolerance = value;
            }
        }


        public bool DumpOutput
        {
            get
            {
                return dumpOutput;
            }
            set
            {
                dumpOutput = value;
            }
        }

        public bool Debug
        {
            get
            {
                return debug;
            }
            set
            {
                debug = value;
            }
        }
    }
}
