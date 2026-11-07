using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Dragons.Common;
using Windows.UI.Xaml.Media.Animation;
using Dragons.Shared;
using Windows.UI.Xaml.Media.Imaging;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Dragons.Views
{
    public sealed partial class BoardButton : UserControl
    {
        public event EventHandler<Coordinate> CoordinateSelected;
        private bool initialized = false;
        private bool IsPlayedOut = false;

        public BoardButton()
        {
            this.InitializeComponent();
        }
        private Coordinate _coordinate;

        public BoardButton(int x, int y, Cell cell = null)
            : this()
        {
            if (cell != null)
            {
                FogImage.Opacity = 0.5;
                Init(cell);
            }
            _coordinate = new Coordinate() { x = x, y = y };
        }

        private void Init(Cell c)
        {
            initialized = true;
            if (c.PieceKey.Contains("Body"))
            {
                DragonImage.Source = ImageHelper.GetImageFromURI(this.BaseUri, "/Assets/buttons/body.png");
                DragonImage.Opacity = 1;
            }
            else if (c.PieceKey.Contains("Head"))
            {
                DragonImage.Source = ImageHelper.GetImageFromURI(this.BaseUri, "/Assets/buttons/head.png");
                DragonImage.Opacity = 1;
            }
            else if (c.PieceKey.Contains("Mana"))
            {
                Mana.Begin();
            }
        }

        /*public Button Button
        {
            //get { return this.test; }
        }*/

        public async Task Update(Cell c)
        {
            if (!initialized)
                Init(c);

            if (!IsPlayedOut)
            {
                if (c.PieceKey.Contains("Body"))
                {
                    DragonImage.Source = ImageHelper.GetImageFromURI(this.BaseUri, "/Assets/buttons/body.png");
                    DragonImage.Opacity = 1;
                    Spell.Begin();
                    RevealPiece.Begin();
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => { await SoundManager.PlaySound("dragonpiecereveal.mp3"); });
                    await Task.Delay(100);
                }
                else if (c.PieceKey.Contains("Head"))
                {
                    DragonImage.Source = ImageHelper.GetImageFromURI(this.BaseUri, "/Assets/buttons/head.png");
                    DragonImage.Opacity = 1;
                    Spell.Begin();
                    RevealPiece.Begin();
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => { await SoundManager.PlaySound("dragonpiecereveal.mp3"); });
                    await Task.Delay(100);
                }
                else if (c.PieceKey.Contains("Mana"))
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => { await SoundManager.PlaySound("newmana.mp3"); });
                    await Task.Delay(100);
                    Spell.Begin();
                    RevealPiece.Begin();
                    Mana.Begin();
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => { await SoundManager.PlaySound("gulp.mp3"); });
                    await Task.Delay(100);
                    ManaConsume.Begin();
                }
                else if (c.PieceKey.Contains("Ground"))
                {
                    RevealPiece.Begin();
                    var random = new Random(_coordinate.x * _coordinate.y);
                    var index = random.Next(0, 3);
                    switch (index)
                    {
                        case 0:
                            Scorch1.Begin();
                            break;
                        case 1:
                            Scorch2.Begin();
                            break;
                        default:
                            Scorch3.Begin();
                            break;
                    }
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => { await SoundManager.PlaySound("revealground.mp3"); });
                    await Task.Delay(100);
                }

                IsPlayedOut = true;
            }

        }

        public async Task UpdateKill(Cell c)
        {
            if (c.PieceKey.Contains("Body"))
                DeadImage.Source = ImageHelper.GetImageFromURI(this.BaseUri, "/Assets/buttons/bodybone.png");
            else
                DeadImage.Source = ImageHelper.GetImageFromURI(this.BaseUri, "/Assets/buttons/headbone.png");

            KillPiece.Begin();
        }

        private void ButtonButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (CoordinateSelected != null)
                CoordinateSelected(this, _coordinate);
            SelectImage.Opacity = 1;
        }

        public void UnHighlight()
        {
            SelectImage.Opacity = 0;
        }

        //public string Image
        //{
        //    set
        //    {
        //        switch(value)
        //        {
        //            case "Scorch1":
        //                SpellImage.Source = ImageHelper.GetImageFromURI(this.BaseUri, "/Assets/buttons/fire.png");
        //                break;
        //        }
        //        /*
        //        muddark.Visibility = Visibility.Visible;
        //        mudlight.Visibility = Visibility.Collapsed;
        //        mana.Visibility = Visibility.Collapsed;
        //        head.Visibility = Visibility.Collapsed;
        //        body.Visibility = Visibility.Collapsed;
        //        headbone.Visibility = Visibility.Collapsed;
        //        fire.Visibility = Visibility.Collapsed;

        //        switch (value)
        //        {
        //            case "mudlight":
        //                mudlight.Visibility = Visibility.Visible;
        //                muddark.Visibility = Visibility.Collapsed;
        //                break;
        //            case "muddark":
        //                muddark.Visibility = Visibility.Visible;
        //                break;
        //            case "mana":
        //                mana.Visibility = Visibility.Visible;
        //                break;
        //            case "head":
        //                head.Visibility = Visibility.Visible;
        //                break;
        //            case "Dragon_1_Body":
        //            case "body":
        //                body.Visibility = Visibility.Visible;
        //                mudlight.Visibility = Visibility.Visible;
        //                muddark.Visibility = Visibility.Collapsed;
        //                break;
        //            case "headbone":
        //                headbone.Visibility = Visibility.Visible;
        //                break;
        //            case "bodybone":
        //                bodybone.Visibility = Visibility.Visible;
        //                break;
        //            case "fire":
        //                muddark.Visibility = Visibility.Visible;
        //                fire.Visibility = Visibility.Visible;
        //                break;*/
        //        }
        //    }

    }
}
