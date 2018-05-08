using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShortcuts.Classes
{
    public static class Files
    {
        #region Public Methods
        public static bool fileExists(string path)
        {
            return File.Exists(path);
        }
        #endregion
    }
}
