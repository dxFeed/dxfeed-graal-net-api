// <copyright file="PriceType.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

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
    /// Preliminary price (preliminary settlement price), usually posted prior to {@link #FINAL} price.
    /// </summary>
    Preliminary,

    /// <summary>
    /// Final price (final settlement price).
    /// </summary>
    Final,
}
