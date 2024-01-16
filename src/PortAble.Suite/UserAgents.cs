namespace PortAble;

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SilDev;

/// <summary>
///     Sets the browser application.
/// </summary>
public enum UserAgentBrowser
{
    /// <summary>
    ///     Google Chrome.
    /// </summary>
    Chrome,

    /// <summary>
    ///     Microsoft Edge.
    /// </summary>
    Edge,

    /// <summary>
    ///     Mozilla Firefox.
    /// </summary>
    Firefox
}

/// <summary>
///     Sets the operating system, or platform.
/// </summary>
public enum UserAgentPlatform
{
    /// <summary>
    ///     Current running system.
    /// </summary>
    Auto,

    /// <summary>
    ///     Windows NT.
    /// </summary>
    Windows,

    /// <summary>
    ///     Mac OS X.
    /// </summary>
    Linux
}

/// <summary>
///     Provides different fake user agents for different use cases.
/// </summary>
public static class UserAgents
{
    private static string _browser, _pa;

    /// <summary>
    ///     Windows Edge browser.
    /// </summary>
    public static string Browser => _browser ??= CreateFakeBrowserAgent();

    /// <summary>
    ///     Equal to <see cref="string.Empty"/>.
    /// </summary>
    public static string Empty => string.Empty;

    /// <summary>
    ///     Port-Able apps suite agent.
    /// </summary>
    public static string Pa
    {
        get
        {
            var realOsVersion = EnvironmentEx.OperatingSystemVersion;
            return _pa ??= $"Wget/{EnvironmentEx.Version} (Windows NT/{realOsVersion.Major}.{realOsVersion.Minor}.{realOsVersion.Build}.{realOsVersion.Revision}; {(Environment.Is64BitOperatingSystem ? "Win64; x64" : "Win32; x86")}) {AssemblyInfo.Product}/{AssemblyInfo.Version}";
        }
    }

    /// <summary>
    ///     PortableApps.com platform agent.
    /// </summary>
    public static string Pac => "PortableApps.comPlatform/26.3";

    /// <summary>
    ///     GNU Wget agent.
    /// </summary>
    public static string Wget => "Wget/1.21.4";

    /// <summary>
    ///     Creates a fake browser user agent.
    /// </summary>
    /// <param name="platform">
    ///     The agent`s platform.
    /// </param>
    /// <param name="webBrowser">
    ///     The agent`s browser.
    /// </param>
    /// <returns>
    ///     A user agent string that mimics an original pc browser agent.
    /// </returns>
    public static string CreateFakeBrowserAgent(UserAgentPlatform platform = UserAgentPlatform.Auto, UserAgentBrowser webBrowser = UserAgentBrowser.Chrome)
    {
        var osVersion = Environment.OSVersion;
        var osIsX64 = Environment.Is64BitOperatingSystem;

        if (platform == UserAgentPlatform.Auto)
            platform = osVersion.Platform switch
            {
                PlatformID.Unix => UserAgentPlatform.Linux,
                _ => UserAgentPlatform.Windows
            };

        var system = platform switch
        {
            UserAgentPlatform.Linux => $"X11; Linux {(osIsX64 ? "x86_64" : "i686")}",
            _ => $"Windows NT {osVersion.Version.Major}.{osVersion.Version.Minor}; {(osIsX64 ? "Win64; x64" : "WOW64")}"
        };

        var version = default(Version);
        try
        {
            var dir = PathEx.Combine(osIsX64 ? "%ProgramFiles(x86)%" : "%ProgramFiles%", "Microsoft");
            var path = PathEx.Combine(dir, "Edge\\Application\\msedge.exe");
            if (File.Exists(path))
                version = Version.Parse(FileVersionInfo.GetVersionInfo(path).FileVersion);
            else
            {
                path = Path.Combine(dir, "EdgeWebView\\Application");
                version = Version.Parse(Directory.EnumerateDirectories(path).Select(Path.GetFileName).Last(s => char.IsDigit(s[0])));
            }
        }
        catch (Exception ex) when (ex.IsCaught())
        {
            Log.Write(ex);
        }
        if (version?.Major is null or < 120)
            version = Version.Parse("120.0.0.0");

        string browser;
        if (webBrowser != UserAgentBrowser.Firefox)
            browser = $"AppleWebKit/537.36 (KHTML, like Gecko) Chrome/{version.Major}.0.0.0 Safari/537.36";
        else
        {
            system += $"; rv:{version.Major}.0";
            browser = $"Gecko/20100101 Firefox/{version.Major}.0";
        }
        if (webBrowser == UserAgentBrowser.Edge)
            browser += $" Edg/{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";

        return $"Mozilla/5.0 ({system}) {browser}";
    }
}