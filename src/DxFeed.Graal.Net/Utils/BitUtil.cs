// <copyright file="BitUtil.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace DxFeed.Graal.Net.Utils;

/// <summary>
/// A collection of utility methods for bitwise operations.
/// <br/>
/// Porting Java class <c>com.dxfeed.event.market.Util</c>.
/// </summary>
public static class BitUtil
{
    /// <summary>
    /// Extracts bits from the specified value.
    /// </summary>
    /// <param name="value">The specified value.</param>
    /// <param name="mask">The bit mask.</param>
    /// <param name="shift">The bit shift.</param>
    /// <returns>The extracted bits.</returns>
    public static int GetBits(int value, int mask, int shift) =>
        (value >> shift) & mask;

    /// <summary>
    /// Sets bits to the specified value.
    /// </summary>
    /// <param name="value">The specified value.</param>
    /// <param name="mask">The bit mask.</param>
    /// <param name="shift">The bit shift.</param>
    /// <param name="bits">The bits set.</param>
    /// <returns>Returns a value with bits set.</returns>
    public static int SetBits(int value, int mask, int shift, int bits) =>
        (value & ~(mask << shift)) | ((bits & mask) << shift);
}
