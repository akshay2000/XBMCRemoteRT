using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using XBMCRemoteRT.Helpers;

namespace XBMCRemoteRT.Converters
{
    public class StringToImageBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string imagePath = (value == null) ? string.Empty : (string)value;
            Uri imageURI = null;
            string proxyScheme = "image://";
            if (imagePath.StartsWith(proxyScheme))
            {
                // Only apply cache logic if authentication is in use. If not,
                // allow the image to be consumed from Kodi.
                if (ConnectionManager.CurrentConnection.HasCredentials())
                {
                    // Get path to locally cached image
                    imageURI = CacheManager.GetCacheUri(imagePath);
                }
                else
                {
                    // Get Kodi proxy image address
                    imageURI = CacheManager.GetRemoteUri(imagePath);
                }
            }

            ImageBrush imageBrush = new ImageBrush();
            imageBrush.Stretch = Stretch.UniformToFill;
            imageBrush.Opacity = 0.6;
            if(imageURI != null)
                imageBrush.ImageSource = new BitmapImage(imageURI);
            return imageBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
