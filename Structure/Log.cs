using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Windows.Storage;
using static System.Net.Mime.MediaTypeNames;

namespace WorkLog.Structure
{
    public class Log
    {
        public Log()//List<StorageFile> files)
        {
            //Files = files;
        }
        public List<Year> Years { get; set; } = [];

        public static List<Entry> Entries { get; set; } = DataAccess.GetData();

        private static Log _instance;
        public static async Task<Log> GetInstance() {
            //if (_instance == null)
            //{
            //    //_instance = new Log();//await LoadFilesAsync());
            //    await _instance.BuildLog();
            //}
            //return _instance;
        }
        private static string workLogDataPath = SystemDataPaths.GetDefault().ProgramData + "\\WorkLog";
        public List<Entry> GetEntries()
        {
            return DataAccess.GetData();
        }
        private async Task BuildLog()
        {
            //Files.Clear();
            //Files = await LoadFilesAsync();
            //Years.Clear();
            //foreach (var file in Files)
            //{
            //    string yearNumber = file.Name.Replace(".json", "");
            //    if (yearNumber.Length == 4 && int.TryParse(yearNumber, out int n))
            //    {
            //        string jsonString = File.ReadAllText(file.Path);
            //        var result = JsonSerializer.Deserialize<List<Month>>(jsonString);
            //        Year year = new(yearNumber, result);
            //        Years.Add(year);
            //    }
            //    else
            //    {
            //        Debug.WriteLine("Ommiting file...");
                    
            //    }
            //}
            //Years.Reverse();
        }

        public bool SaveLog(Year year)
        {
            //List<Month> tempMonthList = [];
            //foreach (var month in year.Months)
            //{
            //    List<Entry> tempEntryList = month.Entries.OrderBy(entry => entry.Date).ThenBy(entry => entry.BeginTime).ToList();
            //    if (tempEntryList.Count > 0)
            //    {
            //        tempMonthList.Add(new Month(month.Name, tempEntryList));
            //    }
            //}
            //var tempYear = new Year(year.Name, tempMonthList);
            //var options = new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
            //try
            //{
            //    string jsonString = JsonSerializer.Serialize(tempYear.Months, options);
            //    File.WriteAllText($"{workLogDataPath}\\{year.Name}.json", jsonString);
            //    return true;
            //}
            //catch (Exception)
            //{
            //    throw new Exception();
            //}
            return false;
        }

        public async Task<bool> AddEntryToLogAsync(Entry entry)
        {
            var selectedYear = Years.Find(year => year.Name == entry.Date.Year.ToString());
            if (selectedYear != null)
            {
                var selectedMonth = selectedYear.Months.Find(month => month.Name == entry.Date.ToString("MM"));
                if (selectedMonth != null)
                { 
                    selectedMonth.Entries.Add(entry);
                    selectedMonth.Entries = selectedMonth.Entries.OrderBy(e => e.Date).ToList();
                }
                else selectedYear.Months.Add(new Month(entry.Date.ToString("MM"), [entry]));
                selectedYear.Months = selectedYear.Months.OrderByDescending(month => month.Name).ToList();
            }
            else
            {
                selectedYear = new Year(entry.Date.Year.ToString(), [new Month(entry.Date.ToString("MM"), [entry])]);
                Years.Add(selectedYear);
            }

            if (SaveLog(selectedYear))
            {
                await BuildLog();
                
                return true;
            }
            else
            {
                return false;
            }

            
        }

        public async Task<bool> EditEntryInLogAsync(Entry oldEntry, Entry newEntry)
        {
            await DeleteEntryFromLogAsync(oldEntry);
            await AddEntryToLogAsync(newEntry);
            return true;
        }

        public async Task<bool> DeleteEntryFromLogAsync(Entry entry, bool logSaving = true)
        {
            var yearInt = entry.Date.Year;
            var year = Years.Find(y => y.Name == yearInt.ToString());
            var monthInt = entry.Date.Month;
            var month = year.Months.Find(m => m.Name.TrimStart('0') == monthInt.ToString());
            var entry2 = month.Entries.Find(e => e.Equals(entry));
            if (!month.Entries.Remove(entry2))
            {
                throw new Exception();
            }

            
                
            if (SaveLog(year))
            {
                if (logSaving)
                {
                    await BuildLog();
                }
                        
                return true;
            }
            else
            {
                return false;
            }
        }

        private List<string> GetYearList()
        {
            var yearList = new List<string>();

            return yearList;
        }

        

    }
}
