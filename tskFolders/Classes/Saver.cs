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
        string path;
        List<Definitions.DataStruct> retList;
        #endregion

        #region Constructors
        public Saver()
        {
            this.path = String.Format(@"{0}\{1}", Folders.getFolder(), Definitions.FILENAME);
            retList = new List<Definitions.DataStruct>();
        }
        #endregion

        #region Public Methods
        public void writeFile(List<Definitions.DataStruct> values, bool append = true)
        {
            StreamWriter sWriter = new StreamWriter(path, append);

            foreach (Definitions.DataStruct value in values)
                writeCoding(ref sWriter, value);

            sWriter.Close();
        }

        public List<Definitions.DataStruct> readFile()
        {
            if (Files.fileExists(path))
            {
                StreamReader sReader = new StreamReader(path);
                this.readLine(Regex.Split(sReader.ReadToEnd(), INIT));
                sReader.Close();

            }
            return retList;
        }
        #endregion

        #region Private Methods
        private void createFile()
        {
            StreamWriter sWriter = new StreamWriter(path);
            sWriter.Close();
        }

        private void readLine(string[] entries)
        {
            foreach (string line in entries)
            {
                if (!String.IsNullOrEmpty(line))
                    this.readValues(Regex.Split(line, SEPARATOR));
            }
        }

        private void readValues(string[] values)
        {
            Definitions.Types sType = Definitions.Types.Drive;
            string sValue = string.Empty;
            string sName = string.Empty;

            foreach (string value in values)
            {
                if (value.IndexOf(ATTRIBUTE.Replace("x", "type").Trim()) != -1)
                    sType = getType(value);

                else if (value.IndexOf(ATTRIBUTE.Replace("x", "value").Trim()) != -1)
                    sValue = getValue(value);

                else if (value.IndexOf(ATTRIBUTE.Replace("x", "name").Trim()) != -1)
                    sName = getValue(value);
            }

            if (!String.IsNullOrEmpty(sValue))
                retList.Add(new Definitions.DataStruct() { type = sType, value = sValue, name = sName });
        }

        private Definitions.Types getType(string value)
        {
            int firstIndex = value.IndexOf(VALUE.Substring(0, 2)) + 2;
            int lastIndex = value.IndexOf(VALUE.Substring(VALUE.Length - 3, 2));

            return (Definitions.Types)Definitions.getTypeFromString(value.Substring(firstIndex, lastIndex - firstIndex));
        }

        private string getValue(string value)
        {
            int firstIndex = value.IndexOf(VALUE.Substring(0, 2)) + 2;
            int lastIndex = value.IndexOf(VALUE.Substring(VALUE.Length - 3, 2));

            return value.Substring(firstIndex, lastIndex - firstIndex);
        }

        private void writeCoding(ref StreamWriter sWriter, Definitions.DataStruct value)
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
