using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Windows.UI.StartScreen;

namespace WorkLog.Structure;

public class Entry(int entryID, int type, DateTime beginTime, DateTime endTime, string localization, string description, double earning)
{
    // Types
    // 0 - standard
    // 1 - nadgodziny
    // 2 - urlop
    // 3 - bezpłatne wolne

    public int EntryID { get; set; } = entryID;
    public int Type { get; set; } = type;
    public DateTime BeginTime { get; set; } = beginTime;
    public DateTime EndTime { get; set; } = endTime;
    public string Localization { get; set; } = localization;
    public string Description { get; set; } = description;
    public double Earning { get; set; } = earning;
    public DateOnly Date = DateOnly.Parse(beginTime.ToShortDateString());
    public string Duration
    {
        get
        {
            var time = TimeSpan.FromMinutes((EndTime - BeginTime).TotalMinutes);
            if (time.TotalHours >= 1)
            {
                if (time.Minutes != 0)
                {
                    return $"{time.Hours}h {time.Minutes}min";
                }
                else
                {
                    return $"{time.Hours}h";
                }
            }
            else
            {
                return $"{time.Minutes}min";
            }
            //string.Format("{0:00}:{1:00}", (int)time.TotalHours, time.Minutes);
        }
    }
    public string DurationRange => BeginTime.ToString("HH:mm") + " - " + EndTime.ToString("HH:mm");
    public string FontIcon => Type switch
    {
        0 => "",
        1 => "\uE823",
        2 => "\uE706",
        3 => "\uE7FD",
        _ => throw new Exception(),
    };
}
