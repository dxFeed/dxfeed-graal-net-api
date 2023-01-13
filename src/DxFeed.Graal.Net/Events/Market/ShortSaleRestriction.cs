// <copyright file="ShortSaleRestriction.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Utils;
using static DxFeed.Graal.Net.Events.Market.ShortSaleRestriction;

namespace DxFeed.Graal.Net.Events.Market;

/// <summary>
/// Short sale restriction on an instrument.
/// </summary>
public enum ShortSaleRestriction
{
    /// <summary>
    /// Short sale restriction is undefined, unknown or inapplicable.
    /// </summary>
    Undefined,

    /// <summary>
    /// Short sale restriction is active.
    /// </summary>
    Active,

    /// <summary>
    /// Short sale restriction is inactive.
    /// </summary>
    Inactive,
}

/// <summary>
/// Class extension for <see cref="ShortSaleRestriction"/> enum.
/// </summary>
internal static class ShortSaleRestrictionExt
{
    private static readonly ShortSaleRestriction[] Values = EnumUtil.BuildEnumBitMaskArrayByValue(Undefined);

    /// <summary>
    /// Returns an enum constant of the <see cref="ShortSaleRestriction"/> by integer code bit pattern.
    /// </summary>
    /// <param name="value">The specified value.</param>
    /// <returns>The enum constant of the specified enum type with the specified value.</returns>
    public static ShortSaleRestriction ValueOf(int value) =>
        Values[value];
}
