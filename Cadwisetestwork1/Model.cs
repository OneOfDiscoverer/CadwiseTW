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
using System.Windows.Media.Media3D;

namespace Cadwisetestwork1
{
    public class Parser : INotifyPropertyChanged
    {
        string path, outPath, sights;
        int minLen = 0;
        bool setDelite, setReplace;
        double progress;
        public bool Ready { get; private set; }
        public string Path { get { return path; } set { path = value; OnPropertyChanged("Path"); } }
        public double Progress { get { return progress; } set { progress = value; OnPropertyChanged("Progress"); } }
        public Parser() // string path, string outpath, string sights, int minlen, bool del, bool rep)
        {
            //this.Path = path;
            //this.outPath = outpath;
            //this.sights = sights;
            //this.minLen = minlen;
            //this.setDelite = del;
            //this.setReplace = rep;
        }
        public void Start() 
        {
            Task.Run(() => Do_work());
        }
        public void OpenSource()
        {
            var OpenDialog = new Microsoft.Win32.OpenFileDialog();
            OpenDialog.Filter = "Text documents (.txt)|*.txt";
            if (OpenDialog.ShowDialog() == true)
            {
                path = OpenDialog.FileName;
            }
        }
        public void OpenDest()
        {
            var SaveDialog = new Microsoft.Win32.SaveFileDialog();
            SaveDialog.Filter = "Text documents (.txt)|*.txt";
            if (SaveDialog.ShowDialog() == true)
            {
                if (SaveDialog.FileName == Path)
                {
                    MessageBox.Show("Файл источник должен отличаться от файла назначения");
                }
                else
                {
                    outPath = SaveDialog.FileName;
                }
            }
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
                cnt++;
                bool sight = false;
                foreach (char i in sights)
                {
                    if (tmp == i)
                    {
                        sight = true;
                        break;
                    }
                }
                if (tmp == ' ' || sight || tmp == -1 || tmp == '\n')
                {
                    int tmpLen = minLen * mpl + 1;
                    if (cnt < tmpLen && tmpLen > 0 && cnt != 1)
                    {
                        opt.Seek(-cnt + 1, SeekOrigin.Current);
                        isSeeked = false;
                    }
                    cnt = 0;
                }
                if (sight && setDelite)
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
