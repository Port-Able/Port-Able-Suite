namespace PortAble
{
    using System;
    using SilDev;

    /// <summary>
    ///     Provides different user agents for different use cases.
    /// </summary>
    public static class UserAgents
    {
        /// <summary>
        ///     Windows Edge browser.
        /// </summary>
        public static string Browser { get; } = $"Mozilla/5.0 (Windows NT 10.0; {(Environment.Is64BitOperatingSystem ? "Win64; x64" : "WOW64")}) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36 Edg/119.0.0.0";

        /// <summary>
        ///     Equal to <see cref="string.Empty"/>.
        /// </summary>
        public static string Empty => string.Empty;

        /// <summary>
        ///     Port-Able suite agent.
        /// </summary>
        public static string Pa { get; } = $"Wget/{EnvironmentEx.Version} (Windows NT/{Environment.OSVersion.Version.Major}.{Environment.OSVersion.Version.Minor}; {(Environment.Is64BitOperatingSystem ? "Win64; x64" : "Win32; x86")}) {AssemblyInfo.Product}/{AssemblyInfo.Version}";

        /// <summary>
        ///     PortableApps.com platform agent.
        /// </summary>
        public static string Pac => "PortableApps.comPlatform/26.3";

        /// <summary>
        ///     GNU Wget agent.
        /// </summary>
        public static string Wget { get; } = $"Wget/1.{DateTime.Now:yy.M}";
    }
}
