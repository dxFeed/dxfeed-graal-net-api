// <copyright file="PriceLevel.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Events.Market;
using DxFeed.Graal.Net.Utils;

namespace PriceLevelBookSample;

/// <summary>
/// Represents a price level in a price level book.
/// </summary>
public class PriceLevel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PriceLevel"/> class using an order.
    /// </summary>
    /// <param name="order">The order to initialize the price level from.</param>
    public PriceLevel(OrderBase order)
    {
        EventSymbol = order.EventSymbol;
        EventSource = order.EventSource;
        Side = order.OrderSide;
        Price = order.Price;
        Size = order.Size;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PriceLevel"/> class by copying an existing price level.
    /// </summary>
    /// <param name="priceLevel">The price level to copy.</param>
    public PriceLevel(PriceLevel priceLevel)
    {
        EventSymbol = priceLevel.EventSymbol;
        EventSource = priceLevel.EventSource;
        Side = priceLevel.Side;
        Price = priceLevel.Price;
        Size = priceLevel.Size;
    }

    /// <summary>
    /// Gets or sets the event symbol.
    /// </summary>
    public string? EventSymbol { get; set; }

    /// <summary>
    /// Gets or sets the event source.
    /// </summary>
    public IndexedEventSource EventSource { get; set; }

    /// <summary>
    /// Gets or sets the side (buy or sell) of the price level.
    /// </summary>
    public Side Side { get; set; }

    /// <summary>
    /// Gets or sets the price of the price level.
    /// </summary>
    public double Price { get; set; }

    /// <summary>
    /// Gets or sets the size of the price level.
    /// </summary>
    public double Size { get; set; }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString() =>
        "PriceLevel{" + StringUtil.EncodeNullableString(EventSymbol) +
        ", source=" + StringUtil.EncodeNullableString(EventSource.ToString()) +
        ", side=" + Side +
        ", price=" + Price +
        ", size=" + Size +
        "}";
}
