using System;
using System.Collections.Generic;
using System.Text;

namespace MovieEdit
{
    public struct RGB
    {
        public byte Red { get; set; }
        public byte R
        {
            get { return Red; }
            set { Red = value; }
        }
        public byte Green { get; set; }
        public byte G
        {
            get { return Green; }
            set { Green = value; }
        }
        public byte Blue { get; set; }
        public byte B
        {
            get { return Blue; }
            set { Blue = value; }
        }
    }

    public struct ValueInfo<T>
    {
        public T Start { get; set; }
        public T End { get; set; }

        public ValueInfo(T start, T end)
        {
            Start = start;
            End = end;
        }

        public static explicit operator ValueInfo<T>(ValueInfo<double> v)
        {
            throw new NotImplementedException();
        }
    }
}
