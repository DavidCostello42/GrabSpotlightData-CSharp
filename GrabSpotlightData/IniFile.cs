using System;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;

namespace com.deltamango.ini
{
    /// <summary>
    /// Create a New INI file to store or load data
    /// </summary>
    public class IniFile
    {
        private Boolean iniExists = false;
        private String path;

        [DllImport("kernel32")]
        private static extern Int64 WritePrivateProfileString(String section, String key, String val, String filePath);
        [DllImport("kernel32")]
        private static extern Int32 GetPrivateProfileString(String section, String key, String def, StringBuilder retVal, Int32 size, String filePath);

        /// <summary>
        /// Default Constructor. If called, the SetPath() method should be called before any other methods.
        /// Otherwise use the IniFile(String INIPath) method to set the ini file path on creation.
        /// </summary>
        public IniFile() { }

        /// <summary>
        /// INIFile Constructor.
        /// </summary>
        /// <param name="INIPath"></param>
        public IniFile(String INIPath)
        {
            SetPath(INIPath);
        }

        /// <summary>
        /// A File Exists check on the already defined ini file path. Sets the iniExists property on completion.
        /// </summary>
        /// <returns></returns>
        public Boolean Exists()
        {
            iniExists = Exists(path);
            return iniExists;
        }

        /// <summary>
        /// A File Exists check on the given String path. Will also validate the existance of the parent directory.
        /// </summary>
        /// <param name="INIPath">String path to validate</param>
        /// <returns>True if file exists. False on if the file or parent directory doesn't exist.</returns>
        public Boolean Exists(String INIPath)
        {
            if (String.IsNullOrEmpty(INIPath)) { return false; }
            try
            {
                FileInfo iniFi = new FileInfo(INIPath);
                if (!Directory.Exists(iniFi.DirectoryName))
                {
                    return false;
                }
                return File.Exists(iniFi.FullName);
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Simply returns the current set path to the ini file.
        /// </summary>
        /// <returns></returns>
        public String GetPath()
        {
            return path;
        }

        /// <summary>
        /// Sets the path for the INI File for reading and writing. If the file or parent directory does not exist it will be created.
        /// </summary>
        /// <param name="INIPath">String Path to the file.</param>
        /// <returns>FALSE on error, true otherwise.</returns>
        public Boolean SetPath(String INIPath)
        {
            try
            {
                FileInfo iniFi = new FileInfo(INIPath);
                path = iniFi.FullName;
                iniExists = File.Exists(path);
                if (!Directory.Exists(iniFi.DirectoryName))
                {
                    Directory.CreateDirectory(iniFi.DirectoryName);
                }
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Write Data to the INI File
        /// </summary>
        /// <param name="Key">The key - eg: "my_key_name" in "my_key_name=My Value"</param>
        /// <param name="Value">The value - eg: "My Value" in "my_key_name=My Value"</param>
        /// <param name="Section">Ini section the key can be found in. Defaults to "Main"</param>
        /// <returns>True on success. False otherwise.</returns>
        public Boolean SetValue(String Key, String Value, String Section = "Main")
        {
            try
            {
                if (!iniExists) { File.WriteAllText(path, "[Main]\r\n"); }
                WritePrivateProfileString(Section, Key, Value, this.path);
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }
        
        /// <summary>
        /// Read Data Value From the Ini File
        /// </summary>
        /// <param name="Key">The key - eg: "my_key_name" in "my_key_name=My Value"</param>
        /// <param name="Section">Ini section the key can be found in. Defaults to "Main"</param>
        /// <returns>The value stored in the key - eg: "My Value" in "my_key_name=My Value"</returns>
        public String GetValue(String Key, String Section = "Main")
        {
            if (!iniExists) { return String.Empty; }
            StringBuilder temp = new StringBuilder(255);
            Int32 i = GetPrivateProfileString(Section, Key, "", temp, 255, this.path);
            return temp.ToString();
        }
    }
}