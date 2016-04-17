using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace XBMCRemoteRT.Converters
{
    class BooleanToVisibilityConverter2 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((bool?)value == true) ? Visibility.Visible : Visibility.Collapsed;

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
