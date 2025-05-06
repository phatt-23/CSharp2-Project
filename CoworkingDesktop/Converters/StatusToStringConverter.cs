using CoworkingDesktop.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CoworkingDesktop.Converters
{
    public class StatusToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = (WorkspaceStatusType)value;

            return status switch
            {
                WorkspaceStatusType.Available => "Available",
                WorkspaceStatusType.Occupied => "Occupied",
                WorkspaceStatusType.Maintenance => "Maintenance",
                _ => throw new UnreachableException(nameof(value))
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
