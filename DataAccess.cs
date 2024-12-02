using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.Windows.Storage;
using Windows.Management.Core;
using Windows.Storage;
using WorkLog.Structure;

namespace WorkLog
{
    public static class DataAccess
    {
        private static string workLogDataPath = SystemDataPaths.GetDefault().ProgramData + "\\WorkLog";
        private static string dbName = "Worklog.db";

        public async static void InitializeDatabase()
        {
            var workLogStorageFolder = await StorageFolder.GetFolderFromPathAsync(workLogDataPath);
            await workLogStorageFolder.CreateFileAsync(dbName, CreationCollisionOption.OpenIfExists);
            string dbPath = workLogDataPath + "\\" + dbName;
            using (var db = new SqliteConnection($"Filename={dbPath}"))
            {
                db.Open();
                string tableCommand = "CREATE TABLE IF NOT " +
                    "EXISTS Entries (EntryID INTEGER PRIMARY KEY NOT NULL, " +
                    "Type INTEGER NOT NULL, " +
                    "BeginTime TEXT NOT NULL, " +
                    "EndTime TEXT NOT NULL," +
                    "Localization TEXT," +
                    "Description TEXT)";

                var createTable = new SqliteCommand(tableCommand, db);

                createTable.ExecuteReader();
            }

        }

        public static void AddData(string inputText)
        {
            string dbpath = workLogDataPath + "\\" + dbName;
            using (var db = new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                var insertCommand = new SqliteCommand();
                insertCommand.Connection = db;

                // Use parameterized query to prevent SQL injection attacks
                // INSERT INTO Entries VALUES (NULL, 0, "2024-09-10 09:00:00.000", "2024-09-10 13:00:00.000", "IFPiLM", "Dyżur");
                insertCommand.CommandText = "INSERT INTO Entries VALUES (NULL, @Entry);";
                insertCommand.Parameters.AddWithValue("@Entry", inputText);

                insertCommand.ExecuteReader();
            }

        }

        public static List<Entry> GetData(string a = "*")
        {
            var entries = new List<Entry>();
            string dbpath = workLogDataPath + "\\" + dbName;
            using (var db = new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                var selectCommand = new SqliteCommand();
                selectCommand.Connection = db;

                // Use parameterized query to prevent SQL injection attacks
                selectCommand.CommandText = "SELECT @a from Entries;";
                selectCommand.Parameters.AddWithValue("@a", a);

                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    Entry entry = new Entry(
                        Int32.Parse(query["EntryID"].ToString()),
                        Int32.Parse(query["Type"].ToString()),
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

        public static List<string> GetYears()
        {
            var years = new List<string>();
            string dbpath = workLogDataPath + "\\" + dbName;
            using (var db = new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                var selectCommand = new SqliteCommand();
                selectCommand.Connection = db;

                // Use parameterized query to prevent SQL injection attacks
                selectCommand.CommandText = "SELECT strftime('%Y', BeginTime) from Entries;";

                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    years.Add(query.ToString());
                }
            }
            return years;
        }
    }
 
}
