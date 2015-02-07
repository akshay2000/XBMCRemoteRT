using System;
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
            BitmapImage image = new BitmapImage();
            string proxyScheme = "image://";
            if (imagePath.StartsWith(proxyScheme))
            {
                image.SetProxySourceAsync(imagePath);
            }

            ImageBrush imageBrush = new ImageBrush();
            imageBrush.Stretch = Stretch.UniformToFill;
            imageBrush.Opacity = 0.6;
            imageBrush.ImageSource = image;
            return imageBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
