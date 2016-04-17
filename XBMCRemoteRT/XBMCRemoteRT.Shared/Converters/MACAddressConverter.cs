using System;
using System.Linq;
using Windows.UI.Xaml.Data;
using XBMCRemoteRT.Models.Network;

namespace XBMCRemoteRT.Converters
{
    class MacAddressConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return null;

            MacAddress mac = (MacAddress)value;
            var hexStrs = mac.Bytes.Select((b) => { return b.ToString("X2"); });
            return String.Join(":", hexStrs);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
