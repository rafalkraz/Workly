namespace WorkLog.Structure;

public static partial class Log
{
    public static async void PrepareLogs()
    {
        DataAccess.SetFirstStartupStatus();
        await DataAccess.InitializeDatabase();
    }
}