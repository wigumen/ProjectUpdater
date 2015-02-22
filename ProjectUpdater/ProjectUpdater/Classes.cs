using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectUpdater
{
    public class Mod
    {
        public string mod { get; set; }
        public string version { get; set; }
    }

    public class Version
    {
        public List<Mod> mods { get; set; }
    }

    public class ModEntry
    {
        public System.Windows.Media.SolidColorBrush Color { get; set; }
        public string mod { get; set; }
        public string version { get; set; }
        public string serverversion { get; set; }
    }

    public class Repos
    {
        public string name { get; set; }
        public string url { get; set; }
        public string server { get; set; }
        public string joinText { get; set; }
        public string password { get; set; }
        public string game { get; set; }
    }

    public class VersionFile
    {
        public string version { get; set; }
        public string download { get; set; }
        public List<Repos> repos { get; set; }
    }
}
