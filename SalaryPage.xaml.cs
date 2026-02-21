using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Workly.Converters;
using Workly.Interfaces;
using Workly.Structure;

namespace Workly;

public sealed partial class SalaryPage : Page, IDataViewPage
{
    private readonly Dictionary<string, HashSet<Month>> data = [];
    private readonly Dictionary<string, List<Month>> finalDict = [];
    private bool isProblem = false;
    public SalaryPage()
    {
        InitializeComponent();
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        Prepare();
    }

    private void Prepare()
    {
        if (Log.Entries.RefreshLog(this))
        {
            foreach (var year in Log.Entries.Years)
            {
                var months = Log.Entries.GetMonthsList(year);
                data.Add(year, months.ToHashSet<Month>());
            }

        }

        if (Log.Mileage.RefreshLog(this))
        {
            foreach (var year in Log.Mileage.Years)
            {
                var months = Log.Mileage.GetMonthsList(year);
                if (data.ContainsKey(year))
                {
                    foreach (var month in months)
                    {
                        data[year].Add(month);
                    }
                }
                else
                {
                    data.Add(year, months.ToHashSet<Month>());
                }
            }
        }

        var result = data.Keys.OrderByDescending(i => i);
        foreach (var key in result)
        {
            var temp = data[key].ToList();
            temp = temp.OrderDescending().ToList();
            finalDict[key] = temp;
        }
        YearSelectionComboBox.ItemsSource = finalDict.Keys;

        if (data.Count == 0)
        {
            YearSelectionComboBox.IsEnabled = false;
            MonthSelectionComboBox.IsEnabled = false;
            SalaryTextBlock.Text = "-";
        }
        else
        {
            YearSelectionComboBox.SelectedIndex = 0;
        }
    }

    private void YearSelectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (YearSelectionComboBox.SelectedItem != null)
        {
            MonthSelectionComboBox.ItemsSource = finalDict[YearSelectionComboBox.SelectedItem.ToString()];
            MonthSelectionComboBox.SelectedIndex = 0;
        }
    }

    private void MonthSelectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (YearSelectionComboBox.SelectedItem != null)
        {
            if (MonthSelectionComboBox.SelectedItem != null)
            {
                CalculateSalary();
            }
        }
    }

    private void CalculateSalary()
    {
        try
        {
            if (YearSelectionComboBox.SelectedItem == null || MonthSelectionComboBox.SelectedItem == null) return;
            var entryList = Log.Entries.GetEntries(YearSelectionComboBox.SelectedItem.ToString(), (Month)MonthSelectionComboBox.SelectedItem, this);
            if (YearSelectionComboBox.SelectedItem == null || MonthSelectionComboBox.SelectedItem == null) return;
            var mileageList = Log.Mileage.GetEntries(YearSelectionComboBox.SelectedItem.ToString(), (Month)MonthSelectionComboBox.SelectedItem, this);

            if (!isProblem)
            {
                // Calculate total salary
                var sum = entryList.Sum(entry => entry.Earning);
                sum += mileageList.Sum(entry => entry.ParkingPrice);
                sum = Math.Round(sum, 2);
                SalaryTextBlock.Text = sum.ToString("F2") + " PLN";

                // Calculate standard entries
                Chart1TextBlock.Text = GetSumOfTime("Standardowy czas pracy", 0, entryList);
                var salaryFromStandardEntries = entryList.Where(entry => entry.Type == 0).Sum(entry => entry.Earning);
                var percentageOfStandardEntries = CalculateRoundedPercentage(salaryFromStandardEntries, sum);
                StandardEarningProgressBar.Value = percentageOfStandardEntries;
                StandardEarningTextBlock.Text = percentageOfStandardEntries + "%";

                // Calculate mileage entries
                var salaryFromMileageEntries = mileageList.Sum(entry => entry.ParkingPrice);
                var percentageOfMileageEntries = CalculateRoundedPercentage(salaryFromMileageEntries, sum);
                MileageEarningProgressBar.Value = percentageOfMileageEntries;
                MileageEarningTextBlock.Text = percentageOfMileageEntries + "%";

                // Calculate leave entries
                Chart3TextBlock.Text = GetSumOfTime("Urlop", 2, entryList);
                var salaryFromLeaveEntries = entryList.Where(entry => entry.Type == 2).Sum(entry => entry.Earning);
                var percentageOfLeaveEntries = CalculateRoundedPercentage(salaryFromLeaveEntries, sum);
                LeaveEarningProgressBar.Value = percentageOfLeaveEntries;
                LeaveEarningTextBlock.Text = percentageOfLeaveEntries + "%";

                // Calculate overtime entries
                Chart4TextBlock.Text = GetSumOfTime("Nadgodziny", 1, entryList);
                var salaryFromOvertimeEntries = entryList.Where(entry => entry.Type == 1).Sum(entry => entry.Earning);
                var percentageOfOvertimeEntries = CalculateRoundedPercentage(salaryFromOvertimeEntries, sum);
                OvertimeEarningProgressBar.Value = percentageOfOvertimeEntries;
                OvertimeEarningTextBlock.Text = percentageOfOvertimeEntries + "%";

                // Calculate unpaid leave
                Chart5TextBlock.Text = GetSumOfTime("Urlop bezp³atny", 3, entryList);

                // Calculate total time
                MinutesToDurationConverter converter = new();
                ChartTimeSummaryTextBlock.Text = "£¹czny zaraportowany czas: " + converter.Convert(entryList.Sum(entry => entry.DurationRaw), null, null, CultureInfo.CurrentCulture.ToString());
            }
        }
        catch (Exception)
        {
            ShowDataError("Coœ posz³o nie tak!", "Nie uda³o siê obliczyæ twojego wynagrodzenia");
        }
    }

    private double CalculateRoundedPercentage(double number, double sum)
    {
        var tempRoundedValue = Math.Round(number / sum * 100, 2);
        return tempRoundedValue is double.NaN ? 0 : tempRoundedValue;
    }

    private string GetSumOfTime(string textBeforeSum, int typeOfEntry, List<Entry> listOfObjects)
    {
        MinutesToDurationConverter converter = new();
        return $"{textBeforeSum} ({converter.Convert(listOfObjects.Where(entry => entry.Type == typeOfEntry).Sum(entry => entry.DurationRaw), null, null, CultureInfo.CurrentCulture.ToString())})";
    }

    public void RefreshEntryList(string year = null, Month month = null)
    {
        return;
    }

    public async void ShowDataError(string title, string content)
    {
        isProblem = true;
        StandardEarningProgressBar.Value = 0;
        MileageEarningProgressBar.Value = 0;
        LeaveEarningProgressBar.Value = 0;
        OvertimeEarningProgressBar.Value = 0;

        StandardEarningTextBlock.Text = "0%";
        MileageEarningTextBlock.Text = "0%";
        LeaveEarningTextBlock.Text = "0%";
        OvertimeEarningTextBlock.Text = "0%";
        SalaryTextBlock.Text = "-";
        ChartTimeSummaryTextBlock.Text = "£¹czny zaraportowany czas: -";

        MonthSelectionComboBox.ItemsSource = null;
        YearSelectionComboBox.ItemsSource = null;
        MonthSelectionComboBox.IsEnabled = false;
        YearSelectionComboBox.IsEnabled = false;

        ContentDialog dialog = new()
        {
            XamlRoot = this.XamlRoot,
            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
            Title = title,
            Content = content,
            CloseButtonText = "OK",
            DefaultButton = ContentDialogButton.Close
        };

        try
        {
            await dialog.ShowAsync();
        }
        catch (Exception) { }
    }
}
