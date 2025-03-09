using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;


namespace MGM_Launcherv1._0
{
    
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
        private static readonly string InstallDirectory = ConfigVariables.WoWInstallDirectory; // Update based on your config

        public static async Task CheckAndUpdateFiles(Progress<int> progress)
        {
            try
            {
                // Download and parse the manifest
                ManifestData manifest = await DownloadManifest();
                if (manifest == null)
                {
                    System.Windows.MessageBox.Show("Failed to download or parse manifest.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Process each file in the manifest
                foreach (var file in manifest.Manifest)
                {
                    Console.WriteLine($"File: {file.Name}, Size: {file.Size}");
                    if (file.Size < 0)
                    {
                        Console.WriteLine($"Warning: Invalid file size for {file.Name}: {file.Size}");
                    }

                    string localFilePath = System.IO.Path.Combine(InstallDirectory, file.Path);
                    bool needsDownload = !File.Exists(localFilePath) || !VerifyFileHash(localFilePath, file.Hash);
                    string downloadPath = System.IO.Path.Combine(BaseUrl, file.Path);
                    if (needsDownload)
                    {
                        await DownloadFile(downloadPath, localFilePath);
                    }
                }

                if (manifest == null || manifest.Manifest == null || manifest.Manifest.Count == 0)
                {
                    Console.WriteLine("Failed to deserialize the manifest or no files in the manifest.");
                    return;
                }

                // Print details for each file in the manifest
                Console.WriteLine("Loaded Manifest Files:");
                foreach (var file in manifest.Manifest)
                {
                    Console.WriteLine($"Name: {file.Name}");
                    Console.WriteLine($"Path: {file.Path}");
                    Console.WriteLine($"URL: {file.Url}");
                    Console.WriteLine($"Hash: {file.Hash}");
                    Console.WriteLine($"Size: {file.Size} bytes");
                    Console.WriteLine("----------------------------------------");
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
                    // Directly set the full absolute URI (for testing)
                    var response = await client.GetStringAsync("http://localhost/website/wordpress/launcher/WoW/WoWLK/manifest.json");

                    if (string.IsNullOrEmpty(response))
                    {
                        Console.WriteLine("Manifest is empty or invalid.");
                        return null;
                    }

                    var manifest = JsonConvert.DeserializeObject<ManifestData>(response);
                    return manifest;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading manifest: {ex.Message}");
                    return null;
                }
            }
        }



        private static async Task DownloadFile(string url, string destination)
        {
            using HttpClient client = new HttpClient();
            byte[] data = await client.GetByteArrayAsync(url);
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(destination) ?? "");
            await File.WriteAllBytesAsync(destination, data);
            System.Windows.MessageBox.Show($"Downloaded: {destination}", "Download Complete", MessageBoxButton.OK, MessageBoxImage.Information);
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
