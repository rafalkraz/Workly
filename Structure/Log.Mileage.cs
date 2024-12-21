using System.Collections.Generic;
using System.Linq;

namespace WorkLog.Structure;

public static partial class Log
{
    public static class Mileage
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
            _years = DataAccess.GetYears("Mileage").OrderDescending().ToList();
            if (_years.Count > 0) return true;
            else return false;
        }

        public static List<Month> GetMonthsList(string year)
        {
            List<Month> months = [];
            foreach (var month in DataAccess.GetMonths(year, "Mileage"))
            {
                months.Add(new Month(month));
            }
            months = months.OrderByDescending(m => m.Number).ToList();
            return months;
        }

        public static List<EntryMileage> GetEntries(string year, Month month)
        {
            return DataAccess.GetMileageDataFromMonth(year, month.Number);
        }

        ///<summary>
        ///Adds entry to database
        ///</summary>
        ///<returns>
        ///true, if adding was successful; otherwise, false
        ///</returns>
        public static bool AddEntry(EntryMileage entry)
        {
            return DataAccess.AddDataToMileage(entry.Type, entry.Date.ToString("yyyy-MM-dd HH:mm:ss"), entry.BeginPoint, entry.EndPoint, entry.Purpose, entry.Distance.ToString(), entry.ParkingPrice.ToString());
        }

        public static bool EditEntry(EntryMileage entry)
        {
            return DataAccess.EditDataInMileage(entry.ID, entry.Type, entry.Date.ToString("yyyy-MM-dd HH:mm:ss"), entry.BeginPoint, entry.EndPoint, entry.Purpose, entry.Distance.ToString(), entry.ParkingPrice.ToString());
        }

        public static bool DeleteEntry(EntryMileage entry)
        {
            return DataAccess.DeleteDataFromEntries(entry.ID);
        }
    }
}
