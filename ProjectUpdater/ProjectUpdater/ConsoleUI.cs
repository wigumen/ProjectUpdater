using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectUpdater
{
    class ConsoleUI
    {
        public static void Init()
        {
            Console.WriteLine("What do you want to do:");
            Console.WriteLine("\n1) Generate Repo");
            Console.WriteLine("2) Update Repo");
            Console.WriteLine("3) Check For Updates");
            Console.WriteLine("4) Print launch paramaters to console (PA REPO NA)");
            Console.WriteLine("5) CheckUpdateFiles");
            Console.WriteLine("6) |Test| Download file (tests fdownloader)");
            Console.WriteLine("7) Exit");

            var input = Console.ReadKey();
            Console.Clear();
            if (input.Key == ConsoleKey.D1)
            {
                Console.WriteLine("Enter outputpath: ");
                var output = Console.ReadLine();
                Console.WriteLine("Enter inputpath: ");
                var inputl = Console.ReadLine();
                RepoUI(output, inputl);
            }
            else if (input.Key == ConsoleKey.D2)
            {
                Console.WriteLine("Enter outputpath: ");
                var output = Console.ReadLine();
                Console.WriteLine("Enter url: ");
                var inputl = Console.ReadLine();
                RepoUI(output, inputl);
            }
            else if (input.Key == ConsoleKey.D3)
            {
                Console.WriteLine("Checking for updates...");
                //Creates a new SV
                SlickVerifier v = new SlickVerifier();

                //gets mods that need updates
                List<ResultType> tmp = v.CheckForUpdates(@"D:\Program Files (x86)\Steam\steamapps\common\Arma 3", "http://arma.projectawesome.net/beta/repo/");

                //prints out mods which needs to be updated
                foreach (ResultType up in tmp)
                {
                    Console.WriteLine(up.file);
                }

                //BEEEP!
                Console.Beep();
                Console.ReadLine();
            }
            else if (input.Key == ConsoleKey.D4)
            {
                //Gets list of mods
                String[] modlist = Utility.WebReadLines("http://arma.projectawesome.net/beta/repo/modlist.cfg");

                //Creates a string with launchparams
                string launchparams = "-mod=\"";

                //Goes through modlist and add each mod
                foreach (string mod in modlist)
                {
                    launchparams += mod + ";";
                }

                //Adds the end of launchparameter
                launchparams += "\"";

                //Print it out!
                Console.WriteLine(launchparams);
                //stupid console programming, this don't work yet
                //Clipboard.SetText(launchparams);
                Console.ReadLine();
            }
            else if (input.Key == ConsoleKey.D5)
            {
                //Creates new verifier
                SlickVerifier v = new SlickVerifier();

                //List of mods to scan through
                List<String> mods = new List<String>();

                //Checks for mods that needs updates
                List<ResultType> updatemods = v.CheckForUpdates(@"D:\Program Files (x86)\Steam\steamapps\common\Arma 3", "http://arma.projectawesome.net/beta/repo/");

                Console.WriteLine(updatemods.Count + " mods need updates");

                //Prints out the mods that needs updates and adds them to file
                foreach (ResultType up in updatemods)
                {
                    Console.WriteLine(up.file);
                    mods.Add(up.file);
                }

                //Gets all files that needs to be updated
                List<ResultType> list = v.CheckUpdateFiles(@"D:\Program Files (x86)\Steam\steamapps\common\Arma 3", "http://arma.projectawesome.net/beta/repo/", mods.ToArray());

                //goes through the results of list
                foreach (ResultType tmp in list)
                {
                    //if the hash is false (file is corrupted) or the file is missing, print out under type "NeedUpdate"
                    if (tmp.type == Type.HashFalse || tmp.type == Type.MissingFile)
                    {
                        Console.WriteLine("NeedUpdate || " + tmp.file);
                        //fdownloader does not work as intended right now
                        //Updater.fDownloader("http://arma.projectawesome.net/beta/repo/" + tmp.file);
                    }
                }
                //beep beep
                Console.Beep();
                Console.WriteLine("Done Scanning");
                Console.ReadLine();
            }
            else if (input.Key == ConsoleKey.D6)
            {
                Updater.fDownloader("http://arma.projectawesome.net/beta/repo/%40bwa3/addons/bwa3_common.pbo.7z");
                Console.ReadLine();
            }
            else if (input.Key == ConsoleKey.D7)
            {

            }
            else
            {
                Console.Clear();
                Init();
            }
        }

        public static void RepoUI(string outputpath, string inputpath)
        {
            var generator = new RepoGenerator();
            generator.Generate(inputpath, outputpath);
            generator.GenerateVersionFiles(inputpath, outputpath);
        }

        public static void DownloadUI(string outputpath, string url)
        {
            var updater = new Updater();
            updater.UpdateRepo(url, outputpath);
        }
    }
}
