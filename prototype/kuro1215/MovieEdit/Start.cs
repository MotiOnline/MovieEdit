using System;
using MovieEdit.IO;
using static MovieEdit.MESystem;

namespace MovieEdit
{
    public class Start
    {
        private static FileWatcher watcher;

        public static void Main(string[] args)
        {
            Starting(args);
            CUI.CUIInput();
            Ending();
        }

        private static void Starting(string[] args)
        {
            for (int i = 0;i < args.Length;i++)
            {
                var arg = args[i];
                if(arg == "--debug")
                {
                    ConsoleInfo = true;
                    Log(LogType.Warn, "Debugging Mode Start...");
                    Program.DebugStart();
                }
                else if (arg == "--outinfo") ConsoleInfo = true;
            }

            new Setting();
            Debugging.CreateSettingFile.ExtentionData();
            watcher = new FileWatcher(JsonWatchPath);
            watcher.StartWatching();
        }

        private static void Ending(int code = 0)
        {
            watcher.EndWatching();
            Log("プログラム終了");
            Environment.Exit(code);
        }
    }
}
