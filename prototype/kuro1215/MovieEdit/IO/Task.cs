﻿using MovieEdit.TL;

namespace MovieEdit
{
    public class Task
    {
        public TimelineObject Type { get; set; }

        public FrameInfo Frame { get; }

        public PositionInfo Position { get; }
        public string content { get; set; }

        public Task(TimelineObject type, string cont)
        {
            Type = type;
            content = cont;
        }
    }
}
