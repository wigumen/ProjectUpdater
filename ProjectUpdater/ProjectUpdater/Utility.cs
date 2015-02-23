using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Microsoft.Win32;

namespace ProjectUpdater
{
    class Utility
    {
        /// <summary>
        /// Basic function for getting web text as a string
        /// </summary>
        /// <param name="URL">Web address to the plain text file</param>
        /// <returns>Returns the raw plain text</returns>
        public static string WebRead(string URL)
        {
            string DownloadedString = "";

            try
            {
                using (System.Net.WebClient client = new System.Net.WebClient())
                {
                    DownloadedString = client.DownloadString(URL);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error in Utility.WebRead: " + ex.ToString());
            }

            return DownloadedString;
        }

        /// <summary>
        /// Basic function for reading web text into a string array
        /// </summary>
        /// <param name="url">Url to textfile</param>
        /// <returns></returns>
        public static string[] WebReadLines(string url)
        {
            System.Net.WebClient client = new System.Net.WebClient();
            System.IO.Stream stream = client.OpenRead(url);
            System.IO.StreamReader reader = new System.IO.StreamReader(stream);
            List<string> list = new List<string>();

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                list.Add(line);
            }

            return list.ToArray();
        }

        /// <summary>
        /// Converts MD5 bytes to a readably HEX
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="upperCase"></param>
        /// <returns></returns>
        public static string ToHex(byte[] bytes, bool upperCase)
        {
            StringBuilder result = new StringBuilder(bytes.Length * 2);

            for (int i = 0; i < bytes.Length; i++)
                result.Append(bytes[i].ToString(upperCase ? "X2" : "x2"));

            return result.ToString();
        }

        /// <summary>
        /// Downloads from the internet
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="Path"></param>
        public static void Download(Uri URL, string Path)
        {
            using(System.Net.WebClient client = new System.Net.WebClient())
            {
                var data = client.DownloadData(URL);
                System.IO.FileInfo file = new System.IO.FileInfo(Path);
                file.Directory.Create();
                System.IO.File.WriteAllBytes(file.FullName, data);
            }
        }

        /// <summary>
        /// Gets as many paths as possible from the windows registry
        /// </summary>
        public static void InitSettingsPaths()
        {
            if (Properties.Settings.Default.Arma3Path == null || Properties.Settings.Default.Arma3Path == String.Empty)
            {
                string RegistryValue = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\bohemia interactive studio\ArmA 3", "main", null);
                if (RegistryValue != null)
                {
                    Properties.Settings.Default.Arma3Path = RegistryValue;
                }
            }

            if (Properties.Settings.Default.Arma2Path == null || Properties.Settings.Default.Arma2Path == String.Empty)
            {
                string RegistryValue = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\bohemia interactive studio\arma 2", "main", null);
                if (RegistryValue != null)
                {
                    Properties.Settings.Default.Arma2Path = RegistryValue;
                }
            }

            if (Properties.Settings.Default.Arma2OAPath == null || Properties.Settings.Default.Arma2OAPath == String.Empty)
            {
                string RegistryValue = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\bohemia interactive studio\arma 2 oa", "main", null);
                if (RegistryValue != null)
                {
                    Properties.Settings.Default.Arma2OAPath = RegistryValue;

                }
            }

            if (Properties.Settings.Default.TeamSpeak3Path == null || Properties.Settings.Default.TeamSpeak3Path == String.Empty)
            {
                string RegistryValue = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Classes\ts3file\shell\open\command", null, null);
                //Clean up the registry value a bit
                char[] trimChars = { '\"', '\\', '%', '1', ' ' };
                RegistryValue = RegistryValue.Trim(trimChars);
                if (RegistryValue.EndsWith(@"\ts3client_win32.exe") || RegistryValue.EndsWith(@"\ts3client_win64.exe"))
                    RegistryValue = RegistryValue.Remove(RegistryValue.Length - @"\ts3client_winXX.exe".Length, @"\ts3client_winXX.exe".Length);

                if (RegistryValue != null)
                {
                    Properties.Settings.Default.TeamSpeak3Path = RegistryValue;

                }
            }

            Properties.Settings.Default.Save();
        }
    }
}
