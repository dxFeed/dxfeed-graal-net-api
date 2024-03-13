// <copyright file="DXEndpointLastEventsTest.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Api;
using DxFeed.Graal.Net.Events.Candles;
using DxFeed.Graal.Net.Events.Market;
using DxFeed.Graal.Net.Native.ErrorHandling;
using static DxFeed.Graal.Net.Api.DXEndpoint.Role;

namespace DxFeed.Graal.Net.Tests.Api;

[TestFixture]
public class DXFeedPromiseTest
{
    [Test]
    public void TestLastEventTask()
    {
        using var endpoint = DXEndpoint.Create(LocalHub);
        var feed = endpoint.GetFeed();
        var publisher = endpoint.GetPublisher();

        var lastEvent = feed.GetLastEventAsync<Quote>("A");
        publisher.PublishEvents(new Quote("A"));
        Assert.That(lastEvent.Result, Is.Not.EqualTo(null));
        Assert.That(lastEvent.Result.EventSymbol, Is.EqualTo("A"));

        Assert.ThrowsAsync<OperationCanceledException>(async () =>
        {
            using var cancelSource = new CancellationTokenSource();
            cancelSource.Cancel();
            await feed.GetLastEventAsync<Quote>("A", cancelSource.Token);
        });

        Assert.ThrowsAsync<JavaException>(async () =>
        {
            await feed.GetLastEventAsync<Quote>(null!);
        });
    }

    [Test]
    public void TestTimeSeriesEventTask()
    {
        using var endpoint = DXEndpoint.Create(LocalHub);
        var feed = endpoint.GetFeed();
        var publisher = endpoint.GetPublisher();

        var lastEvent = feed.GetTimeSeriesAsync<Candle>("A", 0, long.MaxValue);
        publisher.PublishEvents(new Candle(CandleSymbol.ValueOf("A")));
        Assert.That(lastEvent.Result, Is.Not.EqualTo(null));
        Assert.That(lastEvent.Result.First().EventSymbol, Is.EqualTo("A"));

        Assert.ThrowsAsync<OperationCanceledException>(async () =>
        {
            using var cancelSource = new CancellationTokenSource();
            cancelSource.Cancel();
            await feed.GetTimeSeriesAsync<Candle>("A", 0, long.MaxValue, cancelSource.Token);
        });

        Assert.ThrowsAsync<JavaException>(async () =>
        {
            await feed.GetTimeSeriesAsync<Candle>(null!, 0, long.MaxValue);
        });
    }
}
