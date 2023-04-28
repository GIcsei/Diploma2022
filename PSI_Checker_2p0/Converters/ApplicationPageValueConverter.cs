using PSI_Checker_2p0.Enums;
using PSI_Checker_2p0.Pages;
using System;
using System.Globalization;

namespace PSI_Checker_2p0
{
    public class ApplicationPageValueConverter : BaseValueConverter<ApplicationPageValueConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((ApplicationPage)value)
            {
                case ApplicationPage.Init:
                    return new InitPage();
                case ApplicationPage.Checker:
                    return new CheckerPage();
                default:
                    return null;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
