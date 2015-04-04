using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;
using XBMCRemoteRT.RPCWrappers;

namespace XBMCRemoteRT.Converters
{
    public class PlayerTypeToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Players player = (Players)value;
            return player != Players.None;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
