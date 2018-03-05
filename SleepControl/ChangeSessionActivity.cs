using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace SleepControl
{
    [Activity(Label = "SleepControl - Изменить сессию")]
    public class ChangeSessionActivity : Activity
    {
        readonly static string dbPath = Path.Combine(
            System.Environment.GetFolderPath(
            System.Environment.SpecialFolder.Personal),
            "sleepSessions.db3");
        private SleepSession sleepSession;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ChangeSession);

            sleepSession = SQLiteSleepSessionCommands.FindSessionById(
                dbPath, this.Intent.GetIntExtra("id", 0));

            FindViewById<TextView>(Resource.Id.startSleepTextView).Text =
                $"Начало сна:\n{this.sleepSession.StartSleepTime.ToString("G")}";

            FindViewById<TextView>(Resource.Id.endSleepTextView).Text =
                $"Конец сна:\n{this.sleepSession.EndSleepTime.ToString("G")}";

            FindViewById<EditText>(Resource.Id.descriptionEditText).Text =
                this.sleepSession.Caption;

            FindViewById<Switch>(Resource.Id.isTaskDone).Checked =
                this.sleepSession.IsTaskDone;

            var cancelSessionButton = FindViewById<Button>(Resource.Id.cancelSessionButton);
            cancelSessionButton.Click += delegate
            {
                this.SetResult(Result.Canceled);
                this.Finish();
            };

            var addSessionButton = FindViewById<Button>(Resource.Id.saveSessionButton);
            addSessionButton.Click += delegate
            {
                sleepSession.Caption = FindViewById<EditText>(
                    Resource.Id.descriptionEditText).Text;
                sleepSession.IsTaskDone = FindViewById<Switch>(
                    Resource.Id.isTaskDone).Checked;
                SQLiteSleepSessionCommands.ReplaceData(sleepSession, dbPath);
                this.Finish();
            };
        }
    }
}