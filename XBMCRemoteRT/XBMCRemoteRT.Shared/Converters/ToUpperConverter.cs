using System;
using Windows.UI.Xaml.Data;

namespace XBMCRemoteRT.Converters
{
    class ToUpperConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string str = value as string;
            if (string.IsNullOrEmpty(str))
                return string.Empty;
            else return str.ToUpper();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
