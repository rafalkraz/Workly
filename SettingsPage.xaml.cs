using System;
using System.Globalization;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
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
            string text = textBox.Text.Replace(',', '.');
            textBox.Text = text;
            textBox.SelectionStart = textBox.Text.Length;

            if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
            {
                var binding = textBox.GetBindingExpression(TextBox.TextProperty);
                if (binding != null)
                {
                    binding.UpdateSource();
                }
            }
            else
            {
                textBox.Text = string.Empty;
            }
        }
    }

    private void TextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        TextBox textBox = sender as TextBox;
        if (textBox != null && string.IsNullOrEmpty(textBox.Text))
        {
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
                ContentDialog dialog = new()
                {
                    XamlRoot = this.XamlRoot,
                    Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                    Title = "Sukces!",
                    Content = "Baza danych zosta³a zaimportowana",
                    CloseButtonText = "OK",
                    DefaultButton = ContentDialogButton.Close
                };
                await dialog.ShowAsync();
            }
        }
        catch (Exception)
        {
            ContentDialog dialog = new()
            {
                XamlRoot = this.XamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                Title = "Wyst¹pi³ b³¹d!",
                Content = "Nie uda³o siê zaimportowaæ bazy danych",
                CloseButtonText = "OK",
                DefaultButton = ContentDialogButton.Close
            };
            await dialog.ShowAsync();
        }
        
        senderButton.IsEnabled = true;

    }

    private async void ButtonExportDB_Click(object sender, RoutedEventArgs e)
    {
        var senderButton = sender as Button;
        senderButton.IsEnabled = false;
        FileSavePicker savePicker = new();

        var window = App.MainWindow;
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hWnd);

        savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
        savePicker.FileTypeChoices.Add("Plik bazy danych", [".db"]);
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
                await dialog.ShowAsync();
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

    private async void BugRequestCard_Click(object sender, RoutedEventArgs e)
    {
        await Windows.System.Launcher.LaunchUriAsync(new Uri("mailto:dev.rkraz@gmail.com?subject=Workly-BugOrFeatureReport"));
    }
}
