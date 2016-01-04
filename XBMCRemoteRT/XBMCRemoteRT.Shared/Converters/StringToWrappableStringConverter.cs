using System;
using System.Text;
using Windows.UI.Xaml.Data;

namespace XBMCRemoteRT.Converters
{
    // Appends a zero-width-space after dots to allow textboxes wrapping the text at dots
    class StringToWrappableStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                string origdata = (string)value;
                return origdata.Replace(".", ".\u200B");
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
