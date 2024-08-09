// <copyright file="MultipleMarketDepthModel.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using DxFeed.Graal.Net.Api;
using DxFeed.Graal.Net.Api.Osub;
using DxFeed.Graal.Net.Events.Market;

namespace DxFeed.Graal.Net.Samples;

/// <summary>
/// This sample program demonstrates how to use the <see cref="MultipleMarketDepthModel{T}"/>
/// to manage and display order books for multiple symbols.
/// </summary>
[SuppressMessage("Globalization", "CA1305:Specify IFormatProvider")]
internal abstract class Program
{
    private static void Main(string[] args)
    {
        var symbol = new IndexedEventSubscriptionSymbol("AAPL", OrderSource.ntv);
        var model = new MultipleMarketDepthModel<Order>.Builder()
            .WithFeed(DXFeed.GetInstance())
            .WithDepthLimit(10)
            .WithAggregationPeriod(TimeSpan.FromSeconds(10))
            .WithListener(PrintBook)
            .Build();
        model.AddSymbol(symbol);
        model.AddSymbol(new IndexedEventSubscriptionSymbol("TSLA", OrderSource.ntv));

        Console.WriteLine($"Press any key to print {symbol.EventSymbol} order book manually.");
        while (true)
        {
            Console.ReadKey(true);
            if (model.TryGetBook(symbol, out var book))
            {
                Console.WriteLine("=============================Print manually=============================");
                PrintBook(symbol, book);
                Console.WriteLine("========================================================================");
            }
        }
    }

    private static void PrintBook(IndexedEventSubscriptionSymbol symbol, OrderBook<Order> book)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{symbol.EventSymbol}:");

        var maxCount = Math.Max(book.Buy.Count, book.Sell.Count);
        for (var i = 0; i < maxCount; i++)
        {
            var buyTable = i < book.Buy.Count
                ? $"Buy  [Source: {book.Buy[i].EventSource}, Size: {book.Buy[i].Size,5}, Price: {book.Buy[i].Price,8}]"
                : "Buy  [None]";

            var sellTable = i < book.Sell.Count
                ? $"Sell [Source: {book.Sell[i].EventSource}, Size: {book.Sell[i].Size,5}, Price: {book.Sell[i].Price,8}]"
                : "Sell [None]";

            sb.AppendLine($"{buyTable} \t {sellTable}");
        }

        Console.WriteLine(sb.ToString());
    }
}
