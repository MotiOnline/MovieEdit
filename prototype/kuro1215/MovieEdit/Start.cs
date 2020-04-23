using MovieEdit.Timeline;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MovieEdit
{
    class Start
    {
        public static void Main(string[] args)
        {
            foreach (var arg in args)
            {
                switch (arg)
                {
                    case "--outinfo":
                        System.ConsoleInfo = true;
                        break;
                }
            }
        }
    }
}
