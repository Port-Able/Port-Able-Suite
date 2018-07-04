using System.Reflection;
using System.Runtime.InteropServices;

#if x86
[assembly: AssemblyTitle("Port-Able Apps Downloader")]
[assembly: AssemblyProduct("AppsDownloader")]
#else
[assembly: AssemblyTitle("Port-Able Apps Downloader (64-bit)")]
[assembly: AssemblyProduct("AppsDownloader64")]
#endif

[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Si13n7 Dev. ®")]
[assembly: AssemblyCopyright("Copyright © Si13n7 Dev. ® 2018")]
[assembly: AssemblyTrademark("Si13n7 Dev. ®")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]

[assembly: Guid("a3418f24-e739-45c2-b31e-f50f4124e600")]

[assembly: AssemblyVersion("18.7.4.*")]
