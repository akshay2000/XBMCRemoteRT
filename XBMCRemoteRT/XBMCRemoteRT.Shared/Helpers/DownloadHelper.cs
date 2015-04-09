using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using XBMCRemoteRT.Models.Network;

namespace XBMCRemoteRT.Helpers
{
    class DownloadHelper
    {
        /// <summary>
        /// Downloads image content from a remote location, ignoring the cache.
        /// </summary>
        /// <param name="imageUri">Image location</param>
        /// <returns>Stream content of the HTTP response. null if not connected, imageUri is invalid, or HTTP response is not OK</returns>
        private static async Task<Stream> GetImageStreamAsync(Uri imageUri)
        {
            Stream imageStream = null;
            // Download the image with HTTP Basic auth
            HttpClient client = new HttpClient();
            // Ignore the HttpClient cache
            client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue() { NoCache = true };
            ConnectionItem con = ConnectionManager.CurrentConnection;
            if (con != null && con.HasCredentials())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Basic",
                System.Convert.ToBase64String(Encoding.UTF8.GetBytes(
                String.Format("{0}:{1}",
                con.Username,
                con.Password)
                ))
                );
            }
            client.BaseAddress = new Uri(imageUri.Scheme + "://" + imageUri.Authority);
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, imageUri.AbsolutePath);
            HttpResponseMessage res = await client.SendAsync(req);
            if (res.IsSuccessStatusCode)
            {
                imageStream = await res.Content.ReadAsStreamAsync();
            }
            return imageStream;
        }
        /// <summary>
        /// Write the contents of stream to filename in the cache location. If a null stream is provided, the file is created with no contents.
        /// </summary>
        /// <param name="stream">Content to be written to file</param>
        /// <param name="filename">Name of the file to be written in cache location</param>
        private static async Task WriteFileAsync(Stream stream, string filename)
        {
            // Prepare output file stream
            StorageFolder parent = GetCacheFolder();
            StorageFile file = null;
            try
            {
                file = await parent.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            }
            catch (Exception)
            {
            }
            if (file != null && stream != null)
            {
                // Prepare input image stream
                IInputStream inStream = stream.AsInputStream();
                DataReader reader = new DataReader(inStream);
                IRandomAccessStream fileStream = null;
                try
                {
                    fileStream = await file.OpenAsync(FileAccessMode.ReadWrite);
                    // Buffered write to file
                    await reader.LoadAsync(1024);
                    while (reader.UnconsumedBufferLength > 0)
                    {
                        await fileStream.WriteAsync(reader.ReadBuffer(reader.UnconsumedBufferLength));
                        await reader.LoadAsync(1024);
                    }
                }
                catch (Exception)
                {
                }
                finally
                {
                    if (fileStream != null)
                    {
                        fileStream.FlushAsync();
                    }
                }
                inStream.Dispose();
            }
        }
        private static StorageFolder GetCacheFolder()
        {
            // Return folder to store cache files.
            // One day this may return a different folder for each connection.
            return ApplicationData.Current.TemporaryFolder;
        }

        public async static Task<string> DownloadFile(string uriString)
        {
            Uri imageUri = new Uri(GetRemoteUri(uriString));
            var stream = await GetImageStreamAsync(imageUri);
            await WriteFileAsync(stream, "tempFile");
            return Path.Combine(GetCacheFolder().Path, "tempfile");
        }

        public async static Task<string> DownloadImageForTile(string uriString)
        {
            Uri imageUri = new Uri(GetRemoteUri(uriString));
            var stream = await GetImageStreamAsync(imageUri);
            IRandomAccessStream inStream = stream.AsRandomAccessStream();
           // BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream.AsRandomAccessStream());

            var ras = await ResizeImage(inStream, 1024, 1024);
            //BitmapEncoder encoder = await BitmapEncoder.CreateForTranscodingAsync(ras, decoder);

            //encoder.BitmapTransform.ScaledWidth = 1024;
            //encoder.BitmapTransform.sc
            //await encoder.FlushAsync();



            string fileName = "tile.tmp";
            var file = await (await ApplicationData.Current.LocalFolder.CreateFolderAsync("Tiles", CreationCollisionOption.OpenIfExists)).CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName);
            using (var reader = new DataReader(ras))
            {
                await reader.LoadAsync((uint)ras.Size);
                var buffer = new byte[(int)ras.Size];
                reader.ReadBytes(buffer);
                await Windows.Storage.FileIO.WriteBytesAsync(file, buffer);
            }
            return file.Path;
        }

        /// <summary> 

        /// Resizes image data within a stream to a given width and height.

        /// </summary>

        /// <returns>

        /// Returns an image stream with the resized image data.

        /// </returns>

        private static async Task<IRandomAccessStream> ResizeImage(IRandomAccessStream imageStream, uint width, uint height)
        {

            IRandomAccessStream resizedStream = imageStream;

            var decoder = await BitmapDecoder.CreateAsync(imageStream);

            if (decoder.OrientedPixelHeight > height || decoder.OrientedPixelWidth > width)
            {

                resizedStream = new InMemoryRandomAccessStream();

                BitmapEncoder encoder = await BitmapEncoder.CreateForTranscodingAsync(resizedStream, decoder);

                double widthRatio = (double)width / decoder.OrientedPixelWidth;

                double heightRatio = (double)height / decoder.OrientedPixelHeight;



                // Use whichever ratio had to be sized down the most to make sure the image fits within our constraints.

                double scaleRatio = Math.Min(widthRatio, heightRatio);

                uint aspectHeight = (uint)Math.Floor((double)decoder.OrientedPixelHeight * scaleRatio);

                uint aspectWidth = (uint)Math.Floor((double)decoder.OrientedPixelWidth * scaleRatio);



                encoder.BitmapTransform.ScaledHeight = aspectHeight;

                encoder.BitmapTransform.ScaledWidth = aspectWidth;



                // write out to the stream

                await encoder.FlushAsync();



                // Reset the stream location.

                resizedStream.Seek(0);

            }



            return resizedStream;

        }

        /// <summary>
        /// Returns remote location of the image on the currently connected Kodi server.
        /// </summary>
        /// <param name="imagePath">Path on remote server to the resource. Usually begins with "image://"</param>
        /// <returns>Location of the resource on the currently connected Kodi server. null if not connected or if imagePath is invalid.</returns>
        public static string GetRemoteUri(string imagePath)
        {
            string imageUri = null;
            // Build Kodi proxy image address
            ConnectionItem con = ConnectionManager.CurrentConnection;
            if (con != null)
            {
                string baseUrlString = "http://" + con.IpAddress + ":" + con.Port.ToString() + "/image/";
                string encodedUri = WebUtility.UrlEncode(imagePath);
                try
                {
                    imageUri = baseUrlString + encodedUri;
                }
                catch (FormatException) { }
            }
            return imageUri;
        }

    }
}
