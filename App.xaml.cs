using Microsoft.UI.Xaml;

namespace WorkLog;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        Structure.Log.PrepareLogs();
    }

    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        m_window = new MainWindow();
        m_window.Activate();
        m_window.ExtendsContentIntoTitleBar = true;
    }

    private Window m_window;
}
