using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace nicecuppa.graphics
{
    public class ClumsyMotion : Motion
    {
        public override float MotionDetect(string filename1, string filename2, int xmin = 0, int ymin = 0, int xmax = -1, int ymax = -1)
        {
            if ( debug )
                Console.WriteLine("ClumsyMotion::Detecting motion...");

            Bitmap inputBitmap1 = new Bitmap(filename1);
            greyScale(inputBitmap1);
            pixelate(inputBitmap1, 8);

            Bitmap inputBitmap2 = new Bitmap(filename2);
            greyScale(inputBitmap2);
            pixelate(inputBitmap2, 8);

            float delta = CompareBitmap(inputBitmap1, inputBitmap2, xmin, ymin, xmax, ymax);

            if (dumpOutput)
                inputBitmap1.Save("out.jpg");

            return delta;
        }
    }
}
