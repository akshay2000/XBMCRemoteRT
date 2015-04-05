using XBMCRemoteRT.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Hub Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=321224
using XBMCRemoteRT.Helpers;
using XBMCRemoteRT.Models;
using XBMCRemoteRT.Models.Audio;
using XBMCRemoteRT.Models.Common;
using XBMCRemoteRT.Models.Video;
using XBMCRemoteRT.Pages;
using XBMCRemoteRT.Pages.Audio;
using XBMCRemoteRT.Pages.Video;
using XBMCRemoteRT.RPCWrappers;

namespace XBMCRemoteRT
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class CoverPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private DispatcherTimer timer;
        
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

        public CoverPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;

            if (GlobalVariables.CurrentPlayerState == null)
                GlobalVariables.CurrentPlayerState = new PlayerState();
            DataContext = GlobalVariables.CurrentPlayerState;

            PlayerHelper.RefreshPlayerState();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(10);
            timer.Start();
            timer.Tick += timer_Tick;
        }

        void timer_Tick(object sender, object o)
        {
            PlayerHelper.RefreshPlayerState();
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
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // TODO: Assign a collection of bindable groups to this.DefaultViewModel["Groups"]
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
            RefreshListsIfNull();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private List<Album> Albums;
        private List<Episode> Episodes;
        private List<Movie> Movies;

        
        private void MovieWrapper_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var tappedMovie = (sender as Grid).DataContext as Movie;
            GlobalVariables.CurrentMovie = tappedMovie;
            Frame.Navigate(typeof(MovieDetailsHub));
        }


        private async void RefreshListsIfNull()
        {
            if (Albums == null)
            {
                Albums = await AudioLibrary.GetRecentlyAddedAlbums(new Limits { Start = 0, End = 8 });
                MusicHubSection.DataContext = Albums;
            }

            if (Episodes == null)
            {
                Episodes = await VideoLibrary.GetRecentlyAddedEpisodes(new Limits { Start = 0, End = 8 });
                TVHubSection.DataContext = Episodes;
            }

            if (Movies == null)
            {
                Movies = await VideoLibrary.GetRecentlyAddedMovies(new Limits { Start = 0, End = 8 });
                MoviesHubSection.DataContext = Movies;
            }
        }

        private void RemoteAppBarButton_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(InputPage));
        }

        private void MusicHeaderWrapper_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(AllMusicPage));
        }

        private void TVShowsHeaderWrapper_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(AllTVShowsPage));
        }

        private void MoviesHeaderWrapper_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(AllMoviesPage));
        }

        private async void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            await Player.GoTo(GlobalVariables.CurrentPlayerState.PlayerType, GoTo.Previous);
            await PlayerHelper.RefreshPlayerState();
        }

        private async void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            await Player.PlayPause(GlobalVariables.CurrentPlayerState.PlayerType);
            await PlayerHelper.RefreshPlayerState();
        }

        private async void NextButton_Click(object sender, RoutedEventArgs e)
        {
            await Player.GoTo(GlobalVariables.CurrentPlayerState.PlayerType, GoTo.Next);
            await PlayerHelper.RefreshPlayerState();
        }

        private void ConnectionsAppBarButton_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage), true);
        }

        private void AlbumGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var tappedAlbum = e.ClickedItem as Album;
            GlobalVariables.CurrentAlbum = tappedAlbum;
            Frame.Navigate(typeof(AlbumPage));
        }

        private void EpisodeGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var tappedEpisode = e.ClickedItem as Episode;
            Player.PlayEpidose(tappedEpisode);
        }
    }
}
