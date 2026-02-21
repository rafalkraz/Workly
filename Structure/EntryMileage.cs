using System;
using Workly.Interfaces;

namespace Workly.Structure;

public class EntryMileage(int ID, int type, DateOnly date, string beginPoint, string endPoint, string description, int distance, double parkingPrice) : IEntry
{
    // Types
    // 0 - Standard
    // 1 - Parking

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
        _ => "\uE783",
    };
}
