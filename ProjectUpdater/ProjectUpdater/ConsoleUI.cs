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
            Console.WriteLine("3) Exit");

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
