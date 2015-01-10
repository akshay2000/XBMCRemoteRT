using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
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
                // TODO: Apply same logic to StringToImageBrushConverter
                if (ConnectionManager.CurrentConnection.Password != String.Empty)
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