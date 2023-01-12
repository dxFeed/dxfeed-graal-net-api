// <copyright file="StringUtil.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using static System.Globalization.CultureInfo;

namespace DxFeed.Graal.Net.Utils;

/// <summary>
/// Provides utility methods for working with strings.
/// </summary>
public static class StringUtil
{
    /// <summary>
    /// Encodes specified nullable string.
    /// If string equals null, returns <c>"null"</c> string; otherwise returns specified string.
    /// </summary>
    /// <param name="s">The specified string.</param>
    /// <returns>Return specified string or <c>"null"</c> string.</returns>
    public static string EncodeNullableString(string? s) =>
        s ?? "null";

    /// <summary>
    /// Encodes char to string.
    /// If the value of char falls within the range of printable ASCII characters [32-126],
    /// then returns a string containing that character, otherwise return unicode number <c>"(\uffff)"</c>.
    /// For zero char returns <c>"\0"</c>.
    /// </summary>
    /// <param name="c">The char.</param>
    /// <returns>Returns the encoded string.</returns>
    public static string EncodeChar(char c)
    {
        if (c is >= (char)32 and <= (char)126)
        {
            return c.ToString();
        }

        return c == 0
            ? "\\0"
            : $"\\u{(c + 65536).ToString("x", InvariantCulture).AsSpan(1)}";
    }

    /// <summary>
    /// Check that the specified char fits in the bit mask.
    /// </summary>
    /// <param name="c">The char.</param>
    /// <param name="mask">The bit mask.</param>
    /// <param name="name">The char name. Used in the exception message.</param>
    /// <exception cref="ArgumentException">If the specified char dont fits in the mask.</exception>
    public static void CheckChar(char c, int mask, string name)
    {
        if ((c & ~mask) != 0)
        {
            ThrowInvalidChar(c, name);
        }
    }

    private static void ThrowInvalidChar(char c, string name) =>
        throw new ArgumentException($"Invalid {name}: {EncodeChar(c)}");
}
