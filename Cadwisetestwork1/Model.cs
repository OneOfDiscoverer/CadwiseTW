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
using System.Windows.Input;

namespace Cadwisetestwork1
{
    public class Parser : INotifyPropertyChanged
    {
        string path;
        public string Path { get { return path; } set { path = value; OnPropertyChanged("Path"); } }
        string outPath;
        string sights;
        int minLen = 0;
        bool del;
        bool rep;
        double progress;
        public double Progress { get { return progress; } set { progress = value; OnPropertyChanged("Progress"); } }
        public Parser(string path, string outpath, string sights, int minlen, bool del, bool rep)
        {
            this.Path = path;
            this.outPath = outpath;
            this.sights = sights;
            this.minLen = minlen;
            this.del = del;
            this.rep = rep;
        }
        public bool Start()
        {
            if (Path == null || outPath == null || sights == null)
            {
                return false;
            }
            else
            {
                var t = Task.Run(() => Do_work()); //ждать не будем
                return true;
            }
        }
        //private void Do_work_stream() //жирный, медленный, но рабочий метод
        //{
        //    using StreamReader sr = File.OpenText(path);
        //    if (File.Exists(outPath))
        //    {
        //        File.Delete(outPath);
        //    }
        //    using StreamWriter sw = File.CreateText(outPath);
        //    string word = "";
        //    while (true)
        //    {
        //        int tmp = sr.Read();
        //        Progress = 100 * (double)sr.BaseStream.Position / sr.BaseStream.Length;
        //        var ch = () => { foreach (char i in sights) { if (tmp == i) return true; } return false; };
        //        if (EndOfWorld((char)tmp, sights + ' ' + '\n'))
        //        {
        //            if (word.Length >= minLen && minLen > 0)
        //            {
        //                sw.Write(word, 0, word.Length);
        //            }
        //            if((char)tmp == '\n')
        //            {
        //                sw.Write('\n');
        //            }
        //            word = "";
        //        }
        //        if (ch() && del)
        //        {
        //            //if (rep)
        //            //{
        //            //    word += ' ';
        //            //}
        //        }
        //        else
        //        {
        //            word += (char)tmp;
        //        }
        //        if (tmp == -1)
        //        {
        //            return;
        //        }
        //    }
        //}
        //private bool EndOfWorld(char ch, string str)
        //{
        //    foreach (char i in str) { if (ch == i) return true; } return false;
        //}
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
                var ch = () => { foreach (char i in sights) { if (tmp == i) return true; } return false; };
                cnt++;
                if (tmp == ' ' || ch() || tmp == -1 || tmp == '\n')
                {
                    int tmpLen = minLen * mpl;
                    if (cnt < tmpLen && tmpLen > 0 && cnt != 1)
                    {
                        opt.Seek(-cnt + 1, SeekOrigin.Current);
                        isSeeked = false;
                    }
                    cnt = 0;
                }
                if (ch() && del)
                {
                    if (rep)
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
                    opt.SetLength(opt.Position -1);
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
