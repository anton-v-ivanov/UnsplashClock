using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace UnsplashClock
{
    public sealed partial class MainPage : Page
    {
        private const string LastFileName = "last.jpg";
        private const string Category = "nature";

        public MainPage()
        {
            InitializeComponent();

            DateTimeTimerOnTick(null, null);
            var dateTimeTimer = new DispatcherTimer();
            dateTimeTimer.Tick += DateTimeTimerOnTick;
            dateTimeTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            dateTimeTimer.Start();

            SetLastSavedImage(LastFileName);

            BackgroundTimerOnTick(null, null);

            var backgroundTimer = new DispatcherTimer();
            backgroundTimer.Tick += BackgroundTimerOnTick;
            backgroundTimer.Interval = new TimeSpan(0, 0, 10);
            backgroundTimer.Start();

            SettingsPane.GetForCurrentView().CommandsRequested += OnCommandsRequested;
        }

        private static void OnCommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            var setting = new SettingsCommand("ClockSettings", "Settings", handler => new SettingsFlyout1().Show());
            args.Request.ApplicationCommands.Add(setting);
        }

        private async void SetLastSavedImage(string fileName)
        {
            var storageFile = await ApplicationData.Current.LocalFolder.TryGetItemAsync(fileName);
            if (storageFile == null)
                return;

            var source = new BitmapImage(new Uri(storageFile.Path));
            Staging.Source = source;
            ChangeImage(source);
        }

        private void DateTimeTimerOnTick(object sender, object o)
        {
            TimeText.Text = DateTime.Now.ToString("HH:mm");
            DateText.Text = DateTime.Now.ToString("D");
        }

        private async void BackgroundTimerOnTick(object sender, object o)
        {
            var bounds = Window.Current.Bounds;
            var uri = new Uri($"http://source.unsplash.com/category/{Category}/{bounds.Width}x{bounds.Height}", UriKind.Absolute);
            ImageSource source;
            try
            {
                source = await DownloadImage(uri, LastFileName);
            }
            catch
            {
                return;
            }

            Staging.Source = source;
            ChangeImage(source);
        }

        private void ChangeImage(ImageSource source)
        {
            ImageFadeOut.Completed += (s, e) =>
            {
                BackImage.Source = source;
                ImageFadeIn.Begin();
            };
            ImageFadeOut.Begin();
        }

        private static async Task<ImageSource> DownloadImage(Uri uri, string fileName)
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
