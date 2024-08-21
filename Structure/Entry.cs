using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WorkLog.Structure
{
    public class Entry(int type, DateOnly date, TimeOnly beginTime, TimeOnly endTime, string localization, string description)
    {
        // Types
        // 0 - standard
        // 1 - urlop
        // 2 - bezpłatne wolne
        public int Type { get; set; } = type;
        public DateOnly Date { get; set; } = date;
        public TimeOnly BeginTime { get; set; } = beginTime;
        public TimeOnly EndTime { get; set; } = endTime;
        public string Localization { get; set; } = localization;
        public string Description { get; set; } = description;
        [JsonIgnore]
        public double Duration 
        { 
            get 
            {
                return (EndTime - BeginTime).TotalMinutes;
            } 
        }
        [JsonIgnore]
        public string DurationRange {
            get
            {
                return BeginTime.ToString("HH:mm") + " - " + EndTime.ToString("HH:mm");
            }
        }
        [JsonIgnore]
        public string FontIcon 
        {
            get
            {
                return Type switch
                {
                    0 => "",
                    1 => "\uE706",
                    2 => "\uE7FD",
                    _ => throw new Exception(),
                };
            }
        }
    }
}
