using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ProjectUpdater
{
    class VersionWrapper
    {
        static string VersionFileURL = "http://www.shittyplayer.com/riprip/version.json";
        static VersionFile _VersionFile = new VersionFile();

        /// <summary>
        /// Initilization for entire class
        /// </summary>
        public static void Init()
        {
            _VersionFile = JsonConvert.DeserializeObject<VersionFile>(Utility.WebRead(VersionFileURL));
        }

        public static List<Repos> GetRepos(){ return _VersionFile.repos; }
        public static string GetVersion(){ return _VersionFile.version; }
        public static string GetUpdateDownload(){ return _VersionFile.download; }
    }
}
