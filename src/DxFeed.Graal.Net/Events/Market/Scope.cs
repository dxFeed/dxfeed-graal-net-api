// <copyright file="Scope.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Utils;
using static DxFeed.Graal.Net.Events.Market.Scope;

namespace DxFeed.Graal.Net.Events.Market;

/// <summary>
/// Scope of an order.
/// </summary>
public enum Scope
{
    /// <summary>
    /// Represents best bid or best offer for the whole market.
    /// </summary>
    Composite,

    /// <summary>
    /// Represents best bid or best offer for a given exchange code.
    /// </summary>
    Regional,

    /// <summary>
    /// Represents aggregate information for a given price level or
    /// best bid or best offer for a given market maker.
    /// </summary>
    Aggregate,

    /// <summary>
    /// Represents individual order on the market.
    /// </summary>
    Order,
}

/// <summary>
/// Class extension for <see cref="Scope"/> enum.
/// </summary>
internal static class ScopeExt
{
    private static readonly Scope[] Values = EnumUtil.BuildEnumBitMaskArrayByValue(Composite);

    /// <summary>
    /// Returns an enum constant of the <see cref="Scope"/> with the specified value.
    /// </summary>
    /// <param name="value">The specified value.</param>
    /// <returns>The enum constant of the specified enum type with the specified value.</returns>
    public static Scope ValueOf(int value) =>
        Values[value];
}
