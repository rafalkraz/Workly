using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
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

        // Returns list of files located in C:\ProgramData\WorkLog folder
        public static async Task<List<StorageFile>> LoadFilesAsync()
        {
            List<StorageFile> files = new();
            var workLogDataPath = SystemDataPaths.GetDefault().ProgramData + "\\WorkLog";
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
                    throw new Exception();
                }
            }
            Years.Reverse();
        }
    }
}
