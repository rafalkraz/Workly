using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Windows.Storage;

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

    public static bool RefreshLog()
    {
        _years = DataAccess.GetYears().OrderDescending().ToList();
        return true;
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

    public static bool AddEntry(Entry entry)
    {
        return DataAccess.AddData(entry.Type, entry.BeginTime.ToString("yyyy-MM-dd HH:mm:ss"), entry.EndTime.ToString("yyyy-MM-dd HH:mm:ss"), entry.Localization, entry.Description);
    }

    //public bool SaveLog(Year year)
    //{
    //    year.Months.Reverse();
    //    var tempMonthList = new List<Month>();
    //    foreach (var month in year.Months)
    //    {
    //        List<Entry> tempEntryList = month.Entries.OrderBy(entry => entry.Date).ThenBy(entry => entry.BeginTime).ToList();
    //        tempMonthList.Add(new Month(month.Name, tempEntryList));
    //    }
    //    var tempYear = new Year(year.Name, tempMonthList);
    //    var options = new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
    //    try
    //    {
    //        string jsonString = JsonSerializer.Serialize(tempYear.Months, options);
    //        File.WriteAllText($"{workLogDataPath}\\{year.Name}.json", jsonString);
    //        return true;
    //    }
    //    catch (Exception)
    //    {
    //        return false;
    //    }

    //}

    //public bool DeleteEntryFromLog(Year year, Month month, Entry entry)
    //{
    //    year.Months.Find(m => m.Name == month.Name).Entries.Remove(entry);
    //    return true;
    //}
}
