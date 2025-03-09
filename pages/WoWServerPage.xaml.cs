using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32; // For file/folder selection
using System.Windows.Forms;
using MGM_Launcherv1._0.pages;

namespace MGM_Launcherv1._0
{
    /// <summary>
    /// Interaction logic for WoWServerPage.xaml
    /// </summary>
    public partial class WoWServerPage : Page
    {
        MMOServersPage? mw;
        
        public bool WoWInstalled = false;
        public bool WoWDirSet = false;
        public WoWServerPage()
        {
            InitializeComponent();
            setButtonText();
        }

        /// <summary>
        /// Checks if the WoW install directory is set; if not, prompts the user to select one.
        /// </summary>
        public void CheckWoWInstallDirectory()
        {
            if (string.IsNullOrWhiteSpace(ConfigVariables.WoWInstallDirectory))
            {
                System.Windows.MessageBox.Show("WoW install directory is not set. Please select the directory.", "Configuration Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                SelectWoWDirectory();
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
        }
        public async void PlayInstallUpdateClick(object sender, RoutedEventArgs e)
        {
            CheckWoWInstallDirectory();
            // Create a new Progress<int> object
            var progress = new Progress<int>(value =>
            {
                // Update the progress bar value
                DownloadProgressBar.Value = value;
            });

            // Call the method to check and update files
            await ManifestManager.CheckAndUpdateFiles(progress);
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
