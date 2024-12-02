using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Windows.Storage;

namespace WorkLog.Structure;
public partial class Log
{
    private static class DataAccess
    {
        private static readonly string workLogDataPath = SystemDataPaths.GetDefault().ProgramData + "\\WorkLog";
        private static readonly string dbName = "Worklog.db";
        private static readonly string dbPath = workLogDataPath + "\\" + dbName;

        internal static async void InitializeDatabase()
        {
            var workLogStorageFolder = await StorageFolder.GetFolderFromPathAsync(workLogDataPath);
            await workLogStorageFolder.CreateFileAsync(dbName, CreationCollisionOption.OpenIfExists);
            using var db = new SqliteConnection($"Filename={dbPath}");
            db.Open();
            var tableCommand = "CREATE TABLE IF NOT " +
                "EXISTS Entries (EntryID INTEGER PRIMARY KEY NOT NULL, " +
                "Type INTEGER NOT NULL, " +
                "BeginTime TEXT NOT NULL, " +
                "EndTime TEXT NOT NULL," +
                "Localization TEXT," +
                "Description TEXT)";

            var createTable = new SqliteCommand(tableCommand, db);

            createTable.ExecuteReader();

        }

        internal static bool AddData(int type, string beginTime, string endTime, string localization, string description)
        {
            using var db = new SqliteConnection($"Filename={dbPath}");
            db.Open();

            var insertCommand = new SqliteCommand();
            insertCommand.Connection = db;
            insertCommand.CommandText = "INSERT INTO Entries VALUES (NULL, @type, @beginTime, @endTime, @localization, @description);";
            insertCommand.Parameters.AddWithValue("@type", type);
            insertCommand.Parameters.AddWithValue("@beginTime", beginTime);
            insertCommand.Parameters.AddWithValue("@endTime", endTime);
            insertCommand.Parameters.AddWithValue("@localization", localization);
            insertCommand.Parameters.AddWithValue("@description", description);

            insertCommand.ExecuteReader();
            return true;

        }

        internal static List<Entry> GetDataFromMonth(string year, string month)
        {
            var entries = new List<Entry>();
            using (var db = new SqliteConnection($"Filename={dbPath}"))
            {
                db.Open();

                var selectCommand = new SqliteCommand();
                selectCommand.Connection = db;

                // Use parameterized query to prevent SQL injection attacks
                selectCommand.CommandText = "SELECT * from Entries WHERE strftime('%Y', BeginTime) = @year AND strftime('%m', BeginTime) = @month;";
                selectCommand.Parameters.AddWithValue("@year", year);
                selectCommand.Parameters.AddWithValue("@month", month);

                var query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    var entry = new Entry(
                        int.Parse(query["EntryID"].ToString()),
                        int.Parse(query["Type"].ToString()),
                        DateTime.Parse(query["BeginTime"].ToString()),
                        DateTime.Parse(query["EndTime"].ToString()),
                        query["Localization"].ToString(),
                        query["Description"].ToString()
                    );
                    entries.Add(entry);
                }
            }

            return entries;
        }

        internal static List<string> GetYears()
        {
            var years = new List<string>();
            using (var db = new SqliteConnection($"Filename={dbPath}"))
            {
                db.Open();

                var selectCommand = new SqliteCommand();
                selectCommand.Connection = db;

                // Use parameterized query to prevent SQL injection attacks
                selectCommand.CommandText = "SELECT DISTINCT strftime('%Y', BeginTime) from Entries;";

                var query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    years.Add(query.GetString(0));
                }
            }
            return years;
        }

        internal static List<string> GetMonths(string year)
        {
            var months = new List<string>();
            using (var db = new SqliteConnection($"Filename={dbPath}"))
            {
                db.Open();

                var selectCommand = new SqliteCommand();
                selectCommand.Connection = db;

                // Use parameterized query to prevent SQL injection attacks
                selectCommand.CommandText = "SELECT DISTINCT strftime('%m', BeginTime) from Entries WHERE strftime('%Y', BeginTime) = @year;";
                selectCommand.Parameters.AddWithValue("@year", year);

                var query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    months.Add(query.GetString(0));
                }
            }
            return months;
        }
    }
}
