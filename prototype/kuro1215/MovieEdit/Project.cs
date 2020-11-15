using MovieEdit.TL;
using OpenCvSharp;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MovieEdit
{
    public class Project
    {
        public static Project OpeningProject { get; private set; }
        public Size OutputSize { get; private set; }
        public string ProjectPath { get; }
        public string ProjectName { get; }
        public IReadOnlyList<string> ObjectsPath { get; }
        public Timeline Timeline { get; }

        private Project(string path, string name, Size size, Timeline timeline)
        {
            OutputSize = size;
            ProjectPath = Path.GetDirectoryName(path);
            ProjectName = name;
            Timeline = timeline;
        }

        public void ChangeSize(int width, int heigth)
        {
            OutputSize = new Size(width, heigth);
        }

        public static void Create(string path, string name, Size size)
        {
            OpeningProject = new Project(path, name, size, new Timeline());
        }

        public static Project Load(string path)
        {
            if(OpeningProject != null)
            {
                throw new System.Exception();
            }
            return Load(path);
        }

        public void Save()
        {
            SaveNewFile(ProjectPath, FileMode.Open);
        }

        public void SaveNewFile(string file, FileMode mode = FileMode.CreateNew)
        {
            using var stream = new FileStream(file, mode, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(stream, this);
        }
    }
}
