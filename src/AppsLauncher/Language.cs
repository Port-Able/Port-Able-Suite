using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using AppsLauncher.Libraries;
using SilDev;
using SilDev.Legacy;

internal static class Language
{
    internal const string DefaultLang = "en-US";
    private static string _baseName, _currentLang, _systemLang, _userLang;
    private static Assembly _currentAssembly;

    internal static string BaseName =>
        _baseName ??= ResourcesNamespace != default ? ResourcesNamespace + ".LangResources." : default;

    internal static Assembly CurrentAssembly
    {
        get
        {
            if (_currentAssembly == default)
                _currentAssembly = Assembly.Load(Assembly.GetEntryAssembly()?.GetName().Name ?? throw new InvalidOperationException());
            return _currentAssembly;
        }
    }

    internal static string CurrentLang
    {
        get => _currentLang ??= SystemLang;
        set => _currentLang = value;
    }

    internal static string SystemLang =>
        _systemLang ??= CultureInfo.InstalledUICulture.Name;

    internal static string UserLang
    {
        get => _userLang ??= Ini.Read<string>("Launcher", "Language", SystemLang);
        set => _userLang = value;
    }

    internal static string ResourcesNamespace { get; set; }

    internal static string GetText(string lang, string key)
    {
        string text;
        switch (lang)
        {
            case "de-DE":
            case DefaultLang:
                try
                {
                    var rm = new ResourceManager(BaseName + lang, CurrentAssembly);
                    text = rm.GetString(key, CultureInfo.CurrentUICulture);
                }
                catch (Exception ex) when (ex.IsCaught())
                {
                    Log.Write(ex);
                    text = key;
                }
                break;
            default:
                text = GetText(DefaultLang, key);
                break;
        }
        return !string.IsNullOrWhiteSpace(text) ? text : key;
    }

    internal static string GetText(string lang, Control control) => 
        control is not { } c ? string.Empty : GetText(lang, c.Name);

    internal static string GetText(string key)
    {
        if (!CurrentLang.Equals(UserLang, StringComparison.Ordinal))
            CurrentLang = UserLang;
        return GetText(CurrentLang, key);
    }

    internal static string GetText(Control control)
    {
        if (control is not { } c)
            return string.Empty;
        if (!CurrentLang.Equals(UserLang, StringComparison.Ordinal))
            CurrentLang = UserLang;
        return GetText(CurrentLang, c.Name);
    }

    internal static void SetControlLang(Control control, bool recursive = true)
    {
        if (control is not { } parent)
            return;
        if (!string.IsNullOrWhiteSpace(parent.Text))
        {
            var text = GetText(parent);
            if (!text.EqualsEx(parent.Name))
                parent.Text = text;
        }
        if (!recursive)
            return;
        var childs = parent.Controls.OfType<Control>();
        foreach (var child in childs)
            SetControlLang(child);
    }
}
