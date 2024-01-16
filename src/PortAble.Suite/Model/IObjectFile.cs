namespace PortAble.Model;

using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

/// <summary>
///     Allows an object to control its own serialization and deserialization to
///     and from a object file.
/// </summary>
public interface IObjectFile : ISerializable
{
    /// <summary>
    ///     The full path to the object file that stores the data of the instance.
    /// </summary>
    [JsonIgnore]
    string FilePath { get; }

    /// <summary>
    ///     The exact time when this instance was created, updated via the
    ///     <see cref="LoadFromFile"/> method, or when the <see cref="SaveToFile"/>
    ///     method was called. So this <see cref="DateTime"/> is used to synchronize
    ///     with the file on the hard drive.
    /// </summary>
    [JsonIgnore]
    DateTime InstanceTime { get; }

    /// <summary>
    ///     Converts the values of this instance to a string.
    /// </summary>
    /// <param name="formatted">
    ///     <see langword="true"/> to format the result; otherwise,
    ///     <see langword="false"/>.
    /// </param>
    /// <returns>
    ///     A string representation of this instance.
    /// </returns>
    string ToString(bool formatted);

    /// <inheritdoc cref="ToString(bool)"/>
    string ToString();

    /// <summary>
    ///     Determines whether the serialized file exists.
    /// </summary>
    bool FileExists();

    /// <summary>
    ///     Load the data from a file if newer than this instance.
    /// </summary>
    /// <param name="force">
    ///     Determines whether to force an update from the file.
    /// </param>
    void LoadFromFile(bool force = false);

    /// <summary>
    ///     Saves the instance data into a file if newer than the file.
    /// </summary>
    /// <param name="force">
    ///     Determines whether to force an update to the file.
    /// </param>
    void SaveToFile(bool force = false);

    /// <summary>
    ///     Removes the serialized file.
    /// </summary>
    void RemoveFile();
}

/// <inheritdoc cref="IObjectFile"/>
/// <typeparam name="TFile">
///     The settings type.
/// </typeparam>
public interface IObjectFile<TFile> : IObjectFile, IEquatable<TFile> { }
