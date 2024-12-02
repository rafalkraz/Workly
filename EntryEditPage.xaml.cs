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

namespace WorkLog;

public sealed partial class EntryEditPage : Page
{
    private Window helperWindow;
    private LogPage parentPageReference;  
    private readonly List<string> entryTypes = ["Standardowy", "Urlop", "Bezp³atne wolne"];
    private Entry editedEntry = null;
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
    }

    private void LoadEntryDetails()
    {
        if (editedEntry == null) EntryTypeComboBox.SelectedItem = entryTypes[0];
        else
        {
            EntryTypeComboBox.SelectedItem = entryTypes[editedEntry.Type];
            EventDatePicker.Date = editedEntry.Date.ToDateTime(new TimeOnly());
            BeginTimePicker.Time = editedEntry.BeginTime.TimeOfDay;
            EndTimePicker.Time = editedEntry.EndTime.TimeOfDay;
            LocationTextBox.Text = editedEntry.Localization;
            DescriptionTextBox.Text = editedEntry.Description;
        }
    }

    private void CancelEntryButton_Click(object sender, RoutedEventArgs e)
    {
        helperWindow.Close();
    }

    private void SaveEntryButton_Click(object sender, RoutedEventArgs e)
    {
        if (editedEntry == null)
        {
            var beginTime = EventDatePicker.Date.Value.Date.Add(BeginTimePicker.Time);
            var endTime = EventDatePicker.Date.Value.Date.Add(EndTimePicker.Time);
            var newEntry = new Entry(0, EntryTypeComboBox.SelectedIndex, beginTime, endTime, LocationTextBox.Text, DescriptionTextBox.Text);
            var addResult = Log.AddEntry(newEntry);
            if (addResult)
            {
                helperWindow.Close();
                parentPageReference.RefreshEntryList(beginTime.ToString("yyyy"), new Month(beginTime.ToString("MM")));
            }
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
