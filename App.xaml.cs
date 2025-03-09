using System.Configuration;
using System.Data;
using System.Windows;

namespace MGM_Launcherv1._0
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ConfigManager.InitializeConfig();
        }
    }

}
