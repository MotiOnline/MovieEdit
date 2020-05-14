using MovieEdit.Effects;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using static MovieEdit.MESystem;

namespace MovieEdit.TL
{
    public class TimelineMovie : TimelineObject
    {
        public string FilePath { get; }

        public TimelineMovie(PositionInfo pos, string path)
            : base(TimelineObjectType.Movie, pos)
        {
            if (File.Exists(path) && IsMovie(path))
            {
                FilePath = path;
            }
            else
            {
                throw new FileNotFoundException("This file was not found, so you cannot make object.", path);
            }
        }

        public override Task[] ToTasks()
        {
            throw new NotImplementedException();
        }
    }

    public class TimelinePicture : TimelineObject
    {
        private Mat Pic = null;
        [JsonIgnore]
        public Mat Picture {
            get
            {
                if (Pic == null) Pic = new Mat(FilePath);
                return Pic;
            }
            set { Pic = value; }
        }
        public string FilePath { get; }

        public TimelinePicture(PositionInfo pos, string path)
            : base(TimelineObjectType.Picture, pos)
        {
            if(File.Exists(path) && IsPicture(path))
            {
                FilePath = path;
                Picture = new Mat(path);
            }
            else
            {
                throw new FileNotFoundException("This file was not found, so you cannot make object.", path);
            }
        }

        public override Task[] ToTasks()
        {
            throw new NotImplementedException();
        }
    }

    public class TimelineAudio : TimelineObject
    {
        public string FilePath { get; }

        public TimelineAudio(PositionInfo pos, string path)
            : base(TimelineObjectType.Audio, pos)
        {
            if (File.Exists(path) && IsAudio(path))
            {
                FilePath = path;
            }
            else
            {
                throw new FileNotFoundException("This file was not found, so you cannot make object.", path);
            }
        }

        public override Task[] ToTasks()
        {
            throw new NotImplementedException();
        }
    }

    public class TimelineSquare : TimelineObject
    {
        public struct Vertex
        {
            public IReadOnlyList<Point2d> Conection { get; private set; }
            public Point2d Position { get; private set; }

            public Vertex(Point2d pos, Point2d[] conect = null)
            {
                Position = pos;
                if (conect == null) Conection = new List<Point2d>();
                else Conection = new List<Point2d>(conect);
            }

            internal void AddConection(Point2d pos)
            {
                Conection = new List<Point2d>(Conection) { pos };
            }

            internal void ChangePos(Point2d pos)
            {
                Position = pos;
            }

            internal void ReplaceConection(Point2d before, Point2d after)
            {
                var list = new List<Point2d>(Conection);
                list.Remove(before);
                list.Add(after);
                Conection = list;
            }

            public override bool Equals(object obj)
            {
                if (obj is Vertex vert)
                {
                    return Position == vert.Position;
                }
                else return false;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Position);
            }

            public static implicit operator Vertex(Point2d pos)
            {
                return new Vertex(pos);
            }
            public static implicit operator Point2d(Vertex vert)
            {
                return vert.Position;
            }
        }

        public Color Color { get; private set; }
        public IReadOnlyList<Vertex> Vertexes { get; private set; }
        public int Polygon { get { return Vertexes.Count; } } //Circle = -1

        public TimelineSquare(PositionInfo pos, Point2d[] vert)
            : base(TimelineObjectType.Square, pos)
        {
            var list = new List<Vertex>();
            /*for(int i = 0; i < vert.Length; i++)
            {
                int back = i - 1, next = i + 1;
                if (i == 0) back = vert.Length - 1;
                else if (i == vert.Length - 1) next = 0;
                list.Add(new Vertex(vert[i], new Point2d[] { vert[back], vert[next] }));
            }*/
            Vertexes = list;
        }

        public void AddVertex(Point2d pos, Point2d conect1, Point2d conect2)
        {
            var list = new List<Vertex>(Vertexes);
            if (list.Contains(pos))
            {
                Log(LogType.Error, "This position's vertex is exsisted.");
            }
            else if(list.Contains(conect1) && list.Contains(conect2))
            {
                list[list.IndexOf(conect1)].AddConection(pos);
                list[list.IndexOf(conect2)].AddConection(pos);
                var vert = new Vertex(pos, new Point2d[] { conect1, conect2 });
                list.Add(vert);
                ChangeAction();
            }
        }

        public void ChangeColor(Color color)
        {
            Color = color;
            ChangeAction();
        }

        public void ChangeVertexPos(Point2d before, Point2d after)
        {
            var list = new List<Vertex>(Vertexes);
            if (list.Contains(before))
            {
                list[list.IndexOf(before)].ChangePos(after);
                ChangeAction();
            }
        }

        public void RemoveVertex(Point2d pos)
        {
            var list = new List<Vertex>(Vertexes);
            if (list.Contains(pos))
            {
                var arr = list[list.IndexOf(pos)].Conection.ToArray();
                list[list.IndexOf(arr[0])].ReplaceConection(pos, arr[1]);
                list[list.IndexOf(arr[1])].ReplaceConection(pos, arr[0]);
                list.Remove(pos);
                Vertexes = list;
                ChangeAction();
            }
        }

        public override Task[] ToTasks()
        {
            throw new NotImplementedException();
        }
    }

    public class TimelineText : TimelineObject
    {
        public string Text { get; private set; }
        public string Font { get; private set; }
        public Color Color { get; private set; }

        public TimelineText(PositionInfo pos, string text, string font, Color color)
            : base(TimelineObjectType.Text, pos)
        {
            Text = text;
            Font = font;
            Color = Color;
        }

        public void ChangeColor(Color color)
        {
            Color = color;
            ChangeAction();
        }

        public void ChangeFont(string font)
        {
            Font = font;
            ChangeAction();
        }

        public void ChangeText(string text)
        {
            Text = text;
            ChangeAction();
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

        public TimelineObjectType ObjectType { get; }
        public PositionInfo Position { get; }
        public IReadOnlyList<EffectBase> Effects { get; private set; }
        public List<FilterBase> Filter { get; private set; }

        public TimelineObject(TimelineObjectType type, PositionInfo pos)
        {
            ObjectType = type;
            Position = pos;
        }

        public void AddEffect(EffectBase effect)
        {
            if (CanEffect(effect))
            {
                var list = new List<EffectBase>(Effects);
                list.Add(effect);
                Effects = list;
            }
            else throw new InvalidCastException();
        }

        public bool CanEffect(EffectBase effect)
        {
            return effect.CanEffect(ObjectType);
        }

        public void ChangePos(double pos_x, double pos_y)
        {
            Position.Change(pos_x, pos_y);
            ChangeAction();
        }

        public void ChangePosX(double pos_x)
        {
            ChangePos(pos_x, Position.Y);
        }

        public void ChangePosY(double pos_y)
        {
            ChangePos(Position.X, pos_y);
        }

        public void ChageReference(PositionInfo.ReferencePos reference)
        {
            Position.Change(reference);
            ChangeAction();
        }

        protected void ChangeAction()
        {

        }

        public abstract Task[] ToTasks();

    }
}
