using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Diagnostics;

namespace ProjectUpdater
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : Window
    {
        public Options()
        {
            InitializeComponent();
            FillTextFields();
        }

        private void openRPT_click(object sender, RoutedEventArgs e)
        {
            Process.Start( Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\..\local\Arma 3");
        }

        private void a3path_Update(object sender, TextChangedEventArgs e)
        {
            Properties.Settings.Default.Arma3Path = a3path_textbox.Text;
        }

        private void a2path_Update(object sender, TextChangedEventArgs e)
        {
            Properties.Settings.Default.Arma2Path = a2path_textbox.Text;
        }

        private void a2OApath_Update(object sender, TextChangedEventArgs e)
        {
            Properties.Settings.Default.Arma2OAPath = a2oapath_textbox.Text;
        }

        private void ts3path_Update(object sender, TextChangedEventArgs e)
        {
            Properties.Settings.Default.TeamSpeak3Path = tspath_textbox.Text;
        }

        /// <summary>
        /// Fills all the text fields with data in the settings
        /// </summary>
        void FillTextFields()
        {
            a3path_textbox.Text = Properties.Settings.Default.Arma3Path;

            a2path_textbox.Text = Properties.Settings.Default.Arma2Path;

            a2oapath_textbox.Text = Properties.Settings.Default.Arma2OAPath;

            tspath_textbox.Text = Properties.Settings.Default.TeamSpeak3Path;
        }
    }
}
