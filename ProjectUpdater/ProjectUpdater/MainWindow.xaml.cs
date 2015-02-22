using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;

namespace ProjectUpdater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string URL = "http://shittyplayer.com/riprip/";
        ObservableCollection<ModEntry> ListViewCollection = new ObservableCollection<ModEntry>();

        public MainWindow()
        {
            InitializeComponent();
            ModList.ItemsSource = ListViewCollection;
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            string[] Mods = Utility.WebReadLines("http://shittyplayer.com/riprip/modlist.cfg");

            List<string> Version = new List<string>();
            foreach (string mod in Mods)
            {
                Version.Add(Utility.WebRead(URL + "\\" + mod + "\\SU.version"));
            }

            ListViewCollection.Clear();

            for (int i = 0; i < Mods.Length; i++)
            {
                ListViewCollection.Add(new ModEntry
                {
                    Color = Brushes.Green,
                    mod = Mods[i],
                    version = Version.ToArray()[i],
                    serverversion = Version.ToArray()[i]
                });
            }
        }

    }

}
