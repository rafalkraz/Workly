using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkLog.Structure
{
    public class Month(string name, List<Entry> entries)
    {
        public string Name { get; } = name;
        public List<Entry> Entries { get; } = entries;

        public override string ToString()
        {
            
            if (int.TryParse(Name, out int number))
            {
                return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(number);
            }
            else { return "err"; }
            
            
        }
    }
}
