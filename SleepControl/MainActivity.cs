using System;
using System.Collections.Generic;
using System.IO;

using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Content;

namespace SleepControl
{
    [Activity(Label = "SleepControl", MainLauncher = true)]
    public class MainActivity : Activity
    {
        readonly static string dbPath = Path.Combine(
            System.Environment.GetFolderPath(
            System.Environment.SpecialFolder.Personal), 
            "sleepSessions.db3");
        RecyclerView mRecyclerView;
        RecyclerView.LayoutManager mLayoutManager;
        SleepSessionAdapter mAdapter;
        List<SleepSession> mSleepSessions;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SQLiteSleepSessionCommands.CreateDatabase(dbPath);
            //SQLiteSleepSessionCommands.InsertUpdateData(new SleepSession()
            //{
            //    Caption = "Hello",
            //    StartSleepTime = DateTime.Now,
            //    EndSleepTime = new DateTime(2018, 2, 23, 11, 0, 0)
            //}, dbPath);
            mSleepSessions = SQLiteSleepSessionCommands.FindAllSessions(dbPath);
            mAdapter = new SleepSessionAdapter(mSleepSessions, this);

            SetContentView(Resource.Layout.Main);
            mRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            FindViewById<Button>(Resource.Id.addButton).Click += delegate
            {
                var activity = new Intent(this, typeof(AddSessionActivity));
                StartActivity(activity);
            };

            mAdapter.OnRecyclerViewItemClickAction += (() =>
            {
                var activity = new Intent(this, typeof(AddSessionActivity));
                StartActivity(activity);
            });
            mRecyclerView.SetAdapter(mAdapter);

            mLayoutManager = new LinearLayoutManager(this);
            mRecyclerView.SetLayoutManager(mLayoutManager);
        }
    }
}

