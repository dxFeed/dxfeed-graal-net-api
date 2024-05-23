// <copyright file="Program.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using DxFeed.Graal.Net.Api;
using DxFeed.Graal.Net.Utils;

namespace DxFeed.Graal.Net.Samples;

/// <summary>
/// Reads events form a tape file.
/// </summary>
internal abstract class DxFeedFileParser
{
    private static int _eventCounter;

    private static void PrintUsage()
    {
        var eventTypeNames = ReflectionUtil.CreateTypesString(DXEndpoint.GetEventTypes());
        var usageString = $@"
Usage:
DxFeedFileParser <file> <type> <symbol>

Where:
    file   - Is a file name.
    type   - Is comma-separated list of dxfeed event types ({eventTypeNames}).
    symbol - Is comma-separated list of symbol names to get events for (e.g. ""IBM,AAPL,MSFT"").";
        Console.WriteLine(usageString);
    }

    public static void Main(string[] args)
    {
        if (args.Length != 3)
        {
            PrintUsage();
            return;
        }

        // Parse args.
        var argFile = args[0];
        var argType = CmdArgsUtil.ParseTypes(args[1]);
        var argSymbol = CmdArgsUtil.ParseSymbols(args[2]);

        // Create endpoint specifically for file parsing.
        var endpoint = DXEndpoint.Create(DXEndpoint.Role.StreamFeed);
        var feed = endpoint.GetFeed();

        // Subscribe to a specified event and symbol.
        var sub = feed.CreateSubscription(argType);
        sub.AddEventListener(events =>
        {
            foreach (var e in events)
            {
                Console.WriteLine($"{++_eventCounter}: {e}");
            }
        });

        // Add symbols.
        sub.AddSymbols(argSymbol);

        // Connect endpoint to a file.
        endpoint.Connect($"file:{argFile}[speed=max]");

        // Wait until file is completely parsed.
        endpoint.AwaitNotConnected();

        // Close endpoint when we're done.
        // This method will gracefully close endpoint, waiting while data processing completes.
        endpoint.CloseAndAwaitTermination();
    }
}
