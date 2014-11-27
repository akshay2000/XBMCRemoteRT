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
            string imagePath = (string)value;
            string imageURL = string.Empty;
            if (imagePath == null)
                imagePath = String.Empty;
            if (imagePath.Length > 8)
            {
                string uri = imagePath.Substring(8);
                if (uri.StartsWith("http"))
                {
                    imageURL = WebUtility.UrlDecode(uri).TrimEnd('/');
                }
                else
                {
                    var encodedUri = WebUtility.UrlEncode(uri);
                    string baseUrlString = "http://" + ConnectionManager.CurrentConnection.IpAddress + ":" + ConnectionManager.CurrentConnection.Port.ToString() + "/image/image://";
                    imageURL = baseUrlString + encodedUri;
                }
            }
            else
            {
                imageURL = "";
            }

            ImageBrush imageBrush = new ImageBrush();
            imageBrush.Stretch = Stretch.UniformToFill;
            imageBrush.Opacity = 0.6;
            if(!string.IsNullOrEmpty(imageURL))
                imageBrush.ImageSource = new BitmapImage(new Uri(imageURL, UriKind.RelativeOrAbsolute));
            return imageBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
