// <copyright file="Program.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Api;
using DxFeed.Graal.Net.Events.Market;

namespace DxFeed.Graal.Net.Samples;

internal abstract class Program
{
    /// <summary>
    /// Write events to a tape file.
    /// </summary>
    public static void Main(string[] args)
    {
        // Create an appropriate endpoint.
        using var endpoint = DXEndpoint
            .NewBuilder()
            // Is required for tape connector to be able to receive everything.
            .WithProperty(DXEndpoint.DXFeedWildcardEnableProperty, "true")
            .WithRole(DXEndpoint.Role.Publisher)
            .Build();

        // Connect to the address.
        endpoint.Connect("tape:WriteTapeFile.out.bin");

        // Get publisher.
        var pub = endpoint.GetPublisher();

        // Publish events.
        var quote1 = new Quote("TEST1") { BidPrice = 10.1, AskPrice = 10.2 };
        var quote2 = new Quote("TEST2") { BidPrice = 17.1, AskPrice = 17.2 };
        pub.PublishEvents(quote1, quote2);

        // Wait until all data is written, close, and wait until it closes.
        endpoint.AwaitProcessed();
        endpoint.CloseAndAwaitTermination();
    }
}
