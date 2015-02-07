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
                MemoryStream ms = new MemoryStream();
                stream.CopyTo(ms);
                //This convulated way of dealing with streams gives a lot better performance
                var randomAccessStream = await ConvertToRandomAccessStream(ms);
                await image.SetSourceAsync(randomAccessStream);
                return true;
            }
            else
            {
                return false;
            }
        }

        private static async Task<IRandomAccessStream> ConvertToRandomAccessStream(MemoryStream memoryStream)
        {
            var randomAccessStream = new InMemoryRandomAccessStream();
            var outputStream = randomAccessStream.GetOutputStreamAt(0);
            var dw = new DataWriter(outputStream);
            
            var task = Task.Factory.StartNew(() => dw.WriteBytes(memoryStream.ToArray()));
            await task;

            await dw.StoreAsync();
            await outputStream.FlushAsync();

            return randomAccessStream;
        }
    }
}