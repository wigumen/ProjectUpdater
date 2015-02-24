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
        ObservableCollection<ModEntry> ModListCollection = new ObservableCollection<ModEntry>();
        Updater MainUpdater = new Updater();
        String CurrentPath = "";
        Repo CurrentRepo;
        ModEntryState Mods = null;

        public MainWindow()
        {
            InitializeComponent();
            /////^Make sure that runs before even thinking of touching GUI objects
            VersionWrapper.Init();
            Init();
            ModList.ItemsSource = ModListCollection;
        }

        public void Init()
        {
            //Init Repo dropdown
            UpdateRepoSelector();
            Utility.InitSettingsPaths();
            //Check if repo is up to date
            CheckRepoUpdated();
        }

        public void CheckRepoUpdated()
        {
            CurrentRepo = VersionWrapper.GetRepos()[RepoSelector.SelectedIndex];

            if (CurrentRepo.game == "arma3")
            {
                Mods = MainUpdater.UpdateRepo(CurrentRepo.url, Properties.Settings.Default.Arma3Path);
                if (Mods.isUptodate == false)
                {
                    launch_button.Content = "Update Arma 3";
                }
            }
            if (CurrentRepo.game == "arma2")
            {
                Mods = MainUpdater.UpdateRepo(CurrentRepo.url, Properties.Settings.Default.Arma2OAPath);
                if (MainUpdater.UpdateRepo(CurrentRepo.url, Properties.Settings.Default.Arma2OAPath).isUptodate == false)
                {
                    launch_button.Content = "Update Arma 2";
                }
            }

            string URL = CurrentRepo.url;

            ModListCollection.Clear();

            for (int i = 0; i < Mods.ModNames.Length; i++)
            {
                string tooltip = "LOLDUNNO";
                System.Windows.Media.SolidColorBrush color = Brushes.Pink;

                if (Mods.State[i] == state.New)
                {
                    color = Brushes.Blue;
                    tooltip = "New Mod Avaliable";
                }

                if (Mods.State[i] == state.Outdated)
                {
                    color = Brushes.Red;
                    tooltip = "Mod is outdated";
                }

                if (Mods.State[i] == state.Updated)
                {
                    color = Brushes.Green;
                    tooltip = "Mod is up to date";
                }

                if (Mods.State[i] == state.MissingVersion)
                {
                    color = Brushes.GhostWhite;
                    tooltip = "Mod is missing version file";
                }

                ModListCollection.Add(new ModEntry
                {
                    Color = color,
                    Tooltip = tooltip,
                    mod = Mods.ModNames[i],
                    version = Mods.version[i],
                    serverversion = Mods.versionOnServer[i]
                });
            }
        }

        void UpdateRepoSelector()
        {
            List<Repo> repos = VersionWrapper.GetRepos();
            foreach(Repo repo in repos)
            {
                RepoSelector.Items.Add(new ComboBoxItem
                {
                    Tag = repo.url,
                    Content = repo.name
                });
            }
            RepoSelector.SelectedIndex = Properties.Settings.Default.SelectedRepo;
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            CheckRepoUpdated();
        }

        private void RepoChanged(object sender, SelectionChangedEventArgs e)
        {
            Refresh_Click(null, null);
            CurrentRepo = VersionWrapper.GetRepos()[RepoSelector.SelectedIndex];
            Properties.Settings.Default.SelectedRepo = RepoSelector.SelectedIndex;
            Properties.Settings.Default.Save();
        }

        private void Options_click(object sender, RoutedEventArgs e)
        {
            Window newOptionsWindow = new Options();
            Options_Button.IsEnabled = false;
            newOptionsWindow.Closed += newOptionsWindow_Closed;
            newOptionsWindow.Show();
            this.IsEnabled = false;
        }

        private void newOptionsWindow_Closed(object sender, EventArgs e)
        {
            Options_Button.IsEnabled = true;
            this.IsEnabled = true;
            Properties.Settings.Default.Save();
        }

        private void launch_Click(object sender, RoutedEventArgs e)
        {
            if(Mods != null)
            {
                if(Mods.isUptodate == true)
                {
                    //Launch this biatch
                }
                else
                {
                    if (CurrentRepo.game == "arma3")
                        Updater.DownloadExtract(Properties.Settings.Default.Arma3Path, Updater.DownloadQueue.ToArray());

                    if (CurrentRepo.game == "arma2")
                        Updater.DownloadExtract(Properties.Settings.Default.Arma3Path, Updater.DownloadQueue.ToArray());
                
                
                }
            }
        }


    }

}
