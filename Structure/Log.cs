using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Workly.Structure;

public static partial class Log
{
    public static async void PrepareLogs()
    {
        await DataAccess.InitializeDatabaseAsync();
    }

    public static bool DropTables()
    {
        return DataAccess.DropTables();
    }
}