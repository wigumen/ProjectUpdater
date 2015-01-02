using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zip;

namespace ProjectUpdater
{
    class Updater
    {
        List<Uri> DownloadQueue = new List<Uri>();

        /// <summary>
        /// The actual updater class, this downloads everything from internet repos to your local client
        /// </summary>
        /// <param name="URL">The web link to the root of the repository</param>
        /// <param name="Path">Path to where to download repository</param>
        public void UpdateRepo(string URL, string Path)
        {
            string[] Modlist = Utility.WebReadLines(URL + "/" + "modlist.cfg");

            for (int i = 0; i < Modlist.Length; i++)
            {
                Log.add("Download Queue progress: " + (i + 1) + "/" + Modlist.Length);
                GetAllFiles(URL + "/" + Modlist[i]);
            }

            DownloadExtract(Path, DownloadQueue.ToArray(), URL);
        }

        void GetAllFiles(string URL)
        {
            string[] Files = Utility.WebReadLines(URL + "/" + "files.cfg");
            string[] Dirs = Utility.WebReadLines(URL + "/" + "dirs.cfg");

            foreach(string file in Files)
            {
                DownloadQueue.Add(new Uri(URL + "/" + file + ".zip"));
            }

            foreach(string dir in Dirs)
            {
                GetAllFiles(URL + "/" + dir);
            }
        }

        public static void DownloadExtract(string Path, Uri[] Queue, string BaseURL)
        {
            int count = 0;
            foreach(Uri file in Queue)
            {
                string localPath = file.LocalPath.Replace("/", "\\");

                Utility.Download(file, Path + localPath);
                using (ZipFile zip = ZipFile.Read(Path + localPath))
                {
                    
                    foreach (ZipEntry e in zip)
                    {
                        //Really fucking long complicated statement, but basicly gets the path of the folder without the filename
                        e.Extract(Path + localPath.Remove(localPath.Length - file.Segments[file.Segments.Length - 1].Length, file.Segments[file.Segments.Length - 1].Length), ExtractExistingFileAction.OverwriteSilently);
                    }
                }
                System.IO.File.Delete(Path + localPath);

                count++;
                Log.add("Download Progress: " + count + "/" + Queue.Length);
            }
        }
    }
}
