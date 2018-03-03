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

        public static string DeleteDatabase(string path)
        {
            try
            {
                var connection = new SQLiteAsyncConnection(path);
                connection.DropTableAsync<SleepSession>();
                return "Table dropped";
            }
            catch (SQLiteException ex)
            {
                return ex.Message;
            }
        }

        public static string DeleteSession(string path, SleepSession sleepSession)
        {
            try
            {
                var connection = new SQLiteAsyncConnection(path);
                connection.DeleteAsync(sleepSession.ID);
                return "Single data deleted";
            }
            catch (SQLiteException ex)
            {
                return ex.Message;
            }
        }

        public static string ExportDataAsString(string path)
        {
            try
            {
                List<SleepSession> sessions = FindAllSessions(path);
                string str = String.Empty;

                foreach (SleepSession session in sessions)
                {
                    str += $"{session.ID}\t{session.Caption}\t" +
                        $"{session.StartSleepTime}\t{session.EndSleepTime}\t{session.IsTaskDone}\n";
                }

                return str;
            }
            catch (SQLiteException ex)
            {
                return ex.Message;
            }
        }
    }
}