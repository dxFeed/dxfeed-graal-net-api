// <copyright file="DXEndpointLastEventsTest.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Api;
using DxFeed.Graal.Net.Events.Candles;
using DxFeed.Graal.Net.Events.Market;
using static DxFeed.Graal.Net.Api.DXEndpoint;
using static DxFeed.Graal.Net.Api.DXEndpoint.Role;

namespace DxFeed.Graal.Net.Tests.Api;

[TestFixture]
public class DXEndpointLastEventsTest
{
    [Test]
    public async Task TestLastEventTask()
    {
        var endpoint = DXEndpoint.Create(LocalHub);
        var feed = endpoint.GetFeed();
        var publisher = endpoint.GetPublisher();

        var lastEvent = feed.GetLastEventAsync<Quote>("A");
        publisher.PublishEvents(new Quote("A"));
        Console.WriteLine(lastEvent);
        Assert.That(lastEvent.Result.EventSymbol == "A");

        var cancelSource = new CancellationTokenSource();
        cancelSource.Cancel();
        try
        {
            await feed.GetLastEventAsync<Quote>("A", cancelSource.Token);
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            Assert.That(false, $"Unhandled exception {ex}");
        }

        try
        {
            await feed.GetLastEventAsync<Quote>(null);
        }
        catch (NullReferenceException) { }
        catch (Exception ex)
        {
            Assert.That(false, $"Unhandled exception {ex}");
        }

        endpoint.Close();
    }

    [Test]
    public async Task TestTimeSeriesEventTask()
    {
        var endpoint = DXEndpoint.Create(LocalHub);
        var feed = endpoint.GetFeed();
        var publisher = endpoint.GetPublisher();

        var lastEvent = feed.GetTimeSeriesAsync<Candle>("A", 0, long.MaxValue);
        publisher.PublishEvents(new Candle(CandleSymbol.ValueOf("A")));
        Console.WriteLine(lastEvent);
        Assert.That(lastEvent.Result.First().EventSymbol == "A");

        var cancelSource = new CancellationTokenSource();
        cancelSource.Cancel();
        try
        {
            await feed.GetTimeSeriesAsync<Candle>("A", 0, long.MaxValue, cancelSource.Token);
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            Assert.That(false, $"Unhandled exception {ex}");
        }

        try
        {
            await feed.GetTimeSeriesAsync<Candle>(null, 0, long.MaxValue);
        }
        catch (NullReferenceException) { }
        catch (Exception ex)
        {
            Assert.That(false, $"Unhandled exception {ex}");
        }

        endpoint.Close();
    }
}
