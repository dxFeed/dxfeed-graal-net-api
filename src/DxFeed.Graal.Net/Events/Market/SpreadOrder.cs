// <copyright file="SpreadOrder.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Native.Events;
using DxFeed.Graal.Net.Utils;

namespace DxFeed.Graal.Net.Events.Market;

/// <summary>
/// Spread order event is a snapshot for a full available market depth for all spreads
/// on a given underlying symbol. The collection of spread order events of a symbol
/// represents the most recent information that is available about spread orders on
/// the market at any given moment of time.
/// <para> Spread order is similar to a regular <see cref="Order"/>, but it has a
/// <see cref="SpreadSymbol"/> property that contains the symbol
/// of the actual spread that is being represented by spread order object.
/// <see cref="MarketEvent.EventSymbol"/> property contains the underlying symbol
/// that was used in subscription.
/// </para>
/// For more details see <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/event/market/SpreadOrder.html">Javadoc</a>.
/// </summary>
[EventCode(EventCodeNative.SpreadOrder)]
public class SpreadOrder : OrderBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SpreadOrder"/> class.
    /// </summary>
    public SpreadOrder()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpreadOrder"/> class with the specified event symbol.
    /// </summary>
    /// <param name="eventSymbol">The specified event symbol.</param>
    public SpreadOrder(string? eventSymbol)
        : base(eventSymbol)
    {
    }

    /// <summary>
    /// Gets or sets spread symbol of this event.
    /// </summary>
    public string? SpreadSymbol { get; set; }

    /// <summary>
    /// Returns string representation of this spread order event.
    /// </summary>
    /// <returns>The string representation.</returns>
    public override string ToString() =>
        "SpreadOrder{" + BaseFieldsToString() +
        ", spreadSymbol='" + StringUtil.EncodeNullableString(SpreadSymbol) + "'" +
        "}";
}
