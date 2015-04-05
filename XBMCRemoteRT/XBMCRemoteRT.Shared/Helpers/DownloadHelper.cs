using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
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

        public async static Task<string> DownloadFile(String uriString)
        {
            Uri imageUri = GetRemoteUri(uriString);
            var stream = await GetImageStreamAsync(imageUri);
            await WriteFileAsync(stream, "tempFile");
            return Path.Combine(GetCacheFolder().Path, "tempfile");
        }

        /// <summary>
        /// Returns remote location of the image on the currently connected Kodi server.
        /// </summary>
        /// <param name="imagePath">Path on remote server to the resource. Usually begins with "image://"</param>
        /// <returns>Location of the resource on the currently connected Kodi server. null if not connected or if imagePath is invalid.</returns>
        public static Uri GetRemoteUri(string imagePath)
        {
            Uri imageUri = null;
            // Build Kodi proxy image address
            ConnectionItem con = ConnectionManager.CurrentConnection;
            if (con != null)
            {
                string baseUrlString = "http://" + con.IpAddress + ":" + con.Port.ToString() + "/image/";
                string encodedUri = WebUtility.UrlEncode(imagePath);
                try
                {
                    imageUri = new Uri(baseUrlString + encodedUri);
                }
                catch (FormatException) { }
            }
            return imageUri;
        }

    }
}
