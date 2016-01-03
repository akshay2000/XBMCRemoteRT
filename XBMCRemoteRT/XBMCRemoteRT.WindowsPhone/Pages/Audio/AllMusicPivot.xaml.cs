using XBMCRemoteRT.Common;
using System;
using System.Collections.Generic;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using XBMCRemoteRT.Models.Audio;
using XBMCRemoteRT.Helpers;
using XBMCRemoteRT.RPCWrappers;
using Windows.UI.Popups;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace XBMCRemoteRT.Pages.Audio
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AllMusicPivot : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private List<Artist> allArtists;
        private List<Album> allAlbums;
        private List<Song> allSongs;

        public AllMusicPivot()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;            
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
            GlobalVariables.CurrentTracker.SendView("AllMusicPage");
            this.navigationHelper.OnNavigatedTo(e);
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.LandscapeFlipped | DisplayOrientations.Landscape | DisplayOrientations.Portrait;  
            init();
            
        }

        private async void init()
        {
            bool isLargeLibrary = await AudioLibrary.IsLarge();
            if (isLargeLibrary)
            {
                string redirectionStatus = (string)SettingsHelper.GetValue("AudioAutoRedirect", "Unset");
                if (redirectionStatus == "Unset")
                {
                    var loader = new Windows.ApplicationModel.Resources.ResourceLoader();

                    string messageHeader = loader.GetString("LargeLibrary"); //"Large library";
                    string messageContent = loader.GetString("LargeMusicMessage"); //"There seems to be a lot of music here. Would you like us to help you search for the music instead?";
                    string yes = loader.GetString("MessageOptionYes"); // "yes";
                    string no = loader.GetString("MessageOptionNo"); //"no";
                    MessageDialog dialog = new MessageDialog(messageContent, messageHeader);
                    dialog.Commands.Add(new UICommand(yes));
                    dialog.Commands.Add(new UICommand(no));
                    var result = await dialog.ShowAsync();
                    if (result.Label == yes)
                    {
                        SettingsHelper.SetValue("AudioAutoRedirect", "Yes");
                        Frame.Navigate(typeof(SearchMusicPivot));
                        return;
                    }
                    else
                    {
                        SettingsHelper.SetValue("AudioAutoRedirect", "No");
                    }
                }
            }
            if (allAlbums == null || allSongs == null || allArtists == null)
            {
                ReloadAll();
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
        }

        #endregion

        private async void ReloadAll()
        {
            var loadSartTime = DateTime.Now;

            ConnectionManager.ManageSystemTray(true);
            allArtists = await AudioLibrary.GetArtists();
            var groupedAllArtists = GroupingHelper.GroupList(allArtists, (Artist a) => { return a.Label; }, true);
            ArtistsCVS.Source = groupedAllArtists;

            allAlbums = await AudioLibrary.GetAlbums();
            var groupedAllAlbums = GroupingHelper.GroupList(allAlbums, (Album a) => { return a.Label; }, true);
            AlbumsCVS.Source = groupedAllAlbums;

            allSongs = await AudioLibrary.GetAllSongs();
            var groupedAllSongs = GroupingHelper.GroupList(allSongs, (Song s) => { return s.Label; }, true);
            SongsCVS.Source = groupedAllSongs;
            ConnectionManager.ManageSystemTray(false);

            GlobalVariables.CurrentTracker.SendTiming((DateTime.Now.Subtract(loadSartTime)), TimingCategories.LoadTime, "AllMusic", "AllMusic");
        }

        private void AlbumArtWrapper_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Album tappedAlbum = (sender as Grid).DataContext as Album;
            GlobalVariables.CurrentAlbum = tappedAlbum;
            Frame.Navigate(typeof(AlbumPage));
        }

        private async void SongItemWrapper_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var tappedSong = (sender as StackPanel).DataContext as Song;
            await Player.PlaySong(tappedSong);
        }

        private void QueueSongMFI_Click(object sender, RoutedEventArgs e)
        {
            Playlist.AddSong((Song)(sender as MenuFlyoutItem).DataContext);
        }

        private void SongItemWrapper_Holding(object sender, HoldingRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }

        private void ArtistItemWrapper_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Artist tappedArtist = (sender as StackPanel).DataContext as Artist;
            GlobalVariables.CurrentArtist = tappedArtist;
            Frame.Navigate(typeof(ArtistDetailsHub));
        }

        private void RefreshMusicAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            ReloadAll();
        }

        private async void PartyModeAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            await Player.PlayPartyMode();
        }

        private void SearchMusicAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SearchMusicPivot));
        }
    }
}
