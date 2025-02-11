using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage;

namespace WorkLog.Structure;
public partial class Log
{
    public static readonly StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
    public static readonly string dbName = "Worklog.db";

    private static class DataAccess
    {
        private static StorageFile dbFile;

        internal static void SetFirstStartupStatus()
        {
            if (!File.Exists(localFolder.Path + "\\" + dbName)) ; //IsFirstStartup = true;
        }

        internal static async Task<bool> InitializeDatabase()
        {
            dbFile = await localFolder.CreateFileAsync(dbName, CreationCollisionOption.OpenIfExists);
            using var db = new SqliteConnection($"Filename={dbFile.Path}");
            db.Open();
            var entriesTableCreationCommand = "CREATE TABLE IF NOT " +
                "EXISTS Entries (EntryID INTEGER PRIMARY KEY NOT NULL, " +
                "Type INTEGER NOT NULL, " +
                "BeginTime TEXT NOT NULL, " +
                "EndTime TEXT NOT NULL," +
                "Localization TEXT," +
                "Description TEXT," +
                "Earning TEXT)";

            var mileageTableCreationCommand = "CREATE TABLE IF NOT " +
                "EXISTS Mileage (ID INTEGER PRIMARY KEY NOT NULL, " +
                "Type INTEGER NOT NULL, " +
                "Date TEXT NOT NULL, " +
                "BeginPoint TEXT NOT NULL, " +
                "EndPoint TEXT," +
                "Description TEXT," +
                "Distance TEXT," +
                "ParkingPrice TEXT)";

            var createEntriesTable = new SqliteCommand(entriesTableCreationCommand, db);
            var createMileageTable = new SqliteCommand(mileageTableCreationCommand, db);

            try
            {
                createEntriesTable.ExecuteReader();
                createMileageTable.ExecuteReader();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        

        internal static List<Entry> GetEntriesDataFromMonth(string year, string month)
        {
            var result = new List<Entry>();
            using (var db = new SqliteConnection($"Filename={dbFile.Path}"))
            {
                db.Open();

                var selectCommand = new SqliteCommand();
                selectCommand.Connection = db;
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
                        query["Description"].ToString(),
                        double.Parse(query["Earning"].ToString(), CultureInfo.InvariantCulture)
                    );
                    result.Add(entry);
                }
            }
            return result;
        }

        internal static List<EntryMileage> GetMileageDataFromMonth(string year, string month)
        {
            var result = new List<EntryMileage>();
            using (var db = new SqliteConnection($"Filename={dbFile.Path}"))
            {
                db.Open();

                var selectCommand = new SqliteCommand();
                selectCommand.Connection = db;
                selectCommand.CommandText = "SELECT * from Mileage WHERE strftime('%Y', Date) = @year AND strftime('%m', Date) = @month;";
                selectCommand.Parameters.AddWithValue("@year", year);
                selectCommand.Parameters.AddWithValue("@month", month);

                var query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    var entry = new EntryMileage(
                        int.Parse(query["ID"].ToString()),
                        int.Parse(query["Type"].ToString()),
                        DateOnly.Parse(DateTime.Parse(query["Date"].ToString()).ToShortDateString()),
                        query["BeginPoint"].ToString(),
                        query["EndPoint"].ToString(),
                        query["Description"].ToString(),
                        int.Parse(query["Distance"].ToString()),
                        float.Parse(query["ParkingPrice"].ToString())
                    );
                    result.Add(entry);
                }
            }
            return result;
        }

        internal static List<string> GetYears(string table)
        {
            var years = new List<string>();
            using (var db = new SqliteConnection($"Filename={dbFile.Path}"))
            {
                db.Open();

                var column = "";
                if (table == "Entries") column = "BeginTime";
                else if (table == "Mileage") column = "Date";

                var selectCommand = new SqliteCommand();
                selectCommand.Connection = db;
                selectCommand.CommandText = $"SELECT DISTINCT strftime('%Y', {column}) from {table};";
                var query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    years.Add(query.GetString(0));
                }
            }
            return years;
        }

        internal static List<string> GetMonths(string year, string table)
        {
            var months = new List<string>();
            using (var db = new SqliteConnection($"Filename={dbFile.Path}"))
            {
                db.Open();

                var column = "";
                if (table == "Entries") column = "BeginTime";
                else if (table == "Mileage") column = "Date";

                var selectCommand = new SqliteCommand();
                selectCommand.Connection = db;
                selectCommand.CommandText = $"SELECT DISTINCT strftime('%m', {column}) from {table} WHERE strftime('%Y', {column}) = @year;";
                selectCommand.Parameters.AddWithValue("@year", year);

                var query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    months.Add(query.GetString(0));
                }
            }
            return months;
        }

        internal static bool AddDataToEntries(int type, string beginTime, string endTime, string localization, string description, double earning)
        {
            using var db = new SqliteConnection($"Filename={dbFile.Path}");
            db.Open();

            var insertCommand = new SqliteCommand();
            insertCommand.Connection = db;
            insertCommand.CommandText = "INSERT INTO Entries VALUES (NULL, @type, @beginTime, @endTime, @localization, @description, @earning);";
            insertCommand.Parameters.AddWithValue("@type", type);
            insertCommand.Parameters.AddWithValue("@beginTime", beginTime);
            insertCommand.Parameters.AddWithValue("@endTime", endTime);
            insertCommand.Parameters.AddWithValue("@localization", localization);
            insertCommand.Parameters.AddWithValue("@description", description);
            insertCommand.Parameters.AddWithValue("@earning", earning);

            insertCommand.ExecuteReader();
            return true;

        }

        internal static bool AddDataToMileage(int type, string date, string beginPoint, string endPoint, string description, string distance, string parkingPrice)
        {
            using var db = new SqliteConnection($"Filename={dbFile.Path}");
            db.Open();

            var insertCommand = new SqliteCommand();
            insertCommand.Connection = db;
            insertCommand.CommandText = "INSERT INTO Mileage VALUES (NULL, @type, @date, @beginPoint, @endPoint, @description, @distance, @parkingPrice);";
            insertCommand.Parameters.AddWithValue("@type", type);
            insertCommand.Parameters.AddWithValue("@date", date);
            insertCommand.Parameters.AddWithValue("@beginPoint", beginPoint);
            insertCommand.Parameters.AddWithValue("@endPoint", endPoint);
            insertCommand.Parameters.AddWithValue("@description", description);
            insertCommand.Parameters.AddWithValue("@distance", distance);
            insertCommand.Parameters.AddWithValue("@parkingPrice", parkingPrice);

            insertCommand.ExecuteReader();
            return true;

        }

        internal static bool EditDataInEntries(int entryID, int newType, string newBeginTime, string newEndTime, string newLocalization, string newDescription, double newEarning)
        {
            using var db = new SqliteConnection($"Filename={dbFile.Path}");
            db.Open();

            var insertCommand = new SqliteCommand();
            insertCommand.Connection = db;
            insertCommand.CommandText = "UPDATE Entries SET Type = @newType, BeginTime = @newBeginTime, EndTime = @newEndTime, Localization = @newLocalization, Description = @newDescription, Earning = @newEarning WHERE EntryID = @entryID;";
            insertCommand.Parameters.AddWithValue("@entryID", entryID);
            insertCommand.Parameters.AddWithValue("@newType", newType);
            insertCommand.Parameters.AddWithValue("@newBeginTime", newBeginTime);
            insertCommand.Parameters.AddWithValue("@newEndTime", newEndTime);
            insertCommand.Parameters.AddWithValue("@newLocalization", newLocalization);
            insertCommand.Parameters.AddWithValue("@newDescription", newDescription);
            insertCommand.Parameters.AddWithValue("@newEarning", newEarning);

            insertCommand.ExecuteReader();
            return true;

        }

        internal static bool EditDataInMileage(int ID, int newType, string newDate, string newBeginPoint, string newEndPoint, string newDescription, string newDistance, string newParkingPrice)
        {
            using var db = new SqliteConnection($"Filename={dbFile.Path}");
            db.Open();

            var insertCommand = new SqliteCommand();
            insertCommand.Connection = db;
            insertCommand.CommandText = "UPDATE Mileage SET Type = @newType, Date = @newDate, BeginPoint = @newBeginPoint, EndPoint = @newEndPoint, Description = @newDescription, Distance = @newDistance, ParkingPrice = @newParkingPrice WHERE ID = @ID;";
            insertCommand.Parameters.AddWithValue("@ID", ID);
            insertCommand.Parameters.AddWithValue("@newType", newType);
            insertCommand.Parameters.AddWithValue("@newDate", newDate);
            insertCommand.Parameters.AddWithValue("@newBeginPoint", newBeginPoint);
            insertCommand.Parameters.AddWithValue("@newEndPoint", newEndPoint);
            insertCommand.Parameters.AddWithValue("@newDescription", newDescription);
            insertCommand.Parameters.AddWithValue("@newDistance", newDistance);
            insertCommand.Parameters.AddWithValue("@newParkingPrice", newParkingPrice);

            insertCommand.ExecuteReader();
            return true;

        }

        internal static bool DeleteDataFromEntries(int entryID)
        {
            using var db = new SqliteConnection($"Filename={dbFile.Path}");
            db.Open();

            var cmd = new SqliteCommand
            {
                Connection = db,
                CommandText = "DELETE FROM Entries WHERE EntryID = @entryID;"
            };
            cmd.Parameters.AddWithValue("@entryID", entryID);
            cmd.ExecuteReader();
            return true;
        }

        internal static bool DeleteDataFromMileage(int ID)
        {
            using var db = new SqliteConnection($"Filename={dbFile.Path}");
            db.Open();

            var cmd = new SqliteCommand
            {
                Connection = db,
                CommandText = "DELETE FROM Mileage WHERE ID = @ID;"
            };
            cmd.Parameters.AddWithValue("@ID", ID);
            cmd.ExecuteReader();
            return true;
        }
    }
}
