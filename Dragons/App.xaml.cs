using Dragons.Common;
using Dragons.Services;
using Dragons.Shared;
using Dragons.Views;
using Dragons.Views.Settings;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.ApplicationSettings;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml.Media.Imaging;

// The Grid App template is documented at http://go.microsoft.com/fwlink/?LinkId=234226

namespace Dragons
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App
    {
        /// <summary>
        /// Initializes the singleton Application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
            Resuming += (sender, e) => Start();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            // Do not repeat app initialization when already running, just ensure that
            // the window is active
            if (args.PreviousExecutionState == ApplicationExecutionState.Running)
            {
                Window.Current.Activate();
                return;
            }

            await LobbyService.JoinLobby();

            // Create a Frame to act as the navigation context and associate it with
            // a SuspensionManager key
            var rootFrame = new Frame();
            rootFrame.Style = Resources["RootFrameStyle"] as Style;

            SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

            var isRestart = false;
            if (args.PreviousExecutionState == ApplicationExecutionState.Terminated ||
                args.PreviousExecutionState == ApplicationExecutionState.ClosedByUser)
            {
                if (rootFrame.Content != null) isRestart = true;
            }

            if (isRestart)
            {
                while (rootFrame.CanGoBack) rootFrame.GoBack();
            }
            else if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!rootFrame.Navigate(typeof(StartPage)))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            // Place the frame in the current Window and ensure that it is active
            Window.Current.Content = rootFrame;
            Window.Current.Activate();
            Start();
        }

        /// <summary>
        /// Adds various event handlers, starts the game clock, and performs the next move if it is automatic. 
        /// </summary>
        private void Start()
        {
            Window.Current.VisibilityChanged += OnWindowVisibilityChanged;
            Window.Current.SizeChanged += OnWindowSizeChanged;
            DataTransferManager.GetForCurrentView().DataRequested += OnDataRequested;
        }

        /// <summary>
        /// Starts a share operation.
        /// </summary>
        /// <param name="sender">The source of the data request.</param>
        /// <param name="e">Details about the data request.</param>
        private void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
           
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            Window.Current.VisibilityChanged -= OnWindowVisibilityChanged;
            Window.Current.SizeChanged -= OnWindowSizeChanged;
            
            DataTransferManager.GetForCurrentView().DataRequested -= OnDataRequested;
            var deferral = e.SuspendingOperation.GetDeferral();
            await LobbyService.ExitLobby();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }

        /// <summary>
        /// Pauses or resumes the game clock when the window visibility changes.
        /// </summary>
        /// <param name="sender">The source of the property change.</param>
        /// <param name="e">Details about the property change.</param>
        private void OnWindowVisibilityChanged(object sender, 
            VisibilityChangedEventArgs e)
        {
            UpdateViews();
        }

        /// <summary>
        /// Pauses or resumes the game clock when the window enters or leaves snapped state.
        /// </summary>
        /// <param name="sender">The source of the property change.</param>
        /// <param name="e">Details about the property change.</param>
        private void OnWindowSizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            UpdateViews();
        }

        /// <summary>
        /// Updates when the window changes state.
        /// </summary>
        private void UpdateViews()
        {
            if (!Window.Current.Visible || ApplicationView.Value == ApplicationViewState.Snapped)
            {
                //do something
            }
            else if ((Window.Current.Content as Frame).Content is GameBoard)
            {
                //do something
            }
        }

        // In most cases, it is a good idea to pause the game while the 
        // settings pane is open. You can do this by adding the following code
        // to the top of the OnCommandsRequested method. This code is not 
        // used by Dragons because the display settings are simple, and cause 
        // animated changes to the game board that would not be visible if the 
        // game were paused. 
        //if (SettingsViewModel.GameViewModel != null &&
        //    SettingsViewModel.IsClockShowing)
        //    SettingsViewModel.GameViewModel.Clock.Stop();
    }
}
