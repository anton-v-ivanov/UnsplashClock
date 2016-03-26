using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using UnsplashClock.Annotations;

// The Settings Flyout item template is documented at http://go.microsoft.com/fwlink/?LinkId=273769

namespace UnsplashClock
{
    public sealed partial class SettingsFlyout1 : SettingsFlyout, INotifyPropertyChanged
    {
        public SettingsFlyout1()
        {
            this.InitializeComponent();
        }

        private void TimeFormatToggle_Toggled(object sender, RoutedEventArgs e)
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            if (TimeFormatToggle == null)
                return;

            var prevValue = (bool?)localSettings.Values["24hFormat"];
            localSettings.Values["24hFormat"] = TimeFormatToggle.IsOn;
            if(prevValue.HasValue && prevValue.Value != TimeFormatToggle.IsOn)
                OnPropertyChanged("timeFormat");
        }

        private void ThemeSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            if (ThemeSelect == null)
                return;

            var prevValue = (string)localSettings.Values["theme"];
            var curValue = ((ComboBoxItem) ThemeSelect.SelectedItem)?.Content;
            localSettings.Values["theme"] = curValue;
            if (prevValue != null && !prevValue.Equals(curValue))
                OnPropertyChanged("theme");
        }

        private void UpdateInterval_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            if (UpdateInterval == null)
                return;

            var prevValue = (string)localSettings.Values["updInterval"];
            var curValue = ((ComboBoxItem)UpdateInterval.SelectedItem)?.Content;
            localSettings.Values["updInterval"] = curValue;
            if (prevValue != null && !prevValue.Equals(curValue))
                OnPropertyChanged("updInterval");
        }

        private void SettingsFlyout_Loaded(object sender, RoutedEventArgs e)
        {
            var localSettings = ApplicationData.Current.LocalSettings;

            if (localSettings.Values["24hFormat"] != null)
                TimeFormatToggle.IsOn = (bool)localSettings.Values["24hFormat"];

            if (localSettings.Values["theme"] != null)
            {
                var theme = (string)localSettings.Values["theme"];
                foreach (var item in ThemeSelect.Items.Cast<ComboBoxItem>().Where(item => (string)item.Content == theme))
                {
                    ThemeSelect.SelectedItem = item;
                }
            }

            if (localSettings.Values["updInterval"] != null)
            {
                var theme = (string)localSettings.Values["updInterval"];
                foreach (var item in UpdateInterval.Items.Cast<ComboBoxItem>().Where(item => (string)item.Content == theme))
                {
                    UpdateInterval.SelectedItem = item;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
