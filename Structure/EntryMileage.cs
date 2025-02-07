using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Windows.UI.StartScreen;

namespace WorkLog.Structure;

public class EntryMileage(int ID, int type, DateOnly date, string beginPoint, string endPoint, string description, int distance, double parkingPrice)
{
    // Types
    // 0 - standard
    // 1 - parking

    public int ID { get; set; } = ID;
    public int Type { get; set; } = type;
    public DateOnly Date { get; set; } = date;
    public string BeginPoint { get; set; } = beginPoint;
    public string EndPoint { get; set; } = endPoint;
    public string Description { get; set; } = description;
    public int Distance { get; set; } = distance;
    public double ParkingPrice { get; set; } = parkingPrice;
    public string PointsRange => Type == 0 ? BeginPoint + " ➔ " + EndPoint : BeginPoint;
    public string FontIcon => Type switch
    {
        0 => "",
        1 => "\uE707",
        _ => throw new Exception(),
    };
}
