using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using XBMCRemoteRT.Common;
using XBMCRemoteRT.Helpers;
using XBMCRemoteRT.Models.Audio;
using XBMCRemoteRT.RPCWrappers;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace XBMCRemoteRT.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NowPlaying : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public NowPlaying()
        {
            InitializeComponent();
            defaultViewModel["SongsInPlaylist"] = new ObservableCollection<Song>();
            defaultViewModel["TotalPlaytimeHours"] = "0";
            defaultViewModel["TotalPlaytimeMinutes"] = "0";
            defaultViewModel["Tracks"] = "0";

            DataContext = defaultViewModel;

            PlaylistStats.Visibility = Visibility.Collapsed;

            IdToColourConverter.DataContext = GlobalVariables.CurrentPlayerState;
            IdToThicknessConverter.DataContext = GlobalVariables.CurrentPlayerState;

            navigationHelper = new NavigationHelper(this);
            navigationHelper.LoadState += this.NavigationHelper_LoadState;
            navigationHelper.SaveState += this.NavigationHelper_SaveState;
            navigationHelper.GoBackCommand = new RelayCommand(() =>
            {
                navigationHelper.GoBack();
            });

            defaultViewModel["NavigationHelper"] = NavigationHelper;

            Loaded += NowPlaying_OnLoaded;

            /***************************************************************
             * THIS IS AWFULL. THIS IS THE EVIL WITHIN XAML. A needed evil.
             * -------------------------------------------------------------
             * Microsoft won't add multibinding to WinRT, so programmers
             * get creative an pissed off, doing stuff like this.
             * 
             * All this little piece of code does is just refreshing the
             * collection bound to the list of songs.
             ***************************************************************/
            GlobalVariables.CurrentPlayerState.PropertyChanged += async (sender, args) =>
            {
                if (args.PropertyName.Equals("ItemId"))
                {
                    await RefreshSongs();
                }
            };
        }


        private async void NowPlaying_OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            await RefreshSongs();
            RefreshMetadata();

            PlaylistStats.Visibility = Visibility.Visible;
        }

        private async Task RefreshSongs()
        {
            var rpcRequestFallback = true;

            if (DefaultViewModel.ContainsKey("SongsInPlaylist"))
            {
                var songs = ((ObservableCollection<Song>)defaultViewModel["SongsInPlaylist"]);
                if (songs.Any())
                {
                    defaultViewModel["SongsInPlaylist"] = null;
                    defaultViewModel["SongsInPlaylist"] = songs;
                    rpcRequestFallback = false;
                }
            }

            if (rpcRequestFallback)
            {
                ConnectionManager.ManageSystemTray(true);

                try
                {
                    var tmpList = (List<Song>)await Playlist.GetItems(PlayelistType.Audio);
                    if (tmpList.Any())
                        defaultViewModel["SongsInPlaylist"] = new ObservableCollection<Song>(tmpList);
                }
                catch (Exception) { }

                ConnectionManager.ManageSystemTray(false);
            }
        }

        private void RefreshMetadata()
        {
            if (!DefaultViewModel.ContainsKey("SongsInPlaylist"))
                return;

            var songs = ((ObservableCollection<Song>)defaultViewModel["SongsInPlaylist"]);
            if (!songs.Any())
                return;

            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            string track = loader.GetString("Track");
            string tracks = loader.GetString("Tracks");

            long totalPlaytimeSec = songs.Aggregate(0, (x, y) => x + y.Duration);

            TrackCountTextBlock.Text = songs.Count().ToString();
            TracksTextBlock.Text = songs.Count() > 1 ? tracks : track;

            defaultViewModel["Tracks"] = songs.Count.ToString();
            defaultViewModel["TotalPlaytimeHours"] = Math.Floor(totalPlaytimeSec / 3600.0).ToString("F0");
            defaultViewModel["TotalPlaytimeMinutes"] = ((totalPlaytimeSec / 60.0) % 60).ToString("F0").PadLeft(2, '0');

            if (defaultViewModel["TotalPlaytimeHours"].Equals("0"))
            {
                HoursEditTextBlock.Visibility = Visibility.Collapsed;
                HoursLabelTextBlock.Visibility = Visibility.Collapsed;

                if (defaultViewModel["TotalPlaytimeMinutes"].Equals("00"))
                {
                    TotalPlaytime.Visibility = Visibility.Collapsed;
                    TotalPlaytime.Visibility = Visibility.Collapsed;
                }
            }

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
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void RemoveSongMFI_Click(object sender, RoutedEventArgs e)
        {
            var stackPanel = sender as MenuFlyoutItem;
            if (stackPanel == null)
                return;

            var songs = ((ObservableCollection<Song>)defaultViewModel["SongsInPlaylist"]);

            var tappedSong = stackPanel.DataContext as Song;
            await Playlist.Remove(PlayelistType.Audio, songs.IndexOf(tappedSong));

            songs.Remove(tappedSong);

            RefreshMetadata();
        }

        private async void SongItemWrapper_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var stackPanel = sender as StackPanel;
            if (stackPanel == null)
                return;

            var songs = ((ObservableCollection<Song>)defaultViewModel["SongsInPlaylist"]);

            var tappedSong = stackPanel.DataContext as Song;
            await Player.GoTo(Players.Audio, songs.IndexOf(tappedSong));
        }

        private void SongItemWrapper_OnRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }
    }
}
