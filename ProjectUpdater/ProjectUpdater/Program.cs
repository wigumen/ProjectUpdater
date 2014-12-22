using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zip;

namespace ProjectUpdater
{
    class Program
    {
        /// <summary>
        /// Main class for SlickUpdater
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //Parse all arguments
            /*
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "update")
                {

                }

                if (args[i] == "gen")
                {
                    var gen = new RepoGenerator();
                    gen.ConvertFolder(@"D:\Games\SteamApps\common\Arma 3\@stui");
                }
            }*/

            var gen = new RepoGenerator();
            gen.ConvertFolder(@"D:\Games\SteamApps\common\Arma 3\@stui", @"D:\Games\SteamApps\common\Arma 3\RepoGenTest");

            Console.Read();
        }
    }
}
