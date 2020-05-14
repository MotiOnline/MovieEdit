using System;
using System.Globalization;
using System.Text.Json.Serialization;

namespace MovieEdit.TL
{
    public class TLInfo<T> where T : TimelineObject
    {
        public ushort Layer { get; internal set; }
        public FrameInfo Frame { get; internal set; }
        public T Object { get; }

        public TLInfo(ushort layer, FrameInfo frame, T obj)
        {
            Layer = layer;
            Frame = frame;
            Object = obj;
        }
    }

    public struct FrameInfo
    {
        public uint Begin { get; private set; }
        public uint End { get; private set; }
        [JsonIgnore]
        public uint Length { get { return End - Begin; } }

        public FrameInfo(uint begin, uint end)
        {
            if (begin > end)
            {
                Begin = end;
                End = begin;
            }
            else
            {
                Begin = begin;
                End = end;
            }
        }

        public void Change(uint begin, uint end)
        {
            if (begin > end)
            {
                Begin = end;
                End = begin;
            }
            else
            {
                Begin = begin;
                End = end;
            }
        }

        public static implicit operator FrameInfo(uint frame)
        {
            return new FrameInfo(frame, frame);
        }

        public override bool Equals(object obj)
        {
            if (obj is FrameInfo frame)
            {
                if (frame.Length == 0) return Begin <= frame.Begin && frame.Begin <= End;
                else return Begin == frame.Begin && End == frame.End;
            }
            else return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Begin, End);
        }
    }
    public struct PositionInfo
    {
        public enum ReferencePos
        {
            LeftUp, MiddleUp, RightUp, LeftMid, Center, RightMid, LeftDown, MiddleDown, RightDown
        }
        public double X { get; private set; }
        public double Y { get; private set; }
        public ReferencePos Reference { get; private set; }

        public PositionInfo(double pos_x, double pos_y, ReferencePos reference = ReferencePos.Center)
        {
            X = pos_x;
            Y = pos_y;
            Reference = reference;
        }

        internal void Change(double pos_x, double pos_y)
        {
            X = pos_x;
            Y = pos_y;
        }

        internal void Change(ReferencePos reference)
        {
            Reference = reference;
        }
    }

    public struct Color
    {
        public byte Red { get; set; }
        public byte Green { get; set; }
        public byte Blue { get; set; }
        [JsonIgnore]
        public string ColorCode
        {
            get
            {
                return "#" + string.Format("{0:X6}", Red * 10000 + Green * 100 + Blue);
            }
        }

        public Color(byte r, byte g, byte b)
        {
            Red = r;
            Green = g;
            Blue = b;
        }
        public Color(string code)
        {
            if (code[0] == '#') code = code.Substring(1);
            Red = (byte)int.Parse(code.Substring(0, 2), NumberStyles.HexNumber);
            Green = (byte)int.Parse(code.Substring(3, 2), NumberStyles.HexNumber);
            Blue = (byte)int.Parse(code.Substring(5, 2), NumberStyles.HexNumber);
        }

        public override string ToString()
        {
            return ColorCode;
        }
    }
}
