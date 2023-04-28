using System;
using System.Globalization;

namespace PSI_Checker_2p0
{
    public class ObjectToStringConverter : BaseValueConverter<ObjectToStringConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                return stringValue;
            }
            return String.Empty;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value as string;
        }
    }
}
