using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace XBMCRemoteRT.Helpers
{
    public static class BitmapImageExtension
    {
        public static async Task<bool> SetProxySourceAsync(this BitmapImage image, string imagePath)
        {
            var stream = await ProxyManager.GetStream(imagePath);
            if (stream != null)
            {
                IRandomAccessStream randomAccessStream = stream.AsRandomAccessStream();
                await image.SetSourceAsync(randomAccessStream);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}