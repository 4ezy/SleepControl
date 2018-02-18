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
using Android.Support.V7.Widget;

namespace SleepControl
{
    class SleepSessionAdapter : RecyclerView.Adapter
    {
        public List<SleepSession> mSessions;

        public SleepSessionAdapter(List<SleepSession> sessions)
        {
            mSessions = sessions;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = LayoutInflater.From(parent.Context).
                Inflate(Resource.Layout.SleepSessionView, parent, false);

            SleepSessionViewHolder vh = new SleepSessionViewHolder(view);

            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            SleepSessionViewHolder vh = holder as SleepSessionViewHolder;

            vh.Caption.Text = mSessions[position].Caption;
            vh.Dates.Text = $"{mSessions[position].StartSleepTime.ToString("G")} " +
                $"- {mSessions[position].EndSleepTime.ToString("G")}";
        }

        public override int ItemCount => mSessions.Count;
    }
}