using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace Cadwisetestwork1
{
    static class MainCollection
    {
        public static ObservableCollection<Parser> parsers = new ObservableCollection<Parser>();
    }
    static class Common
    {
        static Options? options;
        public static void Opener()
        {
            if (options == null)
            {
                options = new Options();
                options.Show();
            }
        }
        public static string? OpenSource()
        {
            var OpenDialog = new Microsoft.Win32.OpenFileDialog();
            OpenDialog.Filter = "Text documents (.txt)|*.txt";
            if (OpenDialog.ShowDialog() == true)
            {
                return OpenDialog.FileName;
            }
            return null;
        }
        public static string? OpenDest()
        {
            var SaveDialog = new Microsoft.Win32.SaveFileDialog();
            SaveDialog.Filter = "Text documents (.txt)|*.txt";
            if (SaveDialog.ShowDialog() == true)
            {
                return SaveDialog.FileName;
            }
            return null;
        }
        public static void Starter(Parser parser)
        {
            if(parser.Path != parser.outPath)
            {
                MainCollection.parsers.Add(parser);
                options.Close();
                options = null;
            }
            else
            {
                MessageBox.Show("Файл источник не должен совпадать с выходным файлом.");
            }
        }
    }

    public class Parser : INotifyPropertyChanged
    {
        public string path, outPath, sights;
        public int minLen = 0;
        public bool setDelite, setReplace;
        double progress;
        public string Path { get { return path; } set { path = value; OnPropertyChanged("Path"); } }
        public double Progress { get { return progress; } set { progress = value; OnPropertyChanged("Progress"); } }
        public Parser()
        {
            Task.Run(() => Do_work());
        }
        private void Do_work() 
        {
            int cnt = 0, mpl = 1;
            bool isSeeked = true;
            using FileStream file = File.Open(path, FileMode.Open);
            if (File.Exists(outPath))
            {
                File.Delete(outPath);
            }
            using FileStream opt = File.Open(outPath, FileMode.Create);
            while (true)
            {
                Progress = 100 * (double)file.Position / file.Length;
                int tmp = file.ReadByte();
                bool sight = false;
                foreach (char i in sights)
                {
                    if (tmp == i)
                    {
                        sight = true;
                        break;
                    }
                }
                if (tmp == ' ' || sight || tmp == -1 || tmp == '\n' || tmp == '\r')
                {
                    int tmpLen = minLen * mpl;
                    if (cnt < tmpLen && tmpLen > 0 && cnt != 0)
                    {
                        opt.Seek(-cnt, SeekOrigin.Current);
                        isSeeked = false;
                    }
                    cnt = 0;
                }
                else
                {
                    cnt++;
                }
                if (sight)
                {
                    if (setReplace)
                    {
                        opt.WriteByte((byte)' ');
                    }
                }
                else
                {
                    if (isSeeked || tmp != ' ')
                    {
                        opt.WriteByte((byte)tmp);
                    }
                    isSeeked = true;
                }
                if (((byte)tmp & 0x80) == 0x00) mpl = 1;
                else if (((byte)tmp & 0xE0) == 0xC0) mpl = 2;
                else if (((byte)tmp & 0xF0) == 0xE0) mpl = 3;
                else if (((byte)tmp & 0xF8) == 0xF0) mpl = 4;
                if (tmp == -1)
                {
                    opt.SetLength(opt.Position - 1);
                    return;
                }
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
    }
}
