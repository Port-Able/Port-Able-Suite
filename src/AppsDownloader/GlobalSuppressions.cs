using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Code Quality", "IDE0067:Dispose objects before losing scope", Justification = "False Positive", Scope = "member", Target = "~M:AppsDownloader.Windows.MainForm.MainForm_Shown(System.Object,System.EventArgs)")]
[assembly: SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "Nested types should be visible if intended for public use", Scope = "type", Target = "~T:AppsDownloader.Libraries.AppData.AppSettings")]
[assembly: SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase", Justification = "Lowercase letters are expected here", Scope = "member", Target = "~M:AppsDownloader.Libraries.CacheData.UpdateAppInfoData(System.String,System.String[],System.Collections.ObjectModel.ReadOnlyCollection{System.Byte})")]
[assembly: SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "False Positive", Scope = "member", Target = "~M:AppsDownloader.Windows.MainForm.MainForm_Shown(System.Object,System.EventArgs)")]
[assembly: SuppressMessage("Usage", "CA2208:Instantiate argument exceptions correctly", Justification = "This can not be avoided in this case", Scope = "member", Target = "~M:AppsDownloader.Libraries.CacheData.ResetAppInfoFile")]
[assembly: SuppressMessage("Usage", "CA2234:Pass system uri objects instead of strings", Justification = "Strings are internally converted anyway")]
