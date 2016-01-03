using System;
using Windows.UI.Xaml.Data;

namespace XBMCRemoteRT.Converters
{
    public class DurationToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string result = "-";

            if (value != null)
            {
                int totalSeconds = (int)value;
                int minutes = (int)Math.Floor(totalSeconds / 60d);
                int seconds = totalSeconds % 60;
                result = minutes.ToString() + ":" + (seconds < 10 ? "0" : "") + seconds.ToString();
            }
            
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
