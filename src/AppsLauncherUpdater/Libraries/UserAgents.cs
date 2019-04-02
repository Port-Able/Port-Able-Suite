namespace Updater.Libraries
{
    using System;
    using SilDev;

    internal static class UserAgents
    {
        internal static string Default { get; } = "Wget/1.16 (linux-gnu)";

        internal static string Internal { get; } = $"Wget/{EnvironmentEx.Version} (Windows NT/{Environment.OSVersion.Version.Major}.{Environment.OSVersion.Version.Minor}; {(Environment.Is64BitOperatingSystem ? "Win64; x64" : "Win32; x86")}) {AssemblyInfo.Product}/{AssemblyInfo.Version}";
    }
}
