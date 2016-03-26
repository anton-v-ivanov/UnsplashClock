using System;
using System.Globalization;
using Windows.Storage;

namespace UnsplashClock
{
    public class SettingsHelper
    {
        private static DateTime _lastBackgroundChange;

        public static bool LongTimeFormat { get; private set; }
        public static string Theme { get; private set; }
        public static string UpdateInterval { get; private set; }
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
            var localSettings = ApplicationData.Current.LocalSettings;

            var longTimeFormat = localSettings.Values["24hFormat"];
            LongTimeFormat = longTimeFormat == null || (bool)longTimeFormat;

            var theme = (string)localSettings.Values["theme"];
            Theme = !string.IsNullOrEmpty(theme) ? theme : "nature";

            var updInterval = (string)localSettings.Values["updInterval"];
            UpdateInterval = !string.IsNullOrEmpty(updInterval) ? updInterval : "hour";
        }

        public static void UpdateSetting(string settingName)
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            if (settingName == "timeFormat")
            {
                var longTimeFormat = localSettings.Values["24hFormat"];
                if (longTimeFormat != null)
                    LongTimeFormat = (bool)longTimeFormat;
                else
                    LongTimeFormat = true;
            }
            if (settingName == "theme")
            {
                var theme = (string)localSettings.Values["theme"];
                Theme = !string.IsNullOrEmpty(theme) ? theme : "nature";
                RaiseOnThemeChanged(Theme);
            }

            if (settingName == "updInterval")
            {
                var updInterval = (string)localSettings.Values["updInterval"];
                UpdateInterval = !string.IsNullOrEmpty(updInterval) ? updInterval : "hour";
            }
        }

        protected static void RaiseOnThemeChanged(string theme)
        {
            OnThemeChanged?.Invoke(theme);
        }
    }
}