using System;
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

        public static Object GetValue(string key, Object defaultValue)
        {
            if (AppSettings.Values[key] == null)
            {
                AppSettings.Values[key] = defaultValue;
            }
            return AppSettings.Values[key];
        }
    }
}
