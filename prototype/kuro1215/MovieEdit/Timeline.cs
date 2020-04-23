using System;
using System.Collections.Generic;
using System.Text;

namespace MovieEdit.Timeline
{
    public class FrameInfo
    {
        public uint Begin { get; private set; }
        public uint End { get; private set; }
        public uint Length { get { return End - Begin; } }

        public FrameInfo(uint begin, uint end)
        {
            Begin = begin;
            End = end;
        }

        public void Change(uint begin, uint end)
        {
            Begin = begin;
            End = end;
        }

        public bool Include(uint frame)
        {
            return Begin <= frame && frame <= End;
        }
    }

    public class Timeline
    {
        public TimelineObject[] Objects;

        public Timeline()
        {

        }

        public void AddObject(TimelineObject obj)
        {

        }

        public TimelineObject GetObject(ushort layer, uint frame)
        {
            return Objects[0];
        }
    }
}
