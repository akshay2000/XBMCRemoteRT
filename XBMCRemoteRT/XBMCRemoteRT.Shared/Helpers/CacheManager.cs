using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using XBMCRemoteRT.Models;
using XBMCRemoteRT.Models.Common;
using XBMCRemoteRT.RPCWrappers;

namespace XBMCRemoteRT.Helpers
{
    public class CacheManager
    {
        /// <summary>
        /// Fetch images from the server and add the images to the cache. Existing images are not overwritten.
        /// </summary>
        /// <returns>Async operation that will return the number of images that encountered errors.</returns>
        public static IAsyncOperationWithProgress<int, int> UpdateCacheAsync()
        {
            return AsyncInfo.Run<int, int>(async (cancelToken, progress) =>
                {
                    ICollection<string> imagePaths = await GetAllImagePathsAsync();
                    int imageCount = 0;
                    foreach (string imagePath in imagePaths)
                    {
                        if (imagePath == string.Empty)
                        {
                            // Empty string will only occur once in the 
                            // HashSet, but we don't want to indicate an error for it.
                            imageCount++;
                        }
                        else
                        {
                            string filename = GetTempFileName(imagePath);
                            if (await IsFileCachedAsync(filename))
                            {
                                // File exists and will not be overwritten.
                                imageCount++;
                            }
                            else
                            {
                                Stream imageStream = await GetImageStreamAsync(GetRemoteUri(imagePath));
                                if (imageStream != null)
                                {
                                    imageCount++;
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine(GetRemoteUri(imagePath));
                                }
                                // A null imagestream means an error encountered in
                                // GetImageStreamAsync will likely continue. (e.g. HTTP
                                // 404, HTTP 500) Write an the file even with a null
                                // imageStream so download is not attempted repeatedly.
                                await WriteFileAsync(imageStream, filename);
                            }
                        }
                        progress.Report(imageCount * 100 / imagePaths.Count);
                    }

                    return imagePaths.Count - imageCount;
                });
        }

        public static async Task<ICollection<string>> GetAllImagePathsAsync()
        {
            //Use hashset to avoid downloading same url twice
            HashSet<string> imagePaths = new HashSet<string>();

            var albums = await AudioLibrary.GetAlbums();
            foreach (var album in albums)
            {
                imagePaths.Add(album.Thumbnail);
                //imagePaths.Add(album.Fanart); //Not using anywhere
            }

            //var artists = await AudioLibrary.GetArtists();
            //foreach (var artist in artists)
            //{
            //    //imagePaths.Add(artist.Thumbnail);
            //    //imagePaths.Add(artist.Fanart);
            //}

            var movies = await VideoLibrary.GetMovies();
            foreach (var movie in movies)
            {
                imagePaths.Add(movie.Thumbnail);
                //imagePaths.Add(movie.Fanart);
            }

            var tvShows = await VideoLibrary.GetTVShows();
            foreach (var tvShow in tvShows)
            {
                imagePaths.Add(tvShow.Art.Banner);
                //imagePaths.Add(tvShow.Fanart);
                //imagePaths.Add(tvShow.Thumbnail);
            }

            var recentEpisodes = await VideoLibrary.GetRecentlyAddedEpisodes(new Limits { Start = 0, End = 10 });
            foreach (var episode in recentEpisodes)
            {
                imagePaths.Add(episode.Thumbnail);
            }

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

            string storageFileName = GetTempFileName(imagePath);
            string storagePath = Path.Combine(GetCacheFolder().Path, storageFileName);
            imageUri = new Uri(storagePath, UriKind.Absolute);

            VerifyCacheAsync(GetRemoteUri(imagePath), storageFileName);

            return imageUri;
        }

        public static async Task<Stream> GetStream(string imagePath)
        {
            string filename = GetTempFileName(imagePath);
            if (await IsFileCachedAsync(filename))
            {
                StorageFolder parent = GetCacheFolder();
                var file = await parent.GetFileAsync(filename);
                Stream stream = await file.OpenStreamForReadAsync();
                return stream;
            }
            else
            {
                Stream remoteStream = await GetImageStreamAsync(GetRemoteUri(imagePath));
                //This disk IO is what kills the performance. Way to work around that?

                await WriteFileAsync(remoteStream, filename);
                remoteStream.Position = 0;
                return remoteStream;
            }
        }

        private static string GetTempFileName(string imagePath)
        {
            return MD5Core.GetHashString(imagePath) + ".tmp";
        }

        /// <summary>
        /// Ensure filename is cached. If not, download and save it to the cache location.
        /// </summary>
        /// <param name="imageUri">Remote location of the resource</param>
        /// <param name="filename">Cached file name</param>
        private static async Task VerifyCacheAsync(Uri imageUri, string filename)
        {
            // TODO: Consider expiring the cache somehow. As it stands, new 
            // images will be added to the cache at connection select, but
            // changed images are only reflected on manual update by user.
            if (!(await IsFileCachedAsync(filename)))
            {
                // Cache miss, download and save the image
                Stream imageStream = await GetImageStreamAsync(imageUri);
                await WriteFileAsync(imageStream, filename);
            }
        }

        /// <summary>
        /// Determine whether filename is cached.
        /// </summary>
        /// <param name="filename">File name</param>
        /// <returns>Whether filename exists in the cache location</returns>
        private static async Task<bool> IsFileCachedAsync(string filename)
        {
            // Attempt to open file in app temp folder
            StorageFolder parent = GetCacheFolder();
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
                //inStream.Dispose();
            }
        }

        private static StorageFolder GetCacheFolder()
        {
            // Return folder to store cache files.
            // One day this may return a different folder for each connection.
            return ApplicationData.Current.TemporaryFolder;
        }

        public static IAsyncOperationWithProgress<int, int> ClearCacheAsync()
        {
            return AsyncInfo.Run<int, int>(async (cancelToken, progress) =>
                {
                    // Empty the temp folder
                    StorageFolder parent = GetCacheFolder();
                    IReadOnlyList<StorageFile> cachedFiles = await parent.GetFilesAsync();

                    int fileCount = 0;
                    foreach (StorageFile file in cachedFiles)
                    {
                        await file.DeleteAsync();
                        // Report progress to caller
                        progress.Report(++fileCount * 100 /cachedFiles.Count);
                    }

                    return 0;
                });
        }
    }
}
