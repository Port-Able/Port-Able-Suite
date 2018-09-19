namespace Updater.Libraries
{
    using System.Diagnostics.CodeAnalysis;
    using SilDev;

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
    }
}
