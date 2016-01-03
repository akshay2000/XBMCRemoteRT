using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Activation;
using Windows.Media.SpeechRecognition;
using XBMCRemoteRT.RPCWrappers;
using XBMCRemoteRT.Helpers;
using Windows.UI.Popups;
using System.Threading.Tasks;
using XBMCRemoteRT.Models.Audio;
using XBMCRemoteRT.Models.Video;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace XBMCRemoteRT.Pages.Entry
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class VoiceCommandsPage : Page
    {
        private enum SearchHitState { Single, Multiple, All, None }
        private enum SearchType { Album, Artist, Movie}

        private SearchHitState searchHitState;
        private SearchType searchType;
        private List<Artist> allArtists;
        private List<Album> allAlbums;
        private List<Movie> allMovies;

        public VoiceCommandsPage()
        {
            this.InitializeComponent();
            //HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            searchHitState = SearchHitState.None;
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
            GlobalVariables.CurrentTracker.SendView("VoiceCommandsPage");
            var commandArgs = e.Parameter as VoiceCommandActivatedEventArgs;
            SpeechRecognitionResult speechRecognitionResult = commandArgs.Result;
            ExecuteVoiceCommand(speechRecognitionResult);
            Frame.BackStack.Clear();
        }

        private async void ExecuteVoiceCommand(SpeechRecognitionResult result)
        {
            bool isConnected = await LoadAndConnnect();
            if (!isConnected)
                return;

            string voiceCommandName = result.RulePath[0];
            string textSpoken = result.Text;
            
            switch (voiceCommandName)
            {
                case "PlayArtist":
                    searchType = SearchType.Artist;
                    string artistName = SemanticInterpretation("musicTopic", result);
                    allArtists = await AudioLibrary.GetArtists();
                    var filteredArtists = allArtists.Where(t => t.Label.ToLower().Contains(artistName.ToLower())).ToList();
                    if (filteredArtists.Count > 1)
                    {
                        searchHitState = SearchHitState.Multiple;
                        ReceivedCommandTextBlock.Text = "We found multiple artists. Choose one...";
                        SearchedItemsListView.ItemsSource = filteredArtists;
                    }
                    else if (filteredArtists.Count > 0)
                    {
                        searchHitState = SearchHitState.Single;
                        ReceivedCommandTextBlock.Text = "This is the artist we found...";
                        SearchedItemsListView.ItemsSource = filteredArtists;
                        Player.PlayArtist(filteredArtists[0]);
                        QuestionNameTextBlock.Text = "Did we get the right one?";
                        QuestionWrapper.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    }
                    else
                    {
                        searchHitState = SearchHitState.None;
                        ReceivedCommandTextBlock.Text = "Sorry, we couldn't find what you asked for.";
                        //SearchedItemsListView.ItemsSource = allArtists;
                        QuestionNameTextBlock.Text = "Would you like to see a list of all artists?";
                        QuestionWrapper.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    }
                    break;
                case "PlayMovie":
                    searchType = SearchType.Movie;
                    string movieName = SemanticInterpretation("movieTopic", result);
                    allMovies = await VideoLibrary.GetMovies();
                    var filteredMovies = allMovies.Where(t => t.Title.ToLower().Contains(movieName.ToLower())).ToList();
                    if (filteredMovies.Count > 1)
                    {
                        searchHitState = SearchHitState.Multiple;
                        ReceivedCommandTextBlock.Text = "We found multiple movies. Choose one...";
                        SearchedItemsListView.ItemsSource = filteredMovies;
                    }
                    else if (filteredMovies.Count > 0)
                    {
                        searchHitState = SearchHitState.Single;
                        ReceivedCommandTextBlock.Text = "This is the movie we found...";
                        SearchedItemsListView.ItemsSource = filteredMovies;
                        Player.PlayMovie(filteredMovies[0]);
                        QuestionNameTextBlock.Text = "Did we find the right one?";
                        QuestionWrapper.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    }
                    else
                    {
                        searchHitState = SearchHitState.None;
                        ReceivedCommandTextBlock.Text = "Sorry, we couldn't find what you asked for. Here is the list of all movies.";
                        //SearchedItemsListView.ItemsSource = allMovies;
                        QuestionNameTextBlock.Text = "Would you like to see a list of all movies?";
                        QuestionWrapper.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    }
                    break;
                case "PlayAlbum":
                    searchType = SearchType.Album;
                    string albumName = SemanticInterpretation("musicTopic", result);
                    allAlbums = await AudioLibrary.GetAlbums();
                    var filteredAlbums = allAlbums.Where(t => t.Title.ToLower().Contains(albumName.ToLower())).ToList();
                    if (filteredAlbums.Count > 1)
                    {
                        searchHitState = SearchHitState.Multiple;
                        ReceivedCommandTextBlock.Text = "We found multiple albums. Choose one...";
                        SearchedItemsListView.ItemsSource = filteredAlbums;
                    }
                    else if (filteredAlbums.Count > 0)
                    {
                        searchHitState = SearchHitState.Single;
                        ReceivedCommandTextBlock.Text = "This is the album we found...";
                        SearchedItemsListView.ItemsSource = filteredAlbums;
                        Player.PlayAlbum(filteredAlbums[0]);
                        QuestionNameTextBlock.Text = "Did we get the right one?";
                        QuestionWrapper.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    }
                    else
                    {
                        searchHitState = SearchHitState.None;
                        ReceivedCommandTextBlock.Text = "Sorry, we couldn't find what you asked for. Here is the list of all albums.";
                        //SearchedItemsListView.ItemsSource = allAlbums;
                        QuestionNameTextBlock.Text = "Would you like to see a list of all albums?";
                        QuestionWrapper.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    }
                    break;
                case "StartParty":
                    await Player.PlayPartyMode();
                    ReceivedCommandTextBlock.Text = "Started party mode!";
                    await Task.Delay(1000);
                    Frame.Navigate(typeof(CoverPage));
                    break;
                default:
                    break;
            }
            if (searchHitState == SearchHitState.Single)
            {
                GlobalVariables.CurrentTracker.SendEvent(EventCategories.VoiceCommand, EventActions.VoiceCommand, "Single" + voiceCommandName, 0);
            }
            else if (searchHitState == SearchHitState.None)
            {
                GlobalVariables.CurrentTracker.SendEvent(EventCategories.VoiceCommand, EventActions.VoiceCommand, "Zero" + voiceCommandName, 0);
            }
        }

        private async Task<bool> LoadAndConnnect()
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
                        return true;
                    }                    
                }
            }
            string message = "Could not connect to a server. Please check the connection on next screen.";
            string messageHeader = "Connection Unsuccessful";
            MessageDialog msg = new MessageDialog(message, messageHeader);
            await msg.ShowAsync();
            Frame.Navigate(typeof(MainPage), false);
            return false;
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

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalVariables.CurrentTracker.SendEvent(EventCategories.VoiceCommand, EventActions.Click, "VoiceCommandYes", 0);
            switch (searchHitState)
            {
                case SearchHitState.Single:
                    Frame.Navigate(typeof(CoverPage));
                    break;
                case SearchHitState.None:
                    switch (searchType)
                    {
                        case SearchType.Album:
                            SearchedItemsListView.ItemsSource = allAlbums;
                            break;
                        case SearchType.Artist:
                            SearchedItemsListView.ItemsSource = allArtists;
                            break;
                        case SearchType.Movie:
                            SearchedItemsListView.ItemsSource = allMovies;
                            break;
                        default:
                            SearchedItemsListView.ItemsSource = null;
                            break;
                    }
                    searchHitState = SearchHitState.All;
                    QuestionWrapper.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    break;
                default:
                    break;
            }
        }

        private async void NoButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalVariables.CurrentTracker.SendEvent(EventCategories.VoiceCommand, EventActions.Click, "VoiceCommandNo", 0);
            if (searchHitState == SearchHitState.Single)
                await Player.Stop(GlobalVariables.CurrentPlayerState.PlayerType);

            Frame.Navigate(typeof(CoverPage));
        }

        private void SearchedItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            // For single results, play is started automatically and tapping
            // would restart the media. We only want to play media for All and
            // Multiple (and None) SearchHitStates.
            if (searchHitState != SearchHitState.Single)
            {
                var tappedItem = (sender as Grid).DataContext;
                if (tappedItem is Artist)
                {
                    Player.PlayArtist(tappedItem as Artist);
                }
                else if (tappedItem is Movie)
                {
                    Player.PlayMovie(tappedItem as Movie);
                }
                else if (tappedItem is Album)
                {
                    Player.PlayAlbum(tappedItem as Album);
                }
                else
                {
                    // Unrecognized media type. Early return so we don't navigate
                    // away from the page
                    return;
                }
            }

            Frame.Navigate(typeof(CoverPage));
        }
    }
}
