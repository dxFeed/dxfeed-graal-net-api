// <copyright file="ConnectTool.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DxFeed.Graal.Net.Api;
using DxFeed.Graal.Net.Api.Osub;
using DxFeed.Graal.Net.Events.Market;

namespace DxFeed.Graal.Net.Tools.Connect;

internal abstract class ConnectTool
{
    private static readonly StreamWriter Output = new(Console.OpenStandardOutput());

    public static void Run(IEnumerable<string> args)
    {
        var cmdArgs = new ConnectArgs().ParseArgs(args);
        if (cmdArgs == null)
        {
            return;
        }

        using var endpoint = DXEndpoint
            .NewBuilder()
            .WithRole(DXEndpoint.Role.Feed)
            .WithProperties(Helper.ParseProperties(cmdArgs.Properties))
            .WithName(nameof(ConnectTool))
            .Build();

        using var sub = endpoint
            .GetFeed()
            .CreateSubscription(Helper.ParseEventTypes(cmdArgs.Types!));

        sub.AddEventListener(events =>
        {
            foreach (var e in events)
            {
                if (!cmdArgs.IsQuite)
                {
                    Output.WriteLine(e);
                }
            }

            Output.Flush();
        });


        IEnumerable<object> symbols = Helper.ParseSymbols(cmdArgs.Symbols!).ToList();
        if (cmdArgs.FromTime != null)
        {
            symbols = symbols.Select(s =>
                new TimeSeriesSubscriptionSymbol(s, (long)cmdArgs.FromTime!));
        }
        else if (cmdArgs.Source != null)
        {
            symbols = symbols.Select(s =>
                new IndexedEventSubscriptionSymbol(s, OrderSource.ValueOf(cmdArgs.Source)));
        }

        sub.AddSymbols(symbols);
        endpoint.Connect(cmdArgs.Address);

        Task.Delay(Timeout.Infinite).Wait();
    }
}
