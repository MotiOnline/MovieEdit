using System;
using System.Collections.Generic;
using OpenCvSharp;
using static MovieEdit.TL.TimelineObject;
using static MovieEdit.TL.TimelineObject.TimelineObjectType;

namespace MovieEdit.Effects
{
    public class Effect
    {
        [Obsolete]
        public static Chromakey CHROMAKEY = new Chromakey();
        public static ColorConvart COLORCONVERT(ColorConversionCodes code)
            => new ColorConvart(code);
        public static Flip FLIP(FlipMode mode) => new Flip(mode);
        [Obsolete]
        public static Opacity OPACITTY(byte value) => new Opacity(value);
        public static Rect RECT(Point point, Size size) => new Rect(point, size);
        public static Resize RESIZE(double scale) => new Resize(scale);
        public static Rotate ROTATE(double value, double scale) => new Rotate(value, scale);
        public static Threshold THRESHOLD(int threst, ThresholdTypes thType, MorphTypes moType)
            => new Threshold(threst, thType, moType);
    }

    public abstract class EffectBase : Base
    {
        private readonly List<TimelineObjectType> Types;

        protected private EffectBase(string name, string explain, object[] value, params TimelineObjectType[] types)
            : base(name, explain, value, BaseType.Effect) { Types = new List<TimelineObjectType>(types); }

        public bool CanEffect(TimelineObjectType type)
        {
            return Types.Contains(type);
        }

        public abstract Mat Processing(Mat source);
    }
    public abstract class EffectBase<A> : EffectBase
    {
        protected private A Value1
        {
            get { return (A)Values[0]; }
            set { Values[0] = value; }
        }

        public EffectBase(string name, string explain, A value1, params TimelineObjectType[] types)
            : base(name, explain, new object[] { value1 }, types) { }
    }
    public abstract class EffectBase<A, B> : EffectBase
    {
        protected private A Value1
        {
            get { return (A)Values[0]; }
            set { Values[0] = value; }
        }
        protected private B Value2
        {
            get { return (B)Values[1]; }
            set { Values[1] = value; }
        }

        public EffectBase(string name, string explain, A value1, B value2, params TimelineObjectType[] types)
            : base(name, explain, new object[] { value1, value2 }, types) { }
    }
    public abstract class EffectBase<A, B, C> : EffectBase
    {
        protected private A Value1
        {
            get { return (A)Values[0]; }
            set { Values[0] = value; }
        }
        protected private B Value2
        {
            get { return (B)Values[1]; }
            set { Values[1] = value; }
        }
        protected private C Value3
        {
            get { return (C)Values[2]; }
            set { Values[2] = value; }
        }

        public EffectBase(string name, string explain, A value1, B value2, C value3, params TimelineObjectType[] types)
            : base(name, explain, new object[] { value1, value2, value3 }, types) { }
    }

    public class Chromakey : EffectBase<string>
    {
        internal Chromakey() : base("", "", "", Movie, Picture) { }

        public override Mat Processing(Mat mat)
        {
            throw new NotImplementedException();
        }
    }

    public class Flip : EffectBase<FlipMode>
    {
        internal Flip(FlipMode mode) : base("", "", mode, Movie, Picture) { }

        public override Mat Processing(Mat source)
        {
            var res = new Mat(); res = new Mat();
            Cv2.Flip(source, res, Value1);
            return res;
        }
    }

    public class ColorConvart : EffectBase<ColorConversionCodes>
    {
        internal ColorConvart(ColorConversionCodes code) : base("", "", code, Movie, Picture) { }

        public override Mat Processing(Mat source)
        {
            var res = new Mat(); res = new Mat();
            Cv2.CvtColor(source, res, Value1);
            return res;
        }
    }

    [Obsolete]
    public class Opacity : EffectBase<byte>
    {
        internal Opacity(byte value) : base("", "", value, Movie, Picture) { }

        public override Mat Processing(Mat mat)
        {
            Mat alpha = new Mat(mat.Size(), MatType.CV_8UC3);
            Cv2.CvtColor(mat, alpha, ColorConversionCodes.BGR2RGBA);
            for (int y = 0; y < alpha.Height; y++)
            {
                for (int x = 0; x < alpha.Width; x++)
                {
                    var px = alpha.At<Vec4b>(y, x);
                    px[3] = Value1;
                    alpha.Set(y, x, px);
                }
            }
            return alpha;
        }
    }

    public class Rect : EffectBase<Point, Size>
    {
        internal Rect(Point point, Size size) : base("", "", point, size, Movie, Picture) { }

        public override Mat Processing(Mat source)
        {
            return source.Clone(new OpenCvSharp.Rect(Value1, Value2));
        }
    }

    public class Resize : EffectBase<double>
    {
        internal Resize(double scale) : base("", "", scale, Movie, Picture) { }

        public override Mat Processing(Mat source)
        {
            var res = new Mat();
            if (Value1 != 1) Cv2.Resize(source, res, new Size(source.Width * Value1, source.Height * Value1));
            else res = source;
            return res;
        }
    }

    public class Rotate : EffectBase<double, double>
    {
        internal Rotate(double value, double scale = 1.0) : base("", "", value, scale, Movie, Picture){ }

        public override Mat Processing(Mat source)
        {
            if (Value1 / 360 >= 1) Value1 %= 360;
            if (Value1 == 0) return source;
            if (Value1 % 90 == 0)
            {
                return (Value1 / 90) switch
                {
                    1 => Rotation(source, RotateFlags.Rotate90Clockwise, Value2),
                    2 => Rotation(source, RotateFlags.Rotate180, Value2),
                    3 => Rotation(source, RotateFlags.Rotate90Counterclockwise, Value2),
                    _ => throw new Exception(),
                };
            }
            else
            {
                var res = new Mat();
                var mat = Cv2.GetRotationMatrix2D(new Point2f(source.Width / 2, source.Height / 2), Value1, Value2);
                double w = 1 / Math.Cos(Value1 * Math.PI / 180) * (source.Width + source.Height);
                double h = 1 / Math.Cos((360 - Value1) * Math.PI / 180) * (source.Width + source.Height);
                Cv2.WarpAffine(source, res, mat, new Size(w, h));
                return res;
            }
        }

        private Mat Rotation(Mat source, RotateFlags flag, double scale)
        {
            Mat mat = new Mat();
            Cv2.Rotate(source, mat, flag);
            return new Resize(scale).Processing(mat);
        }
    }

    public class Threshold : EffectBase<int, ThresholdTypes, MorphTypes?>
    {
        internal Threshold(int threst, ThresholdTypes thType, MorphTypes? moType = null)
            : base("", "", threst, thType, moType, Movie, Picture) { }

        public override Mat Processing(Mat source)
        {
            var res = new Mat();
            Cv2.Threshold(source, res, Value1, 255, Value2);
            if(Value3 != null)
            {
                var type = (MorphTypes)Value3;
                var mat = new Mat();
                Cv2.MorphologyEx(res, mat, type, null);
                res = mat;
            }
            return res;
        }
    }
}