namespace PortAble;

using System;
using System.Linq;
using SilDev;

/// <summary>
///     Provides GUID’s that run actions on the current instance or enable new
///     instances to run.
/// </summary>
public struct ActionGuid
{
    /// <summary>
    ///     Used to allow more than one instance for the current process.
    /// </summary>
    public const string AllowNewInstance = "{0CA7046C-4776-4DB0-913B-D8F81964F8EE}";

    /// <summary>
    ///     Used to explicit disallow more than one instance for the current process.
    /// </summary>
    public const string DisallowInterface = "{9AB50CEB-3D99-404E-BD31-4E635C09AF0F}";

    /// <summary>
    ///     Used to apply the system integration feature.
    /// </summary>
    public const string SystemIntegration = "{3A51735E-7908-4DF5-966A-9CA7626E4E3D}";

    /// <summary>
    ///     Used to apply file type association of an app.
    /// </summary>
    public const string FileTypeAssociation = "{DF8AB31C-1BC0-4EC1-BEC0-9A17266CAEFC}";

    /// <summary>
    ///     Used to apply file type association for all apps.
    /// </summary>
    public const string FileTypeAssociationAll = "{72780D0A-2A5F-4712-928D-EAF79974FFDB}";

    /// <summary>
    ///     Used to restore the app`s previous file types before the association task
    ///     was started.
    /// </summary>
    public const string RestoreFileTypes = "{A00C02E5-283A-44ED-9E4D-B82E8F87318F}";

    /// <summary>
    ///     Used to launch recovery tasks for the entire apps suite.
    /// </summary>
    public const string RepairAppsSuite = "{FB271A84-B5A3-47DA-A873-9CE946A64531}";

    /// <summary>
    ///     Used to launch recovery tasks for the apps suite directories.
    /// </summary>
    public const string RepairDirs = "{48FDE635-60E6-41B5-8F9D-674E9F535AC7}";

    /// <summary>
    ///     Used to launch recovery tasks for the apps suite environment variable.
    /// </summary>
    public const string RepairVariable = "{EA48C7DB-AD36-43D7-80A1-D6E81FB8BCAB}";

    /// <summary>
    ///     Used to launch an `AppsDownloader` update instance.
    /// </summary>
    public const string UpdateInstance = "{F92DAD88-DA45-405A-B0EB-10A1E9B2ADDD}";

    /// <summary>
    ///     Used to start any necessary repair tasks when a new app suite version is
    ///     detected.
    /// </summary>
    public const string VersionValidation = "{C037A1FA-F2D5-4B7C-8438-F5953254884A}";

    /// <summary>
    ///     Determines whether more than one instance is allowed for the current
    ///     process.
    /// </summary>
    public static bool IsAllowNewInstance => IsActive(nameof(AllowNewInstance), AllowNewInstance);

    /// <summary>
    ///     Determines whether more than one instance is explicitly disallowed for the
    ///     current process.
    /// </summary>
    public static bool IsDisallowInterface => IsActive(nameof(DisallowInterface), DisallowInterface);

    /// <summary>
    ///     Determines whether to apply the system integration feature.
    /// </summary>
    public static bool IsSystemIntegration => IsActive(nameof(SystemIntegration), SystemIntegration);

    /// <summary>
    ///     Determines whether to apply the file type association for a specified app.
    /// </summary>
    public static bool IsFileTypeAssociation => IsActive(nameof(FileTypeAssociation), FileTypeAssociation);

    /// <summary>
    ///     Determines whether to apply the file type association for all apps.
    /// </summary>
    public static bool IsFileTypeAssociationAll => IsActive(nameof(FileTypeAssociationAll), FileTypeAssociationAll);

    /// <summary>
    ///     Determines whether to restore the app`s previous file types before the
    ///     association task was started.
    /// </summary>
    public static bool IsRestoreFileTypes => IsActive(nameof(RestoreFileTypes), RestoreFileTypes);

    /// <summary>
    ///     Determines whether to launch recovery tasks for the entire apps suite.
    /// </summary>
    public static bool IsRepairAppsSuite => IsActive(nameof(RepairAppsSuite), RepairAppsSuite);

    /// <summary>
    ///     Determines whether to launch recovery tasks for the apps suite directories.
    /// </summary>
    public static bool IsRepairDirs => IsActive(nameof(RepairDirs), RepairDirs);

    /// <summary>
    ///     Determines whether to launch recovery tasks for the apps suite environment
    ///     variable.
    /// </summary>
    public static bool IsRepairVariable => IsActive(nameof(RepairVariable), RepairVariable);

    /// <summary>
    ///     Determines to launch an `AppsDownloader` update instance.
    /// </summary>
    public static bool IsUpdateInstance => IsActive(nameof(UpdateInstance), UpdateInstance);

    /// <summary>
    ///     Determines whether to start any necessary repair tasks when a new app suite
    ///     version is detected.
    /// </summary>
    public static bool IsVersionValidation => IsActive(nameof(VersionValidation), VersionValidation);

    private static bool IsActive(string name, string guid)
    {
        var args = Environment.GetCommandLineArgs().Skip(1);
        return args.Any(x => x.EqualsEx($"/{name}", guid));
    }
}