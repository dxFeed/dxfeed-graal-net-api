// <copyright file="PriceLevelService.cs" company="Devexperts LLC">
// Copyright © 2025 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using DxFeed.Graal.Net.Events.Candles;
using DxFeed.Graal.Net.Events.Market;
using DxFeed.Graal.Net.Native.Orcs;

namespace DxFeed.Graal.Net.Orcs;

/// <summary>
/// A service that allows to subscribe to orders and quotes at a specified granularity.
/// </summary>
public class PriceLevelService : IDisposable
{
    /// <summary>
    /// Custom <see cref="CandleSymbol"/> attribute specifying granularity of price level updates in response.
    /// </summary>
    public const string GranularityAttributeKey = "gr";

    private readonly PriceLevelServiceHandle handle;

    /// <summary>
    /// Initializes a new instance of the <see cref="PriceLevelService"/> class.
    /// Also connects it to RMI endpoint.
    /// </summary>
    /// <param name="address">The RMI endpoint's address</param>
    public PriceLevelService(string address) => handle = PriceLevelServiceHandle.Create(address);

    /// <summary>
    /// Returns list of price levels for the specified <see cref="CandleSymbol"/> within passed <c>from</c> and <c>to</c> times.
    /// The events are ordered by <see cref="Order.Time">time</see> in the collection.
    /// <p/>
    /// Passed candle symbol shall contain supported <see cref="CandlePeriod"/> and custom attribute
    /// called granularity with the key <c>GranularityAttributeKey</c>. Granularity value shall be represented by
    /// integer value in seconds. Minimal value for granularity is 1 second.
    /// <p/>
    /// If passed <see cref="CandlePeriod"/> or granularity value are not supported by the service
    /// the empty list will be returned.
    /// </summary>
    /// <param name="candleSymbol">The <see cref="CandleSymbol"/> to request.</param>
    /// <param name="orderSource">The <see cref="OrderSource"/> to request.</param>
    /// <param name="from">From time in UTC</param>
    /// <param name="to">To time in UTC</param>
    /// <param name="caller">The caller identifier.</param>
    /// <returns>A list of <see cref="Order"/> events sorted in ascending order by time.</returns>
    public List<Order> GetOrders(
        CandleSymbol candleSymbol,
        OrderSource orderSource,
        DateTimeOffset from,
        DateTimeOffset to,
        string caller = "qdnet") =>
        handle.GetOrders(candleSymbol, orderSource, from, to, caller);

    /// <summary>
    /// Returns available to the client order sources and symbols for each <see cref="OrderSource"/>. Order source and symbols
    /// are filtered according to the client permissions. Symbols and order sources view is built as of now, e.g.
    /// the response contains only existing data (for example, no symbols that were delisted)
    /// </summary>
    /// <returns>The AuthOrderSource instance.</returns>
    public AuthOrderSource GetAuthOrderSource() => new(handle.GetAuthOrderSource());

    /// <summary>
    /// Returns list of quotes for the specified <see cref="CandleSymbol"/> within passed <c>from</c> and <c>to</c> times.
    /// The quotes are ordered by <see cref="Quote.Time">time</see> in the collection.
    /// </summary>
    /// <param name="candleSymbol">The <see cref="CandleSymbol"/> to request.</param>
    /// <param name="from">From time in UTC</param>
    /// <param name="to">To time in UTC</param>
    /// <param name="caller">The caller identifier.</param>
    /// <returns>A list of <see cref="Quote"/> events sorted in ascending order by time.</returns>
    public List<Quote> GetQuotes(CandleSymbol candleSymbol, DateTimeOffset from, DateTimeOffset to, string caller = "qdnet") =>
        handle.GetQuotes(candleSymbol, from, to, caller);

    /// <summary>
    /// Closes (disconnects) this service.
    /// </summary>
    public void Close() => handle.Close();

    /// <inheritdoc />
    public void Dispose()
    {
        Close();
        GC.SuppressFinalize(this);
    }
}
