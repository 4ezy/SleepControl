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

        string phoneNumber;

        RecyclerView mRecyclerView;
        RecyclerView.LayoutManager mLayoutManager;
        SleepSessionAdapter mAdapter;
        List<SleepSession> mSleepSessions;
        readonly static int addSessionRequestCode = 1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if (File.Exists(phoneNumber))
                File.ReadAllText(phoneNumber);

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

            mAdapter.OnRecyclerViewItemClickAction += (() =>
            {
                var activity = new Intent(this, typeof(AddSessionActivity));
                StartActivity(activity);
            });

            mAdapter.OnRecyclerViewItemLongClickAction += ((int position) =>
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle("Внимание");
                builder.SetMessage("Вы точно хотите удалить эту сессию?");

                builder.SetNegativeButton("Отмена", delegate 
                {
                    Toast.MakeText(this, "Действие отменено", ToastLength.Short);
                });

                builder.SetPositiveButton("Подтвердить", delegate
                {
                    SQLiteSleepSessionCommands.DeleteSession(dbPath,
                        mSleepSessions[position]);
                    Toast.MakeText(this, "Удалено", ToastLength.Short);
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
            if (resultCode == Result.Ok)
                this.Recreate();
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
                            Toast.MakeText(this, "Действие отменено", ToastLength.Short);
                        });

                        builder.SetPositiveButton("Подтвердить", delegate
                        {
                            SQLiteSleepSessionCommands.DeleteDatabase(dbPath);
                            Toast.MakeText(this, "Все сессии удалены", ToastLength.Short);
                            this.Recreate();
                        });

                        Dialog dialog = builder.Create();
                        dialog.Show();
                        break;
                    }
                case Resource.Id.action_AddNumber:
                    {
                        LayoutInflater li = LayoutInflater.From(this);
                        View promtsView = li.Inflate(Resource.Layout.promt, null);
                        AlertDialog.Builder builder = new AlertDialog.Builder(this);
                        builder.SetView(promtsView);
                        EditText editText = promtsView.FindViewById<EditText>(
                            Resource.Id.phoneNumber);
                        builder
                            .SetCancelable(true)
                            .SetPositiveButton("Ок", delegate
                        {
                            phoneNumber = editText.Text;
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
                case Resource.Id.action_Export:
                    {
                        string data = SQLiteSleepSessionCommands.ExportDataAsString(dbPath);
                        string tmpFilePath = Path.Combine(
                            System.Environment.GetFolderPath(
                                System.Environment.SpecialFolder.Personal),
                            "data.txt");

                        File.Create(tmpFilePath);

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
                                string email = editText.Text;
                                Intent i = new Intent(Intent.ActionSend);
                                i.SetType("message/rfc822");
                                i.PutExtra(Intent.ExtraEmail, new String[] { email });
                                i.PutExtra(Intent.ExtraSubject, "Экспортируемые данные");
                                i.PutExtra(Intent.ExtraStream, tmpFilePath);

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

                        File.Delete(tmpFilePath);

                        AlertDialog alertDialog = builder.Create();
                        alertDialog.Show();
                        break;
                    }
                default:
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void CancelAction(object sender, DialogClickEventArgs e)
        {
            var dialog = sender as AlertDialog;
            dialog.Cancel();
        }
    }
}