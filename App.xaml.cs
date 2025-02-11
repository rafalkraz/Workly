using Microsoft.UI.Xaml;

namespace WorkLog;

public partial class App : Application
{
    public static MainWindow MainWindow = new();

    public App()
    {
        InitializeComponent();
        Structure.Log.PrepareLogs();
    }

    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        MainWindow.Activate();
        MainWindow.ExtendsContentIntoTitleBar = true;
    }
}
