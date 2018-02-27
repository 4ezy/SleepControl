using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;

namespace SleepControl
{
    static class SQLiteSleepSessionCommands
    {
        public static string CreateDatabase(string path)
        {
            try
            {
                var connection = new SQLiteAsyncConnection(path);
                connection.CreateTableAsync<SleepSession>();
                return "Database created or opened";
            }
            catch (SQLiteException ex)
            {
                return ex.Message;
            }
        }

        public static string InsertUpdateData(SleepSession data, string path)
        {
            try
            {
                var db = new SQLiteAsyncConnection(path);
                if (db.InsertAsync(data).Result != 0)
                    db.UpdateAsync(data);
                return "Single data file inserted or updated";
            }
            catch (SQLiteException ex)
            {
                return ex.Message;
            }
        }

        public static List<SleepSession> FindAllSessions(string path)
        {
            try
            {
                var db = new SQLiteAsyncConnection(path);
                var sessionsList = db.Table<SleepSession>();
                return sessionsList.ToListAsync().Result;
            }
            catch (SQLiteException)
            {
                return null;
            }
        }

        public static void DeleteDatabase(string path)
        {
            try
            {
                var connection = new SQLiteAsyncConnection(path);
                connection.DropTableAsync<SleepSession>();
            }
            catch (SQLiteException)
            {
                return;
            }
        }

        public static void DeleteSession(string path, SleepSession sleepSession)
        {
            try
            {
                var connection = new SQLiteConnection(path);
                connection.Delete<SleepSession>(sleepSession.ID);
            }
            catch (SQLiteException)
            {
                return;
            }
        }
    }
}