using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ProjectUpdater
{
    public class SlickVerifier
    {
        /// <summary>
        /// Version of SlickVerifier
        /// </summary>
        public String SVversion = "2.0 ALPHA";
        List<ResultType> Results = new List<ResultType>();
        
        //Array of not allowed file extensions
        String[] NotAllowedExt = { ".version"};

        /// <summary>
        /// Verify all Files
        /// </summary>
        /// <param name="Url">Url to repo</param>
        /// <param name="Dir">Directory to mods</param>
        /// <returns>Returns a list of Results</returns>
        public List<ResultType> Verify(String Url, String Dir)
        {
            //makes it easier 
            Dir = Dir.Replace("\\", "/") + "/";

            //gets modlist
            String[] mods = WebReadLines(Url + "modlist.cfg");

            //Gets local directories
            List<String> lDirs = GetLocalDirs(Dir, Url, mods);

            //Gets remote directories
            List<String> rDirs = GetRemoteDirsWrapper(Url, lDirs);

            //Gets remote files
            List<String> rFiles = GetRemoteFiles(rDirs, Url);
            
            //Gets local files
            List<String> lFiles = GetLocalFiles(rFiles, Dir);

            //Hashes the files
            foreach (ResultType tmp in Hash(lFiles, Dir, Url))
            {
                Results.Add(tmp);
            }

            //Returns results
            return Results;
        }

        /// <summary>
        /// Gets all the local directories
        /// </summary>
        /// <param name="Dir">The Directory so scan through</param>
        /// <param name="Url">The Url to the repo</param>
        /// <param name="modlist">The list of mods (from modlist.cfg)</param>
        /// <returns>Returns list of mods available and up-to-date mods</returns>
        List<String> GetLocalDirs(String Dir, String Url, String[] modlist)
        {
            List<String> results = new List<String>();

            //Creates a temp list for all mods
            List<String> mods = new List<String>();

            //Gets a list of all directories in the path from Dir
            String[] directories = Directory.GetDirectories(Dir);

            foreach (String dir in directories)
            {
                //creates a string that is without the path (Dir)
                String tmp = dir.Replace(Dir, "");
                
                //Checks if the directory is a modfolder (starts with '@') and if rmods contains it
                if (tmp[0] == '@' && modlist.Contains(tmp) == true)
                {
                    //adds to mods
                    mods.Add(dir.Replace(Dir, ""));
                }
            }

            //Checks if every mod in modlist is in mods (list)
            foreach (String mod in modlist)
            {
                if (mods.Contains(mod) == false)
                {
                    //Adds result if the mods is not listed
                    Results.Add(new ResultType { type = Type.MissingMod, file=mod });
                }
            }

            //Checks versions for mods
            foreach (String mod in mods)
            {
                String tmppath = Dir + "/" + mod + "/SU.version";
                bool FileOk = true;

                
                if (File.Exists(tmppath) == false)
                {
                    //if the path does not exist try with a lowercase version
                    tmppath = Dir + "/" + mod + "/su.version";
                    if (File.Exists(tmppath) == false)
                    {
                        //if the file does not exist it will add it to the list and skip version check
                        Results.Add(new ResultType { type = Type.MissingSUversion, file = mod });
                        FileOk = false;
                    }
                    else
                    {
                        Results.Add(new ResultType { type=Type.WarnFileCaps, file=tmppath});
                    }
                }

                //if File is okay, check version of mod
                if (FileOk == true)
                {
                    if (CheckVersion(Dir + mod + "/SU.version", Url + mod + "/SU.version") == false)
                    {
                        //if the version is wrong, add to results
                        Results.Add(new ResultType { type = Type.WrongVersion, file = mod });
                    }
                    else
                    {
                        //if version is OK, add the mod to list
                        results.Add(mod);
                    }
                }
            }

            //returns the list of mods which exist, has a SU.version file and is the right version
            return mods;
        }
        /// <summary>
        /// Gets all the remote directories
        /// </summary>
        /// <param name="Url">Url to repo</param>
        /// <param name="lDirs">list of directories to scan through</param>
        /// <returns>Returns a list of all directories from all the mods</returns>
        List<String> GetRemoteDirsWrapper(String Url, List<String> lDirs)
        {
            List<String> dirs = new List<String>();

            //getting directories for every mod
            foreach (String mod in lDirs)
            {
                //gets all the directories for the specific mod
                List<String> rmods = GetRemoteDirs(Url + mod, mod, 0);

                //adding each mod to the list
                foreach (String rmod in rmods)
                {
                    dirs.Add(rmod);
                }
            }

            //returns list of all directories
            return dirs;
        }
        /// <summary>
        /// Gets the directories of a specific mod
        /// </summary>
        /// <param name="Url">Url to mod</param>
        /// <param name="Mod">what mod it is</param>
        /// <param name="Depth">recursive depth (ask echocode if you wonder)</param>
        /// <returns>List of directorier</returns>
        List<String> GetRemoteDirs(String Url, String Mod, int Depth)
        {
            //Creates a list of directories
            List<String> Directories = new List<String>();

            try
            {
                //get all the directories on the current url
                String[] rdirs = WebReadLines(Url + "/dirs.cfg");

                //If the depth is 0 (the highest level in the mod directory) it will add the directory to list
                if (Depth == 0)
                {
                    Directories.Add(Mod);
                }

                //if the list of remote directories have more than 0 in the list, add those to the list
                if (rdirs.Length > 0)
                {
                    for (int i = 0; i < rdirs.Length; i++)
                    {
                        Directories.Add(Mod + "/" + rdirs[i]);

                        //if the dirs.cfg file inside the folder is not empty, check that folder for more directories
                        if (CheckFileIsEmpty(Url + "/" + rdirs[i] + "/dirs.cfg") == false)
                        {
                            //use the same method to scan for more directories
                            List<String> rdirs2 = GetRemoteDirs(Url + "/" + rdirs[i], Mod + "/" + rdirs[i], (Depth + 1));

                            //add the directories found to the list of Directories
                            foreach (String dir in rdirs2)
                            {
                                Directories.Add(dir);
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Log.add("Error in GetRemoteDirectories");
            }

            //Returns the list of directories
            return Directories;
        }
        /// <summary>
        /// Gets a list of files from remote directories
        /// </summary>
        /// <param name="Dirs">List of directories it scans through</param>
        /// <param name="Url">Url to repo</param>
        /// <returns>List of all files from provided directorires</returns>
        List<String> GetRemoteFiles(List<String> Dirs, String Url)
        {
            //Creates a list for all the files
            List<String> Files = new List<String>();

            try
            {
                //Goes through all directories
                for (int i = 0; i < Dirs.Count; i++)
                {
                    //Gets all the files in folder
                    String[] tmp = WebReadLines(Url + "/" + Dirs[i] + "/files.cfg");

                    //adds all the files in tmp list to the Files (list)
                    for (int u = 0; u < tmp.Length; u++)
                    {
                        Files.Add(Dirs[i] + "/" + tmp[u]);
                    }
                }
            }
            catch(Exception ex)
            {
                Log.add("Error in GetRemoteFiles: " + ex.ToString());
            }

            //returns all the files
            return Files;
        }
        /// <summary>
        /// Gets local files
        /// </summary>
        /// <param name="RemoteFiles">List of remote files we are going to compare</param>
        /// <param name="Directory">Path of the directory the mods are in</param>
        /// <returns>Returns a list of allowed and files that exist</returns>
        List<String> GetLocalFiles(List<String> RemoteFiles, String Directory)
        {
            //Creates a list for all files
            List<String> Files = new List<String>();

            //Goes through every file in remotefiles
            foreach (String file in RemoteFiles)
            {
                //Checks if the files in the RemoteFiles exists on local
                if (File.Exists(Directory + "/" + file) == false)
                {
                    Results.Add(new ResultType { type=Type.MissingFile, file=file});
                }
                else
                {
                    //if the files is allowed add it to Files (list)
                    if (FileAllowed(Directory + "/" + file, NotAllowedExt) == true)
                    {
                        Files.Add(file);
                    }
                    else
                    {
                        Results.Add(new ResultType { type=Type.FileNotAllowed, file=file});
                    }
                }
            }

            //Returns all the local files
            return Files;
        }
        /// <summary>
        /// Hashes all the files and adds the results to the Results list
        /// </summary>
        /// <param name="Files">List of files to scan through</param>
        /// <param name="Dir">The directory the mods are in</param>
        /// <param name="Url">Url to repo</param>
        List<ResultType> Hash(List<String> Files, String Dir, String Url)
        {
            List<ResultType> Result = new List<ResultType>();
            //goes through every file
            for (int i = 0; i < Files.Count; i++)
            {
                //using MD5Compare to see if the hash for the both files are the same
                String[] hash = MD5Compare(Dir + "/" + Files[i], Url + Files[i]);

                //if the file is somewhat wrong it will be added to redownload list
                if (hash[2] == "False")
                {
                    Result.Add(new ResultType { type= Type.HashFalse, file=Files[i]});
                }
                else
                {
                    Result.Add(new ResultType { type = Type.HashTrue, file = Files[i] });
                }
            }
            return Result;
        }
        /// <summary>
        /// Checks the teamspeak plugin folder for any faulty/missing files
        /// </summary>
        /// <param name="TSDir">Teamspeak directory path</param>
        /// <param name="Dir">path to local</param>
        /// <param name="Url">URL to repo</param>
        /// <returns>list of hashed files from modfolders with inaccurate version</returns>
        public List<ResultType> TSCheck(String TSDir, String Dir, String Url)
        {
            //WARNING, THIS METHOD STILL HAVE SOME ISSUES, IS NOT PROPERLY TESTED AND IS NOT DOCUMENTED

            //Makes it easier
            Dir = Dir.Replace("\\", "/") + "/";

            //Makes it easier
            TSDir = TSDir.Replace("\\", "/");

            List<ResultType> Results = new List<ResultType>();
            List<String> list = new List<String>();
            String Mod = Utility.WebRead("https://gist.githubusercontent.com/erikalm/610aeed9f703c6146ac3/raw/");
            //Url += Mod + "/";

            if (CheckVersion(Dir + Mod + "/SU.version", Url + Mod + "/SU.version") == false)
            {
                Results.Add(new ResultType { type = Type.WrongVersion, file="TSCheck> " + Mod });
            }
            else
            {
                List<String> rDirs = GetRemoteDirs(Url + Mod + "/plugin", Mod + "/plugin", 0);

                List<String> rFiles = GetRemoteFiles(rDirs, Url);

                //Goes through every file in remotefiles
                foreach (String file in rFiles)
                {
                    //Checks if the files in the RemoteFiles exists on local
                    if (File.Exists(TSDir + "/" + file.Replace("@task_force_radio/plugin", "plugins")) == false)
                    {
                        Results.Add(new ResultType { type = Type.MissingFile, file = file.Replace("@task_force_radio/plugin", "plugins") });
                    }
                    else
                    {
                        list.Add(file);
                    }
                }

                //goes through every file
                for (int i = 0; i < list.Count; i++)
                {
                    //using MD5Compare to see if the hash for the both files are the same
                    //.Replace("@task_force_radio/plugin", "plugins")
                    String[] hash = MD5Compare(TSDir + "/" + list[i].Replace("@task_force_radio/plugin", "plugins"), Url + list[i]);

                    //if the file is somewhat wrong it will be added to redownload list
                    if (hash[2] == "False")
                    {
                        Results.Add(new ResultType { type = Type.HashFalse, file = list[i].Replace("@task_force_radio/plugin", "plugins") });
                    }
                    else
                    {
                        Results.Add(new ResultType { type = Type.HashTrue, file = list[i].Replace("@task_force_radio/plugin", "plugins") });
                    }
                }
            }

            return Results;
        }
        /// <summary>
        /// Checks for mods that needs to be updated (checks only by SU.version files)
        /// </summary>
        /// <param name="Dir">Path to the directory that the mods are located</param>
        /// <param name="Url">URL to repo</param>
        /// <returns>Returns list (of ResultType) of mods that needs updates</returns>
        public List<ResultType> CheckForUpdates(String Dir, String Url)
        {
            //Gets list of mods
            String[] mods = WebReadLines(Url + "modlist.cfg");

            //Makes it simpler for me, don't judge ;)
            Dir = Dir.Replace("\\", "/");

            //creates a list of results
            List<ResultType> Result = new List<ResultType>();
            //Creates a list of directories to scan through
            List<String> Dirs = new List<String>();

            //goes through every directory in Dir(path)
            foreach (string dir in Directory.GetDirectories(Dir))
            {
                //creates a temporary string with just the mod
                String tmp = dir.Replace(Dir + "\\", "");

                //if the modlist contains the mod and if the version between the repo and local is false; then add it to the list 
                if (mods.Contains(tmp) && CheckVersion(Dir + "/" + tmp + "/SU.version", Url + tmp + "/SU.version") == false)
                {
                    Result.Add(new ResultType { type=Type.NeedUpdate, file=dir.Replace(Dir + "\\", "")});
                }
            }

            //return the list of mods that needs updates
            return Result;
        }
        /// <summary>
        /// Get files that needs updating
        /// </summary>
        /// <param name="Dir">Path to directory of mods</param>
        /// <param name="Url">URL to repo</param>
        /// <param name="Mods">List of mods that needs updating</param>
        /// <returns>Returns list of hashed files (true or false) from the mods provided</returns>
        public List<ResultType> CheckUpdateFiles(String Dir, String Url, String[] Mods)
        {
            #region Needs consideration
            //disabled due to String[] Mods
            //String[] mods = WebReadLines(Url + "modlist.cfg");
            #endregion

            //makes it easier for me
            Dir = Dir.Replace("\\", "/");

            #region Needs consideration
            //Creates a list for directories to be scanned through
            //List<String> Dirs = new List<String>();

            //foreach (string dir in Directory.GetDirectories(Dir))
            //{
            //    String tmp = dir.Replace(Dir + "\\", "");

            //    if (tmp[0] == '@' && Mods.Contains(tmp) == true && CheckVersion(Dir + "/" + tmp + "/SU.version", Url + tmp + "/SU.version") == false)
            //    {
            //        Dirs.Add(tmp);
            //    }
            //}
            #endregion

            //Gets the remote directories (on repo)
            List<String> rDirs = GetRemoteDirsWrapper(Url, Mods.ToList()); //Dirs exchanged to Mods

            //Gets the files
            List<String> rFiles = GetRemoteFiles(rDirs, Url);
            
            //Uses the remote files and checks local ones
            List<String> lFiles = GetLocalFiles(rFiles, Dir);

            //Hashes the files provided
            List<ResultType> hashlist = Hash(lFiles, Dir, Url);

            //goes through the hashlist and adds it to Result list
            foreach (ResultType tmp in hashlist)
            {
                Results.Add(tmp);
            }

            //Returns the results
            return Results;
        }
        #region Tools
        /// <summary>
        /// Checks if the file is allowed (to scan)
        /// </summary>
        /// <param name="filename">Path to file</param>
        /// <param name="AllowedExt">List of allowed extensions</param>
        /// <returns>If file allowed returns True, else return False</returns>
        Boolean FileAllowed(String filename, String[] NotAllowedExt)
        {
            //create boolean
            bool allowed = true;

            //gets the extension of the file the method is checking
            String exten = Path.GetExtension(filename);

            //goes through all extensions and check if the file is using one of them
            foreach (string ext in NotAllowedExt)
            {
                //if it finds a extension that is the same, set it as false
                if (ext == exten || ext == exten.ToLower())
                {
                    allowed = false;
                }
                //else set as true
            }

            //returns if it is allowed
            return allowed;
        }
        /// <summary>
        /// Checks if the remote file is empty
        /// </summary>
        /// <param name="Url">Url to file</param>
        /// <returns>Returns true if the file is empty, else return False</returns>
        Boolean CheckFileIsEmpty(String Url)
        {
            Boolean FileIsEmpty = true;
            try
            {
                //Requests content length with a WebRequest
                WebRequest _Request = HttpWebRequest.Create(Url);
                _Request.Method = "HEAD";

                using (WebResponse _Resp = _Request.GetResponse())
                {
                    int Size;
                    //sets Size (int) to the contents length
                    if (int.TryParse(_Resp.Headers.Get("Content-Length"), out Size))
                    {
                        if (Size > 0)
                        {
                            //if File is empty, return True
                            FileIsEmpty = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.add("ERROR in CheckFileIsEmpty:\n" + ex.ToString());
            }

            return FileIsEmpty;
        }
        /// <summary>
        /// Checks version of the mod
        /// </summary>
        /// <param name="lMod">path to local SU.version</param>
        /// <param name="rMod">path to remote SU.version</param>
        /// <returns>True if version is the same, false if not</returns>
        Boolean CheckVersion(String lMod, String rMod)
        {
            Boolean VersionSame = false;
            try
            {
                //check if the text is the same
                if (Utility.WebRead(rMod) == File.ReadAllText(lMod))
                {
                    //if true, set VersionSame to True
                    VersionSame = true;
                }
                else
                {
                    //if false, VersionSame should be false
                    VersionSame = false;
                }
            }
            catch (Exception ex)
            {
                Log.add("ERROR in CheckVersion:\n" + ex.ToString());
            }

            //Returns the boolean
            return VersionSame;
        }
        /// <summary>
        /// Checks the MD5 hash of the remote and local file
        /// </summary>
        /// <param name="file">path to the local file</param>
        /// <param name="url">path to remote file</param>
        /// <returns>[0] local hash, [1] remote hash, [2] True/False if they are the same</returns>
        String[] MD5Compare(String file, String url)
        {
            string same = null;
            StringBuilder sb = new StringBuilder();
            FileStream fs = new FileStream(file, FileMode.Open);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] hash = md5.ComputeHash(fs);
            fs.Close();

            foreach (byte hex in hash)
            {
                sb.Append(hex.ToString("X2").ToLower());
            }

            WebClient WClient = new WebClient();
            Stream Stream = WClient.OpenRead(url + ".hash");
            StreamReader Reader = new StreamReader(Stream);
            String Hashfromnet = Reader.ReadToEnd();

            if (sb.ToString() == Hashfromnet)
            {
                same = "True";
            }
            else
            {
                same = "False";
            }

            //returns the hash from local file, the hash from the net, and if they are the same
            string[] returns = { sb.ToString(), Hashfromnet, same };
            return returns;
        }
        /// <summary>
        /// return a list of lines from a text
        /// </summary>
        /// <param name="Url">Url to file to read</param>
        /// <returns>list of lines in the file</returns>
        static String[] WebReadLines(String Url)
        {
            //Creates list in saving files
            List<String> list = new List<String>();

            //Creates a HTTPWebRequest
            HttpWebRequest _Request = (HttpWebRequest)WebRequest.Create(Url);

            //Setting the timout time to 3 seconds
            _Request.Timeout = 3000;
            _Request.ReadWriteTimeout = 3000;
            string line = "";

            try
            {
                //Creates a response
                WebResponse _Response = _Request.GetResponse();

                //Creates a stream to use
                Stream _Stream = _Response.GetResponseStream();

                //reads the stream
                StreamReader _Reader = new StreamReader(_Stream);

                //reads the stream until there is no more lines
                while ((line = _Reader.ReadLine()) != null)
                {
                    //add line to list
                    list.Add(line);
                }
            }
            catch (Exception ex)
            {
                Log.add("ERROR in WebReadLines:\nURL: " + Url + "\n\n\n" + ex.ToString());
            }

            //returns list
            return list.ToArray();
        }       
        #endregion
    }
    /// <summary>
    /// The returntype all the results are using
    /// </summary>
    public class ResultType
    {
        /// <summary>
        /// Type of Result
        /// </summary>
        public Type type { get; set; }
        /// <summary>
        /// Result Message
        /// </summary>
        public String file { get; set; }
    }

    public enum Type
    {
        NeedUpdate,
        HashFalse,
        HashTrue,
        WrongVersion,
        FileNotAllowed,
        MissingFile,
        WarnFileCaps,
        MissingSUversion,
        MissingMod
    }
}
