using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace WorkLog.Structure;

public static partial class Log
{
    public static async void PrepareLogs()
    {
        await DataAccess.InitializeDatabase();
    }

    public static bool DropTables()
    {
        return DataAccess.DropTables();
    }
}