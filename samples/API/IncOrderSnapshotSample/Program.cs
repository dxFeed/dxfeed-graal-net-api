// <copyright file="Program.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DxFeed.Graal.Net.Api;
using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Events.Market;
using DxFeed.Graal.Net.Models;

namespace DxFeed.Graal.Net.Samples;

internal abstract class Program
{
    public static async Task Main(string[] args)
    {
        var symbol = "AAPL";
        var recordPrintLimit = 10;

        var eventPrinter = new EventPrinter(recordPrintLimit);

        // Build the model with the specified configurations.
        using var model = IndexedTxModel<Order>.NewBuilder()
            .WithBatchProcessing(true) // Enable batch processing.
            .WithSnapshotProcessing(true) // Enable snapshot processing.
            .WithFeed(DXFeed.GetInstance()) // Attach the model to the DXFeed instance.
            .WithSymbol(symbol) // Set the symbol to subscribe to.
            .WithSources(OrderSource.NTV) // Set the source for indexed events.
            .WithListener((source, events, isSnapshot) =>
                eventPrinter.Print(symbol, source, events.ToList(), isSnapshot))
            .Build();

        Console.WriteLine($"Subscribed to market depth updates for {symbol}. Press Ctrl+C to exit.");

        // Keep the application running indefinitely.
        await Task.Delay(Timeout.Infinite);
    }

    private sealed class EventPrinter
    {
        private readonly int _recordsPrintLimit;

        public EventPrinter(int recordsPrintLimit) =>
            _recordsPrintLimit = recordsPrintLimit;

        public void Print(string symbol, IndexedEventSource source, List<Order> events, bool isSnapshot)
        {
            Console.WriteLine(
                "Snapshot {{Symbol: '{0}', Source: '{1}', Type: {2}}}",
                symbol,
                source,
                isSnapshot ? "full" : "update");

            var count = 0;
            foreach (var order in events)
            {
                if (!EventFlags.IsRemove(order) && order.HasSize)
                {
                    Console.WriteLine($"    {order}");
                }
                else
                {
                    Console.WriteLine($"    REMOVAL {{ Index: 0x{order.Index:x} }}");
                }

                if (++count < _recordsPrintLimit || _recordsPrintLimit == 0)
                {
                    continue;
                }

                Console.WriteLine("   {{ ... {0} records left ...}}", events.Count - count);
                break;
            }
        }
    }
}
