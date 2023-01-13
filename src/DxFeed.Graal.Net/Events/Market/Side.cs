// <copyright file="Side.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Utils;
using static DxFeed.Graal.Net.Events.Market.Side;

namespace DxFeed.Graal.Net.Events.Market;

/// <summary>
/// Side of an order or a trade.
/// <br/>
/// For more details see <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/event/market/Side.html">Javadoc</a>.
/// </summary>
public enum Side
{
    /// <summary>
    /// Side is undefined, unknown or inapplicable.
    /// </summary>
    Undefined,

    /// <summary>
    /// Buy side (bid).
    /// </summary>
    Buy,

    /// <summary>
    /// Sell side (ask or offer).
    /// </summary>
    Sell,
}

/// <summary>
/// Class extension for <see cref="Side"/> enum.
/// </summary>
internal static class SideExt
{
    private static readonly Side[] Values = EnumUtil.BuildEnumBitMaskArrayByValue(Undefined);

    /// <summary>
    /// Returns an enum constant of the <see cref="Side"/> by integer code bit pattern.
    /// </summary>
    /// <param name="value">The specified value.</param>
    /// <returns>The enum constant of the specified enum type with the specified value.</returns>
    public static Side ValueOf(int value) =>
        Values[value];
}
