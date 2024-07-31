// <copyright file="PriceLevel.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Events.Market;

namespace DxFeed.Graal.Net.Samples;

public class PriceLevel
{
    public PriceLevel(OrderBase order)
    {
        EventSymbol = order.EventSymbol;
        EventSource = order.EventSource;
        Side = order.OrderSide;
        Price = order.Price;
        Size = order.Size;
    }

    public PriceLevel(PriceLevel priceLevel)
    {
        EventSymbol = priceLevel.EventSymbol;
        EventSource = priceLevel.EventSource;
        Side = priceLevel.Side;
        Price = priceLevel.Price;
        Size = priceLevel.Size;
    }

    public string? EventSymbol { get; set; }

    public IndexedEventSource EventSource { get; set; }

    public Side Side { get; set; }

    public double Price { get; set; }

    public double Size { get; set; }

    public override string ToString() =>
        $"Side:{Side}, Price:{Price}, Size:{Size}";
}
