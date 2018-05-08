using System;
using System.IO;

namespace MyShortcuts.Classes
{
    public static class Folders
    {
        public static string getFolder()
        {
            return System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MyShortcuts");
        }

        public static bool folderExists(string path)
        {
            return Directory.Exists(path);
        }

        public static bool isFolder(string path)
        {
            try { return Directory.Exists(path); }
            catch (Exception) { return false; }
        }

        public static bool isDrive(string path)
        {
            foreach (var drive in System.IO.DriveInfo.GetDrives())
                if (drive.Name.Equals(path)) { return true; }

            return false;
        }
        
        public static void createNewFolder(string path)
        {
            Directory.CreateDirectory(path);
        }

        public static void folderChecking()
        {
            string xPath = Folders.getFolder();

            if (!folderExists(xPath))
                createNewFolder(xPath);
        }
    }
}
