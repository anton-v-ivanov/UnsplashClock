using System.Linq;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Common
{
    public sealed partial class SettingsFlyout1 : SettingsFlyout
    {
        public SettingsFlyout1()
        {
            InitializeComponent();
        }

        private void TimeFormatToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (TimeFormatToggle != null)
                SettingsHelper.SaveSetting("timeFormat", TimeFormatToggle.IsOn);
        }

        private void ThemeSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ThemeSelect != null)
                SettingsHelper.SaveSetting("theme", ((ComboBoxItem)ThemeSelect.SelectedItem).Content);
        }

        private void UpdateInterval_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UpdateInterval != null)
                SettingsHelper.SaveSetting("updInterval", ((ComboBoxItem)UpdateInterval.SelectedItem).Content);
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
    }
}
