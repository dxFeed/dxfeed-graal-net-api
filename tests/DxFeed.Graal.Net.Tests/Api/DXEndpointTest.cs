// <copyright file="DXEndpointTest.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Api;

namespace DxFeed.Graal.Net.Tests.Api;

[TestFixture]
public class DXEndpointTest
{
    [Test]
    public void CheckSimpleEndpointListenerStates()
    {
        var timeout = new TimeSpan(0, 0, 5);
        var countdownEvent = new CountdownEvent(1);

        using var publisher = DXEndpoint.NewBuilder()
            .WithRole(DXEndpoint.Role.Publisher)
            .Build()
            .Connect(":7777");

        using var feed = DXEndpoint.NewBuilder()
            .WithRole(DXEndpoint.Role.Feed)
            .Build();

        // First state is not connected.
        Assert.That(feed.GetState(), Is.EqualTo(DXEndpoint.State.NotConnected));

        var expectedState = DXEndpoint.State.Connected;
        feed.AddStateChangeListener((_, newState) =>
        {
            // ReSharper disable once AccessToModifiedClosure
            if (expectedState == newState)
            {
                countdownEvent.Signal();
            }
        });

        // Wait Connected state.
        countdownEvent.Reset();
        feed.Connect("localhost:7777");
        expectedState = DXEndpoint.State.Connected;
        Assert.That(countdownEvent.Wait(timeout), Is.True);

        // Wait NotConnected state.
        countdownEvent.Reset();
        expectedState = DXEndpoint.State.NotConnected;
        feed.Disconnect();
        feed.AwaitNotConnected();
        Assert.That(countdownEvent.Wait(timeout), Is.True);

        // Wait Close state.
        countdownEvent.Reset();
        expectedState = DXEndpoint.State.Closed;
        feed.Close();
        Assert.That(countdownEvent.Wait(timeout), Is.True);
    }
}
