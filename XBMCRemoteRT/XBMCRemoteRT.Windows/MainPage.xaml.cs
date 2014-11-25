﻿using System.Threading.Tasks;
using Windows.UI.Popups;
using XBMCRemoteRT.Common;
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

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237
using XBMCRemoteRT.Helpers;
using XBMCRemoteRT.Models;
using XBMCRemoteRT.Pages;
using XBMCRemoteRT.RPCWrappers;

namespace XBMCRemoteRT
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private enum PageStates { Ready, Connecting }

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public MainPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
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
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
            bool showConnections = e.Parameter as bool? ?? false;
            LoadAndConnnect(showConnections);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion


        private async void LoadAndConnnect(bool showConnections)
        {
            await App.ConnectionsVM.ReloadConnections();
            DataContext = App.ConnectionsVM;
            if (!showConnections)
            {
                var ip = (string)SettingsHelper.GetValue("RecentServerIP");
                if (ip != null)
                {
                    var connectionItem = App.ConnectionsVM.ConnectionItems.FirstOrDefault(item => item.IpAddress == ip);
                    if (connectionItem != null)
                        await ConnectToServer(connectionItem);
                }
            }
        }

        private async Task ConnectToServer(ConnectionItem connectionItem)
        {
            SetPageState(PageStates.Connecting);
            var isSuccessful = await JSONRPC.Ping(connectionItem);
            if (isSuccessful)
            {
                ConnectionManager.CurrentConnection = connectionItem;
                SettingsHelper.SetValue("RecentServerIP", connectionItem.IpAddress);
                Frame.Navigate(typeof(CoverPage));
            }
            else
            {
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

        private void ConnectionItemWrapper_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ConnectionItem selectedConnection = (ConnectionItem)(sender as StackPanel).DataContext;
            ConnectToServer(selectedConnection);
        }

        private void AddConnectionAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddConnectionPage));
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


        private void AboutAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void FeedbackAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ConnectionItemWrapper_OnRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }
    }
}
