using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.Json.Serialization;

namespace MovieEdit
{
    public static class System
    {
        private enum FileType
        {
            Movie, Picture, Audio
        }
        private static IReadOnlyDictionary<FileType, List<Extention>> Extentions { get; set; }
        [JsonIgnore]
        public static List<Extention> MovieExtentions { get { return Extentions[FileType.Movie]; } }
        [JsonIgnore]
        public static List<Extention> PictureExtentions { get { return Extentions[FileType.Picture]; } }
        [JsonIgnore]
        public static List<Extention> AudioExtentions { get { return Extentions[FileType.Audio]; } }

        public enum LogType
        {
            Message, Info, Input, Warn, Error
        }
        public static bool ConsoleInfo { private get; set; } = false;

        public static bool AddMovieExtention(string extention, string filetype = "")
        {
            return AddExtention(FileType.Movie, extention, filetype);
        }

        public static bool AddPictureExtention(string extention, string filetype = "")
        {
            return AddExtention(FileType.Picture, extention, filetype);
        }

        public static bool AddAudioExtention(string extention, string filetype = "")
        {
            return AddExtention(FileType.Audio, extention, filetype);
        }

        private static bool AddExtention(FileType type, string extention, string filetype)
        {
            var list = Extentions[type];
            if (!list.Contains(extention))
            {
                list.Add(new Extention(filetype, extention));
                var dic = new Dictionary<FileType, List<Extention>>(Extentions) { [type] = list };
                Extentions = dic;
                return true;
            }
            return false;
        }

        public static string CuiInLine(string msg)
        {
            Log(LogType.Message, msg);
            string line = Console.ReadLine().Replace("\"", "");
            Log(LogType.Input, $"InputData:\"{line}\"");
            return line;
        }

        public static void Log(object msg)
        {
            Log(LogType.Info, msg);
        }

        public static void Log(LogType type, object msg)
        {
            Log(type, "System", msg);
        }

        public static void Log(LogType type, string place, object msg)
        {
            string date = DateTime.Now.ToString("HH:mm:ss.fff");
            string[] msgs;
            if (msg.GetType().IsArray) msgs = (string[])msg;
            else if (msg.ToString().Contains("\n")) msgs = msg.ToString().Split("\n");
            else msgs = new string[] { msg.ToString() };

            foreach (string s in msgs)
            {
                string output = $"[{type}@{place}] {s}";
                switch (type)
                {
                    case LogType.Message:
                        Console.WriteLine(s);
                        break;
                    case LogType.Warn:
                    case LogType.Error:
                        Console.WriteLine(output);
                        break;
                    case LogType.Info:
                        if (ConsoleInfo) Console.WriteLine(output);
                        break;
                }
                Debug.WriteLine($"[{date}]{output}");
            }
        }

        public static bool IsMovie(string file)
        {
            return MovieExtentions.Contains(Path.GetExtension(file));
        }

        public static bool IsPicture(string file)
        {
            return PictureExtentions.Contains(Path.GetExtension(file));
        }

        public static bool IsAudio(string file)
        {
            return AudioExtentions.Contains(Path.GetExtension(file));
        }
    }

    public struct Extention
    {
        public string Type { get; private set; }
        [JsonPropertyName("Extention")]
        public string Name { get; private set; }

        public Extention(string type, string extention)
        {
            Type = type;
            Name = extention;
        }

        public static implicit operator string(Extention ext)
        {
            return ext.Name;
        }

        public static implicit operator Extention(string ext)
        {
            return new Extention("", ext);
        }

        public override bool Equals(object obj)
        {
            if (obj is string) return Name == (string)obj;
            else return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
