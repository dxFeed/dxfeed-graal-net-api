// <copyright file="Scope.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

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
