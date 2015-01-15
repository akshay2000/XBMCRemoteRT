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
            string converterParam = (string)parameter;

            BitmapImage image = new BitmapImage();
            image.DecodePixelType = DecodePixelType.Logical;

            if (converterParam.StartsWith("w"))
            {
                image.DecodePixelWidth = Int32.Parse(converterParam.Remove(0, 1));
            }
            else if (converterParam.StartsWith("h"))
            {
                image.DecodePixelHeight = Int32.Parse(converterParam.Remove(0, 1));
            }
            //Uri uri = new Uri(uriString);
            
            //image.SetSourceAsync(uriString, "httpwatch", "myPass");

            image.SetCustomSourceAsync(uriString);
            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
