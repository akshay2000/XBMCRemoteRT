using System;
using Windows.UI.Xaml.Data;

namespace XBMCRemoteRT.Converters
{
    public class SpeedToSymbolIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int speed = value == null ? 1 : (int)value;
            return speed == 1 ? "Pause" : "Play";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
