﻿using System;
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
    class SleepSessionAdapter : RecyclerView.Adapter, IItemClickListener
    {
        private List<SleepSession> mSessions;
        private Context context;
        public Action<int> OnRecyclerViewItemClickAction { get; set; }
        public Action<int> OnRecyclerViewItemLongClickAction { get; set; }

        public SleepSessionAdapter(List<SleepSession> sessions, Context context)
        {
            mSessions = sessions;
            this.context = context;
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
                $"-\n{mSessions[position].EndSleepTime.ToString("G")}";
            vh.SetItemClickListener(this);
        }

        public void OnClick(View view, int position, bool isLongClick)
        {
            if (!isLongClick)
            {
                OnRecyclerViewItemClickAction(position);
            }
            else
            {
                OnRecyclerViewItemLongClickAction(position);
            }
        }

        public override int ItemCount => mSessions.Count;
    }
}