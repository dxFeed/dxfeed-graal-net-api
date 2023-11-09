// <copyright file="DumpTool.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.IO;
using DxFeed.Graal.Net.Api;
using DxFeed.Graal.Net.Api.Osub;
using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Tools.Attributes;

namespace DxFeed.Graal.Net.Tools.Dump;

[ToolInfo(
    "Dump",
    ShortDescription = "Dumps all events received from address.",
    Description = """
    Dumps all events received from address.
    Enforces a streaming contract for subscription. A wildcard enabled by default.
    This was designed to receive data from a file.
    If <types> is not specified, creates a subscription for all available event types.
    If <symbol> is not specified, the wildcard symbol is used.
    """,
    Usage = new[]
    {
        "Dump <address> [<options>]",
        "Dump <address> <types> [<options>]",
        "Dump <address> <types> <symbols> [<options>]",
    })]
public class DumpTool : AbstractTool<DumpArgs>
{
    private static readonly StreamWriter Output = new(Console.OpenStandardOutput());

    public override void Run(DumpArgs args)
    {
        SystemProperty.SetProperties(ParseProperties(args.Properties));
        using var inputEndpoint = DXEndpoint
            .NewBuilder()
            .WithRole(DXEndpoint.Role.StreamFeed)
            .WithProperty(DXEndpoint.DXFeedWildcardEnableProperty, "true") // Enabled by default.
            .WithProperties(ParseProperties(args.Properties))
            .WithName(nameof(DumpTool))
            .Build();

        using var sub = inputEndpoint
            .GetFeed()
            .CreateSubscription(args.Types == null
                ? IEventType.GetEventTypes()
                : ParseEventTypes(args.Types));

        if (!args.IsQuite)
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
        if (args.Tape != null)
        {
            outputEndpoint = DXEndpoint
                .NewBuilder()
                .WithRole(DXEndpoint.Role.StreamPublisher)
                .WithProperty(DXEndpoint.DXFeedWildcardEnableProperty, "true") // Enabled by default.
                .WithProperties(ParseProperties(args.Properties))
                .WithName(nameof(DumpTool))
                .Build()
                .Connect(args.Tape.StartsWith("tape:") ? args.Tape : $"tape:{args.Tape}");

            sub.AddEventListener(events => outputEndpoint.GetPublisher().PublishEvents(events));
        }

        sub.AddSymbols(args.Symbols == null
            ? new[] { WildcardSymbol.All }
            : ParseSymbols(args.Symbols));

        inputEndpoint.Connect(args.Address);

        inputEndpoint.AwaitNotConnected();
        inputEndpoint.CloseAndAwaitTermination();

        outputEndpoint?.AwaitProcessed();
        outputEndpoint?.CloseAndAwaitTermination();
    }
}
