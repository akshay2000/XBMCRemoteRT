using System;
using System.Collections.Generic;
using System.Text;
using Windows.ApplicationModel.Resources;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using XBMCRemoteRT.RPCWrappers;

namespace XBMCRemoteRT.Converters
{
    public class IdToColourConverter : FrameworkElement, IValueConverter
    {
        #region CurrentItemId (DependencyProperty)
        public int CurrentItemId
        {
            get { return GetValue(CurrentItemIdProperty) as int? ?? -1; }
            set { SetValue(CurrentItemIdProperty, value); }
        }
        public static readonly DependencyProperty CurrentItemIdProperty = DependencyProperty.Register("CurrentItemId", typeof(int), typeof(IdToColourConverter), new PropertyMetadata("", OnCurrentItemIdChanged));

        private static void OnCurrentItemIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((IdToColourConverter)d).OnCurrentItemIdChanged(e);
        }

        protected virtual void OnCurrentItemIdChanged(DependencyPropertyChangedEventArgs e)
        {
        }
        #endregion

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var currentAccentColorHex = new SolidColorBrush();
            try
            {
                currentAccentColorHex = (SolidColorBrush) Application.Current.Resources["PhoneAccentBrush"];
            }
            catch (Exception) { }

            var normalColorHex = new SolidColorBrush(Colors.Transparent);

            return ((int) value) == CurrentItemId ? currentAccentColorHex : normalColorHex;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
