using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

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

}
