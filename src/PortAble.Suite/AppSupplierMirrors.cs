namespace PortAble;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SilDev;
using SilDev.Network;

/// <summary>
///     Provides server addresses and location information from official app
///     suppliers.
/// </summary>
public static class AppSupplierMirrors
{
    private static ReadOnlyDictionary<string, string> _pa, _pac, _sf;

    /// <summary>
    ///     Gets a dictionary with the <see cref="AppSupplierHosts.Pa"/> mirror
    ///     addresses as the key and their location as the value.
    /// </summary>
    public static ReadOnlyDictionary<string, string> Pa =>
        _pa ??= new(new Dictionary<string, string>
        {
            { "https://port-a.de", "Europe/Germany, Frankfurt" },
            { "https://p-able.de", "Europe/Germany, Frankfurt" },
            { "https://dl.si13n7.de/Port-Able", "BACKUP" },
            { "https://dl.si13n7.com/Port-Able", "BACKUP" },
            { "https://dl-0.de/Port-Able", "RESERVED" },
            { "https://dl-1.de/Port-Able", "RESERVED" },
            { "https://dl-2.de/Port-Able", "RESERVED" },
            { "https://dl-3.de/Port-Able", "RESERVED" },
            { "https://dl-4.de/Port-Able", "RESERVED" },
            { "https://dl-5.de/Port-Able", "RESERVED" }
        });

    /// <summary>
    ///     Gets a dictionary with the <see cref="AppSupplierHosts.Pac"/> mirror
    ///     addresses as the key and their location as the value.
    /// </summary>
    public static ReadOnlyDictionary<string, string> Pac =>
        _pac ??= new(new Dictionary<string, string>
        {
            { "https://portableapps.com", "Europe/France, Paris" },
            { "https://downloads.portableapps.com", "DISABLED" },
            { "https://downloads2.portableapps.com", "DISABLED" },
            { "https://download3.portableapps.com", "DISABLED" },
            { "https://portableapps.com/bouncer?t=", "DISABLED" }
        });

    /// <summary>
    ///     Gets a dictionary with the <see cref="AppSupplierHosts.Sf"/> mirror
    ///     addresses as the key and their location as the value.
    /// </summary>
    public static ReadOnlyDictionary<string, string> Sf =>
        _sf ??= new(new Dictionary<string, string>
        {
            { "https://downloads.sourceforge.net", "America/United States, San Diego" },
            { "https://kumisystems.dl.sourceforge.net", "Europe/Austria" },
            { "https://netix.dl.sourceforge.net", "Europe/Bulgaria, Sofia" },
            { "https://freefr.dl.sourceforge.net", "Europe/France, Paris" },
            { "https://netcologne.dl.sourceforge.net", "Europe/Germany, Cologne" },
            { "https://deac-fra.dl.sourceforge.net", "Europe/Germany, Frankfurt" },
            { "https://deac-riga.dl.sourceforge.net", "Europe/Latvia, Riga" },
            { "https://deac-ams.dl.sourceforge.net", "Europe/Netherlands, Amsterdam" },
            { "https://unlimited.dl.sourceforge.net", "Europe/Serbia, Belgrade" },
            { "https://altushost-swe.dl.sourceforge.net", "Europe/Sweden, Stockholm" },
            { "https://liquidtelecom.dl.sourceforge.net", "Africa/Kenya, Nairobi" },
            { "https://tenet.dl.sourceforge.net", "Africa/South Africa, Johannesburg" },
            { "https://ufpr.dl.sourceforge.net", "America/Brazil, Parana" },
            { "https://razaoinfo.dl.sourceforge.net", "America/Brazil, Rio Grande do Sul" },
            { "https://sinalbr.dl.sourceforge.net", "America/Brazil, Sao Paulo" },
            { "https://gigenet.dl.sourceforge.net", "America/United States, Chicago" },
            { "https://newcontinuum.dl.sourceforge.net", "America/United States, Chicago" },
            { "https://cytranet.dl.sourceforge.net", "America/United States, Las Vegas" },
            { "https://versaweb.dl.sourceforge.net", "America/United States, Las Vegas" },
            { "https://cfhcable.dl.sourceforge.net", "America/United States, New York" },
            { "https://phoenixnap.dl.sourceforge.net", "America/United States, Phoenix" },
            { "https://sitsa.dl.sourceforge.net", "Argentina/Cordoba, Costa Sacate" },
            { "https://yer.dl.sourceforge.net", "Asia/Azerbaijan, Baku" },
            { "https://udomain.dl.sourceforge.net", "Asia/Hong Kong" },
            { "https://zenlayer.dl.sourceforge.net", "Asia/Hong Kong" },
            { "https://excellmedia.dl.sourceforge.net", "Asia/India, Kolkata" },
            { "https://webwerks.dl.sourceforge.net", "Asia/India, Kolkata" },
            { "https://jaist.dl.sourceforge.net", "Asia/Japan, Tokyo" },
            { "https://onboardcloud.dl.sourceforge.net", "Asia/Singapore" },
            { "https://nchc.dl.sourceforge.net", "Asia/Taiwan, Taipei" },
            { "https://ixpeering.dl.sourceforge.net", "Australia/Perth" }
        });

    static AppSupplierMirrors()
    {
        // Some servers are highly unstable, so it is advisable
        // to execute this task at least once every 3 hours.
        var filePath = CachePaths.AppSuppliers;
        var runTask = true;
        if (File.Exists(filePath))
        {
            var fileDate = File.GetLastWriteTime(filePath);
            runTask = (DateTime.Now - fileDate).TotalHours > 3d;
        }
        if (runTask)
        {
            Task.Run(SortSourceForgeByFastestConnection);
            return;
        }
        var sorted = CacheData.AppSuppliers;
        if (sorted?.Count is > 0)
            _sf = new((Dictionary<string, string>)sorted);
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
        if (newOrder.Count - newOrder.Values.Count(x => x.Equals("TIMEOUT")) <= 2)
            return;
        var filePath = CachePaths.AppSuppliers;
        CacheData.SaveDat(newOrder, filePath);
        File.SetLastWriteTime(filePath, DateTime.Now);
        Interlocked.Exchange(ref _sf, new(newOrder));
        if (Log.DebugMode > 0)
            Log.Write($"New server sort order: '{Sf.Keys.Join("'; '")}'.");
    }
}
