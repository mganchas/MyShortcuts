using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyShortcuts.Classes
{
    public class Saver
    {
        #region Identifiers
        private const string INIT = ">> ";
        private const string ATTRIBUTE = "(?x?) ";
        private const string SEPARATOR = "&& ";
        private const string VALUE = "(!x!) ";
        #endregion

        #region Class Paramethers
        private static readonly string path = String.Format(@"{0}\{1}", Folders.getFolder(), Definitions.FILENAME);
        private List<Definitions.EntryData> retList = new List<Definitions.EntryData>();
        #endregion

        #region Public Methods
        public static void WriteFile(List<Definitions.EntryData> values, bool append = true)
        {
            StreamWriter sWriter = new StreamWriter(path, append);

            foreach (Definitions.EntryData value in values)
                WriteCoding(ref sWriter, value);

            sWriter.Close();
        }

        public List<Definitions.EntryData> ReadFile()
        {
            if (Files.fileExists(path))
            {
                StreamReader sReader = new StreamReader(path);
                this.ReadLine(Regex.Split(sReader.ReadToEnd(), INIT));
                sReader.Close();

            }
            return retList;
        }
        #endregion

        #region Private Methods
        private void ReadLine(string[] entries)
        {
            foreach (string line in entries)
            {
                if (!String.IsNullOrEmpty(line))
                    this.ReadValues(Regex.Split(line, SEPARATOR));
            }
        }

        private void ReadValues(string[] values)
        {
            Definitions.Types sType = Definitions.Types.Drive;
            string sValue = string.Empty;
            string sName = string.Empty;

            foreach (string value in values)
            {
                if (value.IndexOf(ATTRIBUTE.Replace("x", "type").Trim()) != -1)
                    sType = GetType(value);

                else if (value.IndexOf(ATTRIBUTE.Replace("x", "value").Trim()) != -1)
                    sValue = GetValue(value);

                else if (value.IndexOf(ATTRIBUTE.Replace("x", "name").Trim()) != -1)
                    sName = GetValue(value);
            }

            if (!String.IsNullOrEmpty(sValue))
                retList.Add(new Definitions.EntryData() { type = sType, value = sValue, name = sName });
        }

        private Definitions.Types GetType(string value)
        {
            int firstIndex = value.IndexOf(VALUE.Substring(0, 2)) + 2;
            int lastIndex = value.IndexOf(VALUE.Substring(VALUE.Length - 3, 2));

            return (Definitions.Types)Definitions.getTypeFromString(value.Substring(firstIndex, lastIndex - firstIndex));
        }

        private string GetValue(string value)
        {
            int firstIndex = value.IndexOf(VALUE.Substring(0, 2)) + 2;
            int lastIndex = value.IndexOf(VALUE.Substring(VALUE.Length - 3, 2));

            return value.Substring(firstIndex, lastIndex - firstIndex);
        }

        private static void WriteCoding(ref StreamWriter sWriter, Definitions.EntryData value)
        {
            sWriter.WriteLine(
                INIT + 
                    ATTRIBUTE.Replace("x", "type") +
                    VALUE.Replace("x", value.type.ToString()) +
                    SEPARATOR +
                    ATTRIBUTE.Replace("x", "value") +
                    VALUE.Replace("x", value.value) +
                    SEPARATOR +
                    ATTRIBUTE.Replace("x", "name") +
                    VALUE.Replace("x", value.name)
            );
        }
        #endregion
    }
}
