﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

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



    }
}
