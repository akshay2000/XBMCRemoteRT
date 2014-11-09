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
using Windows.ApplicationModel.Activation;
using Windows.Media.SpeechRecognition;
using XBMCRemoteRT.RPCWrappers;
using Newtonsoft.Json.Linq;
using XBMCRemoteRT.Helpers;
using XBMCRemoteRT.Models;
using Windows.UI.Popups;
using System.Threading.Tasks;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace XBMCRemoteRT
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class VoiceCommandsPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public VoiceCommandsPage()
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
            var commandArgs = e.Parameter as VoiceCommandActivatedEventArgs;
            SpeechRecognitionResult speechRecognitionResult = commandArgs.Result;
            ExecuteVoiceCommand(speechRecognitionResult);
        }

        private async void ExecuteVoiceCommand(SpeechRecognitionResult result)
        {
            await LoadAndConnnect();
            string voiceCommandName = result.RulePath[0];
            string textSpoken = result.Text;

            switch (voiceCommandName)
            {
                case "PlayArtist":
                    string artistName = SemanticInterpretation("musicTopic", result);
                    var allArtists = await AudioLibrary.GetArtists();
                    var filteredArtists = allArtists.Where(t => t.Label.ToLower().Contains(artistName.ToLower())).ToList();
                    if (filteredArtists.Count > 1)
                    {
                        ReceivedCommandTextBlock.Text = "We found multiple artists. Choose one...";
                        SearchedItemsListView.ItemsSource = filteredArtists;
                    }
                    else if (filteredArtists.Count > 0)
                    {
                        ReceivedCommandTextBlock.Text = "This is the artist we found...";
                        SearchedItemsListView.ItemsSource = filteredArtists;
                        Player.PlayArtist(filteredArtists[0]);
                    }
                    else
                    {
                        ReceivedCommandTextBlock.Text = "Sorry, we couldn't find what you asked for. Here is the list of all artists.";
                        SearchedItemsListView.ItemsSource = allArtists;
                    }
                    break;
                case "PlayMovie":
                    string movieName = SemanticInterpretation("movieTopic", result);
                    var allMovies = await VideoLibrary.GetMovies();
                    var filteredMovies = allMovies.Where(t => t.Title.ToLower().Contains(movieName.ToLower())).ToList();
                    if (filteredMovies.Count > 1)
                    {
                        ReceivedCommandTextBlock.Text = "We found multiple movies. Choose one...";
                        SearchedItemsListView.ItemsSource = filteredMovies;
                    }
                    else if (filteredMovies.Count > 0)
                    {
                        ReceivedCommandTextBlock.Text = "This is the movie we found...";
                        SearchedItemsListView.ItemsSource = filteredMovies;
                        Player.PlayMovie(filteredMovies[0]);
                    }
                    else
                    {
                        ReceivedCommandTextBlock.Text = "Sorry, we couldn't find what you asked for. Here is the list of all movies.";
                        SearchedItemsListView.ItemsSource = allMovies;
                    }
                    break;
                case "PlayAlbum":
                    string albumName = SemanticInterpretation("musicTopic", result);
                    var allAlbums = await AudioLibrary.GetAlbums();
                    var filteredAlbums = allAlbums.Where(t => t.Title.ToLower().Contains(albumName.ToLower())).ToList();
                    if (filteredAlbums.Count > 1)
                    {
                        ReceivedCommandTextBlock.Text = "We found multiple albums. Choose one...";
                        SearchedItemsListView.ItemsSource = filteredAlbums;
                    }
                    else if (filteredAlbums.Count > 0)
                    {
                        ReceivedCommandTextBlock.Text = "This is the album we found...";
                        SearchedItemsListView.ItemsSource = filteredAlbums;
                        Player.PlayAlbum(filteredAlbums[0]);
                    }
                    else
                    {
                        ReceivedCommandTextBlock.Text = "Sorry, we couldn't find what you asked for. Here is the list of all albums.";
                        SearchedItemsListView.ItemsSource = allAlbums;
                    }
                    break;
                default:
                    break;
            }
        }

        private async Task LoadAndConnnect()
        {
            await App.ConnectionsVM.ReloadConnections();
            string ip = (string)SettingsHelper.GetValue("RecentServerIP");
            if (ip != null)
            {
                var connectionItem = App.ConnectionsVM.ConnectionItems.FirstOrDefault(item => item.IpAddress == ip);
                if (connectionItem != null)
                {
                    if (await JSONRPC.Ping(connectionItem))
                    {
                        ConnectionManager.CurrentConnection = connectionItem;
                        return;
                    }                    
                }
            }
            MessageDialog msg = new MessageDialog("Could not connect to a server. Please check the connection.", "Connection Unsuccessful");
            await msg.ShowAsync();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        private string SemanticInterpretation(string key, Windows.Media.SpeechRecognition.SpeechRecognitionResult speechRecognitionResult)
        {
            if (speechRecognitionResult.SemanticInterpretation.Properties.ContainsKey(key))
            {
                return speechRecognitionResult.SemanticInterpretation.Properties[key][0];
            }
            else
            {
                return "unknown";
            }
        }

        #endregion
    }
}
