using System;
using System.Collections.Generic;

namespace MovieEdit.TL
{
    public class Timeline
    {
        public TimelineObject this[ushort layer, uint frame]
        {
            get { return GetObject(layer, frame); }
        }

        public TimelineObject this[ushort layer, FrameInfo frame]
        {
            get { return Objects[layer][frame]; }
        }
        public IReadOnlyDictionary<FrameInfo, TimelineObject>[] Objects { get; private set; }

        public Timeline()
        {
            Objects = new Dictionary<FrameInfo, TimelineObject>[0];
        }

        public void AddObject(ushort layer, FrameInfo frame, TimelineObject obj)
        {
            var objs = Objects;
            if (Objects.Length < layer) Array.Resize(ref objs, Objects.Length);
            else if (objs[layer].ContainsKey(frame)) return;
            objs[layer] = new Dictionary<FrameInfo, TimelineObject>(objs[layer]) { { frame, obj } };
            Objects = objs;
        }

        public TimelineObject GetObject(ushort layer, uint frame)
        {
            return Objects[layer][frame];
        }
    }
}
