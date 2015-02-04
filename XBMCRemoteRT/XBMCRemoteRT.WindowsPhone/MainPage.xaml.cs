using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using XBMCRemoteRT.Common;
using XBMCRemoteRT.Helpers;
using XBMCRemoteRT.Models.Network;
using XBMCRemoteRT.Pages;
using XBMCRemoteRT.RPCWrappers;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace XBMCRemoteRT
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private enum PageStates { Ready, Busy }

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public MainPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            this.NavigationCacheMode = NavigationCacheMode.Required;
            //this.TempImage.DataContext = new { ImageUri = "http://10.0.0.2:8080/image/image://http%253a%252f%252fthetvdb.com%252fbanners%252fposters%252f121361-27.jpg" };
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
            SetPageState(PageStates.Busy, "Connecting...");

            bool isSuccessful = await JSONRPC.Ping(connectionItem);
            if (isSuccessful)
            {
                ConnectionManager.CurrentConnection = connectionItem;
                SettingsHelper.SetValue("RecentServerIP", connectionItem.IpAddress);
                Frame.Navigate(typeof(CoverPage));
            }
            else
            {
                if (connectionItem.AutoWake)
                {
                    if (connectionItem.SubnetMask == null || connectionItem.MACAddress == null)
                    {
                        MessageDialog message = new MessageDialog("Please specify MAC address and subnet mask to use this feature.", "More information needed");
                        await message.ShowAsync();
                        return;
                    }

                    int wakeUpTime = connectionItem.WakeUpTime == 0 ? 5 : connectionItem.WakeUpTime;
                    SetPageState(PageStates.Busy, string.Format("Trying to wake up Kodi server. This usually takes {0} seconds.", wakeUpTime));
                    uint result = await WOLHelper.WakeUp(connectionItem);
                    
                    if (result == 102)
                    {
                        var startTime = DateTime.Now;
                        await Task.Delay(new TimeSpan(0, 0, wakeUpTime));
                        MessageDialog tryMessage = new MessageDialog("Seems like Kodi is taking more time to wake up. What would you like to do?", "Server still not up");
                        tryMessage.Commands.Add(new UICommand("keep trying"));
                        tryMessage.Commands.Add(new UICommand("stop"));
                        int noOfTries = 0;
                        SetPageState(PageStates.Busy, "Trying to connect...");
                        while (!isSuccessful)
                        {
                            isSuccessful = await JSONRPC.Ping(connectionItem);
                            noOfTries++;
                            if (noOfTries % 2 == 0)
                            {
                               var selectedCommand = await tryMessage.ShowAsync();
                               if (selectedCommand.Label == "stop")
                               {
                                   break;
                               }
                            }
                        }
                        if (isSuccessful)
                        {
                            connectionItem.WakeUpTime = (int)(DateTime.Now - startTime).TotalSeconds;
                            ConnectionManager.CurrentConnection = connectionItem;
                            SettingsHelper.SetValue("RecentServerIP", connectionItem.IpAddress);
                            App.ConnectionsVM.UpdateConnectionItem();
                            Frame.Navigate(typeof(CoverPage));
                            return;
                        }
                    }
                }
                MessageDialog cantReachMessage = new MessageDialog("Could not reach the server.", "Connection Unsuccessful");
                await cantReachMessage.ShowAsync();
                SetPageState(PageStates.Ready);
            }            
        }
        private void SetPageState(PageStates pageState, string busyMessage = "Connecting...")
        {
            if (pageState == PageStates.Busy)
            {
                PageStateTextBlock.Text = busyMessage;
                PageStateGrid.Visibility = Windows.UI.Xaml.Visibility.Visible;
                BottomAppBar.Visibility = Visibility.Collapsed;
            }         
            else
            {
                PageStateGrid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                BottomAppBar.Visibility = Visibility.Visible;
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

        private async void WakeUpServerMFI_Click(object sender, RoutedEventArgs e)
        {
            ConnectionItem selectedConnection = (ConnectionItem)(sender as MenuFlyoutItem).DataContext;
            if (selectedConnection.SubnetMask == null || selectedConnection.MACAddress == null)
            {
                MessageDialog message = new MessageDialog("Please specify MAC address and subnet mask to use this feature.", "More information needed");
                await message.ShowAsync();
                return;
            }
            SetPageState(PageStates.Busy, "Waking up...");
            uint result = await WOLHelper.WakeUp(selectedConnection);
            await Task.Delay(3500);
            SetPageState(PageStates.Ready);
            if (result != 102)
            {
                string messageText;
                switch (result)
                {
                    case 10:
                        messageText = "Please specify an IP address rather than a hostname to use the wake feature.";
                        break;
                    default:
                        messageText = "Could not send wake request: broadcast IP not available. Errorcode " + result;
                        break;
                }
                MessageDialog message = new MessageDialog(messageText, "Wake up failed");
                await message.ShowAsync();
            }
        }

        private void ConnectionItemWrapper_Holding(object sender, HoldingRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }      
    }
}
