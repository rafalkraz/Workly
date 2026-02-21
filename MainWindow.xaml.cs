using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel;
using Windows.Storage;
using WinUIEx;

namespace Workly;

public sealed partial class MainWindow : WinUIEx.WindowEx
{

    public MainWindow()
    {
        InitializeComponent();
        TitleBarTextBlock.Text = AppInfo.Current.DisplayInfo.DisplayName;
        MainNavigation.SelectedItem = MainNavigation.MenuItems[0];
        Title = Windows.ApplicationModel.Package.Current.DisplayName;
        this.SetIcon(@"Assets\Calendar.ico");
    }

    public List<StorageFile> monthList;

    private void MainNavigation_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
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
