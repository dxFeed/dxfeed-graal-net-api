// <copyright file="Program.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Api;
using DxFeed.Graal.Net.Api.Osub;

// Sections of code should not be "commented out". Ignored intentionally as a demo.
#pragma warning disable S125

namespace DxFeed.Graal.Net.Samples;

/// <summary>
/// Converts one tape file into another tape file with optional intermediate processing or filtering.
/// This project contains sample tape file ConvertTapeFile.in.
/// </summary>
internal abstract class ConvertTapeFile
{
    public static void Main(string[] args)
    {
        // Determine input and output tapes and specify appropriate configuration parameters.
        var inputAddress = args.Length > 0 ? args[0] : "file:ConvertTapeFile.in[readAs=stream_data,speed=max]";
        var outputAddress = args.Length > 1 ? args[1] : "tape:ConvertTapeFile.out[saveAs=stream_data,format=text]";

        // Create input endpoint configured for tape reading.
        var inputEndpoint = DXEndpoint.NewBuilder()
            .WithRole(DXEndpoint.Role.StreamFeed) // Prevents event conflation and loss due to buffer overflow.
            .WithProperty(DXEndpoint.DXFeedWildcardEnableProperty, "true") // Enables wildcard subscription.
            .WithProperty(DXEndpoint.DXEndpointEventTimeProperty, "true") // Use provided event times.
            .Build();

        // Create output endpoint configured for tape writing.
        var outputEndpoint = DXEndpoint.NewBuilder()
            .WithRole(DXEndpoint.Role.StreamPublisher) // Prevents event conflation and loss due to buffer overflow.
            .WithProperty(DXEndpoint.DXFeedWildcardEnableProperty, "true") // Enables wildcard subscription.
            .WithProperty(DXEndpoint.DXEndpointEventTimeProperty, "true") // Use provided event times.
            .Build();

        // Create and link event processor for all types of events.
        // Note: Set of processed event types could be limited if needed.
        var eventTypes = DXEndpoint.GetEventTypes();
        var sub = inputEndpoint.GetFeed().CreateSubscription(eventTypes);
        sub.AddEventListener(events =>
        {
            // Here event processing occurs. Events could be modified, removed, or new events added.
            // For example, the below code adds 1 hour to event times:
            // foreach (var e in events)
            // {
            //     e.EventTime += 3600_000;
            // }

            // Publish processed events
            var publisher = outputEndpoint.GetPublisher();
            publisher.PublishEvents(events);
        });

        // Subscribe to all symbols.
        // Note: Set of processed symbols could be limited if needed.
        sub.AddSymbols(WildcardSymbol.All);

        // Connect output endpoint and start output tape writing BEFORE starting input tape reading.
        outputEndpoint.Connect(outputAddress);
        // Connect input endpoint and start input tape reading AFTER starting output tape writing.
        inputEndpoint.Connect(inputAddress);

        // Wait until all data is read and processed, and then gracefully close input endpoint.
        inputEndpoint.AwaitNotConnected();
        inputEndpoint.CloseAndAwaitTermination();

        // Wait until all data is processed and written, and then gracefully close output endpoint.
        outputEndpoint.AwaitProcessed();
        outputEndpoint.CloseAndAwaitTermination();
    }
}
