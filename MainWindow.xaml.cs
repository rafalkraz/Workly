using System;
using System.Collections.Generic;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel;
using Windows.Storage;

namespace Workly;

public sealed partial class MainWindow : Window
{

    public MainWindow()
    {
        InitializeComponent();
        TitleBarTextBlock.Text = AppInfo.Current.DisplayInfo.DisplayName;
        mainNavigation.SelectedItem = mainNavigation.MenuItems[0];
        AppWindow.Title = Windows.ApplicationModel.Package.Current.DisplayName;
        AppWindow.SetIcon(@"Assets\Calendar.ico");
        AppWindow.Resize(new Windows.Graphics.SizeInt32(1600, 1000));
        OverlappedPresenter presenter = OverlappedPresenter.Create();
        presenter.PreferredMinimumWidth = 570;
        AppWindow.SetPresenter(presenter);
    }

    public List<StorageFile> monthList;

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
