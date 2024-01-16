namespace PortAble.Model;

using System;

/// <inheritdoc cref="IObjectFile"/>/>
public interface IObjectFileEditor : IObjectFile
{
    /// <summary>
    ///     Gets the value associated with the specified property name.
    /// </summary>
    /// <param name="propertyName">
    ///     The property name to get the value from.
    /// </param>
    /// <param name="fallbackValue">
    ///     The fallback value.
    /// </param>
    /// <returns>
    ///     If found, the property value; otherwise, the fallback value.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     propertyName not found.
    /// </exception>
    object GetValue(string propertyName, object fallbackValue = default);

    /// <inheritdoc cref="GetValue(string,object)"/>
    /// <typeparam name="T">
    ///     The value type.
    /// </typeparam>
    T GetValue<T>(string propertyName, T fallbackValue = default);

    /// <summary>
    ///     Gets the default value associated with the specified property name.
    /// </summary>
    /// <param name="propertyName">
    ///     The property name to get the value from.
    /// </param>
    /// <returns>
    ///     If found, the properties default value; otherwise, value types
    ///     <see langword="default"/> value.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     propertyName not found.
    /// </exception>
    object GetDefault(string propertyName);

    /// <inheritdoc cref="GetDefault(string)"/>
    /// <typeparam name="T">
    ///     The value type.
    /// </typeparam>
    T GetDefault<T>(string propertyName);

    /// <summary>
    ///     Gets the specified value associated with the specified property name.
    /// </summary>
    /// <typeparam name="T">
    ///     The value type.
    /// </typeparam>
    /// <param name="propertyName">
    ///     The property name to set the value.
    /// </param>
    /// <param name="value">
    ///     The value to be set.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the value was set; otherwise,
    ///     <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     propertyName not found.
    /// </exception>
    bool SetValue<T>(string propertyName, T value);

    /// <summary>
    ///     Resets the specified property to its default value.
    /// </summary>
    /// <param name="propertyName">
    ///     The property name to set the value.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the value was set; otherwise,
    ///     <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     propertyName not found.
    /// </exception>
    bool SetDefault(string propertyName);

    /// <summary>
    ///     Resets all properties to their default values.
    /// </summary>
    void SetDefaultAll();
}