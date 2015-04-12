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
using XBMCRemoteRT.Models.Video;
using XBMCRemoteRT.Helpers;
using XBMCRemoteRT.RPCWrappers;
using XBMCRemoteRT.Models.Common;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace XBMCRemoteRT.Pages.Video
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AllMoviesPivot : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private List<Movie> allMovies;
        private List<Movie> unwatchedMovies;
        private List<Movie> watchedMovies;

        public AllMoviesPivot()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;

            LoadMovies();
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
            GlobalVariables.CurrentTracker.SendView("AllMoviesPage");
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void MovieWrapper_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Movie tappedMovie = (sender as Grid).DataContext as Movie;
            GlobalVariables.CurrentMovie = tappedMovie;
            Frame.Navigate(typeof(MovieDetailsHub));
        }

        private void LoadMovies()
        {
            var loadStartTime = DateTime.Now;

            ConnectionManager.ManageSystemTray(true);
            AllMoviesListView.ItemsSource = new MoviesCollection();

            Filter filter = new Filter { Field = "playcount", Operator = "is", value = "0" };
            UnwatchedMoviesListView.ItemsSource = new MoviesCollection(filter: filter);

            filter.Operator = "greaterthan";
            WatchedMoviesListView.ItemsSource = new MoviesCollection(filter: filter);
            ConnectionManager.ManageSystemTray(false);

            GlobalVariables.CurrentTracker.SendTiming(DateTime.Now.Subtract(loadStartTime), TimingCategories.LoadTime, "AllMovies", "AllMovies");
        }

        private void SeachMoviesAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SearchMoviesPage));
        }

        private void RefreshMoviesAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            LoadMovies();
        }

    }
}
