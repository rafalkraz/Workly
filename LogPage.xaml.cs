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
using WorkLog.Structure;
using Microsoft.UI.Windowing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Windows.Globalization.DateTimeFormatting;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WorkLog;

public sealed partial class LogPage : Page
{
    private List<Month> months;
    private string selectedYear;
    private Month selectedMonth;
    private bool isChanging = false;
    public static bool editLock = false;
    public static bool isEntryAddVisible = false;
    private Window h_window;
    private ObservableCollection<Entry> Entries;
    public LogPage()
    {
        this.InitializeComponent();
    }


    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        Page_SizeChanged(null, null);
        RefreshEntryList();
    }

    public void RefreshEntryList(string year = null, Month month = null)
    {
        if (Log.RefreshLog())
        {
            ChangeTimeRange(year, month);
            isChanging = false;
        }
    }

    public void ChangeTimeRange(string year = null, Month month = null)
    {
        isChanging = true;
        YearSelectionComboBox.ItemsSource = Log.Years;
        if (year != null)
        {
            selectedYear = year;
            YearSelectionComboBox.SelectedItem = year;
        }
        else
        {
            selectedYear = Log.Years[0];
            YearSelectionComboBox.SelectedItem = Log.Years[0];
        }

        months = Log.GetMonthsList(selectedYear);
        MonthSelectionComboBox.ItemsSource = months;
        if (month != null)
        {
            selectedMonth = month;
            MonthSelectionComboBox.SelectedItem = months[months.FindIndex(m => m.ToString() == month.ToString())];
        }
        else
        {
            selectedMonth = months[0];
            MonthSelectionComboBox.SelectedItem = months[0];
        }

        Entries = new(Log.GetEntries(selectedYear, selectedMonth));
        var result =
            from entry in Entries
            group entry by entry.Date.ToString("dd.MM") into g
            orderby g.Key
            select g;
        EntriesCollection.Source = result;
    }

    private void YearSelectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!isChanging)
        {
            ChangeTimeRange(YearSelectionComboBox.SelectedItem.ToString());
            isChanging = false;
        }
    }

    private void MonthSelectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!isChanging)
        {
            ChangeTimeRange(selectedYear, (Month)MonthSelectionComboBox.SelectedItem);
            isChanging = false;
        }
    }

    // SELECT ENTRY FROM LIST

    private void MonthEntriesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        LoadEntryDetails((Entry)MonthEntriesListView.SelectedItem);
    }

    private void LoadEntryDetails(Entry entry)
    {
        if (entry == null) return;
        else
        {
            switch (entry.Type)
            {
                case 0:
                    TypeTextBlock.Text = "Standardowy"; 
                    LocationTextBlock.Visibility = Visibility.Visible;
                    LocationStackPanel.Visibility = Visibility.Visible;
                    DescriptionTextBox.Visibility = Visibility.Visible;
                    DescriptionStackPanel.Visibility = Visibility.Visible;
                    break;
                case 1:
                    TypeTextBlock.Text = "Urlop";
                    LocationTextBlock.Visibility = Visibility.Collapsed;
                    LocationStackPanel.Visibility = Visibility.Collapsed;
                    DescriptionTextBox.Visibility = Visibility.Collapsed;
                    DescriptionStackPanel.Visibility = Visibility.Collapsed;
                    break;
                case 2:
                    TypeTextBlock.Text = "Bezp³atne wolne";
                    LocationTextBlock.Visibility = Visibility.Collapsed;
                    LocationStackPanel.Visibility = Visibility.Collapsed;
                    DescriptionTextBox.Visibility = Visibility.Collapsed;
                    DescriptionStackPanel.Visibility = Visibility.Collapsed;
                    break;
                default:
                    throw new Exception();
            }

            DateTextBox.Text = entry.Date.ToString("dd MMMM yyyy");
            DurationRangeTextBox.Text = $"{entry.DurationRange} (X h)";
            LocationTextBlock.Text = entry.Localization;
            DescriptionTextBox.Text = entry.Description;
        }
    }

    private void MonthEntriesListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (AllEntriesButton.Visibility == Visibility.Visible) { RootSplitView.IsPaneOpen = false; }
    }

    private void SaveEntryButton_Click(object sender, RoutedEventArgs e)
    {
        //log.SaveLog(selectedYear);
    }

    private async void EditEntryButton_Click(object sender, RoutedEventArgs e)
    {
        //if (!editLock && !isEntryAddVisible)
        //{
        //    h_window = new HelperWindow(this, (Entry)MonthEntriesListView.SelectedItem, HelperWindow.Action.Edit);
        //    h_window.Activate();
        //}
        //else
        //{
        //    ContentDialog dialog = new ContentDialog();

        //    dialog.XamlRoot = this.XamlRoot;
        //    dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
        //    if (editLock) { dialog.Title = "Zakoñcz najpierw edycjê poprzedniego wpisu!"; }
        //    else { dialog.Title = "Zakoñcz najpierw dodawanie nowego wpisu!"; }
        //    dialog.PrimaryButtonText = "OK";
        //    dialog.DefaultButton = ContentDialogButton.Primary;

        //    var result = await dialog.ShowAsync();
        //}

    }

    private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (AllEntriesButton.Visibility == Visibility.Visible) RootSplitView.OpenPaneLength = this.ActualWidth;
        else
        {
            var requiredPaneWidth = this.ActualWidth * 0.40;
            if (requiredPaneWidth > 1000) RootSplitView.OpenPaneLength = 1000;
            else RootSplitView.OpenPaneLength = requiredPaneWidth; 
        }
    }

    private void MoneyEntryButton_Click(object sender, RoutedEventArgs e)
    {
        if (MoneyEntryTeachingTip.IsOpen) { MoneyEntryTeachingTip.IsOpen = false; }
        else { MoneyEntryTeachingTip.IsOpen = true; }
    }

    private void AllEntriesButton_Click(object sender, RoutedEventArgs e)
    {
        RootSplitView.IsPaneOpen = true;
    }

    private async void AddEntryButton_Click(object sender, RoutedEventArgs e)
    {
        if (!editLock && !isEntryAddVisible)
        {
            h_window = new HelperWindow(this, (Entry)MonthEntriesListView.SelectedItem, HelperWindow.Action.Add);
            h_window.Activate();
        }
        else
        {
            ContentDialog dialog = new ContentDialog();

            dialog.XamlRoot = this.XamlRoot;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            if (editLock) { dialog.Title = "Zakoñcz najpierw edycjê poprzedniego wpisu!"; }
            else { dialog.Title = "Zakoñcz najpierw dodawanie nowego wpisu!"; }
            dialog.PrimaryButtonText = "OK";
            dialog.DefaultButton = ContentDialogButton.Primary;

            var result = await dialog.ShowAsync();
        }
    }

    private async void DeleteEntryButton_Click(object sender, RoutedEventArgs e)
    {
        //ContentDialog dialog = new ContentDialog();

        //// XamlRoot must be set in the case of a ContentDialog running in a Desktop app
        //dialog.XamlRoot = this.XamlRoot;
        //dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
        //dialog.Title = "Czy na pewno chcesz usun¹æ wpis?";
        //dialog.PrimaryButtonText = "Usuñ";
        //dialog.CloseButtonText = "Anuluj";
        //dialog.DefaultButton = ContentDialogButton.Primary;

        //var result = await dialog.ShowAsync();
        //if (result == ContentDialogResult.Primary)
        //{
        //    //log.DeleteEntryFromLog(selectedYear, selectedMonth, (Entry)MonthEntriesListView.SelectedItem);
        //    Entries.Remove((Entry)MonthEntriesListView.SelectedItem);
        //}
    }
}
