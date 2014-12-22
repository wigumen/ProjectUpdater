using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            using(System.Net.WebClient client = new System.Net.WebClient())
            {
                DownloadedString = client.DownloadString(URL);
            }
            return DownloadedString;
        }
    }
}
