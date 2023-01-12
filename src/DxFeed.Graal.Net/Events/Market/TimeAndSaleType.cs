// <copyright file="TimeAndSaleType.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Utils;
using static DxFeed.Graal.Net.Events.Market.TimeAndSaleType;

namespace DxFeed.Graal.Net.Events.Market;

/// <summary>
/// Type of a time and sale event.
/// <br/>
/// For more details see <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/event/market/TimeAndSaleType.html">Javadoc</a>.
/// </summary>
public enum TimeAndSaleType
{
    /// <summary>
    /// Represents new time and sale event.
    /// </summary>
    New,

    /// <summary>
    /// Represents correction time and sale event.
    /// </summary>
    Correction,

    /// <summary>
    /// Represents cancel time and sale event.
    /// </summary>
    Cancel,
}

/// <summary>
/// Class extension for <see cref="TimeAndSaleType"/> enum.
/// </summary>
internal static class TimeAndSaleTypeExt
{
    private static readonly TimeAndSaleType[] Values = EnumUtil.BuildEnumBitMaskArrayByValue(New);

    /// <summary>
    /// Returns an enum constant of the <see cref="TimeAndSaleType"/> with the specified value.
    /// </summary>
    /// <param name="value">The specified value.</param>
    /// <returns>The enum constant of the specified enum type with the specified value.</returns>
    public static TimeAndSaleType ValueOf(int value) =>
        Values[value];
}
