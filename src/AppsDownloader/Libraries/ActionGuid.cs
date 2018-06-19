namespace AppsDownloader.Libraries
{
    using System;
    using System.Linq;

    internal struct ActionGuid
    {
        internal const string RepairAppsSuite = "{FB271A84-B5A3-47DA-A873-9CE946A64531}";
        internal const string RepairDirs = "{48FDE635-60E6-41B5-8F9D-674E9F535AC7}";
        internal const string UpdateInstance = "{F92DAD88-DA45-405A-B0EB-10A1E9B2ADDD}";

        internal static bool IsUpdateInstance =>
            IsActive(UpdateInstance);

        private static bool IsActive(string guid)
        {
            try
            {
                var args = Environment.GetCommandLineArgs().Skip(1);
                return args.Contains(guid);
            }
            catch
            {
                return false;
            }
        }
    }
}
