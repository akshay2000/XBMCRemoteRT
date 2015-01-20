using System;
using Windows.UI.Xaml.Data;
using XBMCRemoteRT.Helpers;

namespace XBMCRemoteRT.Converters
{
    public class ImagePathConverter : IValueConverter
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
            else
            {
                imageURI = new Uri("ms-appx:///Assets/DefaultArt.jpg", UriKind.Absolute);
            }
            return imageURI;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}