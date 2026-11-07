using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Dragons.Common;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Dragons.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SampleGameBoard : Page
    {
        public SampleGameBoard()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private async void CastButton_Click(object sender, RoutedEventArgs e)
        {

            await SoundManager.PlaySound("cast.mp3");
            await Task.Delay(1000);
            await SoundManager.PlaySound("wizardfire.mp3");
            await Task.Delay(1000);
            
            //SoundManager.PlaySound("wizard death scream.mp3");
            
            Scenario1Storyboard.Begin();


            BoardButton bb = (BoardButton)OpponentStackPanel.Children[0];
            //bb.Image = "Scorch1";
            /*Grid g = (Grid)bb.Content;
            Button b = (Button)g.Children[0];
            //g.Resources["Scorch1"]*/


        }
    }
}
