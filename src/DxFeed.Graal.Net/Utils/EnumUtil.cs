// <copyright file="EnumUtil.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;

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
}
