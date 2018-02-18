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
    class SleepSessionViewHolder : RecyclerView.ViewHolder
    {
        public TextView Caption { get; set; }
        public TextView Dates { get; set; }

        public SleepSessionViewHolder(View view) : base(view)
        {
            Caption = view.FindViewById<TextView>(Resource.Id.captionView);
            Dates = view.FindViewById<TextView>(Resource.Id.datesView);
        }
    }
}