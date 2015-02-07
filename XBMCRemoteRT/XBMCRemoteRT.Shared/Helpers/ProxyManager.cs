using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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

            // Download the image with HTTP Basic auth
            HttpClient client = new HttpClient();
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
    }
}
