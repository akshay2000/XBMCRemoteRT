using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace XBMCRemoteRT.UserControls
{
    public sealed partial class ColumnGridView : UserControl
    {
        public static readonly DependencyProperty ItemTemplateProperty =
                    DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(ColumnGridView), new PropertyMetadata(null));

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(object), typeof(ColumnGridView), new PropertyMetadata(null));

        public object ItemsSource
        {
            get { return (object)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register("Columns", typeof(int), typeof(ColumnGridView), new PropertyMetadata(1));

        public int Columns
        {
            get { return (int)GetValue(ColumnsProperty); }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException("Columns must be greater than 0");
                SetValue(ColumnsProperty, value);
            }
        }

        public ColumnGridView()
        {
            this.InitializeComponent();
        }

        private void ItemsWrapGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ItemsWrapGrid itemsWrapGrid = sender as ItemsWrapGrid;
            if (itemsWrapGrid != null)
            {
                itemsWrapGrid.ItemWidth = e.NewSize.Width / Columns;
            }
        }
    }
}
