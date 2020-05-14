using System;
using System.IO;

namespace MovieEdit.IO
{
    public static class FileName
    {
        public static (DateTime?, string) GeneratePath(string type, string extention, bool result = false)
        {
            if (!IsExsitType(type)) return (null, null);
            string s = result ? "_result" : "";
            var date = DateTime.Now;
            string file = $@"{MESystem.JsonWatchPath}\{type}\{type}{date:HHmmssf}{s}.{extention}";
            return (date, file);
        }

        public static string GetDate(string path)
        {
            if (!path.Contains(MESystem.JsonWatchPath)) return null;
            string name = Path.GetFileNameWithoutExtension(path).Replace("_result", "");
            return name.Substring(name.Length - 8);
        }

        public static string GetType(string path)
        {
            if (!path.Contains(MESystem.JsonWatchPath)) return null;
            return Path.GetDirectoryName(path).Replace($@"{MESystem.JsonWatchPath}\", "");
        }

        public static bool IsExsitType(string type)
        {
            return Directory.Exists($@"{MESystem.JsonWatchPath}\{type}");
        }

        public static bool IsResultFile(string path)
        {
            return path.Contains("_result");
        }
    }
}
