// <copyright file="DumpTool.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using DxFeed.Graal.Net.Api;
using DxFeed.Graal.Net.Api.Osub;
using DxFeed.Graal.Net.Events;

namespace DxFeed.Graal.Net.Tools.Dump;

public abstract class DumpTool
{
    private static readonly StreamWriter Output = new(Console.OpenStandardOutput());

    public static void Run(IEnumerable<string> args)
    {
        var cmdArgs = new DumpArgs().ParseArgs(args);
        if (cmdArgs == null)
        {
            return;
        }

        using var endpoint = DXEndpoint
            .NewBuilder()
            .WithRole(DXEndpoint.Role.StreamFeed)
            .WithProperty(DXEndpoint.DXFeedWildcardEnableProperty, "true") // enabled by default.
            .WithProperties(Helper.ParseProperties(cmdArgs.Properties))
            .Build();

        using var sub = endpoint
            .GetFeed()
            .CreateSubscription(cmdArgs.Types == null
                ? IEventType.GetEventTypes()
                : Helper.ParseEventTypes(cmdArgs.Types));

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

        sub.AddSymbols(cmdArgs.Symbols == null
            ? new[] { WildcardSymbol.All }
            : Helper.ParseSymbols(cmdArgs.Symbols));

        endpoint.Connect(cmdArgs.Address);

        endpoint.AwaitNotConnected();
        endpoint.CloseAndAwaitTermination();
    }
}
