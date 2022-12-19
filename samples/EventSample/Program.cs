// <copyright file="Program.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using DxFeed.Graal.Net.Api;
using DxFeed.Graal.Net.Api.Osub;
using DxFeed.Graal.Net.Events;

namespace DxFeed.Graal.Net.Samples;

internal abstract class Program
{
    private const int HostIndex = 0;
    private const int EventIndex = 1;
    private const int SymbolIndex = 2;

    private static IEnumerable<Type> ParseEventTypes(string eventTypeNames)
    {
        if (eventTypeNames.Equals("feed", StringComparison.OrdinalIgnoreCase))
        {
            return IEventType.GetEventTypes();
        }

        var availableTypesDictionary = IEventType.GetEventTypes().ToDictionary(kvp => kvp.Name, kvp => kvp);
        var setTypes = new HashSet<Type>();
        foreach (var typeName in eventTypeNames.Split(','))
        {
            if (!availableTypesDictionary.TryGetValue(typeName, out var type))
            {
                var availableTypesStr = string.Join(", ", availableTypesDictionary.Select(x => x.Key));
                throw new ArgumentException(
                    $"{typeName} event type is not available! List of available event types: {availableTypesStr}.");
            }

            setTypes.Add(type);
        }

        return setTypes;
    }

    private static bool TryParseDateTimeParam(string stringParam, InputParameter<DateTimeOffset> param)
    {
        if (!DateTimeOffset.TryParse(stringParam, out var dateTimeValue))
        {
            return false;
        }

        param.Value = dateTimeValue;

        return true;
    }

    private static bool TryParseTaggedStringParam(string tag, string paramTagString, string paramString,
        InputParameter<string> param)
    {
        if (!paramTagString.Equals(tag, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        param.Value = paramString;

        return true;
    }

    public static void Main(string[] args)
    {
        if (args.Length is < 3 or > 7)
        {
            Console.WriteLine(
                "Usage: dxf_events_sample <host:port> <event> <symbol> [<date>] [-T <token>] [-p]\n" +
                "where\n" +
                "    host:port  - The address of dxFeed server demo.dxfeed.com:7300\n" +
                "    event      - Any of the {Order,Quote,Trade,TimeAndSale,TradeETH,SpreadOrder,AnalyticOrder}\n" +
                "    symbol     - IBM, MSFT, ...\n\n" +
                "    date       - The date of time series event in the format YYYY-MM-DD (optional)\n" +
                "    -T <token> - The authorization token\n" +
                "example: dxf_events_sample demo.dxfeed.com:7300 quote,trade MSFT.TEST,IBM.TEST\n" +
                "or: dxf_events_sample demo.dxfeed.com:7300 TimeAndSale MSFT,IBM 2016-10-10\n"
            );
            return;
        }

        var address = args[HostIndex];
        var eventTypes = ParseEventTypes(args[EventIndex]);
        IEnumerable<object> symbols = args[SymbolIndex].Split(',').ToList();
        var dateTime = new InputParameter<DateTimeOffset>(DateTimeOffset.MinValue);
        var token = new InputParameter<string>(null!);

        for (var i = SymbolIndex + 1; i < args.Length; i++)
        {
            if (!dateTime.IsSet && TryParseDateTimeParam(args[i], dateTime))
            {
                continue;
            }

            if (!token.IsSet && i < args.Length - 1 &&
                TryParseTaggedStringParam("-T", args[i], args[i + 1], token))
            {
                i++;
            }
        }

        using var endpoint = DXEndpoint.Create()
            .Connect(!token.IsSet ? address : address + $"[login=entitle:{token.Value}]");
        using var sub = endpoint.GetFeed()
            .CreateSubscription(eventTypes);
        sub.AddEventListener(events =>
        {
            foreach (var e in events)
            {
                Console.WriteLine(e);
            }
        });

        if (dateTime.IsSet)
        {
            symbols = symbols.Select(s =>
                new TimeSeriesSubscriptionSymbol(s, dateTime.Value.ToUnixTimeMilliseconds()));
        }

        sub.AddSymbols(symbols);

        Console.WriteLine("Press enter to stop");
        Console.ReadLine();
    }
}
