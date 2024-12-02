using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WorkLog.Structure;

public class Entry(int entryID, int type, DateTime beginTime, DateTime endTime, string localization, string description)
{
    // Types
    // 0 - standard
    // 1 - urlop
    // 2 - bezpłatne wolne
    public int EntryID { get; set; } = entryID;
    public int Type { get; set; } = type;
    public DateTime BeginTime { get; set; } = beginTime;
    public DateTime EndTime { get; set; } = endTime;
    public string Localization { get; set; } = localization;
    public string Description { get; set; } = description;
    public DateOnly Date = DateOnly.Parse(beginTime.ToShortDateString());
    public double Duration
    {
        get
        {
            return (EndTime - BeginTime).TotalMinutes;
        }
    }
    public string DurationRange
    {
        get
        {
            return BeginTime.ToString("HH:mm") + " - " + EndTime.ToString("HH:mm");
        }
    }
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
