using Microsoft.UI.Xaml;
using WinUIEx;
using Workly.Interfaces;

namespace Workly;

public sealed partial class HelperWindow : WinUIEx.WindowEx
{
    public enum Action
    {
        Add,
        Edit
    }
    public HelperWindow(IDataViewPage parentPage, object entry, Action action)
    {
        this.InitializeComponent();
        this.ExtendsContentIntoTitleBar = true;
        this.Title = "Wpis";
        this.SetIcon(@"Assets\Calendar.ico");

            LogPage.editLock = true;
        
            if (action == Action.Add)
            {
                TitleBarTextBlock.Text = "Dodawanie nowego wpisu";
                ContentFrame.Content = new EntryEditPage(parentPage, null, this);
            }
            else
            {
                TitleBarTextBlock.Text = "Edycja wpisu";
                ContentFrame.Content = new EntryEditPage(parentPage, entry, this);
            }
    }

    private void Window_Closed(object sender, WindowEventArgs args)
    {
        LogPage.editLock = false;
    }
}
