using System;
using OpenCvSharp;
using MovieEdit.Timeline;
using static MovieEdit.System;
using System.Collections.Generic;
using System.Text;

namespace MovieEdit
{

    public static class Effect
    {
        public unsafe static Chromakey CHROMAKEY = new Chromakey();
        public static Flip FLIP = new Flip();
        public unsafe static Opacity OPACITTY = new Opacity();
        public static Resize RESIZE = new Resize();
        public static Rotate ROTATE = new Rotate();
    }

    public interface IEffectBase { }
    public abstract class EffectBase<A> : IEffectBase
    {
        public abstract Mat Processing(Mat source, A value);
    }
    public abstract class EffectBase<A, B> : IEffectBase
    {
        public abstract Mat Processing(Mat source, A value1, B value2);
    }
    public abstract class EffectBaseMat<A> : IEffectBase
    {
        public abstract Mat Processing(Mat source, Mat basis, A value);
    }
    public abstract class EffectBaseMat<A, B> : IEffectBase
    {
        public abstract Mat Processing(Mat source, Mat basis, A value1, B value2);
    }

    public interface IMovieEffect { }
    public interface IPictureEffect { }
    public interface IAudioEffect { }
    public interface ITextEffect { }
    public interface ISquareEffect { }
    public interface IFileEffect : IMovieEffect, IPictureEffect, IAudioEffect { }
    public interface IScreenEffect : IMovieEffect, IPictureEffect, ITextEffect, ISquareEffect { }

    public class Chromakey : EffectBase<string>
    {
        public override Mat Processing(Mat mat, string s)
        {
            throw new NotImplementedException();
        }
    }

    public class Flip : EffectBase<FlipMode>, IFileEffect
    {
        internal Flip() { }

        public override Mat Processing(Mat source, FlipMode mode)
        {
            Mat mat = new Mat();
            Cv2.Flip(source, mat, mode);
            return mat;
        }
    }

    public unsafe class Opacity : EffectBase<byte>, IScreenEffect
    {
        internal Opacity() { }

        public override Mat Processing(Mat mat, byte value)
        {
            Mat alpha = new Mat(mat.Size(), MatType.CV_8UC3);
            Cv2.CvtColor(mat, alpha, ColorConversionCodes.BGR2RGBA);
            for (int y = 0; y < alpha.Height; y++)
            {
                for (int x = 0; x < alpha.Width; x++)
                {
                    var px = alpha.At<Vec4b>(y, x);
                    px[3] = value;
                    alpha.Set(y, x, px);
                }
            }
            return alpha;
        }
    }

    public class Resize : EffectBase<double>, IScreenEffect
    {
        internal Resize() { }

        public override Mat Processing(Mat source, double scale)
        {
            var mat = new Mat();
            if (scale != 1) Cv2.Resize(source, mat, new Size(source.Width * scale, source.Height * scale));
            else mat = source;
            return mat;
        }
    }

    public class Rotate : EffectBase<double, double>, IScreenEffect
    {
        internal Rotate() { }

        public override Mat Processing(Mat source, double value, double scale = 1.0)
        {
            if (value / 360 >= 1) value %= 360;
            if (value == 0) return source;
            if (value % 90 == 0)
            {
                return (value / 90) switch
                {
                    1 => Rotation(source, RotateFlags.Rotate90Clockwise, scale),
                    2 => Rotation(source, RotateFlags.Rotate180, scale),
                    3 => Rotation(source, RotateFlags.Rotate90Counterclockwise, scale),
                    _ => throw new Exception(),
                };
            }
            else
            {
                var mat = Cv2.GetRotationMatrix2D(new Point2f(source.Width / 2, source.Height / 2), value, scale);
                var res = new Mat();
                double w = 1 / Math.Cos(value * Math.PI / 180) * (source.Width + source.Height);
                double h = 1 / Math.Cos((360 - value) * Math.PI / 180) * (source.Width + source.Height);
                Cv2.WarpAffine(source, res, mat, new Size(w, h));
                return res;
            }
        }

        private Mat Rotation(Mat source, RotateFlags flag, double scale)
        {
            Mat mat = new Mat();
            Cv2.Rotate(source, mat, flag);
            return new Rotate().Processing(mat, scale);
        }
    }
}