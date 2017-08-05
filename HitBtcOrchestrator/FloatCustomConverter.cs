using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HitBtcOrchestrator
{
    public class FloatCustomConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToString(value, new CultureInfo("en-US"));
           // return string.Format(new CultureInfo("en-US"), "{0}", value);

            //  return string.Format(new CultureInfo("en-US"),"{0:N}", value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return System.Convert.ToDouble(value, new CultureInfo("en-US"));

            }
            catch (Exception e)
            {
                return 0f;
            }

        }
    }
}