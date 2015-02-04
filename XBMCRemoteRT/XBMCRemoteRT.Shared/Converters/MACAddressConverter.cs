using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI.Xaml.Data;
using XBMCRemoteRT.Models;

namespace XBMCRemoteRT.Converters
{
    class MACAddressConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return null;

            MacAddress mac = (MacAddress)value;
            var hexStrs = mac.Bytes.Select((b) => { return b.ToString("X"); });
            return String.Join(":", hexStrs);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
