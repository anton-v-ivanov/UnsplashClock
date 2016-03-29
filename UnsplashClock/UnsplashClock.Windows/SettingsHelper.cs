using System;
using System.Globalization;
using Windows.Storage;
using TimeZones;

namespace UnsplashClock
{
    public class SettingsHelper
    {
        private static DateTime _lastBackgroundChange;

        public static bool LongTimeFormat { get; private set; }
        public static string Theme { get; private set; }
        public static string UpdateInterval { get; private set; }
        public static bool WorldTime { get; set; }
        public static string Clock1Name { get; set; }
        public static ITimeZoneEx Clock1TimeZone { get; set; }
        public static string Clock2Name { get; set; }
        public static ITimeZoneEx Clock2TimeZone { get; set; }
        public static string Clock3Name { get; set; }
        public static ITimeZoneEx Clock3TimeZone { get; set; }

        public static DateTime LastBackgroundChange
        {
            get
            {
                if (!_lastBackgroundChange.Equals(DateTime.MinValue))
                    return _lastBackgroundChange;

                var localSettings = ApplicationData.Current.LocalSettings;
                try
                {
                    var lastChanged = localSettings.Values["lastChanged"] == null
                        ? DateTime.UtcNow
                        : DateTime.ParseExact(Convert.ToString(localSettings.Values["lastChanged"]), "o", CultureInfo.InvariantCulture, DateTimeStyles.None);
                    _lastBackgroundChange = lastChanged.ToUniversalTime();
                }
                catch (Exception)
                {
                    _lastBackgroundChange = DateTime.UtcNow;
                }
                return _lastBackgroundChange;
            }
            set
            {
                _lastBackgroundChange = value;
                var localSettings = ApplicationData.Current.LocalSettings;
                localSettings.Values["lastChanged"] = LastBackgroundChange.ToString("o");
            }
        }

        public delegate void ThemeChangedHandler(string theme);

        public static event ThemeChangedHandler OnThemeChanged;
        
        public static void LoadSettings()
        {
            UpdateSetting("timeFormat");
            UpdateSetting("theme");
            UpdateSetting("updInterval");
            UpdateSetting("worldTime");
            UpdateSetting("clock1Name");
            UpdateSetting("clock1TimeZone");
            UpdateSetting("clock2Name");
            UpdateSetting("clock2TimeZone");
            UpdateSetting("clock3Name");
            UpdateSetting("clock3TimeZone");
        }

        public static void UpdateSetting(string settingName)
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            switch (settingName)
            {
                case "timeFormat":
                    var longTimeFormat = localSettings.Values["24hFormat"];
                    if (longTimeFormat != null)
                        LongTimeFormat = (bool)longTimeFormat;
                    else
                        LongTimeFormat = true;
                    break;
                case "theme":
                    var theme = (string)localSettings.Values["theme"];
                    Theme = !string.IsNullOrEmpty(theme) ? theme : "nature";
                    RaiseOnThemeChanged(Theme);
                    break;
                case "updInterval":
                    var updInterval = (string)localSettings.Values["updInterval"];
                    UpdateInterval = !string.IsNullOrEmpty(updInterval) ? updInterval : "hour";
                    break;
                case "worldTime":
                    var worldTime = localSettings.Values["worldTime"];
                    if (worldTime != null)
                        WorldTime = (bool)worldTime;
                    else
                        WorldTime = true;
                    break;
                case "clock1Name":
                    var clock1Name = (string)localSettings.Values["clock1Name"];
                    Clock1Name = !string.IsNullOrEmpty(clock1Name) ? clock1Name : "New York";
                    break;
                case "clock1TimeZone":
                    var clock1TimeZone = (string)localSettings.Values["clock1TimeZone"];
                    Clock1TimeZone = TimeZoneService.FindSystemTimeZoneById(!string.IsNullOrEmpty(clock1TimeZone) ? clock1TimeZone : "Eastern Standard Time");
                    break;
                case "clock2Name":
                    var clock2Name = (string)localSettings.Values["clock2Name"];
                    Clock2Name = !string.IsNullOrEmpty(clock2Name) ? clock2Name : "Los Angeles";
                    break;
                case "clock2TimeZone":
                    var clock2TimeZone = (string)localSettings.Values["clock2TimeZone"];
                    Clock2TimeZone = TimeZoneService.FindSystemTimeZoneById(!string.IsNullOrEmpty(clock2TimeZone) ? clock2TimeZone : "Pacific Standard Time");
                    break;
                case "clock3Name":
                    var clock3Name = (string)localSettings.Values["clock3Name"];
                    Clock3Name = !string.IsNullOrEmpty(clock3Name) ? clock3Name : "Hong Kong";
                    break;
                case "clock3TimeZone":
                    var clock3TimeZone = (string)localSettings.Values["clock3TimeZone"];
                    Clock3TimeZone = TimeZoneService.FindSystemTimeZoneById(!string.IsNullOrEmpty(clock3TimeZone) ? clock3TimeZone : "China Standard Time");
                    break;
            }
        }

        protected static void RaiseOnThemeChanged(string theme)
        {
            OnThemeChanged?.Invoke(theme);
        }

        public static void SaveSetting(string settingName, object value)
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            switch (settingName)
            {
                case "timeFormat":
                    if (LongTimeFormat != (bool)value)
                    {
                        localSettings.Values["24hFormat"] = (bool)value;
                        LongTimeFormat = (bool)value;
                    }
                    break;
                case "theme":
                    if (!Theme.Equals(value))
                    {
                        localSettings.Values["theme"] = Convert.ToString(value);
                        Theme = Convert.ToString(value);
                        RaiseOnThemeChanged(Theme);
                    }
                    break;
                case "updInterval":
                    if (!UpdateInterval.Equals(Convert.ToString(value)))
                    {
                        localSettings.Values["updInterval"] = Convert.ToString(value);
                        UpdateInterval = Convert.ToString(value);
                    }
                    break;
                case "worldTime":
                    if (WorldTime != (bool) value)
                    {
                        localSettings.Values["worldTime"] = (bool) value;
                        WorldTime = (bool) value;
                    }
                    break;
                case "clock1Name":
                    if (!Clock1Name.Equals(Convert.ToString(value)))
                    {
                        localSettings.Values["clock1Name"] = Convert.ToString(value);
                        Clock1Name = Convert.ToString(value);
                    }
                    break;
                case "clock1TimeZone":
                    if (!Clock1TimeZone.Id.Equals(Convert.ToString(value)))
                    {
                        localSettings.Values["clock1TimeZone"] = Convert.ToString(value);
                        Clock1TimeZone = TimeZoneService.FindSystemTimeZoneById(Convert.ToString(value));
                    }
                    break;
                case "clock2Name":
                    if (!Clock2Name.Equals(Convert.ToString(value)))
                    {
                        localSettings.Values["clock2Name"] = Convert.ToString(value);
                        Clock2Name = Convert.ToString(value);
                    }
                    break;
                case "clock2TimeZone":
                    if (!Clock2TimeZone.Id.Equals(Convert.ToString(value)))
                    {
                        localSettings.Values["clock2TimeZone"] = Convert.ToString(value);
                        Clock2TimeZone = TimeZoneService.FindSystemTimeZoneById(Convert.ToString(value));
                    }
                    break;
                case "clock3Name":
                    if (!Clock3Name.Equals(Convert.ToString(value)))
                    {
                        localSettings.Values["clock3Name"] = Convert.ToString(value);
                        Clock3Name = Convert.ToString(value);
                    }
                    break;
                case "clock3TimeZone":
                    if (!Clock3TimeZone.Id.Equals(Convert.ToString(value)))
                    {
                        localSettings.Values["clock3TimeZone"] = Convert.ToString(value);
                        Clock3TimeZone = TimeZoneService.FindSystemTimeZoneById(Convert.ToString(value));
                    }
                    break;
            }
        }
    }
}