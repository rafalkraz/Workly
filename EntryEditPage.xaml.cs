using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using Windows.Storage;
using Workly.Interfaces;
using Workly.Structure;
using myLog = Workly.Structure.Log.Entries;

namespace Workly;

public sealed partial class EntryEditPage : Page
{
    private readonly Window helperWindowRef;
    private readonly IDataViewPage parentPageRef;
    private readonly ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

    private readonly List<string> entryTypes = ["Standardowy", "Nadgodziny", "Urlop", "Bezp³atne wolne"];
    private readonly List<string> mileageEntryTypes = ["Kilometrówka", "Parking"];
    private readonly AppSettings appSettings = new();

    private TimeSpan? beginTime;
    private TimeSpan? endTime;

    private enum EntrySourceType
    {
        Unknown = -1, Log = 0, Mileage = 1
    }
    public enum EditMode
    {
        Create = 0, Edit = 1, Duplicate = 2
    }

    private EntrySourceType sourceType = EntrySourceType.Unknown;
    private readonly EditMode mode;

    private Entry? editedEntry = null;
    private EntryMileage? editedMileage = null;

    public EntryEditPage(Window helperWindowRef, IDataViewPage parentPageRef, EditMode mode, Entry templateEntry)
    {
        this.InitializeComponent();
        EntryTypeComboBox.ItemsSource = entryTypes;
        if (mode != EditMode.Create) editedEntry = templateEntry;
        sourceType = EntrySourceType.Log;
        this.mode = mode;
        this.helperWindowRef = helperWindowRef;
        this.parentPageRef = parentPageRef;
    }

    public EntryEditPage(Window helperWindowRef, IDataViewPage parentPageRef, EditMode mode, EntryMileage templateEntry)
    {
        this.InitializeComponent();
        EntryTypeComboBox.ItemsSource = mileageEntryTypes;
        if (mode != EditMode.Create) editedMileage = templateEntry;
        sourceType = EntrySourceType.Mileage;
        this.mode = mode;
        this.helperWindowRef = helperWindowRef;
        this.parentPageRef = parentPageRef;
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        ValidateWarnings();
        LoadEntryDetails();
    }

    private void ValidateWarnings() {
        if (sourceType == EntrySourceType.Log && mode == EditMode.Edit) {
            MoneyInfoBar1.IsOpen = true;
        }

        if (sourceType == EntrySourceType.Mileage && mode == EditMode.Edit) {
            EntryTypeComboBox.IsEnabled = false;
        }

        if (sourceType == EntrySourceType.Mileage && mode == EditMode.Edit && editedMileage.Type == 0)
        {
            MoneyInfoBar2.IsOpen = true;
        }
    }

    private void LoadEntryDetails()
    {
        switch (sourceType)
        {
            case EntrySourceType.Log:
                
                if (editedEntry == null)
                {
                    //Type
                    EntryTypeComboBox.SelectedItem = entryTypes[0];

                    //Date
                    EventDatePicker.Date = DateTime.Now;

                    // Time
                    TimeSpan timeNow = DateTime.Now.TimeOfDay;
                    beginTime = timeNow;
                    endTime = timeNow + TimeSpan.FromMinutes(15);
                }
                else
                {
                    // Type
                    try
                    {
                        EntryTypeComboBox.SelectedItem = entryTypes[editedEntry.Type];
                    }
                    catch (Exception)
                    {
                        EntryTypeComboBox.SelectedItem = entryTypes[0];
                    }

                    // Date
                    EventDatePicker.Date = editedEntry.Date.ToDateTime(new TimeOnly());

                    // Time
                    beginTime = editedEntry.BeginTime.TimeOfDay;
                    endTime = editedEntry.EndTime.TimeOfDay;

                    // Location
                    LocationTextBox.Text = editedEntry.Localization;

                    // Description
                    DescriptionTextBox.Text = editedEntry.Description;
                }

                // Set time on controls
                if (appSettings.TimePickerType == 0)
                {
                    BeginTimeTextBox.Text = beginTime.ToString();
                    EndTimeTextBox.Text = endTime.ToString();
                }
                else if (appSettings.TimePickerType == 1)
                {
                    BeginTimePicker.Time = (TimeSpan)beginTime;
                    EndTimePicker.Time = (TimeSpan)endTime;
                }

                break;
            case EntrySourceType.Mileage:
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
                    ParkingPriceNumberBox.Text = double.Round(editedMileage.ParkingPrice, 2).ToString();
                    DescriptionTextBox.Text = editedMileage.Description;
                }
                break;
        }
        CheckType();
        CheckTime();
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

        if (sourceType == EntrySourceType.Log)
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

        if (sourceType == EntrySourceType.Mileage)
        {
            if (EntryTypeComboBox.SelectedItem.ToString() == "Kilometrówka")
            {
                // BeginPoint check
                if (BeginPointTextBox.Text == "")
                {
                    IncorrectBeginPointInfoBar.IsOpen = true;
                    isProblem = true;
                }
                else IncorrectBeginPointInfoBar.IsOpen = false;

                // EndPoint check
                if (EndPointTextBox.Text == "")
                {
                    IncorrectEndPointInfoBar.IsOpen = true;
                    isProblem = true;
                }
                else IncorrectEndPointInfoBar.IsOpen = false;

                // Distance check
                if (DistanceNumberBox.Text == "" || !int.TryParse(DistanceNumberBox.Text, out _))
                {
                    IncorrectDistanceInfoBar.IsOpen = true;
                    isProblem = true;

                }
                else IncorrectDistanceInfoBar.IsOpen = false;

                

            }
            else if (EntryTypeComboBox.SelectedItem.ToString() == "Parking")
            {
                // Location check
                if (LocationTextBox.Text == "")
                {
                    IncorrectLocationInfoBar.IsOpen = true;
                    isProblem = true;
                }
                else IncorrectLocationInfoBar.IsOpen = false;

                // ParkingPrice check
                if (ParkingPriceNumberBox.Text == "" || !(double.Parse(ParkingPriceNumberBox.Text) == double.Round(double.Parse(ParkingPriceNumberBox.Text), 2)))
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
        if (appSettings.TimePickerType == 0) TimePickersStackPanel.Visibility = Visibility.Collapsed;
        else if (appSettings.TimePickerType == 1) TimeTextBoxesStackPanel.Visibility = Visibility.Collapsed;
        else throw new Exception("Cannot determine TimePicker type!");

        if (EntryTypeComboBox.SelectedItem != null)
        {
            switch (sourceType)
            {
                case EntrySourceType.Log:
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
                case EntrySourceType.Mileage:
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
        helperWindowRef.Close();
    }

    private void SaveEntryButton_Click(object sender, RoutedEventArgs e)
    {
        if (CheckForm())
        {
            var date = DateOnly.Parse(EventDatePicker.Date.Value.ToString("dd.MM.yyyy"));
            switch (sourceType)
            {
                case EntrySourceType.Log:

                    DateTime beginDateTime;
                    DateTime endDateTime;

                    if (appSettings.TimePickerType == 0)
                    {
                        beginDateTime = EventDatePicker.Date.Value.Date.Add(TimeSpan.Parse(BeginTimeTextBox.Text.ToString()));
                        endDateTime = EventDatePicker.Date.Value.Date.Add(TimeSpan.Parse(EndTimeTextBox.Text.ToString()));
                    }
                    else if (appSettings.TimePickerType == 1)
                    {
                        beginDateTime = EventDatePicker.Date.Value.Date.Add(BeginTimePicker.Time);
                        endDateTime = EventDatePicker.Date.Value.Date.Add(EndTimePicker.Time);
                    }
                    else
                    {
                        throw new Exception("Nie mo¿na okreœliæ typu TimePickera!");
                    }

                    if (EntryTypeComboBox.SelectedIndex != 0 && EntryTypeComboBox.SelectedIndex != 1)
                    {
                        LocationTextBox.Text = "";
                        DescriptionTextBox.Text = "";
                    }
                    var earning = 0.0;
                    var time = TimeSpan.FromMinutes(((TimeSpan)endTime - (TimeSpan)beginTime).TotalMinutes).TotalMinutes;
                    switch (EntryTypeComboBox.SelectedIndex)
                    {
                        case 0:
                            earning = appSettings.StandardSalary / 60 * time;
                            break;
                        case 1:
                            earning = appSettings.OvertimeSalary / 60 * time;
                            break;
                        case 2:
                            earning = appSettings.LeaveSalary / 60 * time;
                            break;
                        default:
                            break;
                    }
                    earning = Math.Round(earning, 2);

                    var requestedID = mode switch
                    {
                        EditMode.Create => 0,
                        EditMode.Duplicate => 0,
                        _ => editedEntry.EntryID,
                    };

                    var entry = new Entry(requestedID, EntryTypeComboBox.SelectedIndex, beginDateTime, endDateTime, LocationTextBox.Text, DescriptionTextBox.Text, earning);

                    if (mode == EditMode.Edit)
                    {
                        var editResult = myLog.EditEntry(entry);
                        if (editResult) SavingFinished(date);
                        else ErrorInfoBar.IsOpen = true;
                    }
                    else
                    {
                        var addResult = myLog.AddEntry(entry);
                        if (addResult) SavingFinished(date);
                        else ErrorInfoBar.IsOpen = true;
                    }
                    break;

                case EntrySourceType.Mileage:
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
                    parkingPrice = Math.Round(parkingPrice, 2);

                    var requestedMileageID = mode switch
                    {
                        EditMode.Create => 0,
                        EditMode.Duplicate => 0,
                        _ => editedMileage.ID,
                    };

                    var entryMileage = new EntryMileage(requestedMileageID, EntryTypeComboBox.SelectedIndex, date, BeginPointTextBox.Text, EndPointTextBox.Text, DescriptionTextBox.Text, distance, parkingPrice);

                    if (mode == EditMode.Edit)
                    {
                        var editResult = Log.Mileage.EditEntry(entryMileage);
                        if (editResult) SavingFinished(date);
                        else ErrorInfoBar.IsOpen = true;
                    }
                    else
                    {
                        var addResult = Log.Mileage.AddEntry(entryMileage);
                        if (addResult) SavingFinished(date);
                        else ErrorInfoBar.IsOpen = true;

                    }
                    break;
            }
        }
    }

    private void SavingFinished(DateOnly beginTime)
    {
        helperWindowRef.Close();
        parentPageRef.RefreshEntryList(beginTime.ToString("yyyy"), new Month(beginTime.ToString("MM")));
    }

    private void EntryTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        CheckType();
        IncorrectDescriptionInfoBar.IsOpen = false;
        IncorrectLocationInfoBar.IsOpen = false;
        IncorrectParkingPriceInfoBar.IsOpen = false;
        IncorrectDistanceInfoBar.IsOpen = false;
        IncorrectBeginPointInfoBar.IsOpen = false;
        IncorrectEndPointInfoBar.IsOpen = false;
    }

    private void EventDatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
    {
        IncorrectDateInfoBar.IsOpen = false;
    }

    // Time controls

    private void BeginTimePicker_TimeChanged(object sender, TimePickerValueChangedEventArgs e)
    {
        IncorrectTimeInfoBar.IsOpen = false;
        beginTime = BeginTimePicker.Time;
        CheckTime();
    }

    private void EndTimePicker_TimeChanged(object sender, TimePickerValueChangedEventArgs e)
    {
        IncorrectTimeInfoBar.IsOpen = false;
        endTime = EndTimePicker.Time;
        CheckTime();
    }

    private void BeginTimeTextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        IncorrectTimeInfoBar.IsOpen = false;
        TimeSpan time;
        if (TimeSpan.TryParse(BeginTimeTextBox.Text, out time)) beginTime = time;
        else beginTime = null;
        CheckTime();
    }

    private void EndTimeTextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        IncorrectTimeInfoBar.IsOpen = false;
        TimeSpan time;
        if (TimeSpan.TryParse(EndTimeTextBox.Text, out time)) endTime = time;
        else endTime = null;
        CheckTime();
    }

    /// <summary>
    /// Checks the validity of work time and calculates and displays the work time
    /// based on the BeginTimePicker and EndTimePicker values.
    /// </summary>
    /// <returns>
    /// 0, if work time is correct; 1, if the beginTime ia greater than endTime; 2, if work time is invalid or null
    /// </returns>
    private int CheckTime()
    {
        if (beginTime == null || endTime == null)
        {
            WorkTimeTextBlock.Text = $"Czas pracy: ?";
            return 2;
        }
        else {
            if (beginTime <= endTime)
            {
                var time = TimeSpan.FromMinutes(((TimeSpan)endTime - (TimeSpan)beginTime).TotalMinutes);
                if (time.TotalHours >= 1)
                {
                    if (time.Minutes != 0) WorkTimeTextBlock.Text = $"Czas pracy: {time.Hours}h {time.Minutes}min";
                    else WorkTimeTextBlock.Text = $"Czas pracy: {time.Hours}h";
                }
                else WorkTimeTextBlock.Text = $"Czas pracy: {time.Minutes}min";
                return 0;
            }
            else if (beginTime > endTime)
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
        IncorrectBeginPointInfoBar.IsOpen = false;
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
