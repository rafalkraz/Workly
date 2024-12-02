using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkLog.Structure;

public class Month(string number)
{
    public string Number { get; } = number;

    public override string ToString()
    {
        if (int.TryParse(Number, out var number))
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(number);
        }
        else { return "Parse error"; }
    }
}
