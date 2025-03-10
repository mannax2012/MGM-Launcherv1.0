using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGM_Launcherv1._0
{
    public class WoWPageViewModel : INotifyPropertyChanged
    {
        private DownloadItem _downloadProgress;

        public event PropertyChangedEventHandler PropertyChanged;

        public DownloadItem DownloadProgress
        {
            get => _downloadProgress;
            set
            {
                if (_downloadProgress != value)
                {
                    _downloadProgress = value;
                    OnPropertyChanged(nameof(DownloadProgress));
                }
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task StartDownload()
        {
            var progress = new Progress<DownloadItem>(item =>
            {
                DownloadProgress = item;
            });

            await ManifestManager.CheckAndUpdateFiles(progress);
        }
    }
}
