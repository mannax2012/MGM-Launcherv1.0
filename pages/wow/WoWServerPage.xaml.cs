using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32; // For file/folder selection
using System.Windows.Forms;
using MGM_Launcherv1._0.pages;
using System.Diagnostics;
using System.IO;
using System.IO;

namespace MGM_Launcherv1._0
{
    /// <summary>
    /// Interaction logic for WoWServerPage.xaml
    /// </summary>
    public partial class WoWServerPage : Page
    {
        private WoWPageViewModel _WoWPageViewModel;
        private DownloadItem _DownloadItem;

        MMOServersPage? mw;
        
        public bool WoWInstalled = ManifestManager.WoWInstalled;
        public bool WoWDirSet = false;
        public WoWServerPage()
        {
            WoWInstalled = ConfigVariables.WoWInstalled;
            InitializeComponent();
            _WoWPageViewModel = new WoWPageViewModel();
            DataContext = _WoWPageViewModel;
            CheckWoWInstallDirectory();
            SetDirectoryPathText(ConfigVariables.WoWInstallDirectory);
            setButtonText();
        }

        public void CheckWoWInstallDirectory()
        {
            if (string.IsNullOrWhiteSpace(ConfigVariables.WoWInstallDirectory))
            {
                System.Windows.MessageBox.Show("WoW install directory is not set. Please select the directory.", "Configuration Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                SelectWoWDirectory();
            }
            else
            {
                WoWDirSet = true;
                SetDirectoryPathText(ConfigVariables.WoWInstallDirectory);
            }
        }

        /// <summary>
        /// Opens a folder selection dialog to choose the WoW install directory.
        /// </summary>
        private void SelectWoWDirectory()
        {
            mw = new MMOServersPage();
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select your World of Warcraft installation directory";
                folderDialog.ShowNewFolderButton = false;

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    ConfigVariables.WoWInstallDirectory = folderDialog.SelectedPath;
                    ConfigManager.SaveConfig(); // Save to config.cfg

                    System.Windows.MessageBox.Show($"WoW install directory set to:\n{ConfigVariables.WoWInstallDirectory}", "Directory Saved", MessageBoxButton.OK, MessageBoxImage.Information);
                    WoWDirSet = true;
                    ConfigManager.SaveConfig();
                    mw.WoWServerPageRefresh();
                    SetDirectoryPathText(ConfigVariables.WoWInstallDirectory);

                }
                else
                {
                    System.Windows.MessageBox.Show("No directory selected. Some features may not work properly.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    WoWInstalled = false;
                    WoWDirSet = false;
                    SetDirectoryPathText(ConfigVariables.WoWInstallDirectory);
                }
            }
            setButtonText();
        }
        public void setButtonText()
        {
            if (WoWInstalled)
            {
                PlayInstallUpdate.Content = "Play";
            }
            else if (WoWDirSet && !WoWInstalled)
            {
                PlayInstallUpdate.Content = "Install";
            }
            else if (!WoWDirSet)
            {
                PlayInstallUpdate.Content = "Select Directory";
            }
        }
        public void changeDirectoryClick(object sender, RoutedEventArgs e)
        {
            SelectWoWDirectory();
        }
        public async void StartDownload()
        {
            var progress = new Progress<DownloadItem>(item =>
            {
                 _DownloadItem = item;
            });

            await ManifestManager.CheckAndUpdateFiles(progress);
        }

        public async void PlayInstallUpdateClick(object sender, RoutedEventArgs e)
        {
            WoWInstalled = ConfigVariables.WoWInstalled;
            if (WoWDirSet && WoWInstalled)
            {
                string exePath = Path.Combine(ConfigVariables.WoWInstallDirectory, "Wow.exe");

                if (File.Exists(exePath))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = exePath,
                        UseShellExecute = true,  // Ensures it opens normally like clicking in Explorer
                        WorkingDirectory = Path.GetDirectoryName(exePath) // Set working directory
                    });
                }
                else
                {
                    System.Windows.MessageBox.Show("Game executable not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                CheckWoWInstallDirectory();
                //StartDownload();
                await _WoWPageViewModel.StartDownload();
                setButtonText();
                // Create a new Progress<int> object
                //var progress = new Progress<int>(value =>
                // {
                //     // Update the progress bar value
                //     DownloadProgressBar.Value = value;
                //  });

                // Call the method to check and update files
                // await ManifestManager.CheckAndUpdateFiles(progress);
            }
        }

        public void SetDirectoryPathText(string path)
        {
            if (WoWDirSet)
            {
                DirectoryPathText.Text = $"{path}";
            }
        }
    }
}
