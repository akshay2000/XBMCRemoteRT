using Windows.UI.Xaml.Media.Animation;
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
using XBMCRemoteRT.Helpers;
using XBMCRemoteRT.RPCWrappers;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace XBMCRemoteRT.Pages
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class InputPage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private bool isVolumeSetProgrammatically;
        int[] Speeds = { -32, -16, -8, -4, -2, -1, 1, 2, 4, 8, 16, 32 };

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


        public InputPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;

            DataContext = GlobalVariables.CurrentPlayerState;
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

        #region Remote Keys
        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            Input.ExecuteAction(InputCommands.Left);
        }

        private void UpButton_Click(object sender, RoutedEventArgs e)
        {
            Input.ExecuteAction(InputCommands.Up);
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            Input.ExecuteAction(InputCommands.Right);
        }

        private void DownButton_Click(object sender, RoutedEventArgs e)
        {
            Input.ExecuteAction(InputCommands.Down);
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            Input.ExecuteAction(InputCommands.Home);
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            Input.ExecuteAction(InputCommands.ContextMenu);
        }

        private void OSDButton_Click(object sender, RoutedEventArgs e)
        {
            Input.ExecuteAction(InputCommands.ShowOSD);
        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            Input.ExecuteAction(InputCommands.Info);
        }

        private void EnterButton_Click(object sender, RoutedEventArgs e)
        {
            Input.ExecuteAction(InputCommands.Select);
        }

        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            Input.ExecuteAction(InputCommands.Back);
        }

        #endregion

        private async void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            await Player.GoTo(GlobalVariables.CurrentPlayerState.PlayerType, GoTo.Previous);
            await PlayerHelper.RefreshPlayerState();
        }

        private async void SpeedDownButton_Click(object sender, RoutedEventArgs e)
        {
            int speed = GlobalVariables.CurrentPlayerState.Speed;

            if (speed != 0 && speed != -32)
            {
                int index = Array.IndexOf(Speeds, speed);
                int newSpeed = Speeds[index - 1];
                await Player.SetSpeed(GlobalVariables.CurrentPlayerState.PlayerType, newSpeed);
                await PlayerHelper.RefreshPlayerState();
            }
        }

        private async void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            await Player.PlayPause(GlobalVariables.CurrentPlayerState.PlayerType);
            await PlayerHelper.RefreshPlayerState();
        }

        private async void StopButton_Click(object sender, RoutedEventArgs e)
        {
            await Player.Stop(GlobalVariables.CurrentPlayerState.PlayerType);
            await PlayerHelper.RefreshPlayerState();
        }

        private async void SpeedUpButton_Click(object sender, RoutedEventArgs e)
        {
            int speed = GlobalVariables.CurrentPlayerState.Speed;

            if (speed != 0 && speed != 32)
            {
                int index = Array.IndexOf(Speeds, speed);
                int newSpeed = Speeds[index + 1];
                await Player.SetSpeed(GlobalVariables.CurrentPlayerState.PlayerType, newSpeed);
                await PlayerHelper.RefreshPlayerState();
            }
        }

        private async void NextButton_Click(object sender, RoutedEventArgs e)
        {
            await Player.GoTo(GlobalVariables.CurrentPlayerState.PlayerType, GoTo.Next);
            await PlayerHelper.RefreshPlayerState();
        }

        private async void VolumeSlider_Loaded(object sender, RoutedEventArgs e)
        {
            int volume = await Applikation.GetVolume();
            SetVolumeSliderValue(volume);
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Applikation.Quit();
        }

        private DispatcherTimer timer;
        private void VolumeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (isVolumeSetProgrammatically)
            {
                isVolumeSetProgrammatically = false;
                return;
            }
            else
            {
                if (timer != null)
                    timer.Stop();

                timer = new DispatcherTimer();

                timer.Interval = new TimeSpan(0, 0, 1);
                timer.Tick += timer_Tick;

                timer.Start();
            }
        }

        void timer_Tick(object sender, object e)
        {
            int value = (int)Math.Round(VolumeSlider.Value);
            Applikation.SetVolume(value);

            timer.Stop();
            timer.Tick -= timer_Tick;
        }

        private async void VolumeDownWrapper_Tapped(object sender, TappedRoutedEventArgs e)
        {
            int currentVolume = await Applikation.GetVolume();
            Applikation.SetVolume(--currentVolume);
            SetVolumeSliderValue(currentVolume);
        }

        private async void VolumeUpWrapper_Tapped(object sender, TappedRoutedEventArgs e)
        {
            int currentVolume = await Applikation.GetVolume();
            Applikation.SetVolume(++currentVolume);
            SetVolumeSliderValue(currentVolume);
        }

        private void SetVolumeSliderValue(int value)
        {
            isVolumeSetProgrammatically = true;
            VolumeSlider.Value = value;
        }

        private void SendTextBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                Input.SendText(SendTextBox.Text, true);
                SendTextBox.Text = string.Empty;
                LoseFocus(sender);
            }
        }

        private void LoseFocus(object sender)
        {
            var control = sender as Control;
            var isTabStop = control.IsTabStop;
            control.IsTabStop = false;
            control.IsEnabled = false;
            control.IsEnabled = true;
            control.IsTabStop = isTabStop;
        }

        private void TextInputButton_Click(object sender, RoutedEventArgs e)
        {
            SendTextBox.Visibility = Visibility.Visible;
            (this.Resources["ShowSendTextBox"] as Storyboard).Begin();
            SendTextBox.Focus(FocusState.Keyboard);
        }

        private void SendTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ((this.Resources["HideSendTextBox"]) as Storyboard).Begin();
        }

    }
}
