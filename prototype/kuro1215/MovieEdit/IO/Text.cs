using System.IO;

namespace MovieEdit.IO
{
    public class Text
    {
        public static string ReadFile(string path)
        {
            return new StreamReader(path).ReadToEnd();
        }

        public static void WriteFile(string path, string content)
        {
            new StreamWriter(path).Write(content);
        }
    }
}
