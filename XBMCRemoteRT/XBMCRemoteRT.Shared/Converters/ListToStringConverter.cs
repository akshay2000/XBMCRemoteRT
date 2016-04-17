using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Data;

namespace XBMCRemoteRT.Converters
{
    public class ListToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            List<string> inputList = (List<String>)value;
            bool showNotAvailable = System.Convert.ToBoolean(parameter);
            string toReturn = showNotAvailable ? "not available" : String.Empty;
            if (inputList == null)
                inputList = new List<string>();
            if (inputList.Count > 0 && inputList[0]!="")
            {
                toReturn = string.Empty;
                int index = 0;
                foreach (string s in inputList)
                {
                    index++;
                    if (index > 5)
                    {
                        toReturn = toReturn + "etc.,";
                        break;
                    }
                    toReturn = toReturn + s + ", ";
                }
                char[] trim = { ',', ' ' };
                toReturn = toReturn.TrimEnd(trim);
            }
            return toReturn;
            //throw new NotImplementedException();
        }

        private string getNotAvailableString()
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            return loader.GetString("NotAvailable");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
