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
            // Zamieñ przecinek na kropkê
            string text = textBox.Text.Replace(',', '.');

            // Aktualizuj wartoœæ TextBox
            textBox.Text = text;

            // Ustawienie kursora na koñcu tekstu
            textBox.SelectionStart = textBox.Text.Length;

            if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
            {
                // Prawid³owy format liczby dziesiêtnej
                // Ustaw wartoœæ w odpowiedniej w³aœciwoœci
                var binding = textBox.GetBindingExpression(TextBox.TextProperty);
                if (binding != null)
                {
                    binding.UpdateSource();
                }
            }
            else
            {
                // Nieprawid³owy format, wyczyœæ pole lub ustaw odpowiedni komunikat
                textBox.Text = string.Empty;
            }
        }
    }

    private void TextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        TextBox textBox = sender as TextBox;
        if (textBox != null && string.IsNullOrEmpty(textBox.Text))
        {
            // Ustaw domyœln¹ wartoœæ na 0, jeœli pole jest puste
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
                dialog.Title = "Sukces!\nBaza danych zosta³a zaimportowana";
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
            dialog.Title = "Wyst¹pi³ b³¹d!\nNie uda³o siê zaimportowaæ bazy danych";
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
                    Content = "Baza danych zosta³a wyeksportowana",
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
                Title = "Wyst¹pi³ b³¹d!",
                Content = "Nie uda³o siê wyeksportowaæ bazy danych",
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
            Title = "UWAGA!\nCzy na pewno chcesz usun¹æ wszystkie dane aplikacji?",
            Content = "Tej czynnoœci nie da siê cofn¹æ!",
            PrimaryButtonText = "USUÑ WSZYSTKO",
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
                    Title = "Wszystkie dane zosta³y pomyœlnie usuniête",
                    Content = "Aplikacja zostanie teraz zamkniêta",
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
                    Title = "Wyst¹pi³ b³¹d",
                    Content = "Aplikacja zostanie teraz zamkniêta",
                    CloseButtonText = "OK",
                    DefaultButton = ContentDialogButton.Close
                };
                await dialog2.ShowAsync();
                throw;
            }
        }
        
    }
}
