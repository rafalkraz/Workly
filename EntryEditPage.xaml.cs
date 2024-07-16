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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WorkLog
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EntryEditPage : Page
    {
        public EntryEditPage(Entry entry)
        {
            this.InitializeComponent();
            LoadEntryDetails(entry);
        }

        private void TypeEntryRadioButton_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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
    }
}
