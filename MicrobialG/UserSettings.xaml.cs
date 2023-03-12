using System;
using System.Collections.Generic;
using System.IO;
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
using Ookii.Dialogs;
using Ookii.Dialogs.Wpf;

namespace MicrobialG
{
    /// <summary>
    /// Interaction logic for UserSettings.xaml
    /// </summary>
    public partial class UserSettings : Window
    {
        public const string UserSettingsFilename = "settings.json";
        public string _UserSettingsPath = UserSettingsFilename;
        static string inputfoldervalue = "";
        static string outputfoldervalue = "";
        public UserSettings()
        {
            InitializeComponent();
            def_input.Text = PersonalSettings.GetInstance().Def_inputfolder;
            def_output.Text = PersonalSettings.GetInstance().Def_outputfolder;
        }
        private void Change_settings(object sender, RoutedEventArgs e)
        {
            def_output.IsEnabled = true;
        }
        private void Changein_settings(object sender, RoutedEventArgs e)
        {
            def_input.IsEnabled = true;
        }
        private void Browseout_Folder(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog folderDialog = new VistaFolderBrowserDialog();
            folderDialog.Description = "Please select the folder";
            folderDialog.UseDescriptionForTitle = true;
            if ((bool)folderDialog.ShowDialog(this))
            {
                inputfoldervalue = folderDialog.SelectedPath;
                def_input.Text = inputfoldervalue;
            }        
        }
        private void Browse_Folder(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog folderDialog = new VistaFolderBrowserDialog();
            folderDialog.Description = "Please select the folder";
            folderDialog.UseDescriptionForTitle = true;
            if ((bool)folderDialog.ShowDialog(this))
            {
                outputfoldervalue = folderDialog.SelectedPath;
                def_output.Text = outputfoldervalue;
            }
        }

        private void Save_settings(object sender, RoutedEventArgs e)
        {
            if(inputfoldervalue != "")
                PersonalSettings.GetInstance().Def_inputfolder = inputfoldervalue;
            if (outputfoldervalue != "")
                PersonalSettings.GetInstance().Def_outputfolder = outputfoldervalue;
            
            PersonalSettings.GetInstance().Save(_UserSettingsPath);
        }
    }
}
