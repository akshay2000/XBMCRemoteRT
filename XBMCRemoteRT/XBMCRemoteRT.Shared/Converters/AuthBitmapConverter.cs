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
            string uriString = (value == null) ? string.Empty : (string)value;

            string converterParam = (string)parameter;

            BitmapImage image = new BitmapImage();
            image.DecodePixelType = DecodePixelType.Logical;

            if (converterParam != null)
            {
                if (converterParam.StartsWith("w"))
                {
                    image.DecodePixelWidth = Int32.Parse(converterParam.Remove(0, 1));
                }
                else if (converterParam.StartsWith("h"))
                {
                    image.DecodePixelHeight = Int32.Parse(converterParam.Remove(0, 1));
                }
            }

            string proxyScheme = "image://";
            if (uriString.StartsWith(proxyScheme))
            {
                image.SetProxySourceAsync(uriString);
            }
            else
            {
                Uri defaultUri = new Uri("ms-appx:///Assets/DefaultArt.jpg", UriKind.Absolute);
                image.UriSource = defaultUri;
            }

            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
