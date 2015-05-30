using XBMCRemoteRT.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json.Linq;
using XBMCRemoteRT.Helpers;
using XBMCRemoteRT.Models.Audio;
using XBMCRemoteRT.Models.Files;
using XBMCRemoteRT.Models.Video;
using XBMCRemoteRT.RPCWrappers;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace XBMCRemoteRT.Pages.Files
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SourceFilesPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private List<File> allFiles;
        private ObservableCollection<File> previousDirectories;

        public SourceFilesPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            HardwareButtons.BackPressed += OnBackPressed;

            DataContext = GlobalVariables.CurrentSource;
            previousDirectories = new ObservableCollection<File>();
        }

        private void OnBackPressed(object sender, BackPressedEventArgs e)
        {
            if (previousDirectories.Count > 1)
            {
                LoadDirectory(new File { Label = "..." });
                e.Handled = true;
            }
            else
                GlobalVariables.CurrentFile = null;
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
            GlobalVariables.CurrentTracker.SendView("SourceFilesPage");
            this.navigationHelper.OnNavigatedTo(e);
            init();

        }
        private void init()
        {
            ReloadAll();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
        private void ReloadAll()
        {
            if (GlobalVariables.CurrentFile != null)
                LoadDirectory(GlobalVariables.CurrentFile);
            else
                LoadSource(GlobalVariables.CurrentSource);
        }

        private async void LoadSource(Source source)
        {
            ConnectionManager.ManageSystemTray(true);
            previousDirectories.Add(new File{ Path = source.Path, Label = source.Label });
            allFiles = await RPCWrappers.Files.GetDirectory(source.Path);
            FilesListView.ItemsSource = allFiles;
            ConnectionManager.ManageSystemTray(false);
        }

        private async void LoadDirectory(File file)
        {
            ConnectionManager.ManageSystemTray(true);
            if (file.Label == "...")
            {
                previousDirectories.Remove(previousDirectories.Last());
                file = previousDirectories.Last();
                GlobalVariables.CurrentFile = file;
            }
            else
            {
                previousDirectories.Add(file);
            }
            allFiles = await RPCWrappers.Files.GetDirectory(file.Path);
            if (previousDirectories.Count > 1)
                allFiles.Insert(0, new File() { Label = "...", Path = "...", FileType = "directory" });
            FilesListView.ItemsSource = allFiles;
            ConnectionManager.ManageSystemTray(false);
        }

        private async void FileItemWrapper_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var file = (File)((FrameworkElement)sender).DataContext;
            if (file.FileType == "directory")
            {
                LoadDirectory(file);
            }
            else
            {
                var fileDetails = await RPCWrappers.Files.GetFileDetails(file.Path, GlobalVariables.CurrentSource.Media);

                switch (fileDetails.Type)
                {
                    case "music":
                        var song = new Song { SongId = fileDetails.Id };
                        await Player.PlaySong(song);
                        break;
                    case "movie":
                        var movie = new Movie { MovieId = fileDetails.Id };
                        Player.PlayMovie(movie);
                        break;
                    case "episode":
                        var episode = new Episode { EpisodeId = fileDetails.Id };
                        Player.PlayEpidose(episode);
                        break;
                }
            }
        }
    }
}
