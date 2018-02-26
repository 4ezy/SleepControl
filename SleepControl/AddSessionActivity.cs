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
using Java.Util;
using Java.Text;
using static Android.App.TimePickerDialog;

namespace SleepControl
{
    [Activity(Label = "SleepControl - Добавить сессию")]
    public class AddSessionActivity : Activity, IOnTimeSetListener
    {
        private int timeSetButtonId;
        private SleepSession sleepSession = new SleepSession();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.AddSession);

            var startSessionButton = FindViewById<ImageButton>(Resource.Id.startSleepChooseButton);
            startSessionButton.Click += delegate
            {
                timeSetButtonId = Resource.Id.startSleepChooseButton;
                TimePickerDialog timePickerDialog = new TimePickerDialog(
                    this, this, DateTime.Now.Hour, DateTime.Now.Minute, true);
                timePickerDialog.Show();
            };

            var endSessionButton = FindViewById<ImageButton>(Resource.Id.endSleepChooseButton);
            endSessionButton.Click += delegate
            {
                timeSetButtonId = Resource.Id.endSleepChooseButton;
                TimePickerDialog timePickerDialog = new TimePickerDialog(
                    this, this, DateTime.Now.Hour, DateTime.Now.Minute, true);
                timePickerDialog.Show();
            };

            var cancelSessionButton = FindViewById<Button>(Resource.Id.cancelSessionButton);
            cancelSessionButton.Click += delegate
            {
                this.Finish();
            };

            var addSessionButton = FindViewById<Button>(Resource.Id.saveSessionButton);
            addSessionButton.Click += delegate
            {
                sleepSession.Caption = FindViewById<EditText>(Resource.Id.descriptionEditText).Text;
                SQLiteSleepSessionCommands.InsertUpdateData(sleepSession,
                    this.Intent.GetStringExtra("dbPath"));
                var activity = new Intent(this, typeof(MainActivity));
                StartActivity(activity);
            };
        }

        public void OnTimeSet(TimePicker view, int hourOfDay, int minute)
        {
            DateTime dt;

            switch (timeSetButtonId)
            {
                case Resource.Id.startSleepChooseButton:
                    {
                        dt = sleepSession.StartSleepTime == DateTime.MinValue ?
                            DateTime.Now : sleepSession.StartSleepTime;

                        if (hourOfDay > dt.Hour && minute > dt.Minute)
                        {
                            sleepSession.StartSleepTime = new DateTime(
                                dt.Year, dt.Month, dt.Day + 1,
                                hourOfDay, minute, 0);
                        }
                        else
                        {
                            sleepSession.StartSleepTime = new DateTime(
                                dt.Year, dt.Month, dt.Day,
                                hourOfDay, minute, 0);
                        }

                        sleepSession.EndSleepTime = DateTime.MinValue;
                        var endSleepTextView = FindViewById<TextView>(Resource.Id.endSleepTextView);
                        endSleepTextView.Text = $"Конец сна:";

                        var startSleepTextView = FindViewById<TextView>(Resource.Id.startSleepTextView);
                        startSleepTextView.Text = $"Начало сна:\n{this.sleepSession.StartSleepTime.ToString("G")}";

                        break;
                    }
                case Resource.Id.endSleepChooseButton:
                    {
                        dt = sleepSession.StartSleepTime == DateTime.MinValue ?
                            DateTime.Now : sleepSession.StartSleepTime;

                        if (hourOfDay > sleepSession.StartSleepTime.Hour &&
                            minute > sleepSession.StartSleepTime.Minute)
                        {
                            sleepSession.EndSleepTime = new DateTime(
                                dt.Year, dt.Month, dt.Day,
                                hourOfDay, minute, 0);
                        }
                        else
                        {
                            sleepSession.EndSleepTime = new DateTime(
                                dt.Year, dt.Month, dt.Day + 1,
                                 hourOfDay, minute, 0);
                        }

                        var endSleepTextView = FindViewById<TextView>(Resource.Id.endSleepTextView);
                        endSleepTextView.Text = $"Конец сна:\n{this.sleepSession.EndSleepTime.ToString("G")}";
                        break;
                    }
                default:
                    break;
            }
        }
    }
}