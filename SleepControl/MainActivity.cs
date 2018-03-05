using System;
using System.Collections.Generic;
using System.IO;

using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Content;
using Android.Runtime;
using Android.Views;
using System.Text;
using Android.Provider;
using Android.Database;

namespace SleepControl
{
    [Activity(Label = "SleepControl", MainLauncher = true)]
    public class MainActivity : Activity
    {
        readonly static string dbPath = Path.Combine(
            System.Environment.GetFolderPath(
            System.Environment.SpecialFolder.Personal), 
            "sleepSessions.db3");

        readonly static string phoneNumberPath = Path.Combine(
            System.Environment.GetFolderPath(
                System.Environment.SpecialFolder.Personal),
            "pn.dat");

        RecyclerView mRecyclerView;
        RecyclerView.LayoutManager mLayoutManager;
        SleepSessionAdapter mAdapter;
        List<SleepSession> mSleepSessions;
        const int addSessionRequestCode = 1;
        const int addPhoneNumberRequestCode = 2;
        const int changeSessionRequestCode = 3;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SQLiteSleepSessionCommands.CreateDatabase(dbPath);

            mSleepSessions = SQLiteSleepSessionCommands.FindAllSessions(dbPath);
            mSleepSessions.Reverse();
            mAdapter = new SleepSessionAdapter(mSleepSessions, this);

            SetContentView(Resource.Layout.Main);
            mRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            FindViewById<Button>(Resource.Id.addSessionButton).Click += delegate
            {
                var activity = new Intent(this, typeof(AddSessionActivity));
                activity.PutExtra("dbPath", dbPath);
                StartActivityForResult(activity, addSessionRequestCode);
            };

            mAdapter.OnRecyclerViewItemClickAction += ((position) =>
            {
                var activity = new Intent(this, typeof(ChangeSessionActivity));
                activity.PutExtra("id", mSleepSessions[position].ID);
                StartActivityForResult(activity, changeSessionRequestCode);
            });

            mAdapter.OnRecyclerViewItemLongClickAction += ((position) =>
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle("Внимание");
                builder.SetMessage("Вы точно хотите удалить эту сессию?");

                builder.SetNegativeButton("Отмена", delegate 
                {
                    Toast.MakeText(this, "Действие отменено", ToastLength.Short).Show();
                });

                builder.SetPositiveButton("Подтвердить", delegate
                {
                    SQLiteSleepSessionCommands.DeleteSession(dbPath,
                        mSleepSessions[position]);
                    Toast.MakeText(this, "Удалено", ToastLength.Short).Show();
                    this.Recreate();
                });

                Dialog dialog = builder.Create();
                dialog.Show();
            });

            mRecyclerView.SetAdapter(mAdapter);

            mLayoutManager = new LinearLayoutManager(this);
            mRecyclerView.SetLayoutManager(mLayoutManager);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            switch (requestCode)
            {
                case addSessionRequestCode:
                    {
                        if (resultCode == Result.Ok)
                        {
                            this.Recreate();
                            Toast.MakeText(this, "Сессия сна успешно добавлена",
                                ToastLength.Short).Show();
                        } 

                        break;
                    }
                case changeSessionRequestCode:
                    {
                        if (resultCode == Result.Ok)
                        {
                            this.Recreate();
                            Toast.MakeText(this, "Сессия сна успешно изменена",
                                ToastLength.Short).Show();
                        }

                        break;
                    }
                case addPhoneNumberRequestCode:
                    {
                        if (data != null)
                        {
                            string phoneNo = null;
                            Android.Net.Uri uri = data.Data;
                            ICursor cursor = ContentResolver.Query(uri, null, null, null, null);

                            if (cursor.MoveToFirst())
                            {
                                int phoneIndex = cursor.GetColumnIndex(
                                    ContactsContract.CommonDataKinds.Phone.Number);
                                phoneNo = cursor.GetString(phoneIndex);
                            }
                            cursor.Close();
                            File.WriteAllText(phoneNumberPath, phoneNo);
                            Toast.MakeText(this, "Номер успешно изменён", ToastLength.Short).Show();
                        }

                        break;
                    }
                default:
                    break;
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.options_menu, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_Delete:
                    {
                        AlertDialog.Builder builder = new AlertDialog.Builder(this);
                        builder.SetTitle("Внимание");
                        builder.SetMessage("Вы точно хотите удалить все сессии?");

                        builder.SetNegativeButton("Отмена", delegate
                        {
                            Toast.MakeText(this, "Действие отменено", ToastLength.Short).Show();
                        });

                        builder.SetPositiveButton("Подтвердить", delegate
                        {
                            SQLiteSleepSessionCommands.DeleteDatabase(dbPath);
                            Toast.MakeText(this, "Все сессии удалены", ToastLength.Short).Show();
                            this.Recreate();
                        });

                        Dialog dialog = builder.Create();
                        dialog.Show();
                        break;
                    }
                case Resource.Id.action_AddNumber:
                    {
                        Intent intent = new Intent(Intent.ActionPick,
                                ContactsContract.CommonDataKinds.Phone.ContentUri);
                        StartActivityForResult(intent,
                            addPhoneNumberRequestCode);
                        break;
                    }
                case Resource.Id.action_Export:
                    {
                        string data = SQLiteSleepSessionCommands.ExportDataAsString(dbPath);
                        string tmpFilePath = Path.Combine(
                            this.ApplicationContext.ExternalCacheDir.AbsolutePath,
                            "data.txt");

                        File.WriteAllText(tmpFilePath, data, Encoding.Default);

                        LayoutInflater li = LayoutInflater.From(this);
                        View exportView = li.Inflate(Resource.Layout.emailExport, null);
                        AlertDialog.Builder builder = new AlertDialog.Builder(this);
                        builder.SetView(exportView);
                        EditText editText = exportView.FindViewById<EditText>(
                            Resource.Id.emailAddress);

                        builder
                            .SetCancelable(true)
                            .SetPositiveButton("Ок", delegate
                            {
                                string to = editText.Text;
                                Intent i = new Intent(Intent.ActionSend);
                                i.AddFlags(ActivityFlags.GrantReadUriPermission);
                                i.SetType("message/rfc822");
                                i.PutExtra(Intent.ExtraEmail, new String[] { to });
                                i.PutExtra(Intent.ExtraSubject, "Экспортируемые данные");
                                Java.IO.File file = new Java.IO.File(tmpFilePath);
                                file.SetReadable(true, false);
                                Android.Net.Uri uri = Android.Net.Uri.FromFile(file);
                                i.PutExtra(Intent.ExtraStream, uri);

                                try
                                {
                                    StartActivity(Intent.CreateChooser(i, "Выберите приложение..."));
                                }
                                catch (ActivityNotFoundException)
                                {
                                    Toast.MakeText(this, "Почтовый клиент не установлен",
                                        ToastLength.Short).Show();
                                }
                            })
                        .SetNegativeButton("Отмена", ((object sender, DialogClickEventArgs e) =>
                        {
                            var dialog = sender as AlertDialog;
                            dialog.Cancel();
                        }));

                        AlertDialog alertDialog = builder.Create();
                        alertDialog.Show();
                        break;
                    }
                default:
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }
    }
}