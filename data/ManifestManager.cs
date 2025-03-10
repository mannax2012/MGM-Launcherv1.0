using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

namespace MGM_Launcherv1._0
{
    public class DownloadItem
    {
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public long BytesRemaining { get; set; }
        public double ProgressPercentage => FileSize > 0 ? (1 - (double)BytesRemaining / FileSize) * 100 : 0;
    }

    public class ManifestFile
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Url { get; set; }
        public string Hash { get; set; }
        public long Size { get; set; }
    }

    public class ManifestData
    {
        public List<ManifestFile> Manifest { get; set; }
    }

    public static class ManifestManager
    {
        private static readonly string BaseUrl = "http://localhost/website/wordpress/launcher/WoW/WoWLK/";
        private static readonly string ManifestFile = "manifest.json";
        private static readonly string InstallDirectory = ConfigVariables.WoWInstallDirectory;

        public static async Task CheckAndUpdateFiles(IProgress<DownloadItem> progress)
        {
            try
            {
                ManifestData manifest = await DownloadManifest();
                if (manifest == null)
                {
                    System.Windows.MessageBox.Show("Failed to download or parse manifest.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                foreach (var file in manifest.Manifest)
                {
                    string localFilePath = Path.Combine(InstallDirectory, file.Path);
                    string directoryPath = Path.GetDirectoryName(localFilePath); // Get the directory portion

                    // Ensure the directory exists
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                    bool needsDownload = !File.Exists(localFilePath) || !VerifyFileHash(localFilePath, file.Hash);

                    if (needsDownload)
                    {
                        string downloadUrl = BaseUrl + file.Path;
                        await DownloadFileWithProgress(downloadUrl, localFilePath, file.Size, progress);
                    }
                }

                System.Windows.MessageBox.Show("Update check complete!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error updating files: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static async Task<ManifestData> DownloadManifest()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    string response = await client.GetStringAsync(BaseUrl + ManifestFile);

                    if (string.IsNullOrEmpty(response))
                    {
                        Console.WriteLine("Manifest is empty or invalid.");
                        return null;
                    }

                    return JsonConvert.DeserializeObject<ManifestData>(response);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading manifest: {ex.Message}");
                    return null;
                }
            }
        }

        private static async Task DownloadFileWithProgress(string url, string destination, long totalSize, IProgress<DownloadItem> progress)
        {
            using HttpClient client = new HttpClient();
            using HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            string fileName = Path.GetFileName(destination);
            long totalBytes = totalSize > 0 ? totalSize : response.Content.Headers.ContentLength ?? 0;

            using FileStream fileStream = new FileStream(destination, FileMode.Create, FileAccess.Write, FileShare.None);
            using Stream stream = await response.Content.ReadAsStreamAsync();

            byte[] buffer = new byte[8192];
            long totalRead = 0;
            int bytesRead;

            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await fileStream.WriteAsync(buffer, 0, bytesRead);
                totalRead += bytesRead;

                DownloadItem downloadItem = new DownloadItem
                {
                    FileName = fileName,
                    FileSize = totalBytes,
                    BytesRemaining = totalBytes - totalRead
                };

                progress.Report(downloadItem);
            }

            //System.Windows.MessageBox.Show($"Downloaded: {url} to: {destination}", "Download Complete", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private static bool VerifyFileHash(string filePath, string expectedHash)
        {
            using var md5 = MD5.Create();
            using var stream = File.OpenRead(filePath);
            byte[] hashBytes = md5.ComputeHash(stream);
            string fileHash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            return fileHash == expectedHash;
        }
    }
}
