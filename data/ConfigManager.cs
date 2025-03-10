using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows;
using System.ComponentModel;

namespace MGM_Launcherv1._0
{
    public static class ConfigVariables
    {
        public static string WoWInstallDirectory = "";
        public static int Setting2 = 100;
        public static bool Setting3 = true;
    }

    class ConfigManager
    {
        private static readonly string ConfigFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.cfg");

        /// <summary>
        /// Checks for config file, loads or creates it.
        /// </summary>
        public static void InitializeConfig()
        {
            if (!File.Exists(ConfigFilePath))
            {
                System.Windows.MessageBox.Show("Config file not found. Creating with default values...", "Configuration", MessageBoxButton.OK, MessageBoxImage.Information);
                SaveConfig(); // Create new file with default values
            }
            else
            {
                LoadConfig();
                System.Windows.MessageBox.Show($"Config file found. Loading settings...ConfigTest: {ConfigVariables.WoWInstallDirectory}", "Configuration", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// Saves all public static fields from ConfigVariables into config.cfg.
        /// </summary>
        public static void SaveConfig()
        {
            List<string> lines = new List<string>();
            foreach (var field in typeof(ConfigVariables).GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                string name = field.Name;
                string value = field.GetValue(null)?.ToString() ?? "";
                lines.Add($"{name}={value}");
            }
            File.WriteAllLines(ConfigFilePath, lines);
        }

        /// <summary>
        /// Loads values from config.cfg into the ConfigVariables class.
        /// </summary>
        public static void LoadConfig()
        {
            if (!File.Exists(ConfigFilePath)) return;

            string[] configLines = File.ReadAllLines(ConfigFilePath);
            Dictionary<string, string> configDict = new Dictionary<string, string>();

            foreach (string line in configLines)
            {
                if (!string.IsNullOrWhiteSpace(line) && line.Contains("="))
                {
                    string[] parts = line.Split('=');
                    string key = parts[0].Trim();
                    string value = parts[1].Trim();
                    configDict[key] = value;
                }
            }

            // Update fields in ConfigVariables dynamically
            foreach (var field in typeof(ConfigVariables).GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                if (configDict.TryGetValue(field.Name, out string value))
                {
                    try
                    {
                        if (field.FieldType == typeof(int) && int.TryParse(value, out int intValue))
                            field.SetValue(null, intValue);
                        else if (field.FieldType == typeof(bool) && bool.TryParse(value, out bool boolValue))
                            field.SetValue(null, boolValue);
                        else
                            field.SetValue(null, Convert.ChangeType(value, field.FieldType));
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show($"Error loading {field.Name}: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}
