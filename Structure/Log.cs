namespace WorkLog.Structure;

public static partial class Log
{
    public static async void PrepareLogs()
    {
        await DataAccess.InitializeDatabase();
    }
}