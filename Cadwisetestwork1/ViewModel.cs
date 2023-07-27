using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;

namespace Cadwisetestwork1
{
    public class ViewModel: INotifyPropertyChanged
    {
        Window1 wnd;
        public int Len { get; set; } = 0;
        bool del, rep;
        public bool Del { get { return del; } set { del = value; DelSet = del; OnPropertyChanged("DelSet"); OnPropertyChanged("Rep"); } }
        public bool DelSet { get; set; }
        public bool Rep { get { return rep & del; } set { rep = value; } }
        public string sights = ";.,:-";
        public string Sights { get { return sights; } set { sights = value; OnPropertyChanged("Sights"); } }
        public bool startSet = false;
        public bool StartSet { get { return startSet; } set { startSet = value; OnPropertyChanged("StartSet"); } }
        public string? path = null, outpath = null;
        public string? Path { get { return path; } set { path = value; if (outpath != null) StartSet = true; else StartSet = false; OnPropertyChanged("Path"); } }
        public string? OutPath { get { return outpath; } set { outpath = value; if (path != null) StartSet = true; else StartSet = false; OnPropertyChanged("Path"); } }
        public ObservableCollection<Parser> Parsers { get; set; }
        public ViewModel()
        {
            Parsers = new ObservableCollection<Parser>();
        }
        public RelayCommand Show
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    wnd = new Window1(this);
                    wnd.Show();
                });
            }
        }
        public RelayCommand Source
        {
            get
            {
                return new RelayCommand(obj =>
                { 
                    var OpenDialog = new Microsoft.Win32.OpenFileDialog();
                    OpenDialog.Filter = "Text documents (.txt)|*.txt";
                    if (OpenDialog.ShowDialog() == true)
                    {
                        Path = OpenDialog.FileName;
                    }
                });
            }
        }
        public RelayCommand OutFile
        {
            get
            {
                return new RelayCommand(obj =>
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
                            OutPath = SaveDialog.FileName;
                        }

                    }
                });
            }
        }
        public RelayCommand Start
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    Parsers.Add(new Parser(Path, OutPath, sights, Len, Del, Rep));
                    Path = OutPath = null;
                    Parsers.Last().Start();
                    wnd.Close();
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
    public class RelayCommand : ICommand
    {
        private Action<object> execute;
        private Func<object, bool> canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return this.canExecute == null || this.canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            this.execute(parameter);
        }
    }
}
