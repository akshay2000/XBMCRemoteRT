using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Data;
using XBMCRemoteRT.Models.Video;

namespace XBMCRemoteRT.Converters
{
    public class ListTrimmer : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int resultCount = 6;
            List<Cast> castList = (value == null) ? new List<Cast>() : (List<Cast>)value;
            if (castList.Count > resultCount)
            {
                int countToRemove = castList.Count - resultCount;
                castList.RemoveRange(resultCount, countToRemove);
            }
            return castList;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
