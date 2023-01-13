// <copyright file="PriceType.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Utils;
using static DxFeed.Graal.Net.Events.Market.PriceType;

namespace DxFeed.Graal.Net.Events.Market;

/// <summary>
/// Type of the price value.
/// </summary>
public enum PriceType
{
    /// <summary>
    /// Regular price.
    /// </summary>
    Regular,

    /// <summary>
    /// Indicative price (derived via math formula).
    /// </summary>
    Indicative,

    /// <summary>
    /// Preliminary price (preliminary settlement price), usually posted prior to <see cref="Final"/>price.
    /// </summary>
    Preliminary,

    /// <summary>
    /// Final price (final settlement price).
    /// </summary>
    Final,
}

/// <summary>
/// Class extension for <see cref="PriceType"/> enum.
/// </summary>
internal static class PriceTypeExt
{
    private static readonly PriceType[] Values = EnumUtil.BuildEnumBitMaskArrayByValue(Regular);

    /// <summary>
    /// Returns an enum constant of the <see cref="PriceType"/> by integer code bit pattern.
    /// </summary>
    /// <param name="value">The specified value.</param>
    /// <returns>The enum constant of the specified enum type with the specified value.</returns>
    public static PriceType ValueOf(int value) =>
        Values[value];
}
