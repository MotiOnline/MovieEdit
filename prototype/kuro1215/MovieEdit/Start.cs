using System;
using System.ComponentModel;
using System.IO;
using MovieEdit.IO;
using static MovieEdit.MESystem;

namespace MovieEdit
{
    public static class Start
    {
        private static FileWatcher watcher;

        public static void ProgramStart(string[] args)
        {
            if (args == null) args = Array.Empty<string>();
            Starting(args);
            CUI.Input();
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
                    Log.Warn("Debugging Mode Start...");
#if DEBUG
                    Program.DebugStart();
                    Debugging.CreateSettingFile.ExtentionData();
                    Project.Create("./", "debug", new OpenCvSharp.Size(1920, 1080));
#endif
                }
                else if (arg == "--outinfo") ConsoleInfo = true;
            }

            //Language.Load();

            Console.WriteLine(AppLocation);
            //Setting.InitLoad();
            watcher = new FileWatcher(JsonWatchPath);
            watcher.StartWatching();
        }

        private static void Ending(int code = 0)
        {
            watcher.EndWatching();
            Log.Info("プログラム終了");
            Environment.Exit(code);
        }
    }
}
