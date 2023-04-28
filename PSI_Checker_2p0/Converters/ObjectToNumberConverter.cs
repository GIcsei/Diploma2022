using System;
using System.Globalization;

namespace PSI_Checker_2p0
{
    public class ObjectToNumberConverter : BaseValueConverter<ObjectToNumberConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double doubleValue)
            {
                return doubleValue;
            }
            return String.Empty;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Double.Parse(value as string);
        }
    }
}
