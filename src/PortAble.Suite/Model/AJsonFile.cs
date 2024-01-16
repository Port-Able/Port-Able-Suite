namespace PortAble.Model;

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;
using Newtonsoft.Json;
using SilDev;

/// <inheritdoc cref="IObjectFile{TFile}"/>
public abstract class AJsonFile<TFile> : IObjectFile<TFile>, IObjectFileEditor where TFile : class
{
    [NonSerialized]
    private string _filePath;

    /// <inheritdoc cref="IObjectFile.FilePath"/>
    [JsonIgnore]
    public string FilePath =>
        _filePath ??= Path.Combine(CorePaths.SettingsDir, $"{typeof(TFile).Name.RemoveText("Json")}.json");

    /// <inheritdoc cref="IObjectFile.InstanceTime"/>
    [JsonIgnore]
    public DateTime InstanceTime { get; protected set; }

    /// <inheritdoc cref="ISerializable.GetObjectData(SerializationInfo, StreamingContext)"/>
    [SecurityCritical]
    public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        if (info == null)
            throw new ArgumentNullException(nameof(info));

        // Do not serialize default values.
        foreach (var pi in typeof(TFile).GetPropertiesEx(typeof(JsonIgnoreAttribute)).Where(pi => !IsDefault(pi)))
            info.AddValue(pi.Name, pi.GetValue(this));
    }

    /// <inheritdoc cref="IObjectFileEditor.GetValue(string, object)"/>
    public object GetValue(string propertyName, object fallbackValue = default) =>
        GetValue<object>(propertyName, fallbackValue);

    /// <inheritdoc cref="IObjectFileEditor.GetValue{T}(string, T)"/>
    public T GetValue<T>(string propertyName, T fallbackValue = default) =>
        this.GetPropertyValue(propertyName, fallbackValue, typeof(JsonIgnoreAttribute));

    /// <inheritdoc cref="IObjectFileEditor.GetDefault(string)"/>
    public abstract object GetDefault(string propertyName);

    /// <inheritdoc cref="IObjectFileEditor.GetDefault{T}(string)"/>
    public T GetDefault<T>(string propertyName) =>
        (T)GetDefault(propertyName);

    /// <inheritdoc cref="IObjectFileEditor.SetValue{T}(string, T)"/>
    public bool SetValue<T>(string propertyName, T value) =>
        this.SetPropertyValue(propertyName, value, typeof(JsonIgnoreAttribute));

    /// <inheritdoc cref="IObjectFileEditor.SetDefault"/>
    public bool SetDefault(string propertyName) =>
        SetValue(propertyName, GetDefault(propertyName));

    /// <inheritdoc cref="IObjectFileEditor.SetDefaultAll"/>
    public virtual void SetDefaultAll() =>
        typeof(TFile).GetPropertiesEx(typeof(NonSerializedAttribute)).ForEach(pi => SetDefault(pi.Name));

    /// <summary>
    ///     Determines whether this instance have same values as the specified
    ///     <see cref="TFile"/> instance.
    ///     <para>
    ///         &#9888; Not consistent with <see cref="GetHashCode"/>, which is
    ///         intentional.
    ///     </para>
    /// </summary>
    /// <param name="other">
    ///     The <see cref="TFile"/> instance to compare.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the values of the specified <see cref="TFile"/>
    ///     instance are equal to the values of the current <see cref="TFile"/>
    ///     instance; otherwise <see langword="false"/>.
    /// </returns>
    public abstract bool Equals(TFile other);

    /// <inheritdoc cref="Equals(TFile)"/>
    public override bool Equals(object other) =>
        other is TFile instance && Equals(instance);

    /// <inheritdoc cref="Type.GetHashCode()"/>
    public override int GetHashCode() =>
        (typeof(TFile).FullName, nameof(TFile)).GetHashCode();

    /// <inheritdoc cref="IObjectFile.ToString(bool)"/>
    public string ToString(bool formatted) =>
        JsonConvert.SerializeObject(this, formatted ? Formatting.Indented : Formatting.None);

    /// <inheritdoc cref="IObjectFile.ToString()"/>
    public override string ToString() =>
        ToString(false);

    /// <inheritdoc cref="IObjectFile.FileExists"/>
    public bool FileExists() =>
        FileEx.Exists(FilePath);

    /// <summary>
    ///     Load the data from a JSON file stored in the
    ///     <see cref="CorePaths.DataDir"/> folder if newer than this instance.
    /// </summary>
    /// <param name="force">
    ///     Determines whether to force an update from the file.
    /// </param>
    public virtual void LoadFromFile(bool force = false)
    {
        if (!FileEx.Exists(FilePath))
        {
            InstanceTime = DateTime.MinValue;
            return;
        }
        if (!force && File.GetLastWriteTime(FilePath) < InstanceTime)
            return;
        if (CacheData.LoadJson<TFile>(FilePath) is not { } item)
            return;
        foreach (var pi in typeof(TFile).GetPropertiesEx(typeof(JsonIgnoreAttribute)))
            pi.SetValue(this, pi.GetValue(item));
        InstanceTime = DateTime.Now;
    }

    /// <summary>
    ///     Saves the data into a JSON file stored in the
    ///     <see cref="CorePaths.DataDir"/> directory if newer than the file.
    /// </summary>
    /// <param name="force">
    ///     Determines whether to force an update to the file.
    /// </param>
    public void SaveToFile(bool force = false)
    {
        try
        {
            if (!force)
            {
                var item = CacheData.LoadJson<TFile>(FilePath) ?? (TFile)Activator.CreateInstance(typeof(TFile));
                if (Equals(item))
                    return;
            }
        }
        catch (Exception ex) when (ex.IsCaught())
        {
            Log.Write(ex);
        }
        CacheData.SaveJson(this, FilePath);
        InstanceTime = DateTime.Now;
    }

    /// <inheritdoc cref="IObjectFile.RemoveFile"/>
    public void RemoveFile() => FileEx.TryDelete(FilePath);

    /// <summary>
    ///     Determines whether the specified property has the default value, which is
    ///     used in <see cref="GetObjectData(SerializationInfo, StreamingContext)"/> to
    ///     decide whether a property can be written to or deleted from the file.
    /// </summary>
    /// <param name="pi">
    ///     The property to check.
    /// </param>
    protected virtual bool IsDefault(PropertyInfo pi)
    {
        if (pi == null)
            return true;
        var defValue = GetDefault(pi.Name);
        var curValue = GetValue(pi.Name);
        if (curValue == null && defValue == null)
            return true;
        if (curValue == null || defValue == null)
            return false;
        return curValue.Equals(defValue);
    }

    /// <summary>
    ///     Determines whether two specified <see cref="AJsonFile{TFile}"/> instances
    ///     have same values.
    /// </summary>
    /// <param name="left">
    ///     The first <see cref="AJsonFile{TFile}"/> instance to compare.
    /// </param>
    /// <param name="right">
    ///     The second <see cref="AJsonFile{TFile}"/> instance to compare.
    /// </param>
    public static bool operator ==(AJsonFile<TFile> left, AJsonFile<TFile> right) => Equals(left, right);

    /// <summary>
    ///     Determines whether two specified <see cref="AJsonFile{TFile}"/> instances
    ///     have different values.
    /// </summary>
    /// <inheritdoc cref="operator ==(AJsonFile{TFile}, AJsonFile{TFile})"/>
    public static bool operator !=(AJsonFile<TFile> left, AJsonFile<TFile> right) => !Equals(left, right);
}
