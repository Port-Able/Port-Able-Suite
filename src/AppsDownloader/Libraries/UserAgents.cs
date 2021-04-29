namespace AppsDownloader.Libraries
{
    using System;
    using SilDev;

    internal static class UserAgents
    {
        internal static string Default { get; } = $"Wget/1.{DateTime.Now:yy.M}";

        internal static string Empty => string.Empty;

        internal static string Internal { get; } = $"Wget/{EnvironmentEx.Version} (Windows NT/{Environment.OSVersion.Version.Major}.{Environment.OSVersion.Version.Minor}; {(Environment.Is64BitOperatingSystem ? "Win64; x64" : "Win32; x86")}) {AssemblyInfo.Product}/{AssemblyInfo.Version}";

        internal static string WindowsChrome { get; } = $"Mozilla/5.0 (Windows NT 10.0; {(Environment.Is64BitOperatingSystem ? "Win64; x64" : "WOW64")}) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.85 Safari/537.36";
    }
}
