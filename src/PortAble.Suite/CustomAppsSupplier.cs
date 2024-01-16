namespace PortAble;

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Security;
using Model;
using Newtonsoft.Json;
using SilDev;
using SilDev.Network;
using static SilDev.Crypto;

/// <summary>
///     Provides information about a custom app supplier server and credentials.
/// </summary>
[Serializable]
public sealed class CustomAppSupplier : IObjectFile<CustomAppSupplier>
{
    [NonSerialized]
    private string _filePath;

    /// <summary>
    ///     The server address to retrieve data from.
    /// </summary>
    public string Address { get; }

    /// <summary>
    ///     The username credentials.
    /// </summary>
    public string User { get; set; }

    /// <summary>
    ///     The password credentials.
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    ///     The user agent used for all connections.
    /// </summary>
    public string UserAgent { get; set; }

    /// <inheritdoc cref="IObjectFile.FilePath"/>
    [JsonIgnore]
    public string FilePath
    {
        get
        {
            if (_filePath != null)
                return _filePath;
            var file = $"{Address.Encrypt().ToUpperInvariant().Substring((int)Math.Floor(Sha256.HashLength * .875f))}.dat";
            return _filePath = PathEx.Combine(CorePaths.CustomAppSuppliersDir, file);
        }
    }

    /// <summary>
    ///     The exact time when this instance was created, updated via the
    ///     <see cref="LoadFromFile"/> method, or when the <see cref="SaveToFile"/>
    ///     method was called. So this <see cref="DateTime"/> is used to synchronize
    ///     with the file on the hard drive.
    /// </summary>
    [JsonIgnore]
    public DateTime InstanceTime { get; private set; }

    /// <summary>
    ///     Initialize the <see cref="CustomAppSupplier"/> class.
    /// </summary>
    /// <param name="address">
    ///     The server address to retrieve data from.
    /// </param>
    /// <param name="user">
    ///     The username credentials, if required.
    ///     <para>
    ///         Please note that this may only work for basic authentications. Visit
    ///         <see href="https://httpd.apache.org/docs/current/programs/htpasswd.html"/>
    ///         for more information.
    ///     </para>
    /// </param>
    /// <param name="password">
    ///     The password credentials, if required.
    ///     <para>
    ///         Please note that this may only work for basic authentications. Visit
    ///         <see href="https://httpd.apache.org/docs/current/programs/htpasswd.html"/>
    ///         for more information.
    ///     </para>
    /// </param>
    /// <param name="userAgent">
    ///     The user agent used for all connections.
    ///     <para>
    ///         <see cref="UserAgents.Wget"/> by default and after several failures
    ///         <see cref="UserAgents.Browser"/>.
    ///     </para>
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     address is null.
    /// </exception>
    /// <exception cref="ArgumentInvalidException">
    ///     address is not a valid HTTP(S) address.
    /// </exception>
    public CustomAppSupplier(string address,
                             string user = default,
                             string password = default,
                             string userAgent = default)
    {
        if (address == null)
            throw new ArgumentNullException(nameof(address));
        if (!address.StartsWithEx("http://", "https://") || address.EndsWithEx("/") || NetEx.GetFullHost(address) == null)
            throw new ArgumentInvalidException(nameof(address));

        Address = address;
        User = user;
        Password = password;
        UserAgent = userAgent;
        InstanceTime = DateTime.Now;
        LoadFromFile(true);
    }

    private CustomAppSupplier(SerializationInfo info, StreamingContext context)
    {
        if (info == null)
            throw new ArgumentNullException(nameof(info));

        Address = info.GetString(nameof(Address));
        User = info.GetString(nameof(User));
        Password = info.GetString(nameof(Password));
        UserAgent = info.GetString(nameof(UserAgent));
        InstanceTime = DateTime.Now;
    }

    /// <inheritdoc cref="ISerializable.GetObjectData(SerializationInfo, StreamingContext)"/>
    [SecurityCritical]
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        if (info == null)
            throw new ArgumentNullException(nameof(info));

        info.AddValue(nameof(Address), Address);
        info.AddValue(nameof(User), User);
        info.AddValue(nameof(Password), Password);
        info.AddValue(nameof(UserAgent), UserAgent);
    }

    /// <summary>
    ///     Determines whether this instance have same values as the specified
    ///     <see cref="CustomAppSupplier"/> instance.
    ///     <para>
    ///         &#9888; Not consistent with <see cref="GetHashCode"/>, which is
    ///         intentional.
    ///     </para>
    /// </summary>
    /// <param name="other">
    ///     The <see cref="CustomAppSupplier"/> instance to compare.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the values of the specified
    ///     <see cref="CustomAppSupplier"/> instance are equal to the values of the
    ///     current <see cref="CustomAppSupplier"/> instance; otherwise
    ///     <see langword="false"/>.
    /// </returns>
    public bool Equals(CustomAppSupplier other) =>
        other != null &&
        Address == other.Address &&
        User == other.User &&
        Password == other.Password &&
        UserAgent == other.UserAgent;

    /// <inheritdoc cref="Equals(CustomAppSupplier)"/>
    public override bool Equals(object other)
    {
        if (other is CustomAppSupplier instance)
            return Equals(instance);
        return false;
    }

    /// <inheritdoc cref="Type.GetHashCode()"/>
    public override int GetHashCode() =>
        (typeof(CustomAppSupplier).FullName, Address).GetHashCode();

    /// <see cref="IObjectFile.ToString(bool)"/>
    public string ToString(bool formatted) =>
        JsonConvert.SerializeObject(this, formatted ? Formatting.Indented : Formatting.None);

    /// <inheritdoc cref="IObjectFile.ToString()"/>
    public override string ToString() =>
        ToString(false);

    /// <summary>
    ///     Determines the availability of the <see langword="AppInfo.ini"/> file using
    ///     the instance`s connection data.
    /// </summary>
    /// <param name="timeout">
    ///     The time-out value in milliseconds.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the internet resource was found; otherwise,
    ///     <see langword="false"/>.
    /// </returns>
    public bool IsValid(int timeout = 60000)
    {
        var url = PathEx.AltCombine(default(char[]), Address, "AppInfo.ini");
        if (Log.DebugMode > 0)
            Log.Write($"Custom: Looking for '{url}'.");
        return NetEx.FileIsAvailable(url, User, Password, timeout, UserAgent);
    }

    /// <summary>
    ///     Determines whether the JSON file exists in the
    ///     <see cref="CorePaths.CustomAppSuppliersDir"/> folder.
    /// </summary>
    public bool FileExists() =>
        FileEx.Exists(FilePath);

    /// <summary>
    ///     Load the data from a DAT file stored in the
    ///     <see cref="CorePaths.CustomAppSuppliersDir"/> folder if newer than this
    ///     instance.
    /// </summary>
    /// <param name="force">
    ///     Determines whether to force an update from the file.
    /// </param>
    public void LoadFromFile(bool force = false)
    {
        if (!FileExists())
        {
            InstanceTime = DateTime.MinValue;
            return;
        }
        if (!force && File.GetLastWriteTime(FilePath) < InstanceTime)
            return;
        var item = CacheData.LoadDat<CustomAppSupplier>(FilePath);
        if (item == default)
            return;
        User = item.User;
        Password = item.Password;
        UserAgent = item.UserAgent;
        InstanceTime = DateTime.Now;
    }

    /// <summary>
    ///     Saves the data into a DAT file stored in the
    ///     <see cref="CorePaths.CustomAppSuppliersDir"/> directory if newer than the
    ///     file.
    /// </summary>
    /// <param name="force">
    ///     Determines whether to force an update to the file.
    /// </param>
    public void SaveToFile(bool force = false)
    {
        if (!force)
        {
            var item = CacheData.LoadDat<CustomAppSupplier>(FilePath);
            if (this == item)
                return;
        }
        CacheData.SaveDat(this, FilePath);
        InstanceTime = DateTime.Now;
    }

    /// <summary>
    ///     Deletes the previously saved file from the
    ///     <see cref="CorePaths.CustomAppSuppliersDir"/> directory, if exists.
    /// </summary>
    public void RemoveFile() =>
        FileEx.TryDelete(FilePath);

    /// <summary>
    ///     Determines whether two specified <see cref="CustomAppSupplier"/> instances
    ///     have same values.
    ///     <para>
    ///         &#9888; Not consistent with <see cref="GetHashCode"/>, which is
    ///         intentional.
    ///     </para>
    /// </summary>
    /// <param name="left">
    ///     The first <see cref="CustomAppSupplier"/> instance to compare.
    /// </param>
    /// <param name="right">
    ///     The second <see cref="CustomAppSupplier"/> instance to compare.
    /// </param>
    public static bool operator ==(CustomAppSupplier left, CustomAppSupplier right) =>
        Equals(left, right);

    /// <summary>
    ///     Determines whether two specified <see cref="CustomAppSupplier"/> instances
    ///     have different values.
    ///     <para>
    ///         &#9888; Not consistent with <see cref="GetHashCode"/>, which is
    ///         intentional.
    ///     </para>
    /// </summary>
    /// <inheritdoc cref="operator ==(CustomAppSupplier, CustomAppSupplier)"/>
    public static bool operator !=(CustomAppSupplier left, CustomAppSupplier right) =>
        !Equals(left, right);
}