using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
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
            // TODO: are there other possible schemes for imagePath?
            if (imagePath.StartsWith(proxyScheme))
            {
                // Build Kodi proxy image address
                var encodedUri = WebUtility.UrlEncode(imagePath);
                string baseUrlString = "http://" + ConnectionManager.CurrentConnection.IpAddress + ":" + ConnectionManager.CurrentConnection.Port.ToString() + "/image/";
                imageURL = baseUrlString + encodedUri;

                // Only apply cache logic if authentication is in use. If not,
                // allow the image to be consumed from Kodi.
                if (ConnectionManager.CurrentConnection.Password != String.Empty)
                {
                    Uri imageUri = new Uri(imageURL);

                    // Hash the image proxy path into local storage file name
                    string storageFileName = MD5Core.GetHashString(imagePath) + ".tmp";
                    imageURL = ApplicationData.Current.TemporaryFolder.Path + '\\' + storageFileName;

                    CacheImage(imageUri, storageFileName);
                }
            }
            else
            {
                imageURL = "ms-appx:///Assets/DefaultArt.jpg";
            }
            Uri imageURI = new Uri(imageURL, UriKind.Absolute);
            return imageURI;
        }

        // TODO: Externalize these methods so the same logic can be applied to StringToImageBrushConverter
        private async void CacheImage(Uri imageUri, string storageFileName)
        {
            string filename = storageFileName;

            // Attempt to open file in app temp folder
            StorageFolder parent = ApplicationData.Current.TemporaryFolder;
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
                Stream imageStream = await GetImage(imageUri);

                if (imageStream != null)
                {
                    // Prepare input image stream
                    IInputStream inStream = imageStream.AsInputStream();
                    DataReader reader = new DataReader(inStream);

                    // Prepare output file stream
                    StorageFile file = await parent.CreateFileAsync(filename);
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
                    // TODO: If this image was just created, it is likely that the
                    // ImageSource consuming the path aborted due to file not 
                    // found. Can we "refresh" the ImageSource?
                }
            }
        }

        private async Task<Stream> GetImage(Uri imageUri)
        {
            Stream imageStream = null;

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
            if (res.IsSuccessStatusCode)
            {
                imageStream = await res.Content.ReadAsStreamAsync();
            }

            return imageStream;
        }


        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}