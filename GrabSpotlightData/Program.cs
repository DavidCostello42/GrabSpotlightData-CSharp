using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrabSpotlightData
{
    /// <summary>
    /// This is a simple Command Line tool to extract the Microsoft 
    /// Spotlight imagery to local folders.
    /// 
    /// The application runs once then terminates. To fully utilise this 
    /// you should have this triggered by a Scheduled Task or other cron
    /// service.
    /// 
    /// To configure the directory path information use the settings.ini
    /// file that should be present with the application. If one is not
    /// present it will be automatically created on first run using built
    /// in default values.
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            Program p = new Program();
            p.Process();
            Console.WriteLine("Finished.");
        }

        public String SpotlightDataPath { get; set; }
        public String CollectionPath { get; set; }
        public String CollectionPathPngs { get; set; }
        public String CollectionPathLandscapes { get; set; }
        public String CollectionPathPortraits { get; set; }

        private String IniFilePath = String.Format(@"{0}\settings.ini", AppDomain.CurrentDomain.BaseDirectory).Replace(@"\\", @"\");


        public Program()
        {
            IniSettings.SetIniPath(IniFilePath);
            if (!IniSettings.IniFileExists())
            {
                //Default settings.ini values
                IniSettings.SetValue("spotlight_path", GetConfigurationValue("spotlight_path"));
                IniSettings.SetValue("output_path", GetConfigurationValue("output_path"));
                IniSettings.SetValue("output_pngs", GetConfigurationValue("output_pngs"));
                IniSettings.SetValue("output_landscapes", GetConfigurationValue("output_landscapes"));
                IniSettings.SetValue("output_portraits", GetConfigurationValue("output_portraits"));
            }

            SpotlightDataPath = String.Format(IniSettings.GetValue("spotlight_path").Replace("{username}", "{0}"), Environment.UserName);
            CollectionPath = String.Format(IniSettings.GetValue("output_path").Replace("{username}", "{0}"), Environment.UserName);
            CollectionPathPngs = String.Format(IniSettings.GetValue("output_pngs").Replace("{username}", "{0}"), Environment.UserName);
            CollectionPathLandscapes = String.Format(IniSettings.GetValue("output_landscapes").Replace("{username}", "{0}"), Environment.UserName);
            CollectionPathPortraits = String.Format(IniSettings.GetValue("output_portraits").Replace("{username}", "{0}"), Environment.UserName);
        }

        public void Process()
        {
            if (!Directory.Exists(SpotlightDataPath))
            {
                Console.WriteLine("The Spotlight Directory could not be found.");
                return;
            }
            if (!Directory.Exists(CollectionPath))
            {
                Console.WriteLine("Creating the Collection directory.");
                Directory.CreateDirectory(CollectionPath);
            }
            if (!Directory.Exists(CollectionPathPngs))
            {
                Console.WriteLine("Creating the Collection PNGs directory.");
                Directory.CreateDirectory(CollectionPathPngs);
            }
            if (!Directory.Exists(CollectionPathLandscapes))
            {
                Console.WriteLine("Creating the Collection Landscapes directory.");
                Directory.CreateDirectory(CollectionPathLandscapes);
            }
            if (!Directory.Exists(CollectionPathPortraits))
            {
                Console.WriteLine("Creating the Collection Portraits directory.");
                Directory.CreateDirectory(CollectionPathPortraits);
            }
            foreach (String filePath in Directory.GetFiles(SpotlightDataPath))
            {
                if (File.Exists(filePath))
                {
                    String mimeType = MimeTypeDetection.getMimeFromFile(filePath);
                    String ext = "";
                    switch (mimeType.ToLower())
                    {
                        case "image/jpeg":
                        case "image/x-citrix-jpeg":
                        case "image/pjpeg":
                            ext = "jpg";
                            break;
                        case "image/gif":
                            ext = "gif";
                            break;
                        case "image/png":
                        case "image/x-citrix-png":
                        case "image/x-png":
                            ext = "png";
                            break;
                        case "image/bmp":
                            ext = "bmp";
                            break;
                        default:
                            continue;
                    }
                    FileInfo fi = new FileInfo(filePath);
                    String newFile = String.Format(@"{0}\{1}.{2}", CollectionPath, fi.Name, ext);
                    // If the file is PNG, send it to the PNG directory.
                    if (ext.Equals("png")) { newFile = String.Format(@"{0}\{1}.{2}", CollectionPathPngs, fi.Name, ext); }
                    // Check the orientation
                    // But only for jpg's where the file size is larger than 100KB
                    if (fi.Length > 102400 && ext.Equals("jpg"))
                    {
                        System.Drawing.Image img = System.Drawing.Image.FromFile(filePath);
                        if (img.Width > img.Height)
                        {
                            newFile = String.Format(@"{0}\{1}.{2}", CollectionPathLandscapes, fi.Name, ext);
                        }
                        else if (img.Height > img.Width)
                        {
                            newFile = String.Format(@"{0}\{1}.{2}", CollectionPathPortraits, fi.Name, ext);
                        }
                    }
                    if (File.Exists(newFile)) { continue; }
                    File.Copy(filePath, newFile);
                }
            }
        }

        /// <summary>
        /// Simple error-protection wrapper around the App.config AppSettings value retrieval process.
        /// </summary>
        /// <param name="key">Key from the app.config/appsettings xml to retrieve.</param>
        /// <returns>The value stored in appsettings (if there is one), else String.Empty is returned.</returns>
        private String GetConfigurationValue(String key)
        {
            try
            {
                return (String.IsNullOrEmpty(ConfigurationManager.AppSettings[key]) ? "" : ConfigurationManager.AppSettings[key]);
            }
            catch (Exception e)
            {
                return String.Empty;
            }
        }
    }
}
