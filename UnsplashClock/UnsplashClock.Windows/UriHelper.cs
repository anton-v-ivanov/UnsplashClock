using System;
using Windows.UI.Xaml;

namespace UnsplashClock
{
    internal class UriHelper
    {
        public static Uri GetUri(string theme, string updateInterval)
        {
            var bounds = Window.Current.Bounds;
            var uri = new Uri($"http://source.unsplash.com/category/{theme}/{bounds.Width}x{bounds.Height}", UriKind.Absolute);
            if (updateInterval.Equals("day"))
                uri = new Uri($"http://source.unsplash.com/category/{theme}/{bounds.Width}x{bounds.Height}/daily", UriKind.Absolute);
            return uri;
        }
    }
}