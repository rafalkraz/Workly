using Microsoft.UI.Xaml;

namespace Workly;

public partial class App : Application
{
    public static MainWindow MainWindow = new();

    public App()
    {
        InitializeComponent();
        Structure.Log.PrepareLogs();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        MainWindow.Activate();
        MainWindow.ExtendsContentIntoTitleBar = true;
    }
}
