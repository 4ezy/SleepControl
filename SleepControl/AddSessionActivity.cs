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

        public void OnTimeSet(TimePicker view, int hourOfDay, int minute)
        {
            DateTime now = DateTime.Now;

            switch (timeSetButtonId)
            {
                case Resource.Id.startSleepChooseButton:
                    {
                        if (hourOfDay > now.Hour && minute > now.Minute)
                        {
                            sleepSession.StartSleepTime = new DateTime(
                                now.Year, now.Month, now.Day + 1,
                                hourOfDay, minute, 0);
                        }
                        else
                        {
                            sleepSession.StartSleepTime = new DateTime(
                                now.Year, now.Month, now.Day,
                                hourOfDay, minute, 0);
                        }

                        var startSleepTextView = FindViewById<TextView>(Resource.Id.startSleepTextView);
                        startSleepTextView.Text = $"Начало сна:\n{this.sleepSession.StartSleepTime.ToString("G")}";
                        break;
                    }
                case Resource.Id.endSleepChooseButton:
                    {
                        if (hourOfDay > sleepSession.StartSleepTime.Hour &&
                            minute > sleepSession.StartSleepTime.Minute)
                        {
                            sleepSession.EndSleepTime = new DateTime(
                                now.Year, now.Month, now.Day,
                                hourOfDay, minute, 0);
                        }
                        else
                        {
                            sleepSession.EndSleepTime = new DateTime(
                                now.Year, now.Month, now.Day + 1,
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
        }
    }
}