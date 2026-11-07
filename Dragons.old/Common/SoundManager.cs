using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Dragons.Common
{
    public static class SoundManager
    {
        async public static Task PlaySound(string soundFileName)
        {
            var package = Windows.ApplicationModel.Package.Current;
            var installedLocation = package.InstalledLocation;
            var storageFile = await installedLocation.GetFileAsync(String.Format("Assets\\sounds\\{0}", soundFileName));
            if (storageFile != null)
            {
                var stream = await storageFile.OpenAsync(Windows.Storage.FileAccessMode.Read);
                MediaElement snd = new MediaElement();
                snd.SetSource(stream, storageFile.ContentType);
                snd.Play();
            }

        }
    }
}
