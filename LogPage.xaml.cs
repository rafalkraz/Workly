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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WorkLog
{
    public sealed partial class LogPage : Page
    {
        private Log log;
        private List<Month> monthList = [];
        private List<Year> yearList = [];
        private Year selectedYear;
        private Month selectedMonth;
        public LogPage()
        {
            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            log = await Log.GetInstance();
            yearList = log.Years;
            YearSelectionComboBox.ItemsSource = yearList;
            YearSelectionComboBox.SelectedItem = yearList[0];
        }

        private void YearSelectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            selectedYear = (Year)YearSelectionComboBox.SelectedItem;
            MonthSelectionComboBox.ItemsSource = selectedYear.Months;
            if (MonthSelectionComboBox.SelectedItem == null)
            {
                MonthSelectionComboBox.SelectedIndex = 0;
            }
        }

        private void MonthSelectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedMonth = (Month)MonthSelectionComboBox.SelectedItem;
            if (selectedMonth != null)
            {
                ObservableCollection<Entry> Entries = new(selectedMonth.Entries);
                var result =
                    from entry in Entries
                    group entry by entry.BeginTime.Date.ToString("dd.MM") into g
                    orderby g.Key
                    select g;
                EntriesCollection.Source = result;
            }
        }

        private void LoadEntryDetails(Entry entry)
        {
            if (entry == null) return;
            else
            {
                if (!entry.IsDayOff && !entry.IsUnpaid) { StandardEntryRadioButton.IsChecked = true; }
                else if (entry.IsDayOff) { DayOffEntryRadioButton.IsChecked = true; }
                else if (entry.IsUnpaid) { UnpaidEntryRadioButton.IsChecked = true; }
                else
                {
                    throw new Exception();
                }

                EventDatePicker.Date = entry.BeginTime;
                BeginTimePicker.Time = entry.BeginTime.TimeOfDay;
                EndTimePicker.Time = entry.EndTime.TimeOfDay;
                LocalizationTextBox.Text = entry.Localization;
                DescriptionTextBox.Text = entry.Description;
            }
        }

        private void MonthEntriesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadEntryDetails((Entry)MonthEntriesListView.SelectedItem);
            
        }

        private void TypeEntryRadioButton_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DayOffEntryRadioButton.IsChecked == false && UnpaidEntryRadioButton.IsChecked == false)
            {
                LocalizationTextBox.Visibility = Visibility.Visible;
                DescriptionTextBox.Visibility = Visibility.Visible;
            }
            else if (UnpaidEntryRadioButton.IsChecked == false)
            {
                LocalizationTextBox.Visibility = Visibility.Collapsed;
                DescriptionTextBox.Visibility = Visibility.Collapsed;
            }
            else if (DayOffEntryRadioButton.IsChecked == false)
            {
                LocalizationTextBox.Visibility = Visibility.Collapsed;
                DescriptionTextBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                throw new Exception();
            }
        }
    }
}
