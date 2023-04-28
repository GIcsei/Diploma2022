using System;
using System.Collections;
using System.Globalization;

namespace PSI_Checker_2p0
{
    public class ListToIsEnabledConverter : BaseValueConverter<ListToIsEnabledConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ICollection list && list.Count > 0)
            {
                return true;
            }
            return false;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
