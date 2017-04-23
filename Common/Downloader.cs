using System;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Common
{
    public class Downloader
    {
        public static async Task<ImageSource> DownloadImage(Uri uri, string fileName)
        {
            var bitmapImage = new BitmapImage();
            var httpClient = new HttpClient();
            var httpResponse = await httpClient.GetAsync(uri);
            var bytes = await httpResponse.Content.ReadAsByteArrayAsync();
            var storageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

            using (var stream = new InMemoryRandomAccessStream())
            {
                using (var dw = new DataWriter(stream))
                {
                    dw.WriteBytes(bytes);
                    await dw.StoreAsync();

                    stream.Seek(0);
                    bitmapImage.SetSource(stream);

                    await FileIO.WriteBytesAsync(storageFile, bytes);

                    return bitmapImage;
                }
            }
        }
    }
}