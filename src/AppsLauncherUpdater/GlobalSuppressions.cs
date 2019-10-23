using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Code Quality", "IDE0067:Dispose objects before losing scope", Justification = "False Positive", Scope = "member", Target = "~M:Updater.Windows.MainForm.MainForm_Shown(System.Object,System.EventArgs)")]
[assembly: SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "False Positive", Scope = "member", Target = "~M:Updater.Windows.MainForm.MainForm_Shown(System.Object,System.EventArgs)")]
[assembly: SuppressMessage("Usage", "CA2208:Instantiate argument exceptions correctly", Justification = "This can not be avoided in this case", Scope = "member", Target = "~M:Updater.Libraries.Network.DownloadArchiver")]
[assembly: SuppressMessage("Usage", "CA2208:Instantiate argument exceptions correctly", Justification = "This can not be avoided in this case", Scope = "member", Target = "~M:Updater.Windows.MainForm.SetChangeLog(System.String[])")]
[assembly: SuppressMessage("Usage", "CA2208:Instantiate argument exceptions correctly", Justification = "This can not be avoided in this case", Scope = "member", Target = "~M:Updater.Windows.MainForm.SetUpdateInfo(System.Boolean,System.String[])")]
[assembly: SuppressMessage("Usage", "CA2234:Pass system uri objects instead of strings", Justification = "Strings are internally converted anyway")]
