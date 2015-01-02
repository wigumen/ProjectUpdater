using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ProjectUpdater
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            for(int i = 0; i < args.Length; i++)
            {
                var arg = args[i];

                if (arg == "gen")
                {
                    var RepoGen = new RepoGenerator();
                    RepoGen.Generate(args[i + 1], args[i + 2]);
                }
            }
             * */

            var updater = new Updater();
            updater.UpdateRepo("http://localhost", @"C:\TestGenerate\download");
            
        }
    }
}
