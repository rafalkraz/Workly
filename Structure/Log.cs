using System.Collections.Generic;
using System.Linq;

namespace WorkLog.Structure;

public static partial class Log
{
    private static List<string> _years;
    public static List<string> Years => _years;

    public static bool BuildLog()
    {
        DataAccess.InitializeDatabase();
        _years = DataAccess.GetYears();
        return true;
    }


    ///<summary>
    ///Checks if database contains any entries (if any 'years' exist)
    ///</summary>
    ///<returns>
    ///true, if there is any correct data in database; otherwise, false
    ///</returns>
    public static bool RefreshLog()
    {
        _years = DataAccess.GetYears().OrderDescending().ToList();
        if (_years.Count > 0)  return true;
        else return false;
    }

    public static List<Month> GetMonthsList(string year)
    {
        List<Month> months = [];
        foreach (var month in DataAccess.GetMonths(year))
        {
            months.Add(new Month(month));
        }
        months = months.OrderByDescending(m => m.Number).ToList();
        return months;
    }

    public static List<Entry> GetEntries(string year, Month month)
    {
        return DataAccess.GetDataFromMonth(year, month.Number);
    }

    ///<summary>
    ///Adds entry to database
    ///</summary>
    ///<returns>
    ///true, if adding was successful; otherwise, false
    ///</returns>
    public static bool AddEntry(Entry entry)
    {
        return DataAccess.AddData(entry.Type, entry.BeginTime.ToString("yyyy-MM-dd HH:mm:ss"), entry.EndTime.ToString("yyyy-MM-dd HH:mm:ss"), entry.Localization, entry.Description);
    }

    public static bool EditEntry(Entry entry)
    {
        return DataAccess.EditData(entry.EntryID, entry.Type, entry.BeginTime.ToString("yyyy-MM-dd HH:mm:ss"), entry.EndTime.ToString("yyyy-MM-dd HH:mm:ss"), entry.Localization, entry.Description);
    }

    public static bool DeleteEntry(Entry entry)
    {
        return DataAccess.DeleteData(entry.EntryID);
    }
}
