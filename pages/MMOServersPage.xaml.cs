﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MGM_Launcherv1._0.pages
{
    /// <summary>
    /// Interaction logic for MMOServersPage.xaml
    /// </summary>
    public partial class MMOServersPage : Page
    {
        WoWServerPage WowPage = new WoWServerPage();
        public MMOServersPage()
        {
            InitializeComponent();
            ContentFrame.Navigate(WowPage);
        }
        public void WoWServerPageClick(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new WoWServerPage()); 
            WowPage.CheckWoWInstallDirectory();
        }
        public void WoWServerPageRefresh()
        {
            ContentFrame.Navigate(new WoWServerPage());
            WowPage.CheckWoWInstallDirectory();
        }

    }
}
