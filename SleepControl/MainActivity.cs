using System;
using System.Collections.Generic;
using System.IO;

using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.Widget;

namespace SleepControl
{
    [Activity(Label = "SleepControl", MainLauncher = true)]
    public class MainActivity : Activity
    {
        readonly string dbPath = Path.Combine(
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

            if (!File.Exists(dbPath))
                SQLiteSleepSessionCommands.CreateDatabase(dbPath);
            
            mSleepSessions = SQLiteSleepSessionCommands.FindAllSessions(dbPath);
            mSleepSessions = new List<SleepSession>();
            mAdapter = new SleepSessionAdapter(mSleepSessions);

            SetContentView(Resource.Layout.Main);
            mRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);

            mRecyclerView.SetAdapter(mAdapter);

            mLayoutManager = new LinearLayoutManager(this);
            mRecyclerView.SetLayoutManager(mLayoutManager);

            mAdapter = new SleepSessionAdapter(mSleepSessions);
            mRecyclerView.SetAdapter(mAdapter);
        }
    }
}

