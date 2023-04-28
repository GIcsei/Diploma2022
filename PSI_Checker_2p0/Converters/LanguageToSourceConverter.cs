using System;
using System.Globalization;

namespace PSI_Checker_2p0
{
    public class LanguageToSourceConverter : BaseValueConverter<LanguageToSourceConverter>
    {
        private const string FlagPath = "C:\\Users\\ICG1BP\\source\\repos\\psi-checker-2.0\\PSI_Checker_2p0\\Images\\Flags\\";
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string language)
            {
                return FlagPath + language + ".png";
            }
            return null;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
