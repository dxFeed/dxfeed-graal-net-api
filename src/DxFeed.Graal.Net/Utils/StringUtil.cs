// <copyright file="StringUtil.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;

namespace DxFeed.Graal.Net.Utils;

/// <summary>
/// Provides utility methods for working with strings.
/// </summary>
public static class StringUtil
{
    /// <summary>
    /// Encodes the specified nullable string.
    /// If the string is <see langword="null"/>, returns the string <c>"null"</c>;
    /// otherwise, returns the specified string.
    /// </summary>
    /// <param name="s">The specified string.</param>
    /// <returns>The specified string or the string <c>"null"</c>.</returns>
    public static string EncodeNullableString(string? s) =>
        s ?? "null";

    /// <summary>
    /// Encodes the specified char to a string.
    /// If the value of the char falls within the range of printable ASCII characters [32-126],
    /// returns a string containing that character; otherwise, returns the Unicode escape sequence (<c>"(\uxxxx)"</c>).
    /// For a null char, returns <c>"\0"</c>.
    /// </summary>
    /// <param name="c">The specified char.</param>
    /// <returns>Returns the encoded string.</returns>
    public static string EncodeChar(char c)
    {
        if (c >= 32 && c <= 126)
        {
            return c.ToString();
        }

        return c == 0 ? "\\0" : $"\\u{(int)c:x4}";
    }

    /// <summary>
    /// Checks that the specified char fits within the specified bit mask.
    /// </summary>
    /// <param name="c">The specified char.</param>
    /// <param name="mask">The specified bit mask.</param>
    /// <param name="name">The char name used in the exception message.</param>
    /// <exception cref="ArgumentException">If the specified char does not fit within the mask.</exception>
    public static void CheckChar(char c, int mask, string name)
    {
        if ((c & ~mask) != 0)
        {
            throw new ArgumentException($"Invalid {name}: {EncodeChar(c)}");
        }
    }
}
