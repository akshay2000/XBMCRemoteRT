using XBMCRemoteRT.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using XBMCRemoteRT.Models;
using System.Threading.Tasks;
using XBMCRemoteRT.RPCWrappers;
using XBMCRemoteRT.Helpers;
using Windows.UI.Popups;
using System.Diagnostics;
using XBMCRemoteRT.Pages;
using GoogleAnalytics;
using GoogleAnalytics.Core;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace XBMCRemoteRT
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private enum PageStates { Ready, Connecting }

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public MainPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            this.NavigationCacheMode = NavigationCacheMode.Required;
            this.TempImage.DataContext = new { ImageUri = "http://www.httpwatch.com/httpgallery/authentication/authenticatedimage/default.aspx?0.10196295308417402" };
            DataContext = App.ConnectionsVM;
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);

            GlobalVariables.CurrentTracker.SendView("MainPage");

            await App.ConnectionsVM.ReloadConnections();

            bool isAutoConnectEnabled = (bool)SettingsHelper.GetValue("AutoConnect", true);

            SetPageState(PageStates.Ready);
            Frame.BackStack.Clear();
            bool tryAutoLoad = true;
            if (e.Parameter.ToString() != string.Empty)
                tryAutoLoad = (bool)e.Parameter;

            if (e.NavigationMode != NavigationMode.Back && isAutoConnectEnabled && tryAutoLoad)
            {
                ConnnectRecent();
            }

        }

        private void ConnnectRecent()
        {
            //await App.ConnectionsVM.ReloadConnections();
            //DataContext = App.ConnectionsVM;
            string ip = (string)SettingsHelper.GetValue("RecentServerIP");
            if (ip != null)
            {
                var connectionItem = App.ConnectionsVM.ConnectionItems.FirstOrDefault(item => item.IpAddress == ip);
                if (connectionItem != null)
                    ConnectToServer(connectionItem);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void ConnectionItemWrapper_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ConnectionItem selectedConnection = (ConnectionItem)(sender as StackPanel).DataContext;
            ConnectToServer(selectedConnection);
        }

        private void AddConnectionAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddConnectionPage));
        }

        private void AboutAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AboutPivot));
        }

        private async Task ConnectToServer(ConnectionItem connectionItem)
        {
            SetPageState(PageStates.Connecting);

            bool isSuccessful = await JSONRPC.Ping(connectionItem);
            if (isSuccessful)
            {
                ConnectionManager.CurrentConnection = connectionItem;
                SettingsHelper.SetValue("RecentServerIP", connectionItem.IpAddress);
                Frame.Navigate(typeof(CoverPage));
            }
            else
            {
                //GlobalVariables.CurrentTracker.SendException("Ping failed", false);
                MessageDialog message = new MessageDialog("Could not reach the server.", "Connection Unsuccessful");
                await message.ShowAsync();
                SetPageState(PageStates.Ready);
            }            
        }
        private void SetPageState(PageStates pageState)
        {
            if (pageState == PageStates.Connecting)
            {
                ConnectionsListView.IsEnabled = false;
                BottomAppBar.Visibility = Visibility.Collapsed;
                ProgressRing.IsActive = true;
            }
            else
            {
                ConnectionsListView.IsEnabled = true;
                BottomAppBar.Visibility = Visibility.Visible;
                ProgressRing.IsActive = false;
            }
        }

        private void DeleteConnectionMFI_Click(object sender, RoutedEventArgs e)
        {
            ConnectionItem selectedConnection = (ConnectionItem)(sender as MenuFlyoutItem).DataContext;
            App.ConnectionsVM.RemoveConnectionItem(selectedConnection);
        }

        private void EditConnectionMFI_Click(object sender, RoutedEventArgs e)
        {
            ConnectionItem selectedConnection = (ConnectionItem)(sender as MenuFlyoutItem).DataContext;
            Frame.Navigate(typeof(EditConnectionPage), selectedConnection);
        }

        private void ConnectionItemWrapper_Holding(object sender, HoldingRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }      
    }
}
