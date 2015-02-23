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
        public string Tooltip { get; set; }
        public string mod { get; set; }
        public string version { get; set; }
        public string serverversion { get; set; }
    }

    public class ModEntryState
    {
        public bool isUptodate { get; set; }
        public string[] ModNames { get; set; }
        public state[] State { get; set; }
        public string[] version { get; set; }
        public string[] versionOnServer { get; set; }
    }

    public enum state
    {
        Outdated,
        MissingVersion,
        New,
        Updated
    }

    public class Repo
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
        public List<Repo> repos { get; set; }
    }
}
