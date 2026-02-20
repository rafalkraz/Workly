using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;

namespace Workly.Converters;

public class MinutesToDurationConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value == null)
            return "";

        var totalMinutes = (int)(long)value;

        var hours = totalMinutes / 60;
        var minutes = totalMinutes % 60;

        if (hours == 0)
            return $"{minutes} min";

        if (minutes == 0)
            return $"{hours}h";

        return $"{hours}h {minutes} min";
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}