using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace MyShortcuts.Classes
{
    public class ListItems : INotifyPropertyChanged
    {
        public string _title { get; set; }
        public string Title
        {
            get { return _title; }
            set { _title = value; OnPropertyChanged(nameof(Title)); }
        }
        
        public string path { get; set; }
        public double nivel { get; set; }
        public Visibility error { get; set; }
        public ImageSource image { get; set; }
        public string imagePath { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
    }
}
