using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workly.Structure;

public class Month(string number) : IEquatable<Month>, IComparable<Month>
{
    public string Number { get; } = number;

    public int CompareTo(Month other)
    {
        if (int.Parse(this.Number) > int.Parse(other.Number)) return 1;
        else if (int.Parse(this.Number) == int.Parse(other.Number)) return 0;
        else return -1;
    }

    public bool Equals(Month other)
    {
        if (this.Number == other.Number) return true;
        return false;
    }

    public override int GetHashCode()
    {
        return int.Parse(Number);
    }

    public override string ToString()
    {
        if (int.TryParse(Number, out var number))
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(number);
        }
        else { return "Parse error"; }
    }
}
