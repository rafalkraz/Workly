using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WorkLog.Structure;

namespace WorkLog
{
    public sealed partial class EntryEditPage : Page
    {
        private Log log;
        private Window helperWindow;
        private LogPage parentPageReference;  
        private readonly List<string> entryTypes = ["Standardowy", "Urlop", "Bezp³atne wolne"];
        private Entry editedEntry;
        public EntryEditPage(LogPage parentPage, Entry entry, Window helperWindowReference)
        {
            this.InitializeComponent();
            EntryTypeComboBox.ItemsSource = entryTypes;
            helperWindow = helperWindowReference;
            editedEntry = entry;
            parentPageReference = parentPage;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadEntryDetails();
            log = await Log.GetInstance();
        }

        private void LoadEntryDetails()
        {
            if (editedEntry == null) EntryTypeComboBox.SelectedItem = entryTypes[0];
            else
            {
                EntryTypeComboBox.SelectedItem = entryTypes[editedEntry.Type];
                EventDatePicker.Date = editedEntry.Date.ToDateTime(new TimeOnly());
                //BeginTimePicker.Time = editedEntry.BeginTime.ToTimeSpan();
                //EndTimePicker.Time = editedEntry.EndTime.ToTimeSpan();
                LocationTextBox.Text = editedEntry.Localization;
                DescriptionTextBox.Text = editedEntry.Description;
            }
        }

        private void CancelEntryButton_Click(object sender, RoutedEventArgs e)
        {
            helperWindow.Close();
        }

        private async void SaveEntryButton_Click(object sender, RoutedEventArgs e)
        {
            bool addResult = false;
            if (editedEntry == null)
            {
                //var newEntry = new Entry(-1, EntryTypeComboBox.SelectedIndex, DateOnly.FromDateTime(EventDatePicker.Date.Value.Date), TimeOnly.FromTimeSpan(BeginTimePicker.Time), TimeOnly.FromTimeSpan(EndTimePicker.Time), LocationTextBox.Text, DescriptionTextBox.Text);
                //addResult = await log.AddEntryToLogAsync(newEntry);
            }
            else
            {
                //var newEntry = new Entry(EntryTypeComboBox.SelectedIndex, DateOnly.FromDateTime(EventDatePicker.Date.Value.Date), TimeOnly.FromTimeSpan(BeginTimePicker.Time), TimeOnly.FromTimeSpan(EndTimePicker.Time), LocationTextBox.Text, DescriptionTextBox.Text);
                //addResult = await log.EditEntryInLogAsync(editedEntry, newEntry);
            }
            
            if (addResult)
            {
                parentPageReference.LoadEntriesToList();
                helperWindow.Close();
            }
        }

        private void EntryTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void EventDatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {

        }

        private void BeginTimePicker_TimeChanged(object sender, TimePickerValueChangedEventArgs e)
        {
            VerifyTime();
        }

        private void EndTimePicker_TimeChanged(object sender, TimePickerValueChangedEventArgs e)
        {
            VerifyTime();
        }

        private void VerifyTime()
        {
            if (BeginTimePicker.Time > EndTimePicker.Time) { IncorrectTimeInfoBar.IsOpen = true; }
            else { IncorrectTimeInfoBar.IsOpen = false; }
        }

        private void LocationTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (LocationTextBox.Text == "") { LocationEmptyInfoBar.IsOpen = true; }
            else { LocationEmptyInfoBar.IsOpen = false; }
        }

        private void DescriptionTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (DescriptionTextBox.Text == "") { DescriptionEmptyInfoBar.IsOpen = true; }
            else { DescriptionEmptyInfoBar.IsOpen = false; }
        }

        enum Action
        {
            show,
            hide
        }

        private void ShowOrHideWarning(Action action, int number) // 1 - time
        {
            List<string> warningsList = [];
            IncorrectTimeInfoBar.Message = "Godzina zakoñczenia nie mo¿e byæ wczeœniejsza ni¿ rozpoczêcia!\nDrugi wiersz";
        }


    }
}
