using System;
using Workly.Interfaces;

namespace Workly.Structure;

public class Entry(int ID, int type, DateTime beginTime, DateTime endTime, string localization, string description, double earning) : IEntry
{
    // Types
    // 0 - Standard
    // 1 - Overtime
    // 2 - Leave
    // 3 - UnpaidLeave

    public int ID { get; set; } = ID;

    private int _type = type;
    public int Type
    {
        get
        {
            if (DurationRaw > 1440)
                return -1;

            return _type;
        }

        set => _type = value;
    }
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
            if (time.TotalMinutes > 1440)
            {
                return "Błąd";
            }
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
        }
    }

    public string DurationRange => BeginTime.ToString("HH:mm") + " - " + EndTime.ToString("HH:mm");

    public long DurationRaw => (long)(EndTime - BeginTime).TotalMinutes;

    public string FontIcon => Type switch
    {
        0 => "",
        1 => "\uE823",
        2 => "\uE706",
        3 => "\uE7FD",
        _ => "\uE783",
    };
}
