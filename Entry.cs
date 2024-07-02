using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkLog
{
    public class Entry(DateTime beginTime, DateTime endTime, string localization, string description, bool isDayOff, bool isUnpaid)
    {
        public DateTime BeginTime { get; set; } = beginTime;
        public DateTime EndTime { get; set; } = endTime;
        public string Localization { get; set; } = localization;
        public string Description { get; set; } = description;
        public bool IsDayOff { get; set; } = isDayOff;
        public bool IsUnpaid { get; set; } = isUnpaid;
        public string Duration { get; } = beginTime.ToString("HH:mm") + " - " + endTime.ToString("HH:mm");
    }


}
