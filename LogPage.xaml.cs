using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using System.Text.Json;
using Windows.Storage;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WorkLog
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LogPage : Page
    {
        private List<StorageFile> MonthList;
        public LogPage()
        {
            this.InitializeComponent();

            ObservableCollection<Entry> Entries = MainWindow.Deserialize();
            var result =
                from entry in Entries
                group entry by entry.BeginTime.Date.ToString("dd.MM") into g
                orderby g.Key
                select g;
            EntriesCollection.Source = result;
        }

        public void UpdateMonthList(List<StorageFile> list)
        {
            MonthList = list;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var months = await MainWindow.LoadFilesAsync();
            MonthList = months;
        }
    }
}
