using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MGM_Launcherv1._0
{
    public class WoWPageViewModel : INotifyPropertyChanged
    {
        private DownloadItem _downloadProgress;

        public event PropertyChangedEventHandler PropertyChanged;

        public async Task StartDownload()
        {
            var progress = new Progress<DownloadItem>(item =>
            {
                DownloadItem _progressPercentage = item;
            });
            //System.Windows.MessageBox.Show($"Download Progress: {DownloadProgress}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            await ManifestManager.CheckAndUpdateFiles(progress);
        }
    }
}
