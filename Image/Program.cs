using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace nicecuppa.graphics
{
    class Program
    {
        static Dictionary<string, string> argFlags = new Dictionary<string, string>();
        static List<string> argFiles = new List<string>();

        static void Main(string[] args)
        {
            //Motion motion = new ClumsyMotion();
            Motion motion = new EdgeMotion();
            motion.DumpOutput = true;

            processArgs(args);

            if (argFlags.ContainsKey("-help") || args.Length == 0 || argFiles.Count == 0)
            {
                help();
            }
            else if (argFiles.Count == 2)
            {
                float motionTolerance = 0.02f;
                int xmin, ymin,xmax, ymax = 0;

                if (argFlags.ContainsKey("-dumpoutput"))
                {
                    motion.DumpOutput = true;
                }

                if (argFlags.ContainsKey("-tolerance"))
                {
                    int t = 10;

                    if ( int.TryParse(argFlags["-tolerance"], out t) )
                    {
                        motion.Tolerance = t;
                    }
                }

                if (argFlags.ContainsKey("-debug"))
                    motion.Debug = true;

                if (argFlags.ContainsKey("-motiontolerance"))
                {
                    float.TryParse(argFlags["-motiontolerance"], out motionTolerance);
                }

                if (argFlags.ContainsKey("-minxy"))
                {
                    string val = argFlags["-minxy"];
                    string[] tuple = val.Split(',');
                    int.TryParse(tuple[0], out xmin);
                    int.TryParse(tuple[1], out ymin);
                }
                else
                {
                    xmin = 0;
                    ymin = 0;
                }

                if (argFlags.ContainsKey("-maxxy"))
                {
                    string val = argFlags["-maxxy"];
                    string[] tuple = val.Split(',');
                    int.TryParse(tuple[0], out xmax);
                    int.TryParse(tuple[1], out ymax);
                }
                else
                {
                    xmax = -1;
                    ymax = -1;
                }

                float mcomp = motion.MotionDetect(argFiles[0], argFiles[1], xmin, ymin, xmax, ymax);

                Console.WriteLine(mcomp > motionTolerance ? "true" : "false");

            }
            else
            {
                help();
            }
       } 

        static void help()
        {
            Console.WriteLine("motion.exe [flags] InputFilename, InputFilename");
            Console.WriteLine("Version 1.1");
            Console.WriteLine("");
            Console.WriteLine("Flags:");
            Console.WriteLine("  -tolerance             Colour tolerance for detecting motion [0-255]");
            Console.WriteLine("                         (default 10");
            Console.WriteLine("  -motiontolerance       Proportion of image to detect change [0.0-1.0]");
            Console.WriteLine("                         (default 0.02");
            Console.WriteLine("  -debug                 Show debug output");
            Console.WriteLine("");
        }

        static void processArgs(string[] args)
        {
            // Loop over all arguments
            for (int i = 0; i < args.Length; i++)
            {
                // If starts with a dash then it's a flag
                if (args[i].StartsWith("-"))
                {
                    // If it contains a colon then we should expect an expression with the flag
                    if (args[i].Contains(":"))
                    {
                        string[] parts = args[i].Split(new char[] { ':' });
                        argFlags.Add(parts[0], parts[1]);
                    }
                    else
                    {
                        argFlags.Add(args[i], args[i]);
                    }
                }
                else
                {
                    argFiles.Add(args[i]);
                }
            }
        }
    }
}
