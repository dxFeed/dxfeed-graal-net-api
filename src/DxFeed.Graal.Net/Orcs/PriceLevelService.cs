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
    ///
    /// </summary>
    /// <param name="candleSymbol"></param>
    /// <param name="orderSource"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="caller"></param>
    /// <returns></returns>
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
