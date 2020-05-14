using System;

namespace MovieEdit.IO
{
    public class Command
    {
        public Command()
        {
            new Cmd("get", "", new CmdArg("param", "", new CmdArg("argname", "", GetAction)));
        }

        public static void RunCommand(string cmd)
        {

        }

        private void GetAction()
        {

        }
    }

    public class Cmd
    {
        public string Name { get; }
        public int ArgsCount { get; }

        public Cmd(string name, string explain, params CmdArg[] args)
        {

        }
    }

    public class CmdArg
    {
        public string Name { get; }


        public CmdArg(string name, string explain, params CmdArg[] args)
        {

        }
        public CmdArg(string name, string expain, Action action)
        {

        }
    }


}
