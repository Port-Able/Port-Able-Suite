namespace PortAble.Model;

/// <summary>
///     Provides app information of installed apps.
/// </summary>
public interface IAppData
{
    /// <summary>
    ///     The app key.
    /// </summary>
    string Key { get; }

    /// <summary>
    ///     The app name.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     The app description.
    /// </summary>
    string Description { get; }

    /// <summary>
    ///     The app category.
    /// </summary>
    string Category { get; }

    /// <summary>
    ///     The default language, used only when more than one language is available
    ///     for download.
    /// </summary>
    string DefaultLanguage { get; }

    /// <summary>
    ///     The app install location.
    /// </summary>
    string InstallDir { get; }

    /// <summary>
    ///     The local settings for this app.
    /// </summary>
    AppSettings Settings { get; }
}
