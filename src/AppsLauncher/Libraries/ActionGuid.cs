namespace AppsLauncher.Libraries
{
    using System;
    using System.Linq;

    internal struct ActionGuid
    {
        internal const string AllowNewInstance = "{0CA7046C-4776-4DB0-913B-D8F81964F8EE}";
        internal const string DisallowInterface = "{9AB50CEB-3D99-404E-BD31-4E635C09AF0F}";
        internal const string SystemIntegration = "{3A51735E-7908-4DF5-966A-9CA7626E4E3D}";
        internal const string FileTypeAssociation = "{DF8AB31C-1BC0-4EC1-BEC0-9A17266CAEFC}";
        internal const string FileTypeAssociationAll = "{72780D0A-2A5F-4712-928D-EAF79974FFDB}";
        internal const string RestoreFileTypes = "{A00C02E5-283A-44ED-9E4D-B82E8F87318F}";
        internal const string RepairAppsSuite = "{FB271A84-B5A3-47DA-A873-9CE946A64531}";
        internal const string RepairDirs = "{48FDE635-60E6-41B5-8F9D-674E9F535AC7}";
        internal const string RepairVariable = "{EA48C7DB-AD36-43D7-80A1-D6E81FB8BCAB}";
        internal const string UpdateInstance = "{F92DAD88-DA45-405A-B0EB-10A1E9B2ADDD}";

        internal static bool IsAllowNewInstance =>
            IsActive(AllowNewInstance);

        internal static bool IsDisallowInterface =>
            IsActive(DisallowInterface);

        internal static bool IsSystemIntegration =>
            IsActive(SystemIntegration);

        internal static bool IsFileTypeAssociation =>
            IsActive(FileTypeAssociation);

        internal static bool IsFileTypeAssociationAll =>
            IsActive(FileTypeAssociationAll);

        internal static bool IsRestoreFileTypes =>
            IsActive(RestoreFileTypes);

        internal static bool IsRepairAppsSuite =>
            IsActive(RepairAppsSuite);

        internal static bool IsRepairDirs =>
            IsActive(RepairDirs);

        internal static bool IsRepairVariable =>
            IsActive(RepairVariable);

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
