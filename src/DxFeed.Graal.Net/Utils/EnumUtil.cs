// <copyright file="EnumUtil.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Numerics;
using DxFeed.Graal.Net.Events.Market;

namespace DxFeed.Graal.Net.Utils;

/// <summary>
/// Provides utility methods for manipulating <see cref="Enum"/>.
/// </summary>
public static class EnumUtil
{
    /// <summary>
    /// Returns an enum constant of the specified enum type with the specified value.
    /// </summary>
    /// <param name="value">The specified value.</param>
    /// <typeparam name="T">The specified enum type.</typeparam>
    /// <returns>An enum constant of the specified enum type with the specified value.</returns>
    /// <exception cref="ArgumentException">
    /// If the specified enum type does not have a constant with the specified value.
    /// </exception>
    public static T ValueOf<T>(int value)
        where T : Enum
    {
        var enumType = typeof(T);
        if (Enum.IsDefined(enumType, value))
        {
            return (T)Enum.ToObject(enumType, value);
        }

        throw new ArgumentException($"{enumType} has no value({value})", nameof(value));
    }

    /// <summary>
    /// Returns an enum constant of the specified enum type with the specified value,
    /// or a default value if the specified enum type does not have a constant with the specified value.
    /// </summary>
    /// <param name="value">The specified value.</param>
    /// <param name="defaultValue">The default enum value.</param>
    /// <typeparam name="T">The specified enum type.</typeparam>
    /// <returns>
    /// The enum constant of the specified enum type with the specified value
    /// or default value, if specified enum type has no constant with the specified value.
    /// </returns>
    public static T ValueOf<T>(int value, T defaultValue)
        where T : Enum
    {
        var enumType = typeof(T);
        if (Enum.IsDefined(enumType, value))
        {
            return (T)Enum.ToObject(enumType, value);
        }

        return defaultValue;
    }

    /// <summary>
    /// Gets the number of values for the specified enum type.
    /// </summary>
    /// <typeparam name="T">The specified enum type.</typeparam>
    /// <returns>Returns the number of values of the specified enum type.</returns>
    public static int GetCountValues<T>()
        where T : Enum =>
        Enum.GetValues(typeof(T)).Length;

    /// <summary>
    /// Creates an array containing elements of the specified enum type T,
    /// of the specified length.
    /// If the length is greater than the number of enum values,
    /// the remaining elements are filled with a default value, otherwise array are truncated.
    /// </summary>
    /// <param name="defaultValue">
    /// The default value that will fill the elements of an array
    /// if its size is greater than the number of enum values.
    /// </param>
    /// <param name="length">The length of result array, must be power of 2.</param>
    /// <typeparam name="T">The specified enum type.</typeparam>
    /// <returns>The created array.</returns>
    /// <exception cref="ArgumentException">If length is not power of 2.</exception>
    /// <remarks>
    /// The elements of the array are sorted by the binary values of the enumeration constants
    /// (that is, by their unsigned magnitude).
    /// </remarks>
    /// <seealso cref="BuildEnumBitMaskArrayByValue{T}(T)"/>
    public static T[] BuildEnumBitMaskArrayByValue<T>(T defaultValue, int length)
        where T : Enum
    {
        if (!MathUtil.IsPowerOfTwo(length))
        {
            throw new ArgumentException("Length must be power of 2", nameof(length));
        }

        var values = (T[])Enum.GetValues(typeof(T));
        var result = new T[length];
        Array.Fill(result, defaultValue);
        Array.Copy(values, result, Math.Min(values.Length, length));
        return result;
    }

    /// <summary>
    /// Creates an array containing elements of the specified enum type T,
    /// where the length of the array is rounded to the nearest power of two,
    /// which is greater than or equal to the number of enum values.
    /// If the calculated length is greater than the number of enum values,
    /// the remaining elements are filled with a default value.
    /// The idea is to quickly convert an int value to an enum value, simply by array index.
    /// But the size of the array is limited by a bit mask, so if the number of enum values
    /// is not a multiple of a power of two, you need to expand the array and fill in new elements with a default value.
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
    /// <seealso cref="BuildEnumBitMaskArrayByValue{T}(T,int)"/>
    public static T[] BuildEnumBitMaskArrayByValue<T>(T defaultValue)
        where T : Enum =>
        BuildEnumBitMaskArrayByValue(
            defaultValue,
            (int)BitOperations.RoundUpToPowerOf2((uint)GetCountValues<Direction>()));
}
