using System;
using Windows.UI.Xaml.Data;

namespace XBMCRemoteRT.Converters
{
    public class SecondsToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int seconds = System.Convert.ToInt32(value);
            if (seconds > 3600)
            {
                return seconds / 3600 + "h " + (seconds % 3600) / 60 + "m";
            }
            else
            {
                return seconds / 60 + "m " + seconds % 60 + "s";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
