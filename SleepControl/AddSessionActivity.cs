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
using System.IO;

namespace SleepControl
{
    [Activity(Label = "SleepControl - Добавить сессию")]
    public class AddSessionActivity : Activity, IOnTimeSetListener
    {
        private string numberStr;
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
                this.SetResult(Result.Canceled);
                this.Finish();
            };

            string path = Path.Combine(System.Environment.GetFolderPath(
                                System.Environment.SpecialFolder.Personal), "pn.dat");

            if (File.Exists(path))
                numberStr = File.ReadAllText(path);

            var smsSwith = FindViewById<Switch>(Resource.Id.smsRemindSwitch);

            if (numberStr is null)
                smsSwith.Visibility = ViewStates.Invisible;
            else
                smsSwith.Visibility = ViewStates.Visible;

            var addSessionButton = FindViewById<Button>(Resource.Id.saveSessionButton);
            addSessionButton.Click += delegate
            {
                if (sleepSession.StartSleepTime == DateTime.MinValue ||
                    sleepSession.EndSleepTime == DateTime.MinValue)
                {
                    Toast.MakeText(ApplicationContext,
                        "Для добавления требуется указать дату начала и конца сна",
                        ToastLength.Long).Show();
                }
                else
                {
                    sleepSession.Caption = FindViewById<EditText>(Resource.Id.descriptionEditText).Text;
                    SQLiteSleepSessionCommands.InsertUpdateData(sleepSession,
                        this.Intent.GetStringExtra("dbPath"));

                    var notificationSwitch = FindViewById<Switch>(Resource.Id.remindSwitch);

                    if (notificationSwitch.Checked)
                    {
                        var alarmIntent = new Intent(this, typeof(AlarmReceiver));
                        alarmIntent.PutExtra("title", "Начало сна");
                        alarmIntent.PutExtra("message", "Ложитесь спать и не забудьте завести будильник");

                        var pending = PendingIntent.GetBroadcast(this, 0,
                            alarmIntent, PendingIntentFlags.UpdateCurrent);

                        var alarmManager = GetSystemService(AlarmService).JavaCast<AlarmManager>();
                        Java.Util.Calendar calendar = Java.Util.Calendar.Instance;
                        calendar.TimeInMillis = Java.Lang.JavaSystem.CurrentTimeMillis();
                        calendar.Set(sleepSession.StartSleepTime.Year,
                            sleepSession.StartSleepTime.Month - 1,
                            sleepSession.StartSleepTime.Day,
                            sleepSession.StartSleepTime.Hour,
                            sleepSession.StartSleepTime.Minute,
                            0);
                        alarmManager.Set(AlarmType.RtcWakeup, calendar.TimeInMillis, pending);
                    }

                    var smsNotificationSwitch = FindViewById<Switch>(Resource.Id.smsRemindSwitch);

                    if (smsNotificationSwitch.Checked)
                    {
                        var alarmIntent = new Intent(this, typeof(PhoneReceiver));

                        alarmIntent.PutExtra("number", numberStr);
                        alarmIntent.PutExtra("message", "Доброе утро");
                        var pending = PendingIntent.GetBroadcast(this, 0,
                            alarmIntent, PendingIntentFlags.UpdateCurrent);
                        var alarmManager = GetSystemService(AlarmService).JavaCast<AlarmManager>();
                        Java.Util.Calendar calendar = Java.Util.Calendar.Instance;
                        calendar.TimeInMillis = Java.Lang.JavaSystem.CurrentTimeMillis();
                        calendar.Set(sleepSession.EndSleepTime.Year,
                            sleepSession.EndSleepTime.Month - 1,
                            sleepSession.EndSleepTime.Day,
                            sleepSession.EndSleepTime.Hour,
                            sleepSession.EndSleepTime.Minute,
                            0);
                        alarmManager.Set(AlarmType.RtcWakeup, calendar.TimeInMillis, pending);
                    }

                    this.SetResult(Result.Ok);
                    this.Finish();
                }
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

                        if (hourOfDay >= dt.Hour && minute >= dt.Minute)
                        {
                            sleepSession.StartSleepTime = new DateTime(
                                dt.Year, dt.Month, dt.Day,
                                hourOfDay, minute, 0);
                        }
                        else
                        {
                            sleepSession.StartSleepTime = new DateTime(
                                dt.Year, dt.Month, dt.Day + 1,
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
                        dt = sleepSession.EndSleepTime == DateTime.MinValue ?
                            DateTime.Now : sleepSession.StartSleepTime;

                        if (hourOfDay > sleepSession.StartSleepTime.Hour &&
                            minute > sleepSession.StartSleepTime.Minute)
                        {
                            sleepSession.EndSleepTime = new DateTime(
                                sleepSession.StartSleepTime.Year, sleepSession.StartSleepTime.Month, 
                                sleepSession.StartSleepTime.Day,
                                hourOfDay, minute, 0);
                        }
                        else
                        {
                            sleepSession.EndSleepTime = new DateTime(
                                sleepSession.StartSleepTime.Year, sleepSession.StartSleepTime.Month,
                                sleepSession.StartSleepTime.Day + 1,
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