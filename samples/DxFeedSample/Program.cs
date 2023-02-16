// <copyright file="Program.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Threading;
using System.Threading.Tasks;
using DxFeed.Graal.Net.Api;
using DxFeed.Graal.Net.Events.Market;

namespace DxFeed.Graal.Net.Samples;

/// <summary>
/// Creates multiple event listener and subscribe to Quote and Trade events.
/// Use default DXFeed instance for that data feed address is defined by "dxfeed.properties" file.
/// </summary>
internal abstract class DxFeedSample
{
    public static async Task Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine($@"
Usage:
DxFeedSample <symbol>

Where:
    symbol - Is security symbol (e.g. IBM, AAPL, SPX etc.).");
            return;
        }

        var symbol = args[0];
        TestQuoteListener(symbol);
        TestQuoteAndTradeListener(symbol);

        await Task.Delay(Timeout.Infinite);
    }

    private static void TestQuoteListener(string symbol)
    {
        var sub = DXFeed.GetInstance().CreateSubscription(typeof(Quote));
        sub.AddEventListener(events =>
        {
            foreach (var e in events)
            {
                if (e is Quote quote)
                {
                    Console.WriteLine($"Mid = {((quote.BidPrice + quote.AskPrice) / 2)}");
                }
            }
        });
        sub.AddSymbols(symbol);
    }

    private static void TestQuoteAndTradeListener(string symbol)
    {
        var sub = DXFeed.GetInstance().CreateSubscription(typeof(Quote), typeof(Trade));
        sub.AddEventListener(events =>
        {
            foreach (var e in events)
            {
                Console.WriteLine(e);
            }
        });
        sub.AddSymbols(symbol);
    }
}
