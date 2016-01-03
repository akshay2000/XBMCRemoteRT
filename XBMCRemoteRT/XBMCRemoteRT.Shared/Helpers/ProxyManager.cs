using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using XBMCRemoteRT.Models.Network;

namespace XBMCRemoteRT.Helpers
{
    public class ProxyManager
    {
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

        public static async Task<Stream> GetStream(string imagePath)
        {
                Stream remoteStream = await GetImageStreamAsync(GetRemoteUri(imagePath));
                return remoteStream;
        }

        /// <summary>
        /// Downloads image content from a remote location, ignoring the cache.
        /// </summary>
        /// <param name="imageUri">Image location</param>
        /// <returns>Stream content of the HTTP response. null if not connected, imageUri is invalid, or HTTP response is not OK</returns>
        private static async Task<Stream> GetImageStreamAsync(Uri imageUri)
        {
            Stream imageStream = null;

            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, imageUri.AbsolutePath);

            HttpResponseMessage res = await ConnectionManager.CurrentConnection.HttpClient.SendAsync(req, HttpCompletionOption.ResponseHeadersRead);
            if (res.IsSuccessStatusCode)
            {
                imageStream = await res.Content.ReadAsStreamAsync();
            }

            return imageStream;
        }
    }
}
