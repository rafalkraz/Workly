using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage;
using Windows.Storage.Pickers;
using WorkLog.Structure;

namespace WorkLog;

public sealed partial class SettingsPage : Page
{
    public SettingsPage()
    {
        this.InitializeComponent();
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        TextBox textBox = sender as TextBox;
        if (textBox != null)
        {
            // Zamie� przecinek na kropk�
            string text = textBox.Text.Replace(',', '.');

            // Aktualizuj warto�� TextBox
            textBox.Text = text;

            // Ustawienie kursora na ko�cu tekstu
            textBox.SelectionStart = textBox.Text.Length;

            if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
            {
                // Prawid�owy format liczby dziesi�tnej
                // Ustaw warto�� w odpowiedniej w�a�ciwo�ci
                var binding = textBox.GetBindingExpression(TextBox.TextProperty);
                if (binding != null)
                {
                    binding.UpdateSource();
                }
            }
            else
            {
                // Nieprawid�owy format, wyczy�� pole lub ustaw odpowiedni komunikat
                textBox.Text = string.Empty;
            }
        }
    }

    private void TextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        TextBox textBox = sender as TextBox;
        if (textBox != null && string.IsNullOrEmpty(textBox.Text))
        {
            // Ustaw domy�ln� warto�� na 0, je�li pole jest puste
            textBox.Text = "0";
            var binding = textBox.GetBindingExpression(TextBox.TextProperty);
            if (binding != null)
            {
                binding.UpdateSource();
            }
        }
    }

    private async void ButtonImportDB_Click(object sender, RoutedEventArgs e)
    {
        var senderButton = sender as Button;
        senderButton.IsEnabled = false;
        var window = App.MainWindow;
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        var openPicker = new Windows.Storage.Pickers.FileOpenPicker();
        WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);
        openPicker.FileTypeFilter.Add(".db");

        var file = await openPicker.PickSingleFileAsync();
        if (file != null)
        {
            await file.CopyAsync(Log.localFolder, Log.dbName, NameCollisionOption.ReplaceExisting);
        }
        else
        {

        }
        senderButton.IsEnabled = true;

    }

    private async void ButtonExportDB_Click(object sender, RoutedEventArgs e)
    {
        var senderButton = sender as Button;
        senderButton.IsEnabled = false;
        FileSavePicker savePicker = new Windows.Storage.Pickers.FileSavePicker();

        var window = App.MainWindow;
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hWnd);

        savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
        savePicker.FileTypeChoices.Add("Plik bazy danych", new List<string>() { ".db" });
        savePicker.SuggestedFileName = "Worklog-" + DateTime.Now.ToString("yyyy-MM-dd") + ".db";

        StorageFile destinationFile = await savePicker.PickSaveFileAsync();
        StorageFile dbFile = await Log.localFolder.GetFileAsync(Log.dbName);
        await dbFile.CopyAndReplaceAsync(destinationFile);
   
        senderButton.IsEnabled = true;
    }
}
