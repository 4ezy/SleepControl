using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;

namespace SleepControl
{
    class SleepSession
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public string Caption { get; set; }

        public DateTime StartSleepTime { get; set; }

        public DateTime EndSleepTime { get; set; }

        public bool IsTaskDone { get; set; }

        public override string ToString()
        {
            return $"[SleepSession: ID={this.ID}, " +
                $"Caption={this.Caption}, " +
                $"StartSleepTime={this.StartSleepTime.ToString("G")}, " +
                $"EndSleepTime={this.EndSleepTime.ToString("G")}, " +
                $"IsTaskDone={this.IsTaskDone}]";
        }
    }
}