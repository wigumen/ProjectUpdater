using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zip;
using System.IO;
using System.Net;
using System.ComponentModel;

namespace ProjectUpdater
{
    class Updater
    {
        public static List<Uri> DownloadQueue = new List<Uri>();

        /// <summary>
        /// The actual updater class, this checks if everything from internet repos to your local client are up to date
        /// </summary>
        /// <param name="URL">The web link to the root of the repository</param>
        /// <param name="Path">Path to where to download repository</param>
        public ModEntryState UpdateRepo(string URL, string Path)
        {
            
            string[] Modlist = Utility.WebReadLines(URL + "/" + "modlist.cfg");
            ModEntryState ModState = new ModEntryState();
            ModState.ModNames = Modlist;

            List<string> ModVersions = new List<string>();
            List<string> ServerModVersions = new List<string>();
            List<state> States = new List<state>();

            for (int i = 0; i < Modlist.Length; i++)
            {
                string Mod = Modlist[i];
                string ServerVersion = Utility.WebRead(URL + "/" + Mod + "/SU.version");
                string LocalVersion = "-";
                state State = state.New;

                Log.add("Modlist filling progress: " + (i + 1) + "/" + Modlist.Length);

                //Mod is not downloaded
                if(!Directory.Exists(Path + "\\" + Mod))
                {
                    GetAllFiles(URL + "/" + Mod);
                    State = state.New;
                }

                if(Directory.Exists(Path + "\\" + Mod))
                {
                    
                    if (File.Exists(Path + "\\" + Mod + "\\SU.version"))
                    {
                        LocalVersion = File.ReadAllText(Path + "\\" + Mod + "\\SU.version");
                        //Mod found and Up to date with server
                        if (LocalVersion == ServerVersion)
                        {
                            State = state.Updated;
                        }

                        //Mod found, but not up to date
                        if (LocalVersion != ServerVersion)
                        {
                            GetAllFiles(URL + "/" + Mod);
                            State = state.Outdated;
                        }
                    }
                    else
                    {
                        //Version file not found (At this point run verifyer and let that add to download queue
                        //GetAllFiles(URL + "/" + Mod);
                        SlickVerifier verifyer = new SlickVerifier();
                        List<ResultType> Results = verifyer.Verify(URL + "/" + Mod, Path + "\\" + Mod);
                        foreach(var result in Results)
                        {
                            if (result.type == Type.MissingFile || result.type == Type.HashFalse)
                                DownloadQueue.Add(new Uri(URL + "/" + result.file + ".zip"));
                        }
                        State = state.MissingVersion;
                    }
                }
                ModVersions.Add(LocalVersion);
                ServerModVersions.Add(ServerVersion);
                States.Add(State);
            }

            if(DownloadQueue.Count == 0)
            {
                ModState.isUptodate = true;
            }

            if(DownloadQueue.Count != 0)
            {
                ModState.isUptodate = false;
            }

            ModState.version = ModVersions.ToArray();
            ModState.versionOnServer = ServerModVersions.ToArray();
            ModState.State = States.ToArray();

                return ModState;
            //Dont need to run this if not the console UI version
            #if ConsoleMode
                DownloadExtract(Path, DownloadQueue.ToArray());
            #endif
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
                if (file != "SU.version")
                {
                    DownloadQueue.Add(new Uri(URL + "/" + file + ".zip"));
                } else
                {
                    DownloadQueue.Add(new Uri(URL + "/" + file));
                }
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
                if (file.Segments[file.Segments.Length - 1] != "SU.version")
                {
                    using (ZipFile zip = ZipFile.Read(Path + localPath))
                    {
                        //This thing can throw a IOexption if a .tmp file is already there
                        foreach (ZipEntry e in zip)
                        {
                            //Really fucking long complicated statement, but basicly gets the path of the folder without the filename
                            e.Extract(Path + localPath.Remove(localPath.Length - file.Segments[file.Segments.Length - 1].Length, file.Segments[file.Segments.Length - 1].Length), ExtractExistingFileAction.OverwriteSilently);
                        }
                    }
                    System.IO.File.Delete(Path + localPath);
                }
                

                count++;
                Log.add("Download Progress: " + count + "/" + Queue.Length);
            }
        }
        #region downloader (does not work correctly with cues)
        public static void fDownloader(string url)
        {
            string filename = System.IO.Path.GetFileName(url);
            System.Net.WebClient client = new System.Net.WebClient();
            Console.WriteLine("\n");
            client.DownloadProgressChanged += client_ProgressChanged;
            client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_Completed);
            try
            {
                client.DownloadFileAsync(new Uri(url), (filename));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: \n" + ex.ToString());
                System.Threading.Thread.Sleep(100);
            }
        }
        private static void client_ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            double bytesIn = double.Parse(e.BytesReceived.ToString());
            double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
            double percentage = bytesIn / totalBytes * 100;
            double d_recieved = ((e.BytesReceived));
            double d_total = ((e.TotalBytesToReceive));

            string[] range = setprefix(d_recieved);
            string[] range2 = setprefix(d_total);
            string per = e.ProgressPercentage.ToString();
            Console.Write("\r");
            Console.Write("\r" + range[0] + " " + range[1] + " / " + range2[0] + " " + range2[1] + " (" + per + " %)");
        }
        static String[] setprefix(Double number)
        {
            String result = null;
            String prefix = null;
            int tmp = Convert.ToInt32(number);
            if (number >= 1024 && number < 1048576)
            {
                prefix = "KB";
                result = (tmp / 1024).ToString();
            }
            if (number >= 1048576 && number < 1073741824)
            {
                prefix = "MB";
                result = (tmp / 1024 / 1024).ToString();
            }
            if (number >= 1073741824)
            {
                prefix = "GB";
                result = (tmp / 1024 / 1024 / 1024).ToString();
            }
            string[] returnresult = { result, prefix };
            return returnresult;
        }
        private static void client_Completed(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                Console.WriteLine("Download Canceled");
            }
            else
            {
                Console.WriteLine("\n\nDownload done!");
            }
        }
        #endregion
    }
}
