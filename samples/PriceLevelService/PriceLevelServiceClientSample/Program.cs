// <copyright file="Program.cs" company="Devexperts LLC">
// Copyright © 2026 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Diagnostics;
using DxFeed.Graal.Net.Events.Candles;
using DxFeed.Graal.Net.Events.Market;
using DxFeed.Graal.Net.Orcs;
using DxFeed.Graal.Net.Utils;

namespace DxFeed.Graal.Net.Samples;

/// <summary>
/// This sample that shows how to use ORCS API by PriceLevelService.
/// </summary>
internal abstract class Program
{
    private static void PrintUsage()
    {
        const string usageString = $@"
Usage:
<address> <symbol> <source> <from> <to> [--validate] [--verbose]

Where:
    address    - The server RMI address. To pass an authorization token, add to the address: ""[login=entitle:<token>]"":,
                 e.g.: <address>[login=entitle:<token>]
    symbol     - Is symbol to retrieve, for example 'AAPL{{=d,gr=s}}'
    source     - Is source to retrieve, for example 'NTV'
    from       - Is from-time for history subscription in standard formats.
                 Same examples of valid from-time:
                  20070101-123456
                  20070101-123456.123
                  2005-12-31 21:00:00
                  2005-12-31 21:00:00.123+03:00
                  2005-12-31 21:00:00.123+0400
                  2007-11-02Z
                  123456789 - value-in-milliseconds
    to         - Is to-time.
    --validate

Examples:
    orcs.dxfeed.com:7777 AAPL{{=d,gr=s}} NTV 20260110-100000 20260110-230000 --validate --verbose
    ""orcs.dxfeed.com:7777[login=entitle:<token>]"" AAPL{{=d,gr=s}} NTV 20260110-100000 20260110-230000 --validate --verbose
";
        Console.WriteLine(usageString);
    }

    public static void Main(string[] args)
    {
        if (args.Length < 5)
        {
            PrintUsage();
            return;
        }

        // Parses input arg.
        var address = args[0];
        var symbol = CandleSymbol.ValueOf(args[1]);
        var source = OrderSource.ValueOf(args[2]);
        var fromTime = CmdArgsUtil.ParseFromTime(args[3]);
        var toTime = CmdArgsUtil.ParseFromTime(args[4]);
        var timeGapBound = TimeSpan.FromMinutes(1);
        var printQuotes = true;

        var argsSet = args.ToHashSet();
        var validate = false;
        var verbose = false;

        if (argsSet.Contains("--validate"))
        {
            validate = true;
        }

        if (argsSet.Contains("--verbose"))
        {
            verbose = true;
        }

        using var service = new PriceLevelService(address);
        var authOrderSource = service.GetAuthOrderSource();
        var symbolsBySource = authOrderSource.GetByOrderSources();

        Console.WriteLine($"Authorized sources: [{string.Join(", ", symbolsBySource.Keys)}]");

        if (verbose)
        {
            foreach (var sourceAndSymbols in symbolsBySource)
            {
                Console.WriteLine($"{sourceAndSymbols.Key}: [{string.Join(", ", sourceAndSymbols.Value)}]");
            }
        }

        var stopwatch = Stopwatch.StartNew();
        var orders = service.GetOrders(symbol, source, fromTime, toTime, "sample");

        stopwatch.Stop();
        Console.WriteLine($"Received {orders.Count} orders in {stopwatch.ElapsedMilliseconds} ms");

        if (validate)
        {
            PriceLevelChecker.Validate(orders, timeGapBound, printQuotes);
        }

        stopwatch.Restart();

        var quotes = service.GetQuotes(CandleSymbol.ValueOf(symbol.BaseSymbol + "{gr=s}"), fromTime, toTime, "sample");

        stopwatch.Stop();
        Console.WriteLine($"Received {quotes.Count} quotes in {stopwatch.ElapsedMilliseconds} ms");
    }
}
