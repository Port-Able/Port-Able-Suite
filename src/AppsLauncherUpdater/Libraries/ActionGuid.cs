namespace Updater.Libraries
{
    using System;
    using System.Linq;
    using SilDev;

    internal struct ActionGuid
    {
        internal static readonly string CurrentAction = $"{{{Guid.NewGuid()}}}";
        internal const string QuietUpdate = "{72D58BA5-E612-453E-9AEF-ED7C56608C04}";
        internal const string RepairDirs = "{48FDE635-60E6-41B5-8F9D-674E9F535AC7}";

        internal static bool IsQuietUpdate =>
            IsActive(QuietUpdate);

        private static bool IsActive(string guid)
        {
            try
            {
                var args = Environment.GetCommandLineArgs().Skip(1);
                return args.Contains(guid);
            }
            catch (Exception ex) when (ex.IsCaught())
            {
                return false;
            }
        }
    }
}
