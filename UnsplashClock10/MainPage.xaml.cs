using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.ApplicationSettings;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Common;

namespace UnsplashClock10
{
    public sealed partial class MainPage : Page
    {
        private const string LastFileName = "last.jpg";
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public MainPage()
        {
            InitializeComponent();

            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;

            SettingsHelper.LoadSettings();

            DateTimeTimerOnTick(null, null);
            var dateTimeTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 100) };
            dateTimeTimer.Tick += DateTimeTimerOnTick;
            dateTimeTimer.Start();

            SetLastSavedImage(LastFileName);

            BackgroundTimerOnTick(this, null);
            var backgroundTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 1) };
            backgroundTimer.Tick += BackgroundTimerOnTick;
            backgroundTimer.Start();
            
            //SettingsPane.GetForCurrentView().CommandsRequested += (sender, args) =>
            //{
            //    var settingsFlyout = new Common.SettingsFlyout1();
            //    var setting = new SettingsCommand("ClockSettings", "Settings", handler => settingsFlyout.Show());
            //    args.Request.ApplicationCommands.Add(setting);
            //};

            //SettingsPane.GetForCurrentView().CommandsRequested += (sender, args) =>
            //{
            //    var settingsFlyout = new Common.SettingsFlyout2();
            //    var setting = new SettingsCommand("WorldClock", "World Clock", handler => settingsFlyout.Show());
            //    args.Request.ApplicationCommands.Add(setting);
            //};

            SettingsHelper.OnThemeChanged += theme => { ChangeImage(UriHelper.GetUri(theme, SettingsHelper.UpdateInterval), true); };
        }

        private async void SetLastSavedImage(string fileName)
        {
            var storageFile = await ApplicationData.Current.LocalFolder.TryGetItemAsync(fileName);
            if (storageFile == null)
                return;

            ChangeImage(new Uri(storageFile.Path), false);
        }

        private void DateTimeTimerOnTick(object sender, object o)
        {
            TimeText.Text = DateTime.Now.ToString(SettingsHelper.LongTimeFormat ? "HH:mm" : "t");
            DateText.Text = DateTime.Now.ToString("D");

            if (SettingsHelper.WorldTime)
            {
                Clock1Panel.Visibility = Clock2Panel.Visibility = Clock3Panel.Visibility = Visibility.Visible;

                Clock1Name.Text = SettingsHelper.Clock1Name;
                Clock1Text.Text = SettingsHelper.Clock1TimeZone.ConvertTime(new DateTimeOffset(DateTime.Now)).ToString(SettingsHelper.LongTimeFormat ? "HH:mm" : "t");
                Clock2Name.Text = SettingsHelper.Clock2Name;
                Clock2Text.Text = SettingsHelper.Clock2TimeZone.ConvertTime(new DateTimeOffset(DateTime.Now)).ToString(SettingsHelper.LongTimeFormat ? "HH:mm" : "t");
                Clock3Name.Text = SettingsHelper.Clock3Name;
                Clock3Text.Text = SettingsHelper.Clock3TimeZone.ConvertTime(new DateTimeOffset(DateTime.Now)).ToString(SettingsHelper.LongTimeFormat ? "HH:mm" : "t");
            }
            else
            {
                Clock1Panel.Visibility = Clock2Panel.Visibility = Clock3Panel.Visibility = Visibility.Collapsed;
            }
        }

        private void BackgroundTimerOnTick(object sender, object o)
        {
            ChangeImage(UriHelper.GetUri(SettingsHelper.Theme, SettingsHelper.UpdateInterval), true);
        }

        private async void ChangeImage(Uri uri, bool saveLastChangedTime)
        {
            await _semaphore.WaitAsync();
            try
            {
                if (saveLastChangedTime && !IsTimeToChange())
                {
                    return;
                }

                ProgressRing.IsActive = true;

                var source = await GetImageSource(uri);
                if (source == null)
                    return;

                Staging.Source = source;

                ImageFadeOut.Completed += (s, e) =>
                {
                    BackImage.Source = source;
                    ImageFadeIn.Begin();
                };
                ImageFadeOut.Begin();

                if (saveLastChangedTime)
                {
                    SettingsHelper.LastBackgroundChange = DateTime.UtcNow;
                }
            }
            finally
            {
                ProgressRing.IsActive = false;
                _semaphore.Release();
            }
        }

        private static bool IsTimeToChange()
        {
            var diff = DateTime.UtcNow - SettingsHelper.LastBackgroundChange;

            switch (SettingsHelper.UpdateInterval)
            {
                case "minute":
                    if (diff.TotalMinutes < 1)
                        return false;
                    break;

                case "hour":
                    if (diff.TotalHours < 1)
                        return false;
                    break;

                case "day":
                    if (diff.TotalDays < 1)
                        return false;
                    break;
            }
            return true;
        }

        private static async Task<ImageSource> GetImageSource(Uri uri)
        {
            try
            {
                if (uri.IsFile)
                    return new BitmapImage(uri);
                return await Downloader.DownloadImage(uri, LastFileName);
            }
            catch
            {
                return null;
            }
        }

        private void MainPage_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ChangeImage(UriHelper.GetUri(SettingsHelper.Theme, SettingsHelper.UpdateInterval), false);
        }

        private void MainPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            SizeChanged += MainPage_OnSizeChanged;
        }
    }
}
