using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WorkLog;
/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();
        
    }

    private void mainNavigation_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {

        if (args.IsSettingsSelected)
        {
            contentFrame.Navigate(typeof(SettingsPage));
        }
        else if (args.SelectedItemContainer != null)
        {
            Type navPageType = Type.GetType(args.SelectedItemContainer.Tag.ToString());
            contentFrame.Navigate(navPageType);
            
        }
    }
}
