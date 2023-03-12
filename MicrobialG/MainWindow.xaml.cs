using Microsoft.VisualBasic;
using Microsoft.VisualBasic.Devices;
using Microsoft.Win32;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Input;

namespace MicrobialG
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const string UserSettingsFilename = "settings.json";
        public string _UserSettingsPath = UserSettingsFilename;
        PersonalSettings? Settings;
        public MainWindow()
        {
            InitializeComponent();

            if (File.Exists(_UserSettingsPath))
            {
                Settings = PersonalSettings.GetInstance().Read(_UserSettingsPath);
                PersonalSettings.GetInstance().Def_inputfolder = this.Settings.Def_inputfolder;
                PersonalSettings.GetInstance().Def_outputfolder = this.Settings.Def_outputfolder;
                PersonalSettings.GetInstance().IsPythonavailbale = this.Settings.IsPythonavailbale;
                PersonalSettings.GetInstance().Pythonpackageinstalled = this.Settings.Pythonpackageinstalled;
                if (this.Settings.Custom_colours != null)
                    PersonalSettings.GetInstance().Custom_colours = this.Settings.Custom_colours;
                else
                    PersonalSettings.GetInstance().Custom_colours = new Dictionary<string, string>();
            }
            else
            {
                PersonalSettings.GetInstance().Def_inputfolder = "";
                PersonalSettings.GetInstance().Def_outputfolder = "";
                PersonalSettings.GetInstance().IsPythonavailbale = false;
                PersonalSettings.GetInstance().Pythonpackageinstalled = false;
                PersonalSettings.GetInstance().Custom_colours = new Dictionary<string, string>();
            }

            CheckPythonVersion();
            
        }

        private void CheckPythonVersion()
        {
            if (!PersonalSettings.GetInstance().IsPythonavailbale)
            {
                bool download_finished = false;
                string result = "";
                ProcessStartInfo start = new ProcessStartInfo();
                start.FileName = "python.exe";
                start.Arguments = "--version";
                start.UseShellExecute = false;
                start.RedirectStandardOutput = true;
                start.RedirectStandardError = true;
                start.WindowStyle = ProcessWindowStyle.Normal;
                using (Process process = Process.Start(start))
                {
                    using (StreamReader reader = process.StandardError)
                    {
                        result = reader.ReadToEnd();
                    }
                    using (StreamReader reader = process.StandardOutput)
                    {
                        result = reader.ReadToEnd();
                        result = result.Trim();
                    }
                }

                if (result.Contains("Python 3."))
                {
                    PersonalSettings.GetInstance().IsPythonavailbale = true;
                }
                else
                {
                    MessageBoxResult installpython = MessageBox.Show("There is no proper python version installed.\nPlease press OK to proceed with the installation.\nYou will be notified after the download was successful.", "Invalid python version", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    if (installpython == MessageBoxResult.OK)
                    {
                        DownloadPythonVersion();
                        download_finished = true;
                    }
                    else
                    {
                        Environment.Exit(0);
                    }

                    if (download_finished)
                    {
                        PersonalSettings.GetInstance().IsPythonavailbale = true;
                        MessageBox.Show("Everything has been installed successfuly.\nPlease restart the application.", "Finished", MessageBoxButton.OK, MessageBoxImage.Information);
                        Application.Current.Shutdown();
                    }
                }

                PersonalSettings.GetInstance().Save(_UserSettingsPath);
            }
            else if(!PersonalSettings.GetInstance().Pythonpackageinstalled)
            {
                InstallPythonPackages();
                PersonalSettings.GetInstance().Pythonpackageinstalled = true;
                PersonalSettings.GetInstance().Save(_UserSettingsPath);
            }
        }

        private void DownloadPythonVersion()
        {
            using (var client = new WebClient())
            {
                client.DownloadFile("https://www.python.org/ftp/python/3.9.13/python-3.9.13-amd64.exe", "python-3.9.13-amd64.exe");
            }
            if (File.Exists("python-3.9.13-amd64.exe"))
            {
                MessageBox.Show("Download was successful.\nNow please follow the instructions of the installer.", "Download succesfull", MessageBoxButton.OK, MessageBoxImage.Information);
                InstallPythonVersion();
            }
            else
            {
                MessageBox.Show("Download was not successful!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            while(e.ProgressPercentage != 100)
            {
                this.Cursor = Cursors.Wait;
            }
        }

        void InstallPythonVersion()
        {
            try
            {
                using (Process exeProcess = Process.Start("python-3.9.13-amd64.exe"))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch
            {
                // Log error.
            }
        }

        void InstallPythonPackages()
        {
            MessageBox.Show("Installing necessary python packages.", "Proceed after installation", MessageBoxButton.OK, MessageBoxImage.Information);
            try
            {
                string packagefile = Path.Join(Directory.GetCurrentDirectory(), "packages.txt");
                string result = "";
                ProcessStartInfo start = new ProcessStartInfo();
                start.FileName = "python.exe";
                start.Arguments = "-m pip install -r " + packagefile;
                start.UseShellExecute = false;
                start.RedirectStandardOutput = true;
                start.RedirectStandardError = true;
                start.WindowStyle = ProcessWindowStyle.Normal;
                using (Process process = Process.Start(start))
                {
                    using (StreamReader reader = process.StandardError)
                    {
                        result = reader.ReadToEnd();
                    }
                    using (StreamReader reader = process.StandardOutput)
                    {
                        result = reader.ReadToEnd();
                        result = result.Trim();
                        MessageBox.Show(result);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            
        }
    }
}
