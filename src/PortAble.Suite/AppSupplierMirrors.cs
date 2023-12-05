namespace PortAble
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using SilDev;
    using SilDev.Network;

    /// <summary>
    ///     Provides server addresses and location information from official app
    ///     suppliers.
    /// </summary>
    public static class AppSupplierMirrors
    {
        private static Dictionary<string, Dictionary<string, string>> _coreAppSuppliers;

        /// <summary>
        ///     Gets a dictionary with the <see cref="AppSupplierHosts.Pa"/> mirror
        ///     addresses as the key and their location as the value.
        /// </summary>
        public static ReadOnlyDictionary<string, string> Pa { get; private set; } = new(GetCoreSuppliers()[nameof(Pa)]);

        /// <summary>
        ///     Gets a dictionary with the <see cref="AppSupplierHosts.Pac"/> mirror
        ///     addresses as the key and their location as the value.
        /// </summary>
        public static ReadOnlyDictionary<string, string> Pac { get; private set; } = new(GetCoreSuppliers()[nameof(Pac)]);

        /// <summary>
        ///     Gets a dictionary with the <see cref="AppSupplierHosts.Sf"/> mirror
        ///     addresses as the key and their location as the value.
        /// </summary>
        public static ReadOnlyDictionary<string, string> Sf { get; private set; } = new(GetCoreSuppliers()[nameof(Sf)]);

        /// <summary>
        ///     Update all mirror addresses with the data from
        ///     <see cref="CacheData.AppSuppliers"/> cache.
        /// </summary>
        /// <returns>
        ///     If running, an asynchronous <see cref="Task"/> that sorts <see cref="Sf"/>
        ///     mirrors by their connection speed; otherwise, <see langword="null"/>.
        /// </returns>
        public static Task UpdateSuppliers()
        {
            ClearCoreSuppliers();
            return UpdateSuppliers(CacheData.AppSuppliers);
        }

        private static void ClearCoreSuppliers() =>
            _coreAppSuppliers = default;

        private static Dictionary<string, Dictionary<string, string>> GetCoreSuppliers() =>
            _coreAppSuppliers ??= CacheData.LoadDat<Dictionary<string, Dictionary<string, string>>>(CorePaths.AppSuppliers);

        private static Task UpdateSuppliers(IReadOnlyDictionary<string, Dictionary<string, string>> suppliers)
        {
            ClearCoreSuppliers();
            Pa = new(suppliers[nameof(Pa)]);
            Pac = new(suppliers[nameof(Pac)]);
            Sf = new(suppliers[nameof(Sf)]);

            // Some servers are very unstable, so this
            // task should be run at least once a day.
            var filePath = CacheFiles.AppSuppliers;
            if (!File.Exists(filePath))
                return default;
            var fileDate = File.GetLastWriteTime(filePath);
            var runTask = (DateTime.Now - fileDate).TotalHours > 23d;
            return runTask ? Task.Run(SortSourceForgeByFastestConnection) : default;
        }

        private static void SortSourceForgeByFastestConnection()
        {
            if (!NetEx.IPv4IsAvalaible)
                return;
            if (Log.DebugMode > 0)
                Log.Write("Try to sort collection of servers. . .");
            const int timeout = 1000;
            var sortHelper = new Dictionary<string, (string, long)>();
            foreach (var (key, value) in Sf.Select(p => (p.Key, p.Value)))
            {
                if (sortHelper.Keys.ContainsItem(key))
                    continue;
                var time = NetEx.Ping(key, timeout);
                if (Log.DebugMode > 0)
                    switch (time)
                    {
                        case >= timeout:
                            Log.Write($"'{key}' has a timeout.");
                            break;
                        default:
                            Log.Write($"Reply from '{key}'; time={time}ms.");
                            break;
                    }
                sortHelper.Add(key, (time == timeout ? "TIMEOUT" : value, time));
            }
            var newOrder = sortHelper.OrderBy(x => x.Value.Item2).ToDictionary(x => x.Key, x => x.Value.Item1);
            if (Log.DebugMode > 0)
                Log.Write($"New server sort order: '{newOrder.Keys.Join("'; '")}'.");
            if (newOrder.Count - newOrder.Values.Count(x => x.Equals("TIMEOUT")) <= 2)
                return;
            Sf = new(newOrder);
            var filePath = CacheFiles.AppSuppliers;
            if (!File.Exists(filePath))
                return;
            ((Dictionary<string, Dictionary<string, string>>)CacheData.AppSuppliers)[nameof(Sf)] = newOrder;
            CacheData.SaveDat(CacheData.AppSuppliers, filePath);
            File.SetLastWriteTime(filePath, DateTime.Now);
        }
    }
}
