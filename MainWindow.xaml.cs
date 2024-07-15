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

    //public void CopyData()
    //{
    //    string result = Assembly.GetExecutingAssembly().Location;
    //    int index = result.LastIndexOf("\\");
    //    string dPath = $"{result.Substring(0, index)}\\2024-07.json";
    //    string destinationPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}\\WorkLog\\2024-07.json";
    //    string destinationFolder = $"{Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}\\WorkLog\\";
    //    if (!File.Exists(destinationPath))
    //    {
    //        Directory.CreateDirectory(destinationFolder);
    //        File.Copy(dPath, destinationPath, true);
    //    }
    //}

    


    public static ObservableCollection<Entry> Deserialize(string year)
    {
        string fileName = year + ".json";
        string jsonString = File.ReadAllText($"{Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}\\WorkLog\\{year}.json");
        ObservableCollection<Entry> result = JsonSerializer.Deserialize<ObservableCollection<Entry>>(jsonString);
        return result;
    }

}
