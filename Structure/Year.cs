using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace WorkLog.Structure
{
    public class Year(string name, List<Month> months)
    {
        public string Name { get; } = name;
        public List<Month> Months { get; set; } = months;

        public override string ToString()
        {
            return Name;
        }
    }
}
