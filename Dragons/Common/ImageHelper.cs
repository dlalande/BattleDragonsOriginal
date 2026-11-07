using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Dragons.Common
{
    public class ImageHelper
    {
        public static ImageSource GetImageFromURI(Uri baseUri, string uri)
        {
            BitmapImage bi = new BitmapImage();
            bi.UriSource = new Uri(baseUri, uri);
            return bi;
        }
    }
}
