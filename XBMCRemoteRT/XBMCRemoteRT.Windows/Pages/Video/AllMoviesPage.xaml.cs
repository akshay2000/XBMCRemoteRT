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
    public sealed partial class AllMoviesPage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private List<Movie> allMovies;
        private List<Movie> unwatchedMovies;
        private List<Movie> watchedMovies;

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


        public AllMoviesPage()
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
            LoadMovies();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void MovieWrapper_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Movie tappedMovie = (sender as Grid).DataContext as Movie;
            GlobalVariables.CurrentMovie = tappedMovie;
            Frame.Navigate(typeof(MovieDetailsHub));
        }

        private async void LoadMovies()
        {
            ProgressRing.IsActive = true;
            ConnectionManager.ManageSystemTray(true);

            allMovies = await VideoLibrary.GetMovies();
            unwatchedMovies = allMovies.Where(movie => movie.PlayCount == 0).ToList<Movie>();
            watchedMovies = allMovies.Where(movie => movie.PlayCount > 0).ToList<Movie>();

            var groupedAllMovies = GroupingHelper.GroupList(allMovies, (Movie a) => a.Label, true);
            AllCVS.Source = groupedAllMovies;
            (AllSemanticZoom.ZoomedOutView as ListViewBase).ItemsSource = AllCVS.View.CollectionGroups;

            var groupedUnwatchedMovies = GroupingHelper.GroupList(unwatchedMovies, (Movie a) => a.Label, true);
            NewCVS.Source = groupedUnwatchedMovies;
            (NewSemanticZoom.ZoomedOutView as ListViewBase).ItemsSource = NewCVS.View.CollectionGroups;

            var groupedWatchedMovies = GroupingHelper.GroupList(watchedMovies, (Movie a) => a.Label, true);
            WatchedCVS.Source = groupedWatchedMovies;
            (WatchedSemanticZoom.ZoomedOutView as ListViewBase).ItemsSource = WatchedCVS.View.CollectionGroups;

            ConnectionManager.ManageSystemTray(false);
            ProgressRing.IsActive = false;
        }

        private void FilterComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var choice = (sender as ComboBox).SelectedValue.ToString();

            switch (choice)
            {
                case "All":
                    AllCVSGrid.Visibility = Visibility.Visible;
                    NewCVSGrid.Visibility = Visibility.Collapsed;
                    WatchedCVSGrid.Visibility = Visibility.Collapsed;
                    break;
                case "New":
                    NewCVSGrid.Visibility = Visibility.Visible;
                    AllCVSGrid.Visibility = Visibility.Collapsed;
                    WatchedCVSGrid.Visibility = Visibility.Collapsed;
                    break;
                case "Watched":
                    WatchedCVSGrid.Visibility = Visibility.Visible;
                    AllCVSGrid.Visibility = Visibility.Collapsed;
                    NewCVSGrid.Visibility = Visibility.Collapsed;
                    break;
            }
        }
    }
}
