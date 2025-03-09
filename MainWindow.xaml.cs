using MGM_Launcherv1._0.pages;
using System.Configuration;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MGM_Launcherv1._0
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MMOServersPage serversPage = new MMOServersPage();
        }
        public void MMOClick(object sender, RoutedEventArgs e)
        {
            ServerFrame.Navigate(new MMOServersPage());
        }
    }
}