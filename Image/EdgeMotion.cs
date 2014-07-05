using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace nicecuppa.graphics
{

    public class EdgeMotion : Motion
    {
        private const int pixelArea = 2;
        private List<XY> offsets = new List<XY> ()
        { 
            new XY { X = 0, Y = -pixelArea },           // N
            new XY { X = pixelArea, Y = -pixelArea },   // NE
            new XY { X = pixelArea, Y = 0 },            // E
            new XY { X = pixelArea, Y = pixelArea },    // SE
            new XY { X = 0, Y = pixelArea },            // S
            new XY { X = -pixelArea, Y = pixelArea },   // SW
            new XY { X = -pixelArea, Y = 0 },           // W
            new XY { X = 0, Y = -pixelArea }            // NW
        };

        public override float MotionDetect(string filename1, string filename2, int xmin = 0, int ymin = 0, int xmax = -1, int ymax = -1)
        {
            if ( debug )
                Console.WriteLine("EdgeMotion::Detecting motion...");
            
            Bitmap inputBitmap1 = new Bitmap(filename1);
            greyScale(inputBitmap1);
            pixelate(inputBitmap1, pixelArea);
            edgeDetect(inputBitmap1, xmin, ymin, xmax, ymax);

            Bitmap inputBitmap2 = new Bitmap(filename2);
            greyScale(inputBitmap2);
            pixelate(inputBitmap2, pixelArea);
            edgeDetect(inputBitmap2, xmin, ymin, xmax, ymax);

            float delta = CompareBitmap(inputBitmap1, inputBitmap2, xmin, ymin, xmax, ymax);

            if (dumpOutput)
            {
                inputBitmap1.Save("out1.jpg");
                inputBitmap2.Save("out2.jpg");
            }

            return delta;
        }

        public void edgeDetect(Bitmap b, int xmin, int ymin, int xmax, int ymax)
        {
            List<XY> edges = new List<XY>();

            int xmaxActual = xmax == -1 ? b.Width-(pixelArea) : xmax;
            int ymaxActual = ymax == -1 ? b.Height -(pixelArea) : ymax;

            for (int x = xmin; x < xmaxActual; x += pixelArea)
            {
                for (int y = ymin; y < ymaxActual; y += pixelArea)
                {
                    int maxDiff = getColourMax(b, x, y);

                    if (maxDiff > tolerance)
                    {
                        // set pixels
                        for (int xx = 0; xx < pixelArea; ++xx)
                        {
                            for (int yy = 0; yy < pixelArea; ++yy)
                            {
                                if (x + xx >= b.Width || y + yy >= b.Height)
                                {
                                    continue;
                                }
                                
                                edges.Add(new XY() { X = x + xx, Y = y + yy });                                    
                            }
                        }
                    }
                }
            }
            
            foreach ( XY xy in edges )
                b.SetPixel(xy.X, xy.Y, Color.Red);
        }

        private int getColourMax( Bitmap b, int x, int y)
        {
            Color c1 = b.GetPixel(x, y);
            int maxDiff = 0;

            foreach (XY xy in offsets)
            {
                XY newXY = new XY() { X = x + xy.X, Y = y + xy.Y };
                if (newXY.X > 0 && newXY.X < b.Width && newXY.Y > 0 && newXY.Y < b.Height)
                {
                    Color c2 = b.GetPixel(newXY.X, newXY.Y);
                    int difference = Math.Abs(c1.R - c2.R);

                    if (difference > maxDiff)
                        maxDiff = difference;                    
                }
            }

            return maxDiff;
        }
    }

    class XY
    {
        public int X;
        public int Y;
    }
}
