using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Workly.Structure;

namespace Workly;

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
        try
        {
            if (file != null)
            {
                await file.CopyAsync(Log.localFolder, Log.dbName, NameCollisionOption.ReplaceExisting);
                ContentDialog dialog = new ContentDialog();
                dialog.XamlRoot = this.XamlRoot;
                dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
                dialog.Title = "Sukces!\nBaza danych zosta�a zaimportowana";
                dialog.CloseButtonText = "OK";
                dialog.DefaultButton = ContentDialogButton.Close;
                var result = await dialog.ShowAsync();
            }
        }
        catch (Exception)
        {
            ContentDialog dialog = new ContentDialog();
            dialog.XamlRoot = this.XamlRoot;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.Title = "Wyst�pi� b��d!\nNie uda�o si� zaimportowa� bazy danych";
            dialog.CloseButtonText = "OK";
            dialog.DefaultButton = ContentDialogButton.Close;
            var result = await dialog.ShowAsync();
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
        try
        {
            if (destinationFile != null)
            {
                await dbFile.CopyAndReplaceAsync(destinationFile);
                ContentDialog dialog = new()
                {
                    XamlRoot = this.XamlRoot,
                    Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                    Title = "Sukces!",
                    Content = "Baza danych zosta�a wyeksportowana",
                    CloseButtonText = "OK",
                    DefaultButton = ContentDialogButton.Close
                };
                var result = await dialog.ShowAsync();
            }
        }
        catch (Exception)
        {
            ContentDialog dialog = new()
            {
                XamlRoot = this.XamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                Title = "Wyst�pi� b��d!",
                Content = "Nie uda�o si� wyeksportowa� bazy danych",
                CloseButtonText = "OK",
                DefaultButton = ContentDialogButton.Close
            };
            await dialog.ShowAsync();
        }
        
   
        senderButton.IsEnabled = true;
    }

    private async void ButtonDeleteDB_Click(object sender, RoutedEventArgs e)
    {
        ContentDialog dialog = new()
        {
            XamlRoot = this.XamlRoot,
            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
            Title = "UWAGA!\nCzy na pewno chcesz usun�� wszystkie dane aplikacji?",
            Content = "Tej czynno�ci nie da si� cofn��!",
            PrimaryButtonText = "USU� WSZYSTKO",
            CloseButtonText = "Anuluj",
            DefaultButton = ContentDialogButton.Primary
        };

        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            try
            {
                Log.DropTables();
                ContentDialog dialog2 = new()
                {
                    XamlRoot = this.XamlRoot,
                    Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                    Title = "Wszystkie dane zosta�y pomy�lnie usuni�te",
                    Content = "Aplikacja zostanie teraz zamkni�ta",
                    CloseButtonText = "OK",
                    DefaultButton = ContentDialogButton.Close
                };
                await dialog2.ShowAsync();
                Application.Current.Exit();
            }
            catch (Exception)
            {
                ContentDialog dialog2 = new()
                {
                    XamlRoot = this.XamlRoot,
                    Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                    Title = "Wyst�pi� b��d",
                    Content = "Aplikacja zostanie teraz zamkni�ta",
                    CloseButtonText = "OK",
                    DefaultButton = ContentDialogButton.Close
                };
                await dialog2.ShowAsync();
                throw;
            }
        }
        
    }
}
