// <copyright file="Order.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Native.Events;
using DxFeed.Graal.Net.Utils;

namespace DxFeed.Graal.Net.Events.Market;

/// <summary>
/// Order event is a snapshot for a full available market depth for a symbol.
/// The collection of order events of a symbol represents the most recent information
/// that is available about orders on the market at any given moment of time.
/// Order events give information on several levels of details, called scopes - see <see cref="Scope"/>.
/// Scope of an order is available via <see cref="OrderBase.Scope"/>property.
/// <br/>
/// For more details see <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/event/market/Order.html">Javadoc</a>.
/// </summary>
[EventCode(EventCodeNative.Order)]
public class Order : OrderBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Order"/> class.
    /// </summary>
    public Order()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Order"/> class with the specified event symbol.
    /// </summary>
    /// <param name="eventSymbol">The specified event symbol.</param>
    public Order(string? eventSymbol)
        : base(eventSymbol)
    {
    }

    /// <summary>
    /// Gets or sets market maker or other aggregate identifier of this order.
    /// This value is defined for <see cref="Scope.Aggregate"/> and <see cref="Scope.Order"/> orders.
    /// </summary>
    public string? MarketMaker { get; set; }

    /// <summary>
    /// Returns string representation of this order event.
    /// </summary>
    /// <returns>The string representation.</returns>
    public override string ToString() =>
        "Order{" + BaseFieldsToString() +
        ", marketMaker='" + StringUtil.EncodeNullableString(MarketMaker) + "'" +
        "}";
}
