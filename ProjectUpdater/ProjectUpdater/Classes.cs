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

    public class VersionFile
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
}
