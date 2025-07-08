using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Workly.Interfaces;

namespace Workly;

public sealed partial class HelperWindow : Window
{
    public enum Action
    {
        Add,
        Edit
    }
    public HelperWindow(IDataViewPage parentPage, object entry, Action action)
    {
        InitializeComponent();
        ExtendsContentIntoTitleBar = true;
        AppWindow.Title = "Wpis";
        AppWindow.SetIcon(@"Assets\Calendar.ico");
        AppWindow.Resize(new Windows.Graphics.SizeInt32(675, 800));
        OverlappedPresenter presenter = OverlappedPresenter.Create();
        presenter.IsMaximizable = false;
        presenter.PreferredMinimumWidth = 675;
        presenter.PreferredMaximumWidth = 675;
        AppWindow.SetPresenter(presenter);

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
