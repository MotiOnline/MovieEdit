using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static MovieEdit.System;

namespace MovieEdit.Timeline
{
    public class TimelinePicture : TimelineObject
    {
        public Mat Picture { get; set; }
        public string FilePath { get; }
        public IPictureEffect[] Effects { get; private set; }

        public TimelinePicture(ushort layer, FrameInfo frame, PositionInfo pos, string path)
            : base(TimelineObjectType.Picture, layer, frame, pos)
        {
            if(File.Exists(path) && IsPicture(path))
            {
                FilePath = path;
                Picture = new Mat(path);
                //exception
            }
            else
            {
                throw new FileNotFoundException("This file was not found, so you cannot make object.", path);
            }
        }

        public override void AddEffect(IEffectBase effect)
        {
            if(IsCanEffect(effect))
            {
                
            }
            else throw new InvalidCastException();
        }

        public override bool IsCanEffect(IEffectBase effect)
        {
            return effect is IPictureEffect;
        }

        public override Task[] ToTasks()
        {
            throw new NotImplementedException();
        }
    }

    public class TimelineMovie : TimelineObject
    {
        public string FilePath { get; }

        public List<IEffectBase> Effects { get; private set; }

        public TimelineMovie(ushort layer, FrameInfo frame, PositionInfo pos, string path)
            : base(TimelineObjectType.Picture, layer, frame, pos)
        {
            FilePath = path;
            Effects = new List<IEffectBase>();
        }

        public override void AddEffect(IEffectBase effect)
        {
            if (effect is IMovieEffect)
            {
                Effects.Add(effect);
                Log("Movie File was add Effect.");
            }
            else
            {
                Log(LogType.Error, "This Effect can't add this Movie File.");
            }
        }

        public override bool IsCanEffect(IEffectBase effect)
        {
            return effect is IMovieEffect;
        }

        public override Task[] ToTasks()
        {
            throw new NotImplementedException();
        }
    }

    public abstract class TimelineObject
    {
        public enum TimelineObjectType
        {
            Movie, Picture, Audio, Text, Square
        }
        public struct PositionInfo
        {
            public enum ReferencePos
            {
                LeftUp
            }
            public double X { get; private set; }
            public double Y { get; private set; }
            public double Z { get; private set; }
            public ReferencePos Reference { get; private set; }

            public PositionInfo(double pos_x, double pos_y, double pos_z, ReferencePos reference = 0)
            {
                X = pos_x;
                Y = pos_y;
                Z = pos_z;
                Reference = reference;
            }

            internal void Change(double pos_x, double pos_y, double pos_z)
            {
                X = pos_x;
                Y = pos_y;
                Z = pos_z;
            }

            internal void Change(ReferencePos reference)
            {
                Reference = reference;
            }
        }

        public TimelineObjectType ObjectType { get; }

        private ushort Lay;
        public ushort Layer
        {
            get { return Lay; }
            private set
            {
                Lay = value;
                ChangeAction();
            }
        }
        public FrameInfo Frame { get; }
        public PositionInfo Position { get; }

        public TimelineObject(TimelineObjectType type, ushort layer, FrameInfo frame, PositionInfo pos)
        {
            ObjectType = type;
            Layer = layer;
            Frame = frame;
            Position = pos;
        }

        public void ChangePos(double pos_x, double pos_y, double pos_z)
        {
            Position.Change(pos_x, pos_y, pos_z);

            ChangeAction();
        }

        public void ChangePosX(double pos_x)
        {
            ChangePos(pos_x, Position.Y, Position.Z);
        }

        public void ChangePosY(double pos_y)
        {
            ChangePos(Position.X, pos_y, Position.Z);
        }

        public void ChangePosZ(double pos_z)
        {
            ChangePos(Position.X, Position.Y, pos_z);
        }

        public void ChageReference(PositionInfo.ReferencePos reference)
        {
            Position.Change(reference);
            ChangeAction();
        }

        private void ChangeAction()
        {

        }
        public abstract void AddEffect(IEffectBase effect);
        public abstract bool IsCanEffect(IEffectBase effect);
        public abstract Task[] ToTasks();

    }
}
