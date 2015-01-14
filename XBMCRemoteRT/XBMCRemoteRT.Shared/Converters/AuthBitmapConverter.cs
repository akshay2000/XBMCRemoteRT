using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;
using XBMCRemoteRT.Helpers;

namespace XBMCRemoteRT.Converters
{
    public class AuthBitmapConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string uriString = (string)value;
            BitmapImage image = new BitmapImage();
            image.DecodePixelType = DecodePixelType.Logical;
            image.DecodePixelWidth = 100;
            image.SetSourceAsync(uriString, "httpwatch", "myPass");
            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
