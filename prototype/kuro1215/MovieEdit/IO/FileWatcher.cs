using System;
using System.IO;
using System.Collections.Generic;
using MovieEdit.TL;
using static MovieEdit.MESystem;

namespace MovieEdit.IO
{
    public class FileWatcher : IDisposable
    {
        private FileSystemWatcher watcher = null;
        public string WatchingPath { get; }
        public IReadOnlyDictionary<string, Action<string>> CreatedEvents { get; private set; }

        public FileWatcher(string path, string filter = "")
        {
            WatchingPath = path;
            watcher = new FileSystemWatcher
            {
                Path = WatchingPath,
                NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite,
                IncludeSubdirectories = true,
                Filter = filter,
            };
            watcher.Created += new FileSystemEventHandler(CreatedEvent);
            var dic = new Dictionary<string, Action<string>>
            {
                { "Command", CommandEvent }, { "MsgBox", MsgBoxEvent }, { "Task", TaskEvent }
            };
        }

        public void StartWatching()
        {
            if (IsWatcher())
            {
                watcher.EnableRaisingEvents = true;
                Log(LogType.Warn, $"フォルダ\"{ WatchingPath }\"のファイル監視を開始");
            }
        }

        public void EndWatching(bool dispose = true)
        {
            if (IsWatcher())
            {
                watcher.EnableRaisingEvents = false;
                Log(LogType.Warn, $"フォルダ\"{ WatchingPath }\"のファイル監視を終了");
                if (dispose) Dispose();
            }
        }

        public string WaitResult()
        {
            if (!watcher.EnableRaisingEvents)
            {
                var result = watcher.WaitForChanged(WatcherChangeTypes.Created);
                if (result.TimedOut)
                {
                    Log(LogType.Error, "ファイル監視がタイムアウトしました");
                    return null;
                }
                else return result.Name;
            }
            else
            {
                return null;
            }
        }

        private bool IsWatcher()
        {
            if (watcher == null)
            {
                Log(LogType.Error, "指定されたFileWatcherは解放済みのため、再定義して使用してください");
                return false;
            }
            else return true;
        }

        public void Dispose()
        {
            if (IsWatcher())
            {
                watcher.Dispose();
                watcher = null;
            }
        }

        public bool AddEvent(string trigger, Action<string> action)
        {
            var dic = new Dictionary<string, Action<string>>(CreatedEvents);
            if (!dic.ContainsKey(trigger)) dic.Add(trigger, action);
            else return false;
            return true;
        }

        private void CreatedEvent(object source, FileSystemEventArgs e)
        {
            string path = e.FullPath;
            string current = path.Replace($@"{JsonWatchPath}\", "");
            if(CreatedEvents.TryGetValue(current, out var evt)) evt(path);
        }
        private void CommandEvent(string path)
        {
            string cmd = new StreamReader(path).ReadToEnd();
            Command.RunCommand(cmd);
        }
        private void MsgBoxEvent(string path)
        {
            if (FileName.IsResultFile(path))
            {
                Dialog.Result = int.Parse(Text.ReadFile(path)) == 0;
                Dialog.Waiting = false;
            }
        }
        private void TaskEvent(string path)
        {
            string json = new StreamReader(path).ReadToEnd();
            var tl = Json.ReadJsonFile<TLInfo<TimelineObject>>(json);
            OpeningProject.Timeline.AddObject(tl.Layer, tl.Frame, tl.Object);
        }
    }
}
