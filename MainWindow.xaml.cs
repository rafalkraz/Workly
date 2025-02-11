using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel;
using Windows.Storage;

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
