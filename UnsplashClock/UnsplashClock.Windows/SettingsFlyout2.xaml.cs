using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using TimeZones;

namespace UnsplashClock
{
    public sealed partial class SettingsFlyout2 : SettingsFlyout
    {
        public SettingsFlyout2()
        {
            InitializeComponent();
        }

        private void SettingsFlyout_Loaded(object sender, RoutedEventArgs routedEventArgs)
        {
            WorldClock.IsOn = SettingsHelper.WorldTime;

            foreach (var timeZone in TimeZoneService.AllTimeZones.OrderBy(s => s.BaseUtcOffset))
            {
                var offsetString = timeZone.BaseUtcOffset.TotalMinutes > 0
                    ? timeZone.BaseUtcOffset.ToString(@"\+hh\:mm", System.Globalization.CultureInfo.InvariantCulture)
                    : timeZone.BaseUtcOffset.ToString(@"\-hh\:mm", System.Globalization.CultureInfo.InvariantCulture);

                Clock1TimeZone.Items.Add(new ComboBoxItem { Content = $"UTC {offsetString} {timeZone.StandardName}", Tag = timeZone.Id });
                Clock2TimeZone.Items.Add(new ComboBoxItem { Content = $"UTC {offsetString} {timeZone.StandardName}", Tag = timeZone.Id });
                Clock3TimeZone.Items.Add(new ComboBoxItem { Content = $"UTC {offsetString} {timeZone.StandardName}", Tag = timeZone.Id });
            }


            Clock1Name.Text = SettingsHelper.Clock1Name;
            foreach (var item in Clock1TimeZone.Items.Cast<ComboBoxItem>().Where(item => (string)item.Tag == SettingsHelper.Clock1TimeZone.Id))
            {
                Clock1TimeZone.SelectedItem = item;
            }

            Clock2Name.Text = SettingsHelper.Clock2Name;
            foreach (var item in Clock2TimeZone.Items.Cast<ComboBoxItem>().Where(item => (string)item.Tag == SettingsHelper.Clock2TimeZone.Id))
            {
                Clock2TimeZone.SelectedItem = item;
            }

            Clock3Name.Text = SettingsHelper.Clock3Name;
            foreach (var item in Clock3TimeZone.Items.Cast<ComboBoxItem>().Where(item => (string)item.Tag == SettingsHelper.Clock3TimeZone.Id))
            {
                Clock3TimeZone.SelectedItem = item;
            }
        }

        private void WorldClock_Toggled(object sender, RoutedEventArgs e)
        {
            if (WorldClock == null)
                return;

            SettingsHelper.SaveSetting("worldTime", WorldClock.IsOn);
            Clock1Name.IsEnabled = WorldClock.IsOn;
            Clock2Name.IsEnabled = WorldClock.IsOn;
            Clock3Name.IsEnabled = WorldClock.IsOn;
            Clock1TimeZone.IsEnabled = WorldClock.IsOn;
            Clock2TimeZone.IsEnabled = WorldClock.IsOn;
            Clock3TimeZone.IsEnabled = WorldClock.IsOn;
        }

        private void Clock1Name_LostFocus(object sender, RoutedEventArgs e)
        {
            SettingsHelper.SaveSetting("clock1Name", Clock1Name.Text);
        }

        private void Clock1TimeZone_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SettingsHelper.SaveSetting("clock1TimeZone", ((ComboBoxItem)Clock1TimeZone.SelectedItem).Tag);
        }

        private void Clock2Name_LostFocus(object sender, RoutedEventArgs e)
        {
            SettingsHelper.SaveSetting("clock2Name", Clock2Name.Text);
        }

        private void Clock2TimeZone_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SettingsHelper.SaveSetting("clock2TimeZone", ((ComboBoxItem)Clock2TimeZone.SelectedItem).Tag);
        }

        private void Clock3Name_LostFocus(object sender, RoutedEventArgs e)
        {
            SettingsHelper.SaveSetting("clock3Name", Clock3Name.Text);
        }

        private void Clock3TimeZone_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SettingsHelper.SaveSetting("clock3TimeZone", ((ComboBoxItem)Clock3TimeZone.SelectedItem).Tag);
        }
    }
}
