using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Workly.Converters;
using Workly.Interfaces;
using Workly.Structure;

namespace Workly
{
    public sealed partial class SalaryPage : Page, IDataViewPage
    {
        private Dictionary<string, HashSet<Month>> data = [];
        private Dictionary<string, List<Month>> finalDict = [];
        public SalaryPage()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Prepare();
        }

        private void Prepare()
        {
            if (Log.Entries.RefreshLog())
            {
                foreach (var year in Log.Entries.Years)
                {
                    var months = Log.Entries.GetMonthsList(year);
                    data.Add(year, months.ToHashSet<Month>());
                }

            }
            if (Log.Mileage.RefreshLog())
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
            if (YearSelectionComboBox.SelectedItem != null) {
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
                var entryList = Log.Entries.GetEntries(YearSelectionComboBox.SelectedItem.ToString(), (Month)MonthSelectionComboBox.SelectedItem, this);
                var mileageList = Log.Mileage.GetEntries(YearSelectionComboBox.SelectedItem.ToString(), (Month)MonthSelectionComboBox.SelectedItem, this);
                var sum = entryList.Sum(entry => entry.Earning);
                sum += mileageList.Sum(entry => entry.ParkingPrice);
                sum = Math.Round(sum, 2);
                SalaryTextBlock.Text = sum.ToString("F2") + " PLN";

                MinutesToDurationConverter converter = new();
                Chart1TextBlock.Text = "Standardowy czas pracy (" + converter.Convert(entryList.Where(entry => entry.Type == 0).Sum(entry => entry.DurationRaw), null, null, CultureInfo.CurrentCulture.ToString()) + ")";
                var salaryFromStandardEntries = entryList.Where(entry => entry.Type == 0).Sum(entry => entry.Earning);
                var percentageOfStandardEntries = Math.Round(salaryFromStandardEntries / sum * 100, 2);
                StandardEarningProgressBar.Value = percentageOfStandardEntries;
                StandardEarningTextBlock.Text = percentageOfStandardEntries + "%";

                var salaryFromMileageEntries = mileageList.Sum(entry => entry.ParkingPrice);
                var percentageOfMileageEntries = Math.Round(salaryFromMileageEntries / sum * 100, 2);
                MileageEarningProgressBar.Value = percentageOfMileageEntries;
                MileageEarningTextBlock.Text = percentageOfMileageEntries + "%";

                Chart3TextBlock.Text = "Urlop (" + converter.Convert(entryList.Where(entry => entry.Type == 2).Sum(entry => entry.DurationRaw), null, null, CultureInfo.CurrentCulture.ToString()) + ")";
                var salaryFromLeaveEntries = entryList.Where(entry => entry.Type == 2).Sum(entry => entry.Earning);
                var percentageOfLeaveEntries = Math.Round(salaryFromLeaveEntries / sum * 100, 2);
                LeaveEarningProgressBar.Value = percentageOfLeaveEntries;
                LeaveEarningTextBlock.Text = percentageOfLeaveEntries + "%";

                Chart4TextBlock.Text = "Nadgodziny (" + converter.Convert(entryList.Where(entry => entry.Type == 3).Sum(entry => entry.DurationRaw), null, null, CultureInfo.CurrentCulture.ToString()) + ")";
                var salaryFromOvertimeEntries = entryList.Where(entry => entry.Type == 1).Sum(entry => entry.Earning);
                var percentageOfOvertimeEntries = Math.Round(salaryFromOvertimeEntries / sum * 100, 2);
                OvertimeEarningProgressBar.Value = percentageOfOvertimeEntries;
                OvertimeEarningTextBlock.Text = percentageOfOvertimeEntries + "%";

                ChartTimeSummaryTextBlock.Text = "£¹czny zaraportowany czas: " + converter.Convert(entryList.Sum(entry => entry.DurationRaw), null, null, CultureInfo.CurrentCulture.ToString());
            }
            catch (Exception)
            {
                StandardEarningProgressBar.Value = 0;
                MileageEarningProgressBar.Value = 0;
                LeaveEarningProgressBar.Value = 0;
                OvertimeEarningProgressBar.Value = 0;

                StandardEarningTextBlock.Text = "0%";
                MileageEarningTextBlock.Text = "0%";
                LeaveEarningTextBlock.Text = "0%";
                OvertimeEarningTextBlock.Text = "0%";
                SalaryTextBlock.Text = "-";

                MonthSelectionComboBox.ItemsSource = null;
                YearSelectionComboBox.ItemsSource = null;
                MonthSelectionComboBox.IsEnabled = false;
                YearSelectionComboBox.IsEnabled = false;
            }
        }

        public void RefreshEntryList(string year = null, Month month = null)
        {
            return;
        }

        public async void ShowDataError(string title, string content)
        {
            StandardEarningProgressBar.Value = 0;
            MileageEarningProgressBar.Value = 0;
            LeaveEarningProgressBar.Value = 0;
            OvertimeEarningProgressBar.Value = 0;

            StandardEarningTextBlock.Text = "0%";
            MileageEarningTextBlock.Text = "0%";
            LeaveEarningTextBlock.Text = "0%";
            OvertimeEarningTextBlock.Text = "0%";
            SalaryTextBlock.Text = "-";

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
            await dialog.ShowAsync();
        }

        
    }
}
