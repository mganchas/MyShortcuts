using System.Windows;
using System.Windows.Media;

namespace MyShortcuts.Classes
{
    public class ListItems
    {
        public string title { get; set; }
        public string path { get; set; }
        public double nivel { get; set; }
        public Visibility error { get; set; }
        public ImageSource image { get; set; }
    }
}
