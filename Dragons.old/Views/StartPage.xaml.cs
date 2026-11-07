using Dragons.Common;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Dragons.Views
{
    /// <summary>
    /// The start page.
    /// </summary>
    public sealed partial class StartPage
    {
        async void OnLoad(object sender, RoutedEventArgs e)
        {
            // Grab global MediaElement
            DependencyObject rootGrid = VisualTreeHelper.GetChild(Window.Current.Content, 0);
            MediaElement rootMediaElement = (MediaElement)VisualTreeHelper.GetChild(rootGrid, 0);

            // Don't restart the tune if it's already playing
            if (rootMediaElement.CurrentState != MediaElementState.Playing)
            { 
            
                var package = Windows.ApplicationModel.Package.Current;
                var installedLocation = package.InstalledLocation;
                var storageFile = await installedLocation.GetFileAsync("Assets\\sounds\\thememusic.mp3");
                if (storageFile != null)
                {
                    var stream = await storageFile.OpenAsync(Windows.Storage.FileAccessMode.Read);
                    rootMediaElement.SetSource(stream, storageFile.ContentType);
                    rootMediaElement.IsLooping = true;
                    rootMediaElement.Play();
                }
            }

        }
        /// <summary>
        /// Initializes a new instance of the StartPage class.
        /// </summary>
        public StartPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

        /// <summary>
        /// Navigates to the a new page in response to a button click. 
        /// </summary>
        /// <param name="sender">The source of the click event.</param>
        /// <param name="e">The event data.</param>
        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            switch ((sender as Button).Tag as string)
            {
                case "sample":
                    Frame.Navigate(typeof(SampleGameBoard));
                    break;
                case "new":
                    Frame.Navigate(typeof(GameBoard));
                    break;
                case "learn":
                    Frame.Navigate(typeof(HelpPage));
                    break;
            }            
        }
    }
}
