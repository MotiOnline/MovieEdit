using System;
using System.Text.Json.Serialization;
using static MovieEdit.IO.DialogInfo;

namespace MovieEdit.IO
{
    class Dialog
    {
        public static bool Waiting { get; internal set; }
        public static bool Result { private get; set; }
        public static DateTime WaitingFileDate { get; private set; }

        public static bool ShowDialog(DialogType type, string title, string msg)
        {
            Waiting = true;
            var dialog = new DialogInfo(type, title, msg);
            Json.WriteJsonFile("MsgBox", dialog);
            while (Waiting) ;
            return Result;
        }
    }

    public struct DialogInfo
    {
        public enum DialogType
        {
            OK, YesNo, YesNoCancel
        }

        [JsonPropertyName("Type")]
        public string DialogTypeStr { get; }
        public string Title { get; }
        public string Message { get; }

        internal DialogInfo(DialogType type, string title, string msg)
        {
            DialogTypeStr = type.ToString();
            Title = title;
            Message = msg;
        }
    }
}
