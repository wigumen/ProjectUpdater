using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zip;
using System.IO;

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
                string Mod = Modlist[i];
                Log.add("Modlist filling progress: " + (i + 1) + "/" + Modlist.Length);

                //Mod is not downloaded
                if(!Directory.Exists(Path + "\\" + Mod))
                {
                    GetAllFiles(URL + "/" + Mod);
                }

                if(Directory.Exists(Path + "\\" + Mod))
                {
                    if (File.Exists(Path + "\\" + Mod + "\\SU.version"))
                    {
                        //Mod found and Up to date with server
                        if (File.ReadAllText(Path + "\\" + Mod + "\\SU.version") == Utility.WebRead(URL + "/" + Mod + "/SU.version"))
                        {

                        }

                        //Mod found, but not up to date
                        if (File.ReadAllText(Path + "\\" + Mod + "\\SU.version") != Utility.WebRead(URL + "/" + Mod + "/SU.version"))
                        {
                            GetAllFiles(URL + "/" + Mod);
                        }
                    }
                    else
                    {
                        //Version file not found (At this point run verifyer and let that add to download queue
                        GetAllFiles(URL + "/" + Mod);
                    }
                }
            }

            DownloadExtract(Path, DownloadQueue.ToArray());
        }

        /// <summary>
        /// Class for filling updater download queue.
        /// </summary>
        /// <param name="URL">URL to root of folder of mod</param>
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

        /// <summary>
        /// Public class for actualy downloading mods, needs to be improved with verifyer and such.
        /// </summary>
        /// <param name="Path">Download path</param>
        /// <param name="Queue">Array of URI's to download</param>
        public static void DownloadExtract(string Path, Uri[] Queue)
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
