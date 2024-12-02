using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization.DateTimeFormatting;
using Windows.Security.Cryptography.Certificates;
using Windows.Storage;
using WorkLog.Structure;

namespace WorkLog;

public sealed partial class MainWindow : WinUIEx.WindowEx
{
    public MainWindow()
    {
        this.InitializeComponent();
        TitleBarTextBlock.Text = AppInfo.Current.DisplayInfo.DisplayName;
        mainNavigation.SelectedItem = mainNavigation.MenuItems[0];
    }

    public List<StorageFile> monthList;

    private async void mainNavigation_Loaded(object sender, RoutedEventArgs e)
    {
        
    }

    private void mainNavigation_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.IsSettingsSelected)
        {
            ContentFrame.Navigate(typeof(SettingsPage));
        }
        else if (args.SelectedItemContainer != null)
        {
            Type navPageType = Type.GetType(args.SelectedItemContainer.Tag.ToString());
            ContentFrame.Navigate(navPageType);
        }
    }
}
