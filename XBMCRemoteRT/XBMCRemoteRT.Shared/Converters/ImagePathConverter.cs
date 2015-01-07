using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using XBMCRemoteRT.Helpers;

namespace XBMCRemoteRT.Converters
{
    public class ImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string imagePath = (value == null) ? string.Empty : (string)value;
            string imageURL = string.Empty;
            if (imagePath.Length > 8)
            {
                string uri = imagePath.Substring(8);
                //if (uri.StartsWith("http"))
                //{
                //    imageURL = WebUtility.UrlDecode(uri).TrimEnd('/');
                //}
                //else
                //{
                var encodedUri = WebUtility.UrlEncode(uri);
                string baseUrlString = "http://" + ConnectionManager.CurrentConnection.IpAddress + ":" + ConnectionManager.CurrentConnection.Port.ToString() + "/image/image://";
                imageURL = baseUrlString + encodedUri;
                //}
            }
            else
            {
                imageURL = "ms-appx:///Assets/DefaultArt.jpg";
            }
            Uri imageURI = new Uri(imageURL, UriKind.Absolute);
            return imageURI;
        }


        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}