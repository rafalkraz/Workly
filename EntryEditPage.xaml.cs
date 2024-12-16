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
        if (editedEntry == null)
        {
            EntryTypeComboBox.SelectedItem = entryTypes[0];
            EventDatePicker.Date = DateTime.Now;
            BeginTimePicker.Time = DateTime.Now.TimeOfDay;
            EndTimePicker.Time = DateTime.Now.TimeOfDay + TimeSpan.FromMinutes(15);
        }
        else
        {
            try
            {
                EntryTypeComboBox.SelectedItem = entryTypes[editedEntry.Type];
            }
            catch (Exception)
            {
                EntryTypeComboBox.SelectedItem = entryTypes[0];
            }
            EventDatePicker.Date = editedEntry.Date.ToDateTime(new TimeOnly());
            BeginTimePicker.Time = editedEntry.BeginTime.TimeOfDay;
            EndTimePicker.Time = editedEntry.EndTime.TimeOfDay;
            LocationTextBox.Text = editedEntry.Localization;
            DescriptionTextBox.Text = editedEntry.Description;
        }
        CheckType();
    }

    private bool CheckForm()
    {
        // Date check
        var isProblem = false;
        if (EventDatePicker.Date == null) 
        {
            IncorrectDateInfoBar.IsOpen = true;
            isProblem = true;
        }
        else IncorrectDateInfoBar.IsOpen = false;

        // Time check
        var checkTimeResult = CheckTime(); 
        if (checkTimeResult == 1)
        {
            IncorrectTimeInfoBar.IsOpen = true;
            IncorrectTimeInfoBar.Message = "Godzina zakoñczenia nie mo¿e byæ wczeœniejsza ni¿ rozpoczêcia!";
            isProblem = true;
        }
        else if (checkTimeResult == 2)
        {
            IncorrectTimeInfoBar.IsOpen = true;
            IncorrectTimeInfoBar.Message = "Coœ tu siê nie zgadza!";
            isProblem = true;
        }
        else IncorrectTimeInfoBar.IsOpen = false;

        // Check rest of requirments if entryType = 0
        if (EntryTypeComboBox.SelectedItem.ToString() == "Standardowy")
        {
            // Location check
            if (LocationTextBox.Text == "") 
            {
                IncorrectLocalizationInfoBar.IsOpen = true;
                isProblem = true;
            }
            else IncorrectLocalizationInfoBar.IsOpen = false;

            // Localization check
            if (DescriptionTextBox.Text == "") 
            {
                IncorrectDescriptionInfoBar.IsOpen = true;
                isProblem = true;
            }
            else IncorrectDescriptionInfoBar.IsOpen= false;
        }

        if (isProblem) return false;
        return true;
    }

    private void CheckType()
    {
        if (EntryTypeComboBox.SelectedItem.ToString() == "Standardowy")
        {
            LocationStackPanel.Visibility = Visibility.Visible;
            LocationTextBox.Visibility = Visibility.Visible;
            DescriptionStackPanel.Visibility = Visibility.Visible;
            DescriptionTextBox.Visibility = Visibility.Visible;
        }
        else
        {
            LocationStackPanel.Visibility = Visibility.Collapsed;
            LocationTextBox.Visibility = Visibility.Collapsed;
            DescriptionStackPanel.Visibility = Visibility.Collapsed;
            DescriptionTextBox.Visibility = Visibility.Collapsed;
        }
    }

    private void CancelEntryButton_Click(object sender, RoutedEventArgs e)
    {
        helperWindow.Close();
    }

    private void SaveEntryButton_Click(object sender, RoutedEventArgs e)
    {
        if (CheckForm())
        {
            var beginTime = EventDatePicker.Date.Value.Date.Add(BeginTimePicker.Time);
            var endTime = EventDatePicker.Date.Value.Date.Add(EndTimePicker.Time);
            if (EntryTypeComboBox.SelectedIndex != 0)
            {
                LocationTextBox.Text = "";
                DescriptionTextBox.Text = "";
            }
            if (editedEntry == null)
            {
                var newEntry = new Entry(0, EntryTypeComboBox.SelectedIndex, beginTime, endTime, LocationTextBox.Text, DescriptionTextBox.Text);
                var addResult = Log.AddEntry(newEntry);
                if (addResult) SavingFinished(beginTime);
                else ErrorInfoBar.IsOpen = true;
            }
            else
            {
                var tempEntry = new Entry(editedEntry.EntryID, EntryTypeComboBox.SelectedIndex, beginTime, endTime, LocationTextBox.Text, DescriptionTextBox.Text);
                var editResult = Log.EditEntry(tempEntry);
                if (editResult) SavingFinished(beginTime);
                else ErrorInfoBar.IsOpen = true;
            }
        }
    }

    private void SavingFinished(DateTime beginTime)
    {
        helperWindow.Close();
        parentPageReference.RefreshEntryList(beginTime.ToString("yyyy"), new Month(beginTime.ToString("MM")));
    }

    private void EntryTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        CheckType();
        IncorrectDescriptionInfoBar.IsOpen = false;
        IncorrectLocalizationInfoBar.IsOpen = false;
    }

    private void EventDatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
    {
        IncorrectDateInfoBar.IsOpen = false;
    }

    private void BeginTimePicker_TimeChanged(object sender, TimePickerValueChangedEventArgs e)
    {
        IncorrectTimeInfoBar.IsOpen = false;
        CheckTime();
    }

    private void EndTimePicker_TimeChanged(object sender, TimePickerValueChangedEventArgs e)
    {
        IncorrectTimeInfoBar.IsOpen = false;
        CheckTime();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>
    /// 0, if work time is correct; 1, if beginTime > endTime; 2, if work time is incorrect or null
    /// </returns>
    private int CheckTime()
    {
        if (BeginTimePicker.Time <= EndTimePicker.Time)
        {
            var time = TimeSpan.FromMinutes((EndTimePicker.Time - BeginTimePicker.Time).TotalMinutes);
            if (time.TotalHours >= 1)
            {
                if (time.Minutes != 0) WorkTimeTextBlock.Text = $"Czas pracy: {time.Hours}h {time.Minutes}min";
                else WorkTimeTextBlock.Text = $"Czas pracy: {time.Hours}h";
            }
            else WorkTimeTextBlock.Text = $"Czas pracy: {time.Minutes}min";
            return 0;
        }
        else if (BeginTimePicker.Time > EndTimePicker.Time)
        {
            WorkTimeTextBlock.Text = $"Czas pracy: ?";
            return 1;
        }
        else
        {
            WorkTimeTextBlock.Text = $"Czas pracy: ?";
            return 2;
        }
    }

    private void LocationTextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        IncorrectLocalizationInfoBar.IsOpen = false;
    }

    private void DescriptionTextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        IncorrectDescriptionInfoBar.IsOpen = false;
    }
}
