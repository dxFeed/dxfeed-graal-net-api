// <copyright file="EnumUtil.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Numerics;

namespace DxFeed.Graal.Net.Utils;

/// <summary>
/// Provides utility methods for manipulating enumerations.
/// </summary>
public static class EnumUtil
{
    /// <summary>
    /// Returns an enum constant of the specified enum type with the specified value,
    /// or throws <see cref="ArgumentException"/> if the specified enum type does not have
    /// a constant with the specified value.
    /// </summary>
    /// <param name="value">The specified value.</param>
    /// <typeparam name="T">The specified enum type.</typeparam>
    /// <returns>An enum constant of the specified enum type with the specified value.</returns>
    /// <exception cref="ArgumentException">
    /// If the specified enum type does not have a constant with the specified value.
    /// </exception>
    public static T ValueOf<T>(T value)
        where T : struct, Enum
    {
        if (Enum.IsDefined(value))
        {
            return value;
        }

        throw new ArgumentException($"{typeof(T)} has no value: {value}", nameof(value));
    }

    /// <summary>
    /// Returns an enum constant of the specified enum type with the specified value,
    /// or a default value if the specified enum type does not have
    /// a constant with the specified value.
    /// </summary>
    /// <param name="value">The specified value.</param>
    /// <param name="defaultValue">The default enum value.</param>
    /// <typeparam name="T">The specified enum type.</typeparam>
    /// <returns>
    /// The enum constant of the specified enum type with the specified value
    /// or default value, if specified enum type has no constant with the specified value.
    /// </returns>
    public static T ValueOf<T>(T value, T defaultValue)
        where T : struct, Enum =>
        Enum.IsDefined(value) ? value : defaultValue;

    /// <summary>
    /// Gets the number of values for the specified enum type.
    /// </summary>
    /// <typeparam name="T">The specified enum type.</typeparam>
    /// <returns>Returns the number of values of the specified enum type.</returns>
    public static int GetCountValues<T>()
        where T : struct, Enum =>
        Enum.GetValues(typeof(T)).Length;

    /// <summary>
    /// Creates an array containing elements of the specified enum type <c>T</c>,
    /// where the length of the array is rounded to the nearest power of two,
    /// which is greater than or equal to the number of enum values.
    /// If the calculated length is greater than the number of enum values,
    /// the remaining elements are filled with a default value.
    /// The idea is to quickly convert an <c>int</c> value to an enum value by using the array index.
    /// However, the size of the array is limited by a bit mask. If the number of enum values
    /// isn't a power of two, the array is expanded and the additional elements are filled with the default value.
    /// </summary>
    /// <param name="defaultValue">
    /// The default value that will fill the elements of an array
    /// if its size is greater than the number of enum values.
    /// </param>
    /// <typeparam name="T">The specified enum type.</typeparam>
    /// <returns>The created array.</returns>
    /// <remarks>
    /// The elements of the array are sorted by the binary values of the enumeration constants
    /// (that is, by their unsigned magnitude).
    /// </remarks>
    /// <seealso cref="CreateEnumArrayByValue{T}"/>
    public static T[] CreateEnumBitMaskArrayByValue<T>(T defaultValue)
        where T : struct, Enum =>
        CreateEnumArrayByValue(
            defaultValue,
            (int)BitOperations.RoundUpToPowerOf2((uint)GetCountValues<T>()));

    /// <summary>
    /// Creates an array containing elements of the specified enum type <c>T</c>, of the specified length.
    /// If the length is greater than the number of enum values,
    /// the remaining elements are filled with a default value, otherwise array are truncated.
    /// </summary>
    /// <param name="defaultValue">
    /// The default value that will fill the elements of an array if its size is greater than the number of enum values.
    /// </param>
    /// <param name="length">The length of result array.</param>
    /// <typeparam name="T">The specified enum type.</typeparam>
    /// <returns>The created array.</returns>
    /// <exception cref="ArgumentException">If length is less than zero.</exception>
    /// <remarks>
    /// The elements of the array are sorted by the binary values of the enumeration constants
    /// (that is, by their unsigned magnitude).
    /// </remarks>
    public static T[] CreateEnumArrayByValue<T>(T defaultValue, int length)
        where T : Enum
    {
        if (length < 0)
        {
            throw new ArgumentException($"Length must be greater than zero({length})", nameof(length));
        }

        var values = (T[])Enum.GetValues(typeof(T));
        var result = new T[length];
        Array.Fill(result, defaultValue);
        Array.Copy(values, result, Math.Min(values.Length, length));
        return result;
    }
}
