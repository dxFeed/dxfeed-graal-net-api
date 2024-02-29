// <copyright file="Program.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Threading;
using System.Threading.Tasks;
using DxFeed.Graal.Net.Api;
using DxFeed.Graal.Net.Events.Candles;

namespace DxFeed.Graal.Net.Samples;

/// <summary>
/// Fetches last 30 days of candles for a specified symbol, prints them, and exits.
/// </summary>
internal abstract class FetchDailyCandles
{
    private static void PrintUsage() =>
        Console.WriteLine($@"
Usage:
FetchDailyCandles <symbol>

Where:
    symbol - Is security symbol (e.g. AAPL{{=d}}, IBM{{=d}} etc.).");

    public static async Task Main(string[] args)
    {
        if (args.Length != 1)
        {
            PrintUsage();
            return;
        }

        var symbol = args[0];
        var feed = DXFeed.GetInstance();
        var from = DateTimeOffset.Now.AddDays(-30).ToUnixTimeMilliseconds();
        var candles = await feed.GetTimeSeriesAsync<Candle>(symbol, from, long.MaxValue);
        if (candles != null)
        {
            foreach (var candle in candles)
            {
                Console.WriteLine(candle);
            }
        }
    }

}
