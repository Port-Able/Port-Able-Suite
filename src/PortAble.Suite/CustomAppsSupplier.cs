namespace PortAble
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Security;
    using System.Web.Script.Serialization;
    using SilDev;
    using SilDev.Network;

    /// <summary>
    ///     Provides information about a custom app supplier server and credentials.
    /// </summary>
    [Serializable]
    public sealed class CustomAppSupplier : ISerializable
    {
        [NonSerialized]
        private string _filePath;

        /// <summary>
        ///     The server address to retrieve data from.
        /// </summary>
        public string Address { get; private set; }

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

        /// <summary>
        ///     The path on disk where the instance will be saved and loaded.
        /// </summary>
        [ScriptIgnore]
        public string FilePath
        {
            get
            {
                if (_filePath != null)
                    return _filePath;
                var file = $"{Address.Encrypt(ChecksumAlgorithm.Sha256).Substring(Crypto.Sha256.HashLength - 8)}.dat";
                return _filePath = PathEx.Combine(CorePaths.CustomAppSuppliersDir, file);
            }
        }

        /// <summary>
        ///     The exact time when this instance was created, updated via the
        ///     <see cref="LoadFromFile"/> method, or when the <see cref="SaveToFile"/>
        ///     method was called. So this <see cref="DateTime"/> is used to synchronize
        ///     with the file on the hard drive.
        /// </summary>
        [ScriptIgnore]
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

        /// <summary>
        ///     Sets the <see cref="SerializationInfo"/> object for this instance.
        /// </summary>
        /// <param name="info">
        ///     The object that holds the serialized object data.
        /// </param>
        /// <param name="context">
        ///     The contextual information about the source or destination.
        /// </param>
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
        /// </summary>
        /// <param name="other">
        ///     The <see cref="CustomAppSupplier"/> instance to compare.
        /// </param>
        public bool Equals(CustomAppSupplier other) =>
            other != null &&
            Address == other.Address &&
            User == other.User &&
            Password == other.Password &&
            UserAgent == other.UserAgent;

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is CustomAppSupplier other)
                return Equals(other);
            return false;
        }

        /// <inheritdoc cref="Type.GetHashCode()"/>
        public override int GetHashCode() =>
            Tuple.Create(GetType().FullName).GetHashCode();

        /// <summary>
        ///     Converts the values of this instance to a string.
        /// </summary>
        /// <returns>
        ///     A string representation of this instance.
        /// </returns>
        public override string ToString() =>
            Json.Serialize(this);

        /// <summary>
        ///     Determines the availability of the <see langword="AppInfo.ini"/> file using
        ///     the instance's connection data.
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
        ///     Load the data from a DAT file stored in the
        ///     <see cref="CorePaths.CustomAppSuppliersDir"/> folder if newer than this
        ///     instance.
        /// </summary>
        /// <param name="force">
        ///     Determines whether to force an update from the file.
        /// </param>
        public void LoadFromFile(bool force = false)
        {
            if (!FileEx.Exists(FilePath))
            {
                InstanceTime = DateTime.MinValue;
                return;
            }
            if (!force && File.GetLastWriteTime(FilePath) < InstanceTime)
                return;
            var item = CacheData.LoadDat<CustomAppSupplier>(FilePath);
            if (item == default)
                return;
            Address = item.Address;
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
        /// </summary>
        /// <param name="left">
        ///     The first <see cref="CustomAppSupplier"/> instance to compare.
        /// </param>
        /// <param name="right">
        ///     The second <see cref="CustomAppSupplier"/> instance to compare.
        /// </param>
        public static bool operator !=(CustomAppSupplier left, CustomAppSupplier right) =>
            !Equals(left, right);
    }
}
