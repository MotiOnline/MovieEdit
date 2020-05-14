using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text.Json.Serialization;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace MovieEdit
{
    public static class MESystem
    {
        internal enum FileType
        {
            Movie, Picture, Audio
        }
        internal static IReadOnlyDictionary<FileType, List<ExtentionBase>> Extentions { private get; set; }
        public static List<ExtentionBase> MovieExtentions { get { return Extentions[FileType.Movie]; } }
        public static List<ExtentionBase> PictureExtentions { get { return Extentions[FileType.Picture]; } }
        public static List<ExtentionBase> AudioExtentions { get { return Extentions[FileType.Audio]; } }
        public static string AppLocation { get; internal set; }
        public static string JsonWatchPath { get; internal set; }
        public static string DataPath { get; internal set; }
        public static Project OpeningProject { get; }

        public enum LogType
        {
            Message, Info, Input, Warn, Error
        }
        internal static bool ConsoleInfo { private get; set; } = false;

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
                list.Add(new ExtentionBase(filetype, extention));
                var dic = new Dictionary<FileType, List<ExtentionBase>>(Extentions) { [type] = list };
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
        
        public static void LogProgress(string msg, double percent)
        {
            Log(LogType.Info, $"{msg}: {Math.Round(percent, 3)}%");
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

    [Serializable()]
    public struct ExtentionBase
    {
        public string Type { get; private set; }
        [JsonPropertyName("Extention")]
        public string Name { get; private set; }
        [JsonIgnore]
        public FourCC? CC {
            get
            {
                if (CC_string == null) return null;
                else return VideoWriter.FourCC(CC_string);
            }
        }
        [JsonPropertyName("CC")]
        public string CC_string { get; }
        [JsonIgnore]
        public bool IsOutput { get; }

        public ExtentionBase(string type, string extention)
            : this(type, extention, null) { }
        public ExtentionBase(string type, string extention, string cc)
        {
            Type = type;
            if (extention[0] != '.') extention = "." + extention;
            Name = extention;
            if (cc == null)
            {
                CC_string = "";
                IsOutput = false;
            }
            else
            {
                CC_string = cc;
                IsOutput = false;
            }

        }

        public static implicit operator string(ExtentionBase ext)
        {
            return ext.Name;
        }
        public static implicit operator ExtentionBase(string ext)
        {
            return new ExtentionBase("StringExtention", ext);
        }

        public override bool Equals(object obj)
        {
            if (obj is string) return Name == (string)obj;
            else if (obj is ExtentionBase ext)
            {
                if (ext.Type == "StringExtention") return Name == ext.Name;
                else return base.Equals(ext);
            }
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

    public class Setting
    {
        public Setting()
        {
            Assembly assembly = Assembly.GetEntryAssembly();
            string path = Path.GetDirectoryName(assembly.Location);
            MESystem.AppLocation = path;
            MESystem.JsonWatchPath = $@"{path}\assets\watch";
            MESystem.DataPath = $@"{path}\data";
            var t = typeof(SettingContent);
            var list = new List<Content>();
            var flag = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
            foreach (var f in t.GetFields(flag))
                if(f.FieldType.Name == "SettingContent`1") list.Add((Content)f.GetValue(t));
            SettingContent.SettingList = list;
            LoadAll();
        }

        public static void Load(params Content[] setting)
        {
            foreach (var s in setting) s.Load();
        }

        public static void LoadAll()
        {
            Load(SettingContent.SettingList.ToArray());
        }
    }

    public static class SettingContent
    {
        public static IReadOnlyList<Content> SettingList;

        internal static SettingContent<Dictionary<MESystem.FileType, List<ExtentionBase>>> Extentions
            = new SettingContent<Dictionary<MESystem.FileType, List<ExtentionBase>>>
            ("MovieEdit.MESystem", "Extentions", ".extention");

        public static void AddSetting(Content content)
        {
            var list = new List<Content>(SettingList) { content };
            SettingList = list;
        }
    }

    public abstract class Content
    {
        protected private string FileName { get; }
        protected private FieldInfo Field { get; set; }
        protected private Content(string classname, string fieldname, string filename)
        {
            FileName = filename;
            var t = Type.GetType(classname);
            var flag = BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public;
            Field = t.GetField($"<{fieldname}>k__BackingField", flag);
            Load();
        }

        internal abstract void Load();
        internal abstract void Save();
    }

    public class SettingContent<A> : Content
    {
        internal SettingContent(string classname, string fieldname, string filename)
            : base(classname, fieldname, filename) { }

        internal override void Load()
        {
            using var stream = new FileStream($@"{MESystem.DataPath}\{FileName}", FileMode.Open, FileAccess.Read);
            BinaryFormatter bf = new BinaryFormatter();
            Field.SetValue(Field.FieldType, (A)bf.Deserialize(stream));
        }

        internal override void Save()
        {
            using var stream = new FileStream($@"{MESystem.DataPath}\{FileName}", FileMode.OpenOrCreate, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(stream, (A)Field.GetValue(Field.FieldType));
        }
    }
}
