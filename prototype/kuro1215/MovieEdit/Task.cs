using System;
using System.Collections.Generic;
using System.Text;
using MovieEdit.Timeline;
using static MovieEdit.Timeline.TimelineObject;

namespace MovieEdit
{
    public class Task
    {
        public enum EditType
        {
            Text, Square, Image
        }

        public EditType type { get; set; }

        public FrameInfo Frame { get; }

        public PositionInfo Position { get; }
        public string content { get; set; }

        public Task(EditType t, string cont)
        {
            type = t;
            content = cont;
        }
    }
}
