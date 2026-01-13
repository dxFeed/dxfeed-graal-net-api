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
    /// </summary>
    /// <param name="candleSymbol"><see cref="CandleSymbol"/> to request.</param>
    /// <param name="orderSource"><see cref="OrderSource"/> to request.</param>
    /// <param name="from">From time in UTC</param>
    /// <param name="to">To time in UTC</param>
    /// <param name="caller">The caller identifier.</param>
    /// <returns>A list of <see cref="Order"/> events sorted in ascending order by time.</returns>
    public List<Order> GetOrders(
        CandleSymbol candleSymbol,
        OrderSource orderSource,
        TimeSpan from,
        TimeSpan to,
        string caller) =>
        handle.GetOrders(candleSymbol, orderSource, from, to, caller);

    /// <summary>
    ///
    /// </summary>
    /// <param name="candleSymbol"></param>
    /// <param name="orderSource"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public List<Order> GetOrders(CandleSymbol candleSymbol, OrderSource orderSource, TimeSpan from, TimeSpan to) =>
        handle.GetOrders(candleSymbol, orderSource, from, to);

    /// <summary>
    /// Returns available to the client order sources and symbols for each <see cref="OrderSource"/>. Order source and symbols
    /// are filtered according to the client permissions. Symbols and order sources view is built as of now, e.g.
    /// the response contains only existing data (for example, no symbols that were delisted)
    /// </summary>
    /// <returns>The AuthOrderSource instance.</returns>
    public AuthOrderSource GetAuthOrderSource() => new(handle.GetAuthOrderSource());

    /// <summary>
    ///
    /// </summary>
    /// <param name="candleSymbol"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="caller"></param>
    /// <returns></returns>
    public List<Quote> GetQuotes(CandleSymbol candleSymbol, TimeSpan from, TimeSpan to, string caller) =>
        handle.GetQuotes(candleSymbol, from, to, caller);

    /// <summary>
    ///
    /// </summary>
    /// <param name="candleSymbol"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public List<Quote> GetQuotes(CandleSymbol candleSymbol, TimeSpan from, TimeSpan to) =>
        handle.GetQuotes(candleSymbol, from, to);

    /// <summary>
    /// Closes (disconnects) this service.
    /// </summary>
    public void Close() => handle.Close();

    /// <inheritdoc />
    public void Dispose() => Close();
}
