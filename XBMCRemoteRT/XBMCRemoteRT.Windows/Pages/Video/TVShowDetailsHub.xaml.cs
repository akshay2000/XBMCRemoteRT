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
using XBMCRemoteRT.Models.Video;
using XBMCRemoteRT.RPCWrappers;

namespace XBMCRemoteRT.Pages.Video
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class TVShowDetailsHub : Page
    {

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


        public TVShowDetailsHub()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;

            DataContext = GlobalVariables.CurrentTVShow;

            LoadEpisodes();
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
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void EpisodeWrapper_Tapped(object sender, TappedRoutedEventArgs e)
        {
            GlobalVariables.CurrentTracker.SendEvent(EventCategories.UIInteraction, EventActions.Click, "TVShowDetailsHubEpisodeWrapper", 0);
            var tappedEpisode = (sender as StackPanel).DataContext as Episode;
            Player.PlayEpidose(tappedEpisode);
        }

        private async void LoadEpisodes()
        {
            JObject filter = new JObject(new JProperty("tvshowid", GlobalVariables.CurrentTVShow.TvShowId));
            List<Episode> episodes = await VideoLibrary.GetEpisodes(tvShowID: GlobalVariables.CurrentTVShow.TvShowId);

            List<SeasonItem<Episode>> seasons = GroupEpisodes<Episode>(episodes, epi => epi.Season);
            SeasonsCVS.Source = seasons;
        }

        private List<SeasonItem<T>> GroupEpisodes<T>(IEnumerable<T> items, Func<T, int> getKeyFunc)
        {
            IEnumerable<SeasonItem<T>> group = from item in items
                                               group item by getKeyFunc(item) into g
                                               orderby g.Key
                                               select new SeasonItem<T>(g.Key, g);
            return group.ToList();
        }

        private class SeasonItem<T> : List<T>
        {
            public SeasonItem(int key, IEnumerable<T> items)
                : base(items)
            {
                this.Key = key;
            }
            public int Key { get; private set; }
        }
    }
}
