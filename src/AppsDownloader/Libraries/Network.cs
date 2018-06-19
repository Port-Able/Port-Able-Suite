namespace AppsDownloader.Libraries
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Windows.Forms;
    using LangResources;
    using SilDev;
    using SilDev.Forms;

    internal static class Network
    {
        private static bool? _internetIsAvailable, _ipv4IsAvalaible, _ipv6IsAvalaible;

        internal static bool InternetIsAvailable
        {
            get
            {
                if (!_internetIsAvailable.HasValue)
                    _internetIsAvailable = IPv4IsAvalaible;
                if (_internetIsAvailable == true)
                    return (bool)_internetIsAvailable;
                _internetIsAvailable = IPv6IsAvalaible;
                if (_internetIsAvailable == true)
                    MessageBoxEx.Show(Language.GetText(nameof(en_US.InternetProtocolWarningMsg)), Settings.Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return (bool)_internetIsAvailable;
            }
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        internal static bool IPv4IsAvalaible
        {
            get
            {
                if (!_ipv4IsAvalaible.HasValue)
                    _ipv4IsAvalaible = NetEx.InternetIsAvailable();
                return (bool)_ipv4IsAvalaible;
            }
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        internal static bool IPv6IsAvalaible
        {
            get
            {
                if (!_ipv6IsAvalaible.HasValue)
                    _ipv6IsAvalaible = NetEx.InternetIsAvailable(true);
                return (bool)_ipv6IsAvalaible;
            }
        }

        internal static string GetFullHost(this string url)
        {
            try
            {
                var uri = new Uri(url);
                return uri.Host.ToLower();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
            return null;
        }

        internal static string GetShortHost(this string url)
        {
            var host = url.GetFullHost();
            if (string.IsNullOrEmpty(host))
                return null;
            if (host.Count(x => x == '.') > 1)
                host = host.Split('.').TakeLast(2).Join('.');
            return host;
        }
    }
}
