using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace XBMCRemoteRT.Helpers
{
    public class SettingsHelper
    {
        //private static IsolatedStorageSettings AppSettings = IsolatedStorageSettings.ApplicationSettings;
        private static ApplicationDataContainer AppSettings = ApplicationData.Current.RoamingSettings;

        public static void SetValue(string key, Object value)
        {
            AppSettings.Values[key] = value;
        }

        public static Object GetValue(string key)
        {
            return AppSettings.Values[key];
        }
    }
}
