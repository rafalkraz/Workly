using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Workly.Interfaces;

namespace Workly.Structure;

public static partial class Log
{
    public static class Entries
    {
        private static List<string> _years;
        public static List<string> Years => _years;

        ///<summary>
        ///Checks if database contains any entries (if any 'years' exist)
        ///</summary>
        ///<returns>
        ///true, if there is any correct data in database; otherwise, false
        ///</returns>
        public static bool RefreshLog()
        {
            _years = DataAccess.GetYears("Entries").OrderDescending().ToList();
            if (_years.Count > 0) return true;
            else return false;
        }

        public static List<Month> GetMonthsList(string year)
        {
            List<Month> months = [];
            foreach (var month in DataAccess.GetMonths(year, "Entries"))
            {
                months.Add(new Month(month));
            }
            months = months.OrderByDescending(m => m.Number).ToList();
            return months;
        }

        public static List<Entry> GetEntries(string year, Month month, IDataViewPage sender)
        {
            return DataAccess.GetEntriesDataFromMonth(year, month.Number, sender);
        }

        ///<summary>
        ///Adds entry to database
        ///</summary>
        ///<returns>
        ///true, if adding was successful; otherwise, false
        ///</returns>
        public static bool AddEntry(Entry entry)
        {
            return DataAccess.AddDataToEntries(entry.Type, entry.BeginTime.ToString("yyyy-MM-dd HH:mm:ss"), entry.EndTime.ToString("yyyy-MM-dd HH:mm:ss"), entry.Localization, entry.Description, entry.Earning);
        }

        public static bool EditEntry(Entry entry)
        {
            return DataAccess.EditDataInEntries(entry.EntryID, entry.Type, entry.BeginTime.ToString("yyyy-MM-dd HH:mm:ss"), entry.EndTime.ToString("yyyy-MM-dd HH:mm:ss"), entry.Localization, entry.Description, entry.Earning);
        }

        public static bool DeleteEntry(Entry entry)
        {
            return DataAccess.DeleteDataFromEntries(entry.EntryID);
        }
    }
}
