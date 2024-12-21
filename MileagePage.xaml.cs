using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using WorkLog.Structure;
using myLog = WorkLog.Structure.Log.Mileage;

namespace WorkLog;

public sealed partial class MileagePage : Page
{
    private List<Month> months;
    private string selectedYear;
    private Month selectedMonth;
    private bool isChanging = false;
    public static bool editLock = false;
    public static bool isEntryAddVisible = false;
    private Window h_window;
    private ObservableCollection<EntryMileage> Entries;
    public MileagePage()
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
        if (myLog.RefreshLog())
        {
            MoneyEntryButton.IsEnabled = true;
            EditEntryButton.IsEnabled = true;
            DeleteEntryButton.IsEnabled = true;
            MonthSelectionComboBox.IsEnabled = true;
            YearSelectionComboBox.IsEnabled = true;
            NoEntriesTextBlock.Visibility = Visibility.Collapsed;
            ChangeTimeRange(year, month);
            isChanging = false;
        }
        else
        {
            NoEntriesTextBlock.Visibility = Visibility.Visible;
            YearSelectionComboBox.ItemsSource = null;
            MonthSelectionComboBox.ItemsSource = null;
            EntriesCollection.Source = null;
            EntryIDTextBlock.Text = $"ID: -";
            DateTextBox.Text = "-";
            TypeTextBlock.Text = "-";
            DurationRangeTextBox.Text = "-";
            LocationTextBlock.Text = "-";
            DescriptionTextBox.Text = "";
            MoneyEntryButton.IsEnabled = false;
            EditEntryButton.IsEnabled = false;
            DeleteEntryButton.IsEnabled = false;
            MonthSelectionComboBox.IsEnabled = false;
            YearSelectionComboBox.IsEnabled = false;
            if (MonthEntriesListView.Items.Count == 0) RootSplitView.IsPaneOpen = true;
        }
    }

    public void ChangeTimeRange(string year = null, Month month = null)
    {
        isChanging = true;
        YearSelectionComboBox.ItemsSource = myLog.Years;
        if (year != null)
        {
            selectedYear = year;
            YearSelectionComboBox.SelectedItem = year;
        }
        else
        {
            if (myLog.Years.Count >= 1)
            {
                selectedYear = myLog.Years[0];
                YearSelectionComboBox.SelectedItem = myLog.Years[0];
                NoEntriesTextBlock.Visibility = Visibility.Collapsed;
            }
            else
            {
                NoEntriesTextBlock.Visibility = Visibility.Visible;
            }
        }

        months = myLog.GetMonthsList(selectedYear);
        MonthSelectionComboBox.ItemsSource = months;
        if (month != null)
        {
            selectedMonth = month;
            var a = months.FindIndex(m => m.ToString() == month.ToString());
            if (a < 0)
            {
                if (months.Count >= 1)
                {
                    selectedMonth = months[0];
                    MonthSelectionComboBox.SelectedItem = months[0];
                }
                else
                {
                    RefreshEntryList();
                    return;
                }

            }
            else
            {
                MonthSelectionComboBox.SelectedItem = months[a];
            }

        }
        else
        {
            if (months.Count >= 1)
            {
                selectedMonth = months[0];
                MonthSelectionComboBox.SelectedItem = months[0];
            }
            else
            {
                RefreshEntryList();
                return;
            }
        }

        Entries = new(myLog.GetEntries(selectedYear, selectedMonth));
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
                    TypeTextBlock.Text = "Nadgodziny";
                    LocationTextBlock.Visibility = Visibility.Visible;
                    LocationStackPanel.Visibility = Visibility.Visible;
                    DescriptionTextBox.Visibility = Visibility.Visible;
                    DescriptionStackPanel.Visibility = Visibility.Visible;
                    break;
                case 2:
                    TypeTextBlock.Text = "Urlop";
                    LocationTextBlock.Visibility = Visibility.Collapsed;
                    LocationStackPanel.Visibility = Visibility.Collapsed;
                    DescriptionTextBox.Visibility = Visibility.Collapsed;
                    DescriptionStackPanel.Visibility = Visibility.Collapsed;
                    break;
                case 3:
                    TypeTextBlock.Text = "Bezp³atne wolne";
                    LocationTextBlock.Visibility = Visibility.Collapsed;
                    LocationStackPanel.Visibility = Visibility.Collapsed;
                    DescriptionTextBox.Visibility = Visibility.Collapsed;
                    DescriptionStackPanel.Visibility = Visibility.Collapsed;
                    break;
                default:
                    throw new Exception();
            }
            EntryIDTextBlock.Text = $"ID: {entry.EntryID}";
            DateTextBox.Text = entry.Date.ToString("dd MMMM yyyy");
            DurationRangeTextBox.Text = $"{entry.DurationRange} ({entry.Duration})";
            LocationTextBlock.Text = entry.Localization;
            DescriptionTextBox.Text = entry.Description;
        }
    }

    private void MonthEntriesListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (AllEntriesButton.Visibility == Visibility.Visible) { RootSplitView.IsPaneOpen = false; }
    }

    private async void EditEntryButton_Click(object sender, RoutedEventArgs e)
    {
        if (!editLock && !isEntryAddVisible)
        {
            h_window = new HelperWindow(this, (Entry)MonthEntriesListView.SelectedItem, HelperWindow.Action.Edit);
            h_window.Activate();
        }
        else
        {
            ContentDialog dialog = new()
            {
                XamlRoot = this.XamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                PrimaryButtonText = "OK",
                DefaultButton = ContentDialogButton.Primary
            };
            if (editLock) { dialog.Title = "Zakoñcz najpierw edycjê poprzedniego wpisu!"; }
            else { dialog.Title = "Zakoñcz najpierw dodawanie nowego wpisu!"; }


            var result = await dialog.ShowAsync();
        }

    }

    private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (AllEntriesButton.Visibility == Visibility.Visible)
        {
            RootSplitView.OpenPaneLength = this.ActualWidth;
            if (MonthEntriesListView.Items.Count == 0) RootSplitView.IsPaneOpen = true;
        }
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
        ContentDialog dialog = new()
        {
            XamlRoot = this.XamlRoot,
            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
            Title = "Czy na pewno chcesz usun¹æ wpis?",
            PrimaryButtonText = "Usuñ",
            CloseButtonText = "Anuluj",
            DefaultButton = ContentDialogButton.Primary
        };

        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            myLog.DeleteEntry((EntryMileage)MonthEntriesListView.SelectedItem);
            ChangeTimeRange(selectedYear, selectedMonth);
        }
    }
}
