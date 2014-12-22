using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using Ionic.Zip;
using System.Security.Cryptography;

namespace ProjectUpdater
{
    class RepoGenerator
    {
        /// <summary>
        /// Main repo generation thread
        /// </summary>
        /// <param name="path">File path to root folder of repo</param>
        public void Generate(string path)
        {

        }

        /// <summary>
        /// Main method for compressing and converting files to repo's
        /// </summary>
        /// <param name="path">File path to folder</param>
        /// <param name="outputpath">File path to repogen output</param>
        public void ConvertFolder(string path, string outputpath)
        {
            if (!Directory.Exists(outputpath))
            {
                try
                {
                    Directory.CreateDirectory(outputpath);
                } catch (UnauthorizedAccessException e)
                {
                    Log.add(e.Message);
                }
            }
            else
            {
                Directory.Delete(outputpath, true);
                Directory.CreateDirectory(outputpath);
            }

            if (Directory.Exists(path))
            {
                //Get all files and directories in the current directory with FULL PATH
                string[] RawAllFiles = Directory.GetFiles(path);
                string[] RawAllDirectories = Directory.GetDirectories(path);

                //All files but with trimmed
                string[] ParsedAllFiles = new string[RawAllFiles.Length];
                string[] ParsedAllDirectories = new string[RawAllDirectories.Length];

                for (int i = 0; i < RawAllFiles.Length; i++)
                {
                    ParsedAllFiles[i] = RawAllFiles[i].Remove(0, path.Length);
                }

                for (int i = 0; i < RawAllDirectories.Length; i++)
                {
                    ParsedAllDirectories[i] = RawAllDirectories[i].Remove(0, path.Length);
                }

                //Write data files
                File.WriteAllLines(outputpath + "\\files.cfg", ParsedAllFiles);
                File.WriteAllLines(outputpath + "\\dirs.cfg", ParsedAllDirectories);

                //Compress all known files
                foreach (string file in ParsedAllFiles)
                {
                    using(ZipFile zip = new ZipFile())
                    {
                        Directory.SetCurrentDirectory(path);
                        zip.AddFile(file.Remove(0,1));
                        zip.Save(outputpath + file + ".zip");
                        
                    }

                    //Make hash of file
                    byte[] FileData;
                    using (var md5 = MD5.Create())
                    {
                        using (var stream = File.OpenRead(outputpath + file + ".zip"))
                        {
                            FileData = md5.ComputeHash(stream);
                        }
                    }

                    string ParsedHash = Utility.ToHex(FileData, false);
                    File.WriteAllText(outputpath + file + ".hash", ParsedHash);
                }

                //Re run the function for new folders
                foreach(string dir in ParsedAllDirectories)
                {
                    RepoGenerator generator = new RepoGenerator();
                    generator.ConvertFolder(path + dir, outputpath + dir);
                }
            }
            else
            {
                throw new Exception("Could not find RepoGen folder");
            }
        }
    }
}
