using MovieEdit.Effects;
using MovieEdit.TL;
using OpenCvSharp;
using System.Collections.Generic;
using static MovieEdit.MESystem;
using static OpenCvSharp.VideoCaptureProperties;

namespace MovieEdit.IO
{
    public class Movie
    {
        public static void OutputMovie(VideoCapture cap, FourCC cc, string path, Dictionary<FrameInfo, EffectBase> effect = null)
        {
            Size size = new Size(cap.Get(FrameWidth), cap.Get(FrameHeight));
            double fps = cap.Get(Fps);
            VideoWriter vw = new VideoWriter(path, cc, fps, size);
            Mat frame;
            cap.Set(PosFrames, 0);
            do
            {
                frame = cap.RetrieveMat();
                var f = (uint)cap.Get(PosFrames);
                foreach (var eff in effect)
                {
                    if (eff.Key.Begin <= f && f <= eff.Key.End) frame = eff.Value.Processing(frame);
                }
                vw.Write(frame);
                LogProgress("Outputing Movie", f / cap.Get(FrameCount) * 100);
            }
            while (!frame.Empty());
        }

        public static void OutputMovie()
        {

        }

        private void OutputMovie(FourCC cc, string extention)
        {

        }
    }
}
