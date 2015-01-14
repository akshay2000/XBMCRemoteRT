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
        public static async Task<bool> SetSourceAsync(this BitmapImage image,
               string url, string username, string password)
        {
            return await image.SetSourceAsync(new Uri(url), username, password);
        }

        public static async Task<bool> SetSourceAsync(this BitmapImage image,
               Uri uri, string username, string password)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);

            request.Credentials = new NetworkCredential(username, password);
            request.Method = "GET";
            request.Accept = "image/gif;q=0.3, image/x-xbitmap;q=0.3, " +
              "image/jpeg;q=0.3, image/pjpe;q=0.3g, image/png;q=0.3";

            try
            {
                var response = await request.GetResponseAsync();

                if (response != null)
                {
                    Stream stream = response.GetResponseStream();
                    MemoryStream ms = new MemoryStream();
                    stream.CopyTo(ms);
                    var randomAccessStream = await ConvertToRandomAccessStream(ms);
                    await image.SetSourceAsync(randomAccessStream);
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
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