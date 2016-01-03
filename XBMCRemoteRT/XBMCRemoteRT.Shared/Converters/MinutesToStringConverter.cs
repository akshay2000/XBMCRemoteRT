using System;
using Windows.UI.Xaml.Data;

namespace XBMCRemoteRT.Converters
{
    public class MinutesToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int seconds = (value == null) ? 0 : (int)value;
            int minutes = seconds / 60;
            string stringToReturn = "not available";
            if (minutes > 60)
            {
                int hours = minutes / 60;
                int remainingMinutes = minutes % 60;

                string hourString = (hours > 1) ? " hours" : " hour";
                string minuteString = (remainingMinutes > 1) ? " minutes" : " minute";

                stringToReturn = hours.ToString() + hourString;

                if (remainingMinutes > 0)
                    stringToReturn = stringToReturn + " " + remainingMinutes.ToString() + minuteString;
            }
            else if (minutes > 0)
            {
                stringToReturn = minutes.ToString() + " minutes";
            }
            return stringToReturn;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
