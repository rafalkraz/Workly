using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WorkLog.Structure
{
    public class Entry
    {
        public Entry(DateTime beginTime, DateTime endTime, string localization, string description, bool isDayOff, bool isUnpaid)
        {
            BeginTime = beginTime;
            EndTime = endTime;
            Localization = localization;
            Description = description;
            IsDayOff = isDayOff;
            IsUnpaid = isUnpaid;
            Duration = (endTime - beginTime).TotalMinutes;
            DurationRange = beginTime.ToString("HH:mm") + " - " + endTime.ToString("HH:mm");

            if (IsDayOff)
            {
                FontIcon = "\uE706";
            }
            else if (IsUnpaid)
            {
                FontIcon = "\uE7FD";
            }
        }

        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Localization { get; set; }
        public string Description { get; set; }
        public bool IsDayOff { get; set; }
        public bool IsUnpaid { get; set; }
        [JsonIgnore]
        public double Duration { get; }
        [JsonIgnore]
        public string DurationRange { get; }
        [JsonIgnore]
        public string FontIcon { get; }
    }


}
