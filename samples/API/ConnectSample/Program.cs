// <copyright file="Program.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using DxFeed.Graal.Net.Api;
using DxFeed.Graal.Net.Api.Osub;
using DxFeed.Graal.Net.Utils;

namespace DxFeed.Graal.Net.Samples;

/// <summary>
/// This sample that shows how to subscribe to events.
/// The sample configures via command line, connect to endpoint, subscribes to events and prints received data.
/// </summary>
internal abstract class Program
{
    private static void PrintUsage()
    {
        var eventTypeNames = ReflectionUtil.CreateTypesString(DXEndpoint.GetEventTypes());
        var usageString = $@"
Usage:
<address> <types> <symbols> [<time>]

Where:
    address - The address to connect to retrieve data (remote host or local tape file).
              To pass an authorization token, add to the address: ""[login=entitle:<token>]"",
              e.g.: demo.dxfeed.com:7300[login=entitle:<token>]
    types   - Is comma-separated list of dxfeed event types ({eventTypeNames}).
    symbol  - Is comma-separated list of symbol names to get events for (e.g. ""IBM,AAPL,MSFT"").
              for Candle event specify symbol with aggregation like in ""AAPL{{=d}}""
    time    - Is from-time for history subscription in standard formats.
              Same examples of valid from-time:
                  20070101-123456
                  20070101-123456.123
                  2005-12-31 21:00:00
                  2005-12-31 21:00:00.123+03:00
                  2005-12-31 21:00:00.123+0400
                  2007-11-02Z
                  123456789 - value-in-milliseconds

Examples:
    demo.dxfeed.com:7300 Quote,Trade MSFT,IBM
    demo.dxfeed.com:7300 TimeAndSale AAPL
    demo.dxfeed.com:7300 Candle AAPL{{=d}} 20230201Z";
        Console.WriteLine(usageString);
    }

    public static void Main(string[] args)
    {
        if (args.Length < 3)
        {
            PrintUsage();
            return;
        }

        // Parses input arg.
        var address = args[0];
        var types = CmdArgsUtil.ParseTypes(args[1]);
        var symbols = (IEnumerable<object>)CmdArgsUtil.ParseSymbols(args[2]);
        DateTimeOffset? time = args.Length > 3 ? CmdArgsUtil.ParseFromTime(args[3]) : null;
        // Create endpoint and connect to specified address.
        using var endpoint = DXEndpoint.Create().Connect(address);

        // Create subscription with specified types attached to feed.
        using var sub = endpoint.GetFeed().CreateSubscription(types);

        // Adds event listener.
        sub.AddEventListener(events =>
        {
            foreach (var e in events)
            {
                Console.WriteLine(e);
            }
        });

        // If a time argument was provided, creates a TimeSeriesSubscriptionSymbol with the specified from-time.
        if (time != null)
        {
            symbols = symbols.Select(s => new TimeSeriesSubscriptionSymbol(s, time.Value));
        }

        // Adds symbols to subscription.
        sub.AddSymbols(symbols);

        Console.ReadLine();
    }
}
