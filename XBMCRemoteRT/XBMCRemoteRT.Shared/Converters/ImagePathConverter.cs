using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Data;
using XBMCRemoteRT.Helpers;

namespace XBMCRemoteRT.Converters
{
    public class ImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string imagePath = (value == null) ? string.Empty : (string)value;
            string imageURL = string.Empty;
            string proxyScheme = "image://";
            string httpScheme = "http://";
            // TODO: are there other possible schemes for imagePath?
            if (imagePath.StartsWith(proxyScheme))
            {
                string uri = imagePath.Substring(proxyScheme.Length);

                // Transform URI to local storage path
                string decodedUri = WebUtility.UrlDecode(uri).Trim('/');
                if (decodedUri.StartsWith(httpScheme))
                {
                    // TODO: Proxy http sources through Kodi? Good for consistency I guess: decodedUri = decodedUri.Substring(httpScheme.Length);
                    return decodedUri;
                }
                // TODO: I am running Kodi on Ubuntu. What needs to change for Windows paths?
                string storagePath = decodedUri.Replace('/', '\\');
                // TODO: What happens if the URI passed to XAML has invalid characters?
                imageURL = ApplicationData.Current.TemporaryFolder.Path + '\\' + storagePath;

                // Build Kodi proxy image address
                var encodedUri = WebUtility.UrlEncode(uri);
                string baseUrlString = "http://" + ConnectionManager.CurrentConnection.IpAddress + ":" + ConnectionManager.CurrentConnection.Port.ToString() + "/image/image://";
                Uri imageUri = new Uri(baseUrlString + encodedUri);
                CheckImageStorage(storagePath, imageUri);
            }
            else
            {
                imageURL = "ms-appx:///Assets/DefaultArt.jpg";
            }
            Uri imageURI = new Uri(imageURL, UriKind.Absolute);
            return imageURI;
        }

        private async void CheckImageStorage(string storagePath, Uri imageUri)
        {
            string filename = Path.GetFileName(storagePath);
            string[] path = Path.GetDirectoryName(storagePath).Split('\\');

            // Traverse directories to file location
            StorageFolder parent = ApplicationData.Current.TemporaryFolder;
            try
            {
                foreach (string dir in path)
                {
                    bool dirExists = false;
                    try
                    {
                        parent = await parent.GetFolderAsync(dir);
                        dirExists = true;
                    }
                    catch (FileNotFoundException)
                    {
                        // Image is not cached, continue creating directories
                    }

                    if (!dirExists)
                    {
                        parent = await parent.CreateFolderAsync(dir);
                        System.Diagnostics.Debug.WriteLine("Created " + parent.Path);
                    }
                }

                // Attempt to open file
                bool fileExists = false;
                try
                {
                    await parent.GetFileAsync(filename);
                    fileExists = true;
                    // TODO: Check file age and refresh cached image. Age is a bad way to do it though.
                }
                catch (FileNotFoundException)
                {
                    // Image is not cached, download and save file
                }
                if (!fileExists)
                {
                    // TODO: Can I create the file only after request is complete? That way we avoid empty files from aborted requests
                    // or it might be okay actually because file isn't closed
                    StorageFile file = await parent.CreateFileAsync(filename);
                    System.Diagnostics.Debug.WriteLine("Creating " + file.Path);
                    GetImage(imageUri, file);
                }
            }
            catch (ArgumentException)
            {
                // Invalid characters in folder or file name. See 
                // Path.GetInvalid(Path|FileName)Chars. We could sanitize the 
                // path, but since the paths should also be valid on the host
                // we just swallow the exception and the image won't be
                // downloaded.
            }
        }

        private async void GetImage(Uri imageUri, StorageFile outFile)
        {
            // Download the image with HTTP Basic auth
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(imageUri.Scheme + "://" + imageUri.Authority);
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, imageUri.AbsolutePath);
            // TODO: Test for active connection? What are possible errors with ConnectionManager?
            if (ConnectionManager.CurrentConnection.Password != String.Empty)
            {
                req.Headers.Authorization = new AuthenticationHeaderValue(
                    "Basic",
                    System.Convert.ToBase64String(Encoding.UTF8.GetBytes(
                        String.Format("{0}:{1}",
                        ConnectionManager.CurrentConnection.Username,
                        ConnectionManager.CurrentConnection.Password)
                    ))
                );
            }

            HttpResponseMessage res = await client.SendAsync(req);
            // TODO: Ensure requests are cached
            if (res.IsSuccessStatusCode)
            {
                IInputStream imageStream = (await res.Content.ReadAsStreamAsync()).AsInputStream();
                DataReader reader = new DataReader(imageStream);

                IRandomAccessStream fileStream = await outFile.OpenAsync(FileAccessMode.ReadWrite);

                await reader.LoadAsync(1024);
                while (reader.UnconsumedBufferLength > 0)
                {
                    await fileStream.WriteAsync(reader.ReadBuffer(reader.UnconsumedBufferLength));
                    await reader.LoadAsync(1024);
                }

                await fileStream.FlushAsync();
                imageStream.Dispose();
            }
            // TODO: Handle failed download by ensuring file is not saved
        }


        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}