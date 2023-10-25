// <copyright file="BitUtil.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace DxFeed.Graal.Net.Utils;

/// <summary>
/// A collection of utility methods for bitwise operations.
/// <br/>
/// Ports the Java class <c>com.dxfeed.event.market.Util</c>.
/// </summary>
public static class BitUtil
{
    /// <summary>
    /// Extracts bits from the specified value.
    /// </summary>
    /// <param name="value">The specified value from which bits are extracted.</param>
    /// <param name="mask">The bit mask.</param>
    /// <param name="shift">The number of positions to shift the value.</param>
    /// <returns>The extracted bits.</returns>
    public static int GetBits(int value, int mask, int shift) =>
        (value >> shift) & mask;

    /// <summary>
    /// Sets bits in the specified value.
    /// </summary>
    /// <param name="value">The specified value in which bits are to be set.</param>
    /// <param name="mask">The bit mask.</param>
    /// <param name="shift">The number of positions to shift the value.</param>
    /// <param name="bits">The bits to be set.</param>
    /// <returns>A value with the specified bits set.</returns>
    public static int SetBits(int value, int mask, int shift, int bits) =>
        (value & ~(mask << shift)) | ((bits & mask) << shift);
}
