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
    public class IdToThicknessConverter : FrameworkElement, IValueConverter
    {
        #region CurrentItemId (DependencyProperty)
        public int CurrentItemId
        {
            get { return GetValue(CurrentItemIdProperty) as int? ?? -1; }
            set { SetValue(CurrentItemIdProperty, value); }
        }
        public static readonly DependencyProperty CurrentItemIdProperty = DependencyProperty.Register("CurrentItemId", typeof(int), typeof(IdToThicknessConverter), new PropertyMetadata("", OnCurrentItemIdChanged));

        private static void OnCurrentItemIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((IdToThicknessConverter)d).OnCurrentItemIdChanged(e);
        }

        protected virtual void OnCurrentItemIdChanged(DependencyPropertyChangedEventArgs e)
        {
        }
        #endregion

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double dParam = 0.0;
            double.TryParse((string)parameter, out dParam);
            return ((int) value) == CurrentItemId ? new Thickness(0) : new Thickness(dParam);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
