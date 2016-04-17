using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace XBMCRemoteRT.Converters
{
    class MimeTypeToGlyphConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string mimetype = (value == null) ? string.Empty : (string)value;

            string glyph = null;

            string[] mimevalues = mimetype.Split('/');
            if (mimevalues.Length > 0)
                switch (mimevalues[0])
                {
                    case "video":
                        glyph = "\uE116";
                        break;
                    case "audio":
                        glyph = "\uE189;";
                        break;
                    default:
                        break;
                }

            return glyph;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
