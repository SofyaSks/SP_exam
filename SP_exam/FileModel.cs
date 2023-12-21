using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP_exam
{
    internal class FileModel: INotifyPropertyChanged
    {
        private string path;

        public string Path
        {
            get => path;
            set { 
                path = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(path)));
            }
        }

        private long size;

        public long Size
        {
            get => size;
            set {
                size = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(size)));
            }
        }

        private long copied;

        public long Copied
        {
            get => copied;
            set
            {
                copied = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(copied)));
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        
    }
}
