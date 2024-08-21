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

namespace WorkLog.Structure
{
    public class Log
    {
        public Log(List<StorageFile> files)
        {
            Files = files;
            BuildLog();
        }
        private List<StorageFile> Files;
        public List<Year> Years { get; } = [];

        private static Log _instance;
        public static async Task<Log> GetInstance() {
            if (_instance == null)
            {
                _instance = new Log(await LoadFilesAsync());
            }
            return _instance;
        }
        private static string workLogDataPath = SystemDataPaths.GetDefault().ProgramData + "\\WorkLog";

        // Returns list of files located in C:\ProgramData\WorkLog folder
        public static async Task<List<StorageFile>> LoadFilesAsync()
        {
            List<StorageFile> files = new();
            
            var workLogStorageFolder = await StorageFolder.GetFolderFromPathAsync(workLogDataPath);
            IReadOnlyList<StorageFile> fileList = await workLogStorageFolder.GetFilesAsync();
            foreach (var file in fileList)
            {
                files.Add(file);
            }
            return files;
        }

        void BuildLog()
        {
            foreach (var file in Files)
            {
                string yearNumber = file.Name.Replace(".json", "");
                if (yearNumber.Length == 4)
                {
                    string jsonString = File.ReadAllText(file.Path);
                    var result = JsonSerializer.Deserialize<List<Month>>(jsonString);
                    result.Reverse();
                    Year year = new(yearNumber, result);
                    Years.Add(year);
                }
                else
                {
                    Debug.WriteLine("Ommiting file...");
                }
            }
            Years.Reverse();
        }

        public bool SaveLog(Year year)
        {
            year.Months.Reverse();
            var tempMonthList = new List<Month>();
            foreach (var month in year.Months)
            {
                List<Entry> tempEntryList = month.Entries.OrderBy(entry => entry.Date).ThenBy(entry => entry.BeginTime).ToList();
                tempMonthList.Add(new Month(month.Name, tempEntryList));
            }
            var tempYear = new Year(year.Name, tempMonthList);
            var options = new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
            try
            {
                string jsonString = JsonSerializer.Serialize(tempYear.Months, options);
                File.WriteAllText($"{workLogDataPath}\\{year.Name}.json", jsonString);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            
        }

        public bool AddEntryToLog(Entry entry)
        {
            var selectedYear = Years.Find(year => year.Name == entry.Date.Year.ToString());
            if (selectedYear != null)
            {
                var selectedMonth = selectedYear.Months.Find(month => month.Name == entry.Date.ToString("MM"));
                if (selectedMonth != null)
                { 
                    selectedMonth.Entries.Add(entry);
                    //selectedMonth.Entries = selectedMonth.Entries.OrderBy(date => entry.Date);
                }
                else selectedYear.Months.Add(new Month(entry.Date.ToString("MM"), [entry]));
            }
            else
            {
                selectedYear = new Year(entry.Date.Year.ToString(), [new Month(entry.Date.ToString("MM"), [entry])]);
                Years.Add(selectedYear);
            }

            if (SaveLog(selectedYear))
            {
                BuildLog();
                
                return true;
            }
            else
            {
                return false;
            }

            
        }

        public bool DeleteEntryFromLog(Year year, Month month, Entry entry)
        {
            year.Months.Find(m => m.Name == month.Name).Entries.Remove(entry);
            return true;
        }
    }
}
