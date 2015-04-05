using Newtonsoft.Json.Linq;
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
using XBMCRemoteRT.Models.Audio;
using XBMCRemoteRT.RPCWrappers;

namespace XBMCRemoteRT.Pages.Audio
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class AllMusicPage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private List<Artist> allArtists;
        private List<Album> allAlbums;
        private List<Song> allSongs;

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


        public AllMusicPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;

            FilterComboBox.SelectedIndex = 0;
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
            ReloadAll();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void ReloadAll()
        {
            ConnectionManager.ManageSystemTray(true);
            allArtists = await AudioLibrary.GetArtists();
            var groupedAllArtists = GroupingHelper.GroupList(allArtists, (Artist a) => a.Label, true);
            ArtistsCVS.Source = groupedAllArtists;
            (ArtistsSemanticZoom.ZoomedOutView as ListViewBase).ItemsSource = ArtistsCVS.View.CollectionGroups;

            JObject sortWith = new JObject(new JProperty("method", "label"));
            allAlbums = await AudioLibrary.GetAlbums(sort: sortWith);
            var groupedAllAlbums = GroupingHelper.GroupList(allAlbums, (Album a) => a.Label, true);
            AlbumsCVS.Source = groupedAllAlbums;
            (AlbumsSemanticZoom.ZoomedOutView as ListViewBase).ItemsSource = AlbumsCVS.View.CollectionGroups;

            allSongs = await AudioLibrary.GetSongs(sort: sortWith);
            var groupedAllSongs = GroupingHelper.GroupList(allSongs, (Song s) => s.Label, true);
            SongsCVS.Source = groupedAllSongs;
            (SongsSemanticZoom.ZoomedOutView as ListViewBase).ItemsSource = SongsCVS.View.CollectionGroups;

            ConnectionManager.ManageSystemTray(false);
        }

        private void FilterComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var choice = (sender as ComboBox).SelectedValue.ToString();

            switch (choice.ToLower())
            {
                case "artists" :
                    ArtistsCVSGrid.Visibility = Visibility.Visible;
                    AlbumsCVSGrid.Visibility = Visibility.Collapsed;
                    SongsCVSGrid.Visibility = Visibility.Collapsed;
                    break;
                case "albums":
                    AlbumsCVSGrid.Visibility = Visibility.Visible;
                    ArtistsCVSGrid.Visibility = Visibility.Collapsed;
                    SongsCVSGrid.Visibility = Visibility.Collapsed;
                    break;
                case "songs":
                    SongsCVSGrid.Visibility = Visibility.Visible;
                    ArtistsCVSGrid.Visibility = Visibility.Collapsed;
                    AlbumsCVSGrid.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void ArtistListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Artist tappedArtist = e.ClickedItem as Artist;
            GlobalVariables.CurrentArtist = tappedArtist;
            Frame.Navigate(typeof(ArtistDetailsPage));
        }

        private void AlbumListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Album tappedAlbum = e.ClickedItem as Album;
            GlobalVariables.CurrentAlbum = tappedAlbum;
            Frame.Navigate(typeof(AlbumPage));
        }

        private void SongListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var tappedSong = e.ClickedItem as Song;
            Player.PlaySong(tappedSong);
        }
    }
}
