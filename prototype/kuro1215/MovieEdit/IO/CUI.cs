using MovieEdit.Effects;
using MovieEdit.TL;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using static MovieEdit.Program;
using static MovieEdit.MESystem;

namespace MovieEdit.IO
{
    public class CUI
    {
        public static void CUIInput()
        {
            Console.WriteLine("Enter the command...");
            var cap = new VideoCapture();
            var dic = new Dictionary<FrameInfo, EffectBase>();
            while (true)
            {
                string[] cmd = Console.ReadLine().Split(" ");
                if (cmd[0] == "exit") break;
                else if (cmd[0] == "load")
                {
                    cap = new VideoCapture(cmd[1]);
                    Log("Loading OK");
                }
                else if (cmd[0] == "effect")
                {
                    if (cmd[1] == "flip")
                    {
                        if (cmd[2] == "X") dic.Add(new FrameInfo(uint.Parse(cmd[3]), uint.Parse(cmd[4])), Effect.FLIP(FlipMode.X));
                    }
                }
                else if (cmd[0] == "output")
                {
                    Movie.OutputMovie(cap, VideoWriter.FourCC('J', 'P', 'E', 'G'), cmd[1] + ".avi", dic);
                }
                else if (cmd[0] == "cam")
                {
                    Log("Camera Open");
                    Webcam();
                    Log("Camera Close");
                }
            }
        }
    }
}
