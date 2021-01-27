﻿using XBMCRemoteRT.Common;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using XBMCRemoteRT.Helpers;
using XBMCRemoteRT.Models.Network;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace XBMCRemoteRT.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddConnectionPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public AddConnectionPage()
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
            GlobalVariables.CurrentTracker.SendView("AddConnectionPage");
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

            if (!int.TryParse(PortTextBox.Text, out port))
            {
                MessageDialog msg = new MessageDialog("Please enter a valid port number.", "Invalid Port");
                await msg.ShowAsync();
                return;
            }

            String ipAddress = IPTextBox.Text;

            if (NameTextBox.Text.Equals(string.Empty) || ipAddress.Equals(string.Empty))
            {
                MessageDialog msg = new MessageDialog("Please enter valid name and server address", "Invalid Details");
                await msg.ShowAsync();
                return;
            }

            // ********************************************************************************************* //
            // Michele Mischitelli (11/07/2015) :
            // We should check for ip addresses starting with http:// in case people use dynamic DNS to always 
            // be able to connect to their kodi at home. Https:// is not supported for now.
            const String httpsPrefix = "https://";
            if (ipAddress.ToLower().StartsWith(httpsPrefix))
            {
                var md = new MessageDialog("Https protocol is not currently supported.", "Invalid protocol");
                await md.ShowAsync();
                return;
            }

            const String httpPrefix = "http://";
            if (ipAddress.ToLower().StartsWith(httpPrefix))
                ipAddress = ipAddress.Substring(httpPrefix.Length);
            // ********************************************************************************************* //

            var newConnection = new ConnectionItem
            {
                ConnectionName = NameTextBox.Text,
                IpAddress = ipAddress,
                Port = port,
                Username = UsernameTextBox.Text,
                Password = PasswordTextBox.Text
            };
            await App.ConnectionsVM.AddConnectionItem(newConnection);
            Frame.GoBack();
        }

        private void CancelAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}
