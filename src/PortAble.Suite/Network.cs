namespace PortAble
{
    using System.Windows.Forms;
    using Properties;
    using SilDev;
    using SilDev.Forms;
    using SilDev.Network;

    /// <summary>
    ///     Provides information about the current Internet connection.
    /// </summary>
    public static class Network
    {
        private static bool? _internetIsAvailable;

        /// <summary>
        ///     Determines whether an internet connection is available.
        /// </summary>
        public static bool InternetIsAvailable
        {
            get
            {
                _internetIsAvailable ??= NetEx.IPv4IsAvalaible;
                if (_internetIsAvailable == true)
                    return (bool)_internetIsAvailable;
                _internetIsAvailable = NetEx.IPv6IsAvalaible;
                if (_internetIsAvailable == true)
                    MessageBoxEx.Show(LangStrings.InternetProtocolWarningMsg, AssemblyInfo.Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return (bool)_internetIsAvailable;
            }
        }
    }
}
