using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Telephony.Gsm;
using Android.Views;
using Android.Widget;

namespace SleepControl
{
    [BroadcastReceiver]
    class PhoneReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            var message = intent.GetStringExtra("message");
            var number = intent.GetStringExtra("number");

            try
            {
                SmsManager.Default.SendTextMessage(number, null,
                    message, null, null);
            }
            catch (Java.Lang.IllegalArgumentException)
            {
                var resultIntent = new Intent(context, typeof(MainActivity));
                resultIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);

                var pending = PendingIntent.GetActivity(context, 0, resultIntent,
                    PendingIntentFlags.CancelCurrent);

                var builder = new Notification.Builder(context)
                    .SetContentTitle("Ошибка")
                    .SetContentText($"Сообщение на номер {number} не было отправлено")
                    .SetSmallIcon(Resource.Drawable.alarm)
                    .SetDefaults(NotificationDefaults.All);

                builder.SetContentIntent(pending);

                var notification = builder.Build();

                var manager = NotificationManager.FromContext(context);
                manager.Notify(0, notification);
            }
        }
    }
}