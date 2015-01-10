﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using XBMCRemoteRT.Models;

namespace XBMCRemoteRT.Helpers
{
    public class CacheManager
    {
        public static async void InitCacheAsync()
        {
            IEnumerable<string> imagePaths = await GetAllImagesAsync();
            foreach (string imagePath in imagePaths)
            {
                Stream imageStream = await GetImageStream(GetRemoteUri(imagePath));
                if (imageStream != null)
                {
                    // TODO: Save image
                }
                else
                {
                    // TODO: Handle failed image downloads with friendly error message
                }
            }
        }

        public static async Task<IEnumerable<string>> GetAllImagesAsync()
        {
            List<string> imagePaths = new List<string>();

            // TODO: Compile a collection of potential images from movies, music, and TV.

            return imagePaths;
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

        /// <summary>
        /// Returns path to a locally cached copy of the image. If the imagePath has not yet been cached, download is initiated but the URI remains the same.
        /// </summary>
        /// <param name="imagePath">Path on remote server to the resource. Usually begins with "image://"</param>
        /// <returns>Path to locally cached copy of the resource</returns>
        public static Uri GetCacheUri(string imagePath)
        {
            Uri imageUri = null;

            // TODO: Should each connection have its own cache? Two connections could have different images at the same path, but unlikely.
            string storageFileName = MD5Core.GetHashString(imagePath) + ".tmp";
            string storagePath = Path.Combine(ApplicationData.Current.TemporaryFolder.Path, storageFileName);
            imageUri = new Uri(storagePath, UriKind.Absolute);

            // TODO: Ideally, we've predicted all the possible images and cached them, but cache misses can be handled by simply downloading the image to the cache. Only a mild inconvenience for the user.
            VerifyCache(GetRemoteUri(imagePath), storageFileName);

            return imageUri;
        }

        /// <summary>
        /// Ensure filename is cached. If not, download and save it to the cache location.
        /// </summary>
        /// <param name="imageUri">Remote location of the resource</param>
        /// <param name="filename">Cached file name</param>
        private static async void VerifyCache(Uri imageUri, string filename)
        {
            if (!(await IsFileCached(filename)))
            {

                // TODO: cache miss. download and save.
                Stream imageStream = await GetImageStream(imageUri);
                WriteFile(imageStream, filename);
                // TODO: Check file age and refresh cached image. Age is a bad way to do it though.
            }
        }

        /// <summary>
        /// Determine whether filename is cached.
        /// </summary>
        /// <param name="filename">File name</param>
        /// <returns>Whether filename exists in the cache location</returns>
        private static async Task<bool> IsFileCached(string filename)
        {

            // Attempt to open file in app temp folder
            StorageFolder parent = ApplicationData.Current.TemporaryFolder;
            bool fileExists = false;
            try
            {
                await parent.GetFileAsync(filename);
                fileExists = true;
            }
            catch (FileNotFoundException)
            {
                // Image is not cached
            }
            return fileExists;
        }

        /// <summary>
        /// Downloads image content from a remote location. TODO: Requests are cached.
        /// </summary>
        /// <param name="imageUri">Image location</param>
        /// <returns>Stream content of the HTTP response. null if not connected, imageUri is invalid, or HTTP response is not OK</returns>
        private static async Task<Stream> GetImageStream(Uri imageUri)
        {
            Stream imageStream = null;

            // Download the image with HTTP Basic auth
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(imageUri.Scheme + "://" + imageUri.Authority);
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, imageUri.AbsolutePath);
            ConnectionItem con = ConnectionManager.CurrentConnection;
            if (con != null && con.Password != String.Empty)
            {
                req.Headers.Authorization = new AuthenticationHeaderValue(
                    "Basic",
                    System.Convert.ToBase64String(Encoding.UTF8.GetBytes(
                        String.Format("{0}:{1}",
                        con.Username,
                        con.Password)
                    ))
                );
            }

            HttpResponseMessage res = await client.SendAsync(req);
            if (res.IsSuccessStatusCode)
            {
                imageStream = await res.Content.ReadAsStreamAsync();
            }

            return imageStream;
        }

        /// <summary>
        /// Write the contents of stream to filename in the cache location.
        /// </summary>
        /// <param name="stream">Content to be written to file</param>
        /// <param name="filename">Name of the file to be written in cache location</param>
        private static async void WriteFile(Stream stream, string filename)
        {

            // Prepare input image stream
            IInputStream inStream = stream.AsInputStream();
            DataReader reader = new DataReader(inStream);

            // Prepare output file stream
            StorageFolder parent = ApplicationData.Current.TemporaryFolder;
            StorageFile file = null;
            try
            {
                file = await parent.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            }
            catch (Exception) {
            }

            if (file != null)
            {
                IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.ReadWrite);
                // Buffered write to file
                await reader.LoadAsync(1024);
                while (reader.UnconsumedBufferLength > 0)
                {
                    await fileStream.WriteAsync(reader.ReadBuffer(reader.UnconsumedBufferLength));
                    await reader.LoadAsync(1024);
                }

                await fileStream.FlushAsync();
                inStream.Dispose();
                // TODO: Handle write task canceled exceptions, or prepare for corrupted images
            }
        }

        public static async void ClearCacheAsync()
        {
            // TODO: Empty the temp folder for the current connection
        }
    }
}