﻿using System;
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
            
            /////^Make sure that runs before even thinking of touching GUI objects
            VersionWrapper.Init();
            Init();
            ModList.ItemsSource = ListViewCollection;
        }

        public void Init()
        {
            //Init Repo dropdown
            UpdateRepoSelector();
        }

        void UpdateRepoSelector()
        {
            List<Repos> repos = VersionWrapper.GetRepos();
            foreach(Repos repo in repos)
            {
                RepoSelector.Items.Add(new ComboBoxItem
                {
                    Tag = repo.url,
                    Content = repo.name
                });
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            string URL = VersionWrapper.GetRepos()[RepoSelector.SelectedIndex].url;
            string[] Mods = Utility.WebReadLines(URL + "modlist.cfg");

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
                    Color = Brushes.LightGreen,
                    mod = Mods[i],
                    version = Version.ToArray()[i],
                    serverversion = Version.ToArray()[i]
                });
            }
        }


    }

}
