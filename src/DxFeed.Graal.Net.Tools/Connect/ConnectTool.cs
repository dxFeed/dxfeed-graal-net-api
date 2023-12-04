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
using DxFeed.Graal.Net.Tools.Attributes;
using DxFeed.Graal.Net.Utils;

namespace DxFeed.Graal.Net.Tools.Connect;

[ToolInfo(
    "Connect",
    ShortDescription = "Connects to specified address(es).",
    Description = "Connects to the specified address(es) and subscribes to the specified symbols.",
    Usage = new[] { "Connect <address> <types> <symbols> [<options>]" })]
public sealed class ConnectTool : AbstractTool<ConnectArgs>, IDisposable
{
    private readonly StreamWriter _output = new(Console.OpenStandardOutput());

    public override void Run(ConnectArgs args)
    {
        SystemProperty.SetProperties(ParseProperties(args.Properties));
        using var endpoint = DXEndpoint
            .NewBuilder()
            .WithRole(args.ForceStream ? DXEndpoint.Role.StreamFeed : DXEndpoint.Role.Feed)
            .WithProperties(ParseProperties(args.Properties))
            .WithName(nameof(ConnectTool))
            .Build();

        using var sub = endpoint
            .GetFeed()
            .CreateSubscription(ParseEventTypes(args.Types!));

        if (!args.IsQuite)
        {
            sub.AddEventListener(events =>
            {
                foreach (var e in events)
                {
                    _output.WriteLine(e);
                }

                _output.Flush();
            });
        }

        IEnumerable<object> symbols = ParseSymbols(args.Symbols!).ToList();
        if (args.FromTime != null)
        {
            var fromTime = CmdArgsUtil.ParseFromTime(args.FromTime);
            symbols = symbols.Select(s => new TimeSeriesSubscriptionSymbol(s, fromTime));
        }
        else if (args.Source != null)
        {
            symbols = symbols.Select(s =>
                new IndexedEventSubscriptionSymbol(s, OrderSource.ValueOf(args.Source)));
        }

        if (args.Tape != null)
        {
            var pub = DXEndpoint
                .NewBuilder()
                .WithRole(DXEndpoint.Role.StreamPublisher)
                .WithProperty(DXEndpoint.DXFeedWildcardEnableProperty, "true") // Enabled by default.
                .WithProperties(ParseProperties(args.Properties))
                .WithName(nameof(ConnectTool))
                .Build()
                .Connect(args.Tape.StartsWith("tape:") ? args.Tape : $"tape:{args.Tape}").GetPublisher();

            sub.AddEventListener(pub.PublishEvents);
        }

        sub.AddSymbols(symbols);

        endpoint.Connect(args.Address);

        Task.Delay(Timeout.Infinite).Wait();
    }

    public void Dispose() =>
        _output.Dispose();
}
