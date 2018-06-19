using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using SilDev;

internal static class Language
{
    internal const string DefaultLang = "en-US";
    private static string _baseName, _currentLang, _systemLang, _userLang;
    private static Assembly _currentAssembly;

    internal static string BaseName
    {
        get
        {
            if (_baseName == default(string))
                _baseName = ResourcesNamespace != default(string) ? ResourcesNamespace + ".LangResources." : default(string);
            return _baseName;
        }
    }

    internal static Assembly CurrentAssembly
    {
        get
        {
            if (_currentAssembly == default(Assembly))
                _currentAssembly = Assembly.Load(Assembly.GetEntryAssembly().GetName().Name);
            return _currentAssembly;
        }
    }

    internal static string CurrentLang
    {
        get
        {
            if (_currentLang == default(string))
                _currentLang = SystemLang;
            return _currentLang;
        }
        set => _currentLang = value;
    }

    internal static string SystemLang
    {
        get
        {
            if (_systemLang == default(string))
                _systemLang = CultureInfo.InstalledUICulture.Name;
            return _systemLang;
        }
    }

    internal static string UserLang
    {
        get
        {
            if (_userLang == default(string))
                _userLang = Ini.Read<string>("Launcher", "Language", SystemLang);
            return _userLang;
        }
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
                    text = rm.GetString(key);
                }
                catch (Exception ex)
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

    internal static string GetText(string lang, Control control)
    {
        if (!(control is Control c))
            return string.Empty;
        return GetText(lang, c.Name);
    }

    internal static string GetText(string key)
    {
        if (!CurrentLang.Equals(UserLang))
            CurrentLang = UserLang;
        return GetText(CurrentLang, key);
    }

    internal static string GetText(Control control)
    {
        if (!(control is Control c))
            return string.Empty;
        if (!CurrentLang.Equals(UserLang))
            CurrentLang = UserLang;
        return GetText(CurrentLang, c.Name);
    }

    internal static void SetControlLang(Control control, bool recursive = true)
    {
        if (!(control is Control parent))
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
