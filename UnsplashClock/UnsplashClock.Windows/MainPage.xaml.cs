using System;
using Windows.Storage;
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

        public static bool IsLoadingBackground { get; set; }

        public MainPage()
        {
            InitializeComponent();

            SettingsHelper.LoadSettings();

            DateTimeTimerOnTick(null, null);
            var dateTimeTimer = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 0, 0, 100) };
            dateTimeTimer.Tick += DateTimeTimerOnTick;
            dateTimeTimer.Start();

            SetLastSavedImage(LastFileName);

            BackgroundTimerOnTick(this, null);
            var backgroundTimer = new DispatcherTimer {Interval = new TimeSpan(0, 0, 1) };
            backgroundTimer.Tick += BackgroundTimerOnTick;
            backgroundTimer.Start();

            SettingsPane.GetForCurrentView().CommandsRequested += (sender, args) =>
            {
                var settingsFlyout = new SettingsFlyout1();
                settingsFlyout.PropertyChanged += (o, eventArgs) => { SettingsHelper.UpdateSetting(eventArgs.PropertyName); };
                var setting = new SettingsCommand("ClockSettings", "Settings", handler => settingsFlyout.Show());
                args.Request.ApplicationCommands.Add(setting);
            };

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
        }

        private void BackgroundTimerOnTick(object sender, object o)
        {
            if(IsLoadingBackground)
                return;

            var diff = DateTime.UtcNow - SettingsHelper.LastBackgroundChange;

            switch (SettingsHelper.UpdateInterval)
            {
                case "minute":
                    if (diff.TotalMinutes < 1)
                        return;
                    break;

                case "hour":
                    if (diff.TotalHours < 1)
                        return;
                    break;

                case "day":
                    if (diff.TotalDays < 1)
                        return;
                    break;
            }

            ChangeImage(UriHelper.GetUri(SettingsHelper.Theme, SettingsHelper.UpdateInterval), true);
        }

        private async void ChangeImage(Uri uri, bool saveLastChangedTime)
        {
            IsLoadingBackground = true;
            ImageSource source;
            try
            {
                if(!uri.IsFile)
                    source = await Downloader.DownloadImage(uri, LastFileName);
                else
                    source = new BitmapImage(uri);
            }
            catch
            {
                return;
            }

            Staging.Source = source;

            ImageFadeOut.Completed += (s, e) =>
            {
                BackImage.Source = source;
                ImageFadeIn.Begin();
            };
            ImageFadeOut.Begin();

            if (!saveLastChangedTime)
            {
                IsLoadingBackground = false;
                return;
            }
            
            SettingsHelper.LastBackgroundChange = DateTime.UtcNow;
            IsLoadingBackground = false;
        }
    }
}
