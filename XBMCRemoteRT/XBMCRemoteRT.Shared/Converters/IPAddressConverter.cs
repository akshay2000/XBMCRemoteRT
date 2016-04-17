using System;
using Windows.UI.Xaml.Data;
using XBMCRemoteRT.Models.Network;

namespace XBMCRemoteRT.Converters
{
    class IPAddressConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return null;

            IPAddress ip = (IPAddress)value;
            return ip.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
