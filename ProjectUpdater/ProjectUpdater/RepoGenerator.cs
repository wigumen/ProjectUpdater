﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using Ionic.Zip;
using System.Security.Cryptography;
using Newtonsoft.Json;


namespace ProjectUpdater
{
    class RepoGenerator
    {

        /// <summary>
        /// Main repo generation thread
        /// </summary>
        /// <param name="path">File path to root folder of repo</param>
        /// <param name="outputpath">File path for output directory</param>
        public void Generate(string path, string outputpath)
        {
            //Confirm that directory exists
            if(Directory.Exists(path))
            {
                //Confirm outputpath exists
                if(Directory.Exists(outputpath))
                {
                    //Clear output directory
                    try
                    {
                        Directory.Delete(outputpath, true);
                        Directory.CreateDirectory(outputpath);
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        Log.add(e.Message);
                    }
                }
                if (!Directory.Exists(outputpath))
                {
                    try
                    {
                        Directory.CreateDirectory(outputpath);
                    } catch(UnauthorizedAccessException e)
                    {
                        Log.add(e.Message);
                    }
                }

                string[] RawAllDirectories = Directory.GetDirectories(path);
                string[] ParsedAllDirectories = new string[RawAllDirectories.Length];

                for (int i = 0; i < RawAllDirectories.Length; i++)
                {
                    ParsedAllDirectories[i] = RawAllDirectories[i].Remove(0, path.Length + 1);
                }

                File.WriteAllLines(outputpath + "\\modlist.cfg", ParsedAllDirectories);

                for (int i = 0; i < ParsedAllDirectories.Length; i++)
                {
                    Log.add("Generate progress: " + (i + 1) + "/" + (ParsedAllDirectories.Length));
                    ConvertFolder(path + "\\" + ParsedAllDirectories[i], outputpath + "\\" + ParsedAllDirectories[i]);
                }

            }
            else
            {
                throw new Exception("Could not find Repo Generator path");
            }
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
                }
                catch (UnauthorizedAccessException e)
                {
                    Log.add(e.Message);
                }
            }
            else
            {
                try
                {
                    Directory.Delete(outputpath, true);
                    Directory.CreateDirectory(outputpath);
                }catch (UnauthorizedAccessException e)
                {
                    Log.add(e.Message);
                }
            }

            if (Directory.Exists(path))
            {
                //Get all files and directories in the current directory with FULL PATH
                string[] RawAllFiles = Directory.GetFiles(path);
                string[] RawAllDirectories = Directory.GetDirectories(path);

                //All files but with trim
                string[] ParsedAllFiles = new string[RawAllFiles.Length];
                string[] ParsedAllDirectories = new string[RawAllDirectories.Length];

                // to get rid of extra "\\"
                for (int i = 0; i < RawAllFiles.Length; i++)
                {
                    ParsedAllFiles[i] = RawAllFiles[i].Remove(0, path.Length + 1);
                }

                for (int i = 0; i < RawAllDirectories.Length; i++)
                {
                    ParsedAllDirectories[i] = RawAllDirectories[i].Remove(0, path.Length + 1);
                }

                //Write data files
                File.WriteAllLines(outputpath + "\\files.cfg", ParsedAllFiles);
                File.WriteAllLines(outputpath + "\\dirs.cfg", ParsedAllDirectories);

                //Compress all known files
                foreach (string file in ParsedAllFiles)
                {
                    if (file != "SU.version")
                    {
                        using (ZipFile zip = new ZipFile())
                        {
                            Directory.SetCurrentDirectory(path);
                            zip.AddFile(file);
                            zip.Save(outputpath + "\\" + file + ".zip");

                        }

                        //Make hash of file
                        byte[] FileData;
                        using (var md5 = MD5.Create())
                        {
                            using (var stream = File.OpenRead(outputpath + "\\" + file + ".zip"))
                            {
                                FileData = md5.ComputeHash(stream);
                            }
                        }

                        string ParsedHash = Utility.ToHex(FileData, false);
                        File.WriteAllText(outputpath + "\\" + file + ".hash", ParsedHash);
                    } else if (file == "SU.version")
                    {
                        //File.Copy(path + "\\" + "SU.version", outputpath + "\\" + "SU.version");
                    }
                }

                //Re run the function for new folders
                foreach(string dir in ParsedAllDirectories)
                {
                    RepoGenerator generator = new RepoGenerator();
                    generator.ConvertFolder(path + "\\" + dir, outputpath + "\\" + dir);
                }
            }
            else
            {
                throw new Exception("Could not find RepoGen folder");
            }
        }

        /// <summary>
        /// Class for generateing version files
        /// </summary>
        public void GenerateVersionFiles(string path, string output)
        {
            string[] RawAllDirectories = Directory.GetDirectories(path);
            string[] ParsedAllDirectories = new string[RawAllDirectories.Length];
            List<Mod> Versions = new List<Mod>();

            for (int i = 0; i < RawAllDirectories.Length; i++)
            {
                ParsedAllDirectories[i] = RawAllDirectories[i].Remove(0, path.Length + 1);
            }

            foreach (string dir in ParsedAllDirectories)
            {
                Console.Write("Version for " + dir + ": ");
                var input = Console.ReadLine();
                Mod newMod = new Mod()
                {
                    mod = dir,
                    version = input
                };
                Versions.Add(newMod);
            }

            Version file = new Version()
            {
                mods = Versions
            };

            File.WriteAllText(output + "\\ModVersions.json", JsonConvert.SerializeObject(file));
        }
    }
}
