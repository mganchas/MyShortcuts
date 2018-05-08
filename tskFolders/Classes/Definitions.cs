using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MyShortcuts.Classes
{
    public static class Definitions
    {
        #region Constants
        public const string DRIVEIMAGE = "/MyShortcuts;component/Imagens/drive.png";
        public const string FOLDERIMAGE = "/MyShortcuts;component/Imagens/folder2.png";
        public const string FILEIMAGE = "/MyShortcuts;component/Imagens/file.png";
        public const string ERRORIMAGE = "/MyShortcuts;component/Imagens/delete2.png";
        public const string FILENAME = "shortcuts.tsksh";
        #endregion

        #region Identifiers
        public enum AddTypes
        {
            File,
            Folder
        }

        public enum Types
        {
            File,
            Folder,
            Drive
        }

        public struct DataStruct
        {
            public Types type;
            public string value;
            public string name;
        }
        #endregion

        #region Public Methods
        public static object getTypeFromString(string type)
        {
            foreach (Types aType in Enum.GetValues(typeof(Types)))
            {
                if (aType.ToString().Equals(type))
                    return aType;
            }

            return null;
        }

        public static ImageSource getImageSource(string path)
        {
            try
            {
                IntPtr xBitMap = System.Drawing.Icon.ExtractAssociatedIcon(path).ToBitmap().GetHbitmap();
                return Imaging.CreateBitmapSourceFromHBitmap(xBitMap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            catch (Exception) {
                return new BitmapImage(new Uri(getTypeOfTarget(path), UriKind.Relative));
                /* 
                    Recolher icon de pastas do windows
                    return Imaging.CreateBitmapSourceFromHBitmap(getTypeOfTarget(path).GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                */
            }
        }

        public static string getShortcutName(string path)
        {
            string nome = new DirectoryInfo(path).Name;

            if (Folders.isDrive(nome) || Folders.isFolder(path))
                return nome;
            else
                return nome.Replace(new DirectoryInfo(path).Extension, String.Empty);
        }

        public static string getShortcutType(string path)
        {
            string nome = new DirectoryInfo(path).Name;

            if (Folders.isDrive(nome))
                return "Drive";
            else if (Folders.isFolder(path))
                return "Folder";
            else
                return "File";
        }
        #endregion

        #region Private Methods
        private static string getTypeOfTarget(string path)
        {
            /* 
                Recolher icon de pastas do windows
                Icon icon = Icons.GetFolderIcon(Icons.IconSize.Large, Icons.FolderType.Open);
                return icon.ToBitmap();
            */
            if (Folders.isDrive(path))
                return Definitions.DRIVEIMAGE;
            else if (Folders.isFolder(path))
                return Definitions.FOLDERIMAGE;
            else
                return Definitions.ERRORIMAGE;
        }
        #endregion
    }
}
