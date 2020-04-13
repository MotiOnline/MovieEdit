using System;
using OpenCvSharp;


namespace MovieEdit
{
    class Program
    {
        public static readonly Program program = new Program();
        public static int Main()
        {
            VideoCapture cap = new VideoCapture();
            Console.WriteLine("動画ファイルのパスを入力してください: ");
            string video_path = Console.ReadLine();

            cap.Open(video_path);
            if (!cap.IsOpened())
            {
                Console.WriteLine("ファイルが破損している可能性があります");
                return -1;
            }

            Console.WriteLine("動画に対する操作を選んでください: ");

            string scmd = Console.ReadLine();
            if (!int.TryParse(scmd, out int command))
            {
                Console.WriteLine("操作種別の入力が不正です");
                return -1;
            }
            if (command == 0)
            {
                program.Regeneration(cap);
            }
            return 0;
        }

        public struct Position
        {
            public double _x;
            public double _y;
        };

        public Point PosToPoint(Position pos, VideoCapture cap)
        {
            double width = cap.Get(VideoCaptureProperties.FrameWidth);
            double height = cap.Get(VideoCaptureProperties.FrameHeight);
            return new Point(pos._x + width / 2, pos._y + height / 2);
        }

        public void Regeneration(VideoCapture cap)
        {
            Mat frame;
            while (true)
            {
                frame = cap.RetrieveMat();
                if (frame.Empty()) break;
                Cv2.ImShow("preview", frame);
                int key = Cv2.WaitKey(33);
                double frame_position = cap.Get(VideoCaptureProperties.PosFrames);

                if (key == ' ')
                {
                    Cv2.WaitKey();
                }
                else if (key == 'j')
                {
                    cap.Set(VideoCaptureProperties.PosFrames, frame_position - 20);
                }
                else if (key == 'k')
                {
                    cap.Set(VideoCaptureProperties.PosFrames, frame_position + 20);
                }
                else if (key == 0x1b)
                {
                    break;
                }
            }
        }

        public void OutputMovie(VideoCapture cap)
        {
            VideoWriter vw = GetVideoWriter(cap);
            Mat frame, dst = new Mat();
            while (true)
            {
                frame = cap.RetrieveMat();
                if (frame.Empty()) break;
                OutputProgress(cap);
                Cv2.Flip(frame, dst, FlipMode.Y);
                vw.Write(dst);
            }
        }

        public VideoWriter GetVideoWriter(VideoCapture cap)
        {
            double width = cap.Get(VideoCaptureProperties.FrameWidth);
            double height = cap.Get(VideoCaptureProperties.FrameHeight);
            Size size = new Size(width, height);
            double fps = cap.Get(VideoCaptureProperties.Fps);
            int extension = VideoWriter.FourCC('m', 'p', '4', 'v');
            VideoWriter vw = new VideoWriter();
            vw.Open("output.mp4", extension, fps, size);
            return vw;
        }

        public void OutputProgress(VideoCapture cap)
        {
            double frame_position = cap.Get(VideoCaptureProperties.PosFrames);
            double all_frames = cap.Get(VideoCaptureProperties.FrameCount);
            double frame_percent = frame_position / all_frames * 100;
            Console.WriteLine($"Progress: {frame_percent}%");
        }

        public void OnTextOutput(VideoCapture cap, string text, Position pos)
        {
            VideoWriter vw = GetVideoWriter(cap);
            Mat frame;
            while (true)
            {
                frame = cap.RetrieveMat();
                if (frame.Empty()) break;
                OutputProgress(cap);
                Point pnt = PosToPoint(pos, cap);
                Cv2.PutText(frame, text, pnt, HersheyFonts.HersheyPlain, 5.0,
                    new Scalar(255, 0, 0), 2, LineTypes.AntiAlias);
                vw.Write(frame);
            }
        }
    }

    
}
