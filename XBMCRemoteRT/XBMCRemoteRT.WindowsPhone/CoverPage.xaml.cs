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
using XBMCRemoteRT.Models.Audio;
using XBMCRemoteRT.Models.Video;
using XBMCRemoteRT.RPCWrappers;
using XBMCRemoteRT.Models.Common;
using XBMCRemoteRT.Pages.Audio;
using XBMCRemoteRT.Pages.Video;
using XBMCRemoteRT.Pages;
using XBMCRemoteRT.Helpers;
using XBMCRemoteRT.Models;
using GoogleAnalytics.Core;
using GoogleAnalytics;
using Windows.UI.Xaml.Media.Animation;
using Newtonsoft.Json.Linq;
using XBMCRemoteRT.Pages.Files;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace XBMCRemoteRT
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CoverPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        //private DispatcherTimer timer;
        

        public CoverPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Required;

            if (GlobalVariables.CurrentPlayerState == null)
                GlobalVariables.CurrentPlayerState = new PlayerState();
            DataContext = GlobalVariables.CurrentPlayerState;
            PlayerHelper.RefreshPlayerState();
            PlayerHelper.StartAutoRefresh(1);            
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
            this.navigationHelper.OnNavigatedTo(e);
            GlobalVariables.CurrentTracker.SendView("CoverPage");
            RefreshListsIfNull();
            ServerNameTextBlock.Text = ConnectionManager.CurrentConnection.ConnectionName;
            Frame.BackStack.Clear();
            TileHelper.UpdateAllTiles();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private List<Album> Albums;
        private List<Episode> Episodes;
        private List<Movie> Movies;

        private void AlbumWrapper_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var tappedAlbum = (sender as Grid).DataContext as Album;
            GlobalVariables.CurrentAlbum = tappedAlbum;
            CommonNavigationTransitionInfo infoOverride = new CommonNavigationTransitionInfo();
            Frame.Navigate(typeof(AlbumPage), null, infoOverride);
        }
        
        private void EpisodeWrapper_Tapped(object sender, TappedRoutedEventArgs e)
        {
            GlobalVariables.CurrentTracker.SendEvent(EventCategories.UIInteraction, EventActions.Click, "CoverPageEpisodeWrapper", 0);
            var tappedEpisode = (sender as Grid).DataContext as Episode;
            Player.PlayEpidose(tappedEpisode);
        }

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
                Albums = await AudioLibrary.GetRecentlyAddedAlbums(new Limits { Start = 0, End = 12 });
                MusicHubSection.DataContext = Albums;
            }

            if (Episodes == null)
            {
                Episodes = await VideoLibrary.GetRecentlyAddedEpisodes(new Limits { Start = 0, End = 10 });
                TVHubSection.DataContext = Episodes;
            }

            if (Movies == null)
            {
                Movies = await VideoLibrary.GetRecentlyAddedMovies(new Limits { Start = 0, End = 12 });
                MoviesHubSection.DataContext = Movies;
            }
        }

        private void RemoteAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalVariables.CurrentTracker.SendEvent(EventCategories.UIInteraction, EventActions.Click, "RemoteAppBarButton", 0);
            Frame.Navigate(typeof(InputPage));
        }

        private void MusicHeaderWrapper_Tapped(object sender, TappedRoutedEventArgs e)
        {
            bool isAutoRedirectEnabled = (string)SettingsHelper.GetValue("AudioAutoRedirect", "Unset") == "Yes";
            if (isAutoRedirectEnabled)
            {
                Frame.Navigate(typeof(SearchMusicPivot));
            }
            else
            {
                Frame.Navigate(typeof(AllMusicPivot));
            }
        }

        private void TVShowsHeaderWrapper_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(AllTVShowsPage));
        }

        private void MoviesHeaderWrapper_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(AllMoviesPivot));
        }

        private void NowPlayingHeaderWrapper_Tapped(object sender, TappedRoutedEventArgs e)
        {
            GlobalVariables.CurrentTracker.SendEvent(EventCategories.UIInteraction, EventActions.Click, "NowPlayingHeader", 0);
            Frame.Navigate(typeof(InputPage));
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

        private void AboutAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AboutPivot));
        }

        private void SettingsAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingsPivot));
        }

        private void SwitchServerAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationTransitionInfo transitionInfo = new SlideNavigationTransitionInfo();
            Frame.Navigate(typeof(MainPage), false, transitionInfo);
        }

        Slider slider;
        private void ProgressSlider_Loaded(object sender, RoutedEventArgs e)
        {
            slider = sender as Slider;
            slider.AddHandler(UIElement.PointerReleasedEvent, new PointerEventHandler(slider_PointerReleased), true);
        }

        void slider_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            var percentage = (slider.Value * 100) / slider.Maximum;
            Player.Seek(GlobalVariables.CurrentPlayerState.PlayerType, percentage);
        }

        private void FilesAppBarButton_OnClickAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationTransitionInfo transitionInfo = new SlideNavigationTransitionInfo();
            Frame.Navigate(typeof(AllSourcesPage), false, transitionInfo);
        }

        private void BottomBar_Opened(object sender, object e)
        {
            (sender as CommandBar).Opacity = 0.8;
        }

        private void BottomBar_Closed(object sender, object e)
        {
            (sender as CommandBar).Opacity = 0.5;
        }
    }
}
