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

        using var inputEndpoint = DXEndpoint
            .NewBuilder()
            .WithRole(DXEndpoint.Role.StreamFeed)
            .WithProperty(DXEndpoint.DXFeedWildcardEnableProperty, "true") // Enabled by default.
            .WithProperties(Helper.ParseProperties(cmdArgs.Properties))
            .WithName(nameof(DumpTool))
            .Build();

        using var sub = inputEndpoint
            .GetFeed()
            .CreateSubscription(cmdArgs.Types == null
                ? IEventType.GetEventTypes()
                : Helper.ParseEventTypes(cmdArgs.Types));

        if (!cmdArgs.IsQuite)
        {
            sub.AddEventListener(events =>
            {
                foreach (var e in events)
                {
                    Output.WriteLine(e);
                }

                Output.Flush();
            });
        }

        DXEndpoint? outputEndpoint = null;
        if (cmdArgs.Tape != null)
        {
            outputEndpoint = DXEndpoint
                .NewBuilder()
                .WithRole(DXEndpoint.Role.StreamPublisher)
                .WithProperty(DXEndpoint.DXFeedWildcardEnableProperty, "true") // Enabled by default.
                .WithProperties(Helper.ParseProperties(cmdArgs.Properties))
                .WithName(nameof(DumpTool))
                .Build()
                .Connect(cmdArgs.Tape.StartsWith("tape:") ? cmdArgs.Tape : $"tape:{cmdArgs.Tape}");

            sub.AddEventListener(events => outputEndpoint.GetPublisher().PublishEvents(events));
        }

        sub.AddSymbols(cmdArgs.Symbols == null
            ? new[] { WildcardSymbol.All }
            : Helper.ParseSymbols(cmdArgs.Symbols));

        inputEndpoint.Connect(cmdArgs.Address);

        inputEndpoint.AwaitNotConnected();
        inputEndpoint.CloseAndAwaitTermination();

        outputEndpoint?.AwaitProcessed();
        outputEndpoint?.CloseAndAwaitTermination();
    }
}
