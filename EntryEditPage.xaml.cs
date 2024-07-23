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
        private Window helperWindow;
        private readonly List<string> entryTypes = ["Standardowy", "Urlop", "Bezp³atne wolne"];
        public EntryEditPage(Entry entry, Window helperWindowReference)
        {
            this.InitializeComponent();
            EntryTypeComboBox.ItemsSource = entryTypes;
            LoadEntryDetails(entry);
            helperWindow = helperWindowReference;
        }

        private void LoadEntryDetails(Entry entry)
        {
            if (entry == null) return;
            else
            {
                if (!entry.IsDayOff && !entry.IsUnpaid) { EntryTypeComboBox.SelectedItem = entryTypes[0]; }
                else if (entry.IsDayOff) { EntryTypeComboBox.SelectedItem = entryTypes[1]; ; }
                else if (entry.IsUnpaid) { EntryTypeComboBox.SelectedItem = entryTypes[2]; ; }
                else
                {
                    throw new Exception();
                }

                EventDatePicker.Date = entry.BeginTime;
                BeginTimePicker.Time = entry.BeginTime.TimeOfDay;
                EndTimePicker.Time = entry.EndTime.TimeOfDay;
                LocationTextBox.Text = entry.Localization;
                DescriptionTextBox.Text = entry.Description;
            }
        }

        private void CancelEntryButton_Click(object sender, RoutedEventArgs e)
        {
            helperWindow.Close();
        }

        private void SaveEntryButton_Click(object sender, RoutedEventArgs e)
        {

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
