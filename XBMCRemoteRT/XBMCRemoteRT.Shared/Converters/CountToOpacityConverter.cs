using System;
using Windows.UI.Xaml.Data;

namespace XBMCRemoteRT.Converters
{
    public class CountToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            //if the list is empty return a faded opacity
            int count = (int)value;
            return count == 0 ? 0.6 : 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
