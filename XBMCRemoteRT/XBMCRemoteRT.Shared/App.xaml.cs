using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using XBMCRemoteRT.Common;
#if WINDOWS_PHONE_APP
using XBMCRemoteRT.Pages.Entry;
#endif
using XBMCRemoteRT.ViewModels;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace XBMCRemoteRT
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        private Frame rootFrame;
#if WINDOWS_PHONE_APP
        private TransitionCollection transitions;
#endif

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;

            connectionsVM = new ConnectionsViewModel();
        }

        private static ConnectionsViewModel connectionsVM;

        public static ConnectionsViewModel ConnectionsVM
        {
            get { return connectionsVM; }
        }

        /// <summary>
        /// Both the OnLaunched and OnActivated event handlers need to make sure the root frame has been created, so the common 
        /// code to do that is factored into this method and called from both.
        /// </summary>
        private async void EnsureRootFrame(ApplicationExecutionState previousExecutionState)
        {
            this.rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (this.rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                this.rootFrame = new Frame();

                //Associate the frame with a SuspensionManager key                                
                SuspensionManager.RegisterFrame(this.rootFrame, "AppFrame");

                this.rootFrame.CacheSize = 1;

                if (previousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // Load state from previously suspended application
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (SuspensionManagerException)
                    {
                        //Something went wrong restoring state.
                        //Assume there is no state and continue
                    }
                }

                // Place the frame in the current Window
                Window.Current.Content = this.rootFrame;
            }

            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            Debug.WriteLine("Launched with tile id " + e.TileId);            
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            EnsureRootFrame(e.PreviousExecutionState);
            if (e.TileId.StartsWith("tvShow"))
            {
#if WINDOWS_PHONE_APP
                rootFrame.Navigate(typeof(TVShowThinPivot), e.Arguments);
#endif
            }

            if (rootFrame.Content == null)
            {
#if WINDOWS_PHONE_APP
                // Removes the turnstile navigation for startup.
                if (rootFrame.ContentTransitions != null)
                {
                    this.transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        this.transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += this.RootFrame_FirstNavigated;
#endif

                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter                
                if (!rootFrame.Navigate(typeof(MainPage), e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }
                
            

            // Ensure the current window is active
            Window.Current.Activate();

#if WINDOWS_PHONE_APP
            try
            {
                var storageFile = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///VCD.xml"));
                await Windows.Media.SpeechRecognition.VoiceCommandManager.InstallCommandSetsFromStorageFileAsync(storageFile);
            }
            catch (FileNotFoundException ex)
            {
                Debug.WriteLine(ex.Message);
            }
#endif
        }


#if WINDOWS_PHONE_APP
        /// <summary>
        /// Restores the content transitions after the app has launched.
        /// </summary>
        /// <param name="sender">The object where the handler is attached.</param>
        /// <param name="e">Details about the navigation event.</param>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }
#endif


        protected override void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);
#if WINDOWS_PHONE_APP
            if (args.Kind == ActivationKind.VoiceCommand)
            {
                EnsureRootFrame(args.PreviousExecutionState);
                rootFrame.Navigate(typeof(VoiceCommandsPage), args);
            }
#endif            
        }


        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            // TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}