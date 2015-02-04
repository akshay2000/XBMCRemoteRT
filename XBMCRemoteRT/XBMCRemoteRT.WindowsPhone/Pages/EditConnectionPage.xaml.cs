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
using Windows.UI.Popups;
using XBMCRemoteRT.Models;
using XBMCRemoteRT.Helpers;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace XBMCRemoteRT.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EditConnectionPage : Page
    {
        private NavigationHelper navigationHelper;
        private ConnectionItem currentConnection;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public EditConnectionPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
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
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            GlobalVariables.CurrentTracker.SendView("EditConnectionPage");
            currentConnection = e.Parameter as ConnectionItem;
            this.DataContext = currentConnection;
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void SaveConnectionAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            int port;
            MacAddress mac = null;
            IPAddress subnetMask = null;

            if (!int.TryParse(PortTextBox.Text, out port))
            {
                MessageDialog msg = new MessageDialog("Please enter a valid port number.", "Invalid Port");
                await msg.ShowAsync();
                return;
            }

            if (NameTextBox.Text.Equals(string.Empty) || IPTextBox.Text.Equals(string.Empty))
            {
                MessageDialog msg = new MessageDialog("Please enter valid name and server address", "Invalid Details");
                await msg.ShowAsync();
                return;
            }

            if (!MACAddressTextBox.Text.Equals(string.Empty) && !MacAddress.TryParse(MACAddressTextBox.Text, out mac))
            {
                MessageDialog msg = new MessageDialog("Please enter a valid MAC address in format 00:11:22:33:44:55.", "Invalid MAC address");
                await msg.ShowAsync();
                return;
            }

            if (!SubnetMaskTextBox.Text.Equals(string.Empty) && !IPAddress.TryParse(SubnetMaskTextBox.Text, out subnetMask))
            {
                MessageDialog msg = new MessageDialog("Please enter a valid subnet mask in format 255.255.255.255.", "Invalid subnet mask");
                await msg.ShowAsync();
                return;
            }

            currentConnection.ConnectionName = NameTextBox.Text;
            currentConnection.IpAddress = IPTextBox.Text;
            currentConnection.Port = port;
            currentConnection.Username = UsernameTextBox.Text;
            currentConnection.Password = PasswordTextBox.Text;
            currentConnection.MACAddress = mac;
            currentConnection.SubnetMask = subnetMask;

            App.ConnectionsVM.UpdateConnectionItem();
            Frame.GoBack();
        }

        private void CancelAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}
