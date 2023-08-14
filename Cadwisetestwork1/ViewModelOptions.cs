using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Cadwisetestwork1
{
    class ViewModelOptions : INotifyPropertyChanged
    {
        public int Len { get; set; } = 0;
        bool deleteSet, replaceSet;
        public bool DeleteSet { get { return deleteSet; } set { deleteSet = value; OnPropertyChanged("DeleteSet"); OnPropertyChanged("ReplaceSet"); } }
        public bool ReplaceSet { get { return replaceSet & deleteSet; } set { replaceSet = value; } }
        public string Sights { get; set; } = ";.,:-";
        public bool startSet = false;
        public bool StartSet { get { return startSet; } set { startSet = value; OnPropertyChanged("StartSet"); } }
        public string? path = null, outpath = null;
        public string? Path { get { return path; } set { path = value; if (outpath != null) StartSet = true; else StartSet = false; } }
        public string? OutPath { get { return outpath; } set { outpath = value; if (path != null) StartSet = true; else StartSet = false; } }
        public ViewModelOptions() 
        {

        }
        public RelayCommand Start
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    Common.Starter(new Parser(){
                        minLen = Len,
                        path = Path,
                        outPath = OutPath,
                        setDelete = DeleteSet,
                        setReplace = ReplaceSet,
                        sights = Sights,
                    });
                });
            }
        }
        public RelayCommand Source
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    Path = Common.OpenSource();
                });
            }
        }
        public RelayCommand OutFile
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    OutPath = Common.OpenDest();
                });
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
    }
}
