using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using com.deltamango.ini;

namespace GrabSpotlightData
{
    public static class IniSettings
    {
        private static IniFile iniFile = new IniFile();

        public static void SetIniPath(String path)
        {
            iniFile.SetPath(path);
        }

        public static String GetIniPath()
        {
            return iniFile.GetPath();
        }

        public static Boolean IniFileExists()
        {
            return iniFile.Exists();
        }

        public static Boolean SetValue(String key, String value, String section = "Main")
        {
            return iniFile.SetValue(key, value, section);
        }

        public static String GetValue(String key, String section = "Main")
        {
            return iniFile.GetValue(key, section);
        }

        public static int GetValueAsInt(String key, String section = "Main", int def = 0)
        {
            int i = def;
            int.TryParse(iniFile.GetValue(key, section), out i);
            return i;
        }
    }
}
