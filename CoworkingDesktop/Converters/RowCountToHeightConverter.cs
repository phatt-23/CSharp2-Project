using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CoworkingDesktop.Converters
{
    public class RowCountToHeightConverter : IValueConverter
    {
        public double RowHeight { get; set; } = 30;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is int count) 
                ? (Math.Min(count, 10) * RowHeight) 
                : 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}
