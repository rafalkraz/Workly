using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using Windows.Storage;
using WorkLog.Interfaces;
using WorkLog.Structure;
using myLog = WorkLog.Structure.Log.Entries;

namespace WorkLog;

public sealed partial class EntryEditPage : Page
{
    private readonly Window helperWindow;
    private readonly IDataViewPage parentPageReference;
    private ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
    private readonly List<string> entryTypes = ["Standardowy", "Nadgodziny", "Urlop", "Bezp³atne wolne"];
    private readonly List<string> mileageEntryTypes = ["Kilometrówka", "Parking"];
    private int editedType = -1; // 0 - worklog entry, 1 - mileage entry
    private readonly Entry editedEntry = null;
    private readonly EntryMileage editedMileage = null;

    public EntryEditPage(IDataViewPage parentPage, object entry, Window helperWindowReference)
    {
        this.InitializeComponent();
        if (parentPage.ToString() == "WorkLog.LogPage")
        {
            EntryTypeComboBox.ItemsSource = entryTypes;
            editedEntry = (Entry)entry;
            editedType = 0;
        }
        else if (parentPage.ToString() == "WorkLog.MileagePage") {
            EntryTypeComboBox.ItemsSource = mileageEntryTypes;
            
            editedMileage = (EntryMileage)entry;
            editedType = 1;
            if (entry != null)
            {
                EntryTypeComboBox.IsEnabled = false;
            }
            if (entry != null && editedMileage.Type == 0)
            {
                MoneyInfoBar.IsOpen = true;
            }
        }
        
        helperWindow = helperWindowReference;
        parentPageReference = parentPage;
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        LoadEntryDetails();
    }

    private void LoadEntryDetails()
    {
        switch (editedType)
        {
            case 0:
                
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
                break;
            case 1:
                if (editedMileage == null)
                {
                    EntryTypeComboBox.SelectedItem = mileageEntryTypes[0];
                    EventDatePicker.Date = DateTime.Now;
                }
                else
                {
                    try
                    {
                        EntryTypeComboBox.SelectedItem = mileageEntryTypes[editedMileage.Type];
                    }
                    catch (Exception)
                    {
                        EntryTypeComboBox.SelectedItem = mileageEntryTypes[0];
                    }
                    EventDatePicker.Date = editedMileage.Date.ToDateTime(new TimeOnly());
                    BeginPointTextBox.Text = editedMileage.BeginPoint;
                    EndPointTextBox.Text = editedMileage.EndPoint;
                    LocationTextBox.Text = editedMileage.BeginPoint;
                    DistanceNumberBox.Text = editedMileage.Distance.ToString();
                    ParkingPriceNumberBox.Text = editedMileage.ParkingPrice.ToString();
                    DescriptionTextBox.Text = editedMileage.Description;
                }
                break;
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

        if (editedType == 0)
        {
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
            if (EntryTypeComboBox.SelectedItem.ToString() == "Standardowy" || EntryTypeComboBox.SelectedItem.ToString() == "Nadgodziny")
            {
                // Location check
                if (LocationTextBox.Text == "")
                {
                    IncorrectLocationInfoBar.IsOpen = true;
                    isProblem = true;
                }
                else IncorrectLocationInfoBar.IsOpen = false;

                // Description check
                if (DescriptionTextBox.Text == "")
                {
                    IncorrectDescriptionInfoBar.IsOpen = true;
                    isProblem = true;
                }
                else IncorrectDescriptionInfoBar.IsOpen = false;
            }
        }

        if (editedType == 1)
        {
            if (EntryTypeComboBox.SelectedItem.ToString() == "Kilometrówka")
            {
                // BeginPoint check
                if (BeginPointTextBox.Text == "")
                {
                    IncorrectLocationInfoBar.IsOpen = true;
                    isProblem = true;
                }
                else IncorrectLocationInfoBar.IsOpen = false;

                // EndPoint check
                if (EndPointTextBox.Text == "")
                {
                    IncorrectEndPointInfoBar.IsOpen = true;
                    isProblem = true;
                }
                else IncorrectEndPointInfoBar.IsOpen = false;

                // Distance check
                if (DistanceNumberBox.Text == "")
                {
                    IncorrectDistanceInfoBar.IsOpen = true;
                    isProblem = true;
                }
                else IncorrectDistanceInfoBar.IsOpen = false;

                // Description check
                if (DescriptionTextBox.Text == "")
                {
                    IncorrectDescriptionInfoBar.IsOpen = true;
                    isProblem = true;
                }
                else IncorrectDescriptionInfoBar.IsOpen = false;
            }
            else if (EntryTypeComboBox.SelectedItem.ToString() == "Parking")
            {
                // ParkingPrice check
                if (ParkingPriceNumberBox.Text == "")
                {
                    IncorrectParkingPriceInfoBar.IsOpen = true;
                    isProblem = true;
                }
                else IncorrectParkingPriceInfoBar.IsOpen = false;
            }
        }
        if (isProblem) return false;
        return true;
    }

    private void CheckType()
    {
        if (EntryTypeComboBox.SelectedItem != null)
        {
            switch (editedType)
            {
                case 0:
                    DistanceStackPanel.Visibility = Visibility.Collapsed;
                    DistanceNumberBox.Visibility = Visibility.Collapsed;
                    BeginPointStackPanel.Visibility = Visibility.Collapsed;
                    BeginPointTextBox.Visibility = Visibility.Collapsed;
                    EndPointStackPanel.Visibility = Visibility.Collapsed;
                    EndPointTextBox.Visibility = Visibility.Collapsed;
                    ParkingPriceNumberBox.Visibility = Visibility.Collapsed;
                    ParkingPriceStackPanel.Visibility = Visibility.Collapsed;
                    if (EntryTypeComboBox.SelectedItem.ToString() == "Standardowy" || EntryTypeComboBox.SelectedItem.ToString() == "Nadgodziny")
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
                    break;
                case 1:
                    TimeStackPanel.Visibility = Visibility.Collapsed;
                    TimePickersStackPanel.Visibility = Visibility.Collapsed;
                    WorkTimeTextBlock.Visibility = Visibility.Collapsed;
                    
                    if (EntryTypeComboBox.SelectedItem.ToString() == "Parking")
                    {
                        BeginPointStackPanel.Visibility = Visibility.Collapsed;
                        BeginPointTextBox.Visibility = Visibility.Collapsed;
                        EndPointStackPanel.Visibility = Visibility.Collapsed;
                        EndPointTextBox.Visibility = Visibility.Collapsed;
                        ParkingPriceStackPanel.Visibility = Visibility.Visible;
                        ParkingPriceNumberBox.Visibility = Visibility.Visible;
                        LocationStackPanel.Visibility = Visibility.Visible;
                        LocationTextBox.Visibility = Visibility.Visible;
                        DistanceStackPanel.Visibility = Visibility.Collapsed;
                        DistanceNumberBox.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        BeginPointStackPanel.Visibility = Visibility.Visible;
                        BeginPointTextBox.Visibility = Visibility.Visible;
                        EndPointStackPanel.Visibility = Visibility.Visible;
                        EndPointTextBox.Visibility = Visibility.Visible;
                        ParkingPriceStackPanel.Visibility = Visibility.Collapsed;
                        ParkingPriceNumberBox.Visibility = Visibility.Collapsed;
                        LocationStackPanel.Visibility = Visibility.Collapsed;
                        LocationTextBox.Visibility = Visibility.Collapsed;
                        DistanceStackPanel.Visibility = Visibility.Visible;
                        DistanceNumberBox.Visibility = Visibility.Visible;
                    }
                    break;
            }
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
            var date = DateOnly.Parse(EventDatePicker.Date.Value.ToString("dd.MM.yyyy"));
            switch (editedType)
            {
                case 0:
                    var beginTime = EventDatePicker.Date.Value.Date.Add(BeginTimePicker.Time);
                    var endTime = EventDatePicker.Date.Value.Date.Add(EndTimePicker.Time);
                    if (EntryTypeComboBox.SelectedIndex != 0 && EntryTypeComboBox.SelectedIndex != 1)
                    {
                        LocationTextBox.Text = "";
                        DescriptionTextBox.Text = "";
                    }
                    if (editedEntry == null)
                    {
                        var newEntry = new Entry(0, EntryTypeComboBox.SelectedIndex, beginTime, endTime, LocationTextBox.Text, DescriptionTextBox.Text);
                        var addResult = myLog.AddEntry(newEntry);
                        if (addResult) SavingFinished(date);
                        else ErrorInfoBar.IsOpen = true;
                    }
                    else
                    {
                        var tempEntry = new Entry(editedEntry.EntryID, EntryTypeComboBox.SelectedIndex, beginTime, endTime, LocationTextBox.Text, DescriptionTextBox.Text);
                        var editResult = myLog.EditEntry(tempEntry);
                        if (editResult) SavingFinished(date);
                        else ErrorInfoBar.IsOpen = true;
                    }
                    break;
                case 1:
                    var parkingPrice = 0.0;
                    var distance = DistanceNumberBox.Text == "" ? 0 : int.Parse(DistanceNumberBox.Text);

                    if (EntryTypeComboBox.SelectedIndex == 0)
                    {
                        ParkingPriceNumberBox.Text = "";
                        if (localSettings.Values.ContainsKey("MileageSalary"))
                        {
                            parkingPrice = distance * Convert.ToDouble(localSettings.Values["MileageSalary"]);
                        }
                    }
                    else
                    {
                        parkingPrice = ParkingPriceNumberBox.Text == "" ? 0.0 : float.Parse(ParkingPriceNumberBox.Text);
                        BeginPointTextBox.Text = LocationTextBox.Text;
                    }
                    
                    if (editedMileage == null)
                    {
                        var newEntry = new EntryMileage(0, EntryTypeComboBox.SelectedIndex, date, BeginPointTextBox.Text, EndPointTextBox.Text, DescriptionTextBox.Text, distance, parkingPrice);
                        var addResult = WorkLog.Structure.Log.Mileage.AddEntry(newEntry);
                        if (addResult) SavingFinished(date);
                        else ErrorInfoBar.IsOpen = true;
                    }
                    else
                    {
                        var tempEntry = new EntryMileage(editedMileage.ID, EntryTypeComboBox.SelectedIndex, date, BeginPointTextBox.Text, EndPointTextBox.Text, DescriptionTextBox.Text, distance, parkingPrice);
                        var editResult = WorkLog.Structure.Log.Mileage.EditEntry(tempEntry);
                        if (editResult) SavingFinished(date);
                        else ErrorInfoBar.IsOpen = true;
                    }
                    break;
            }
        }
    }

    private void SavingFinished(DateOnly beginTime)
    {
        helperWindow.Close();
        parentPageReference.RefreshEntryList(beginTime.ToString("yyyy"), new Month(beginTime.ToString("MM")));
    }

    private void EntryTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        CheckType();
        IncorrectDescriptionInfoBar.IsOpen = false;
        IncorrectLocationInfoBar.IsOpen = false;
        IncorrectParkingPriceInfoBar.IsOpen = false;
        IncorrectDistanceInfoBar.IsOpen = false;
        IncorrectEndPointInfoBar.IsOpen = false;
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
        IncorrectLocationInfoBar.IsOpen = false;
    }

    private void DescriptionTextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        IncorrectDescriptionInfoBar.IsOpen = false;
    }

    private void BeginPointTextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        IncorrectLocationInfoBar.IsOpen = false;
    }

    private void EndPointTextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        IncorrectEndPointInfoBar.IsOpen = false;
    }

    private void DistanceNumberBox_LostFocus(object sender, RoutedEventArgs e)
    {
        IncorrectDistanceInfoBar.IsOpen = false;
    }

    private void ParkingPriceNumberBox_LostFocus(object sender, RoutedEventArgs e)
    {
        IncorrectParkingPriceInfoBar.IsOpen = false;
    }
}
