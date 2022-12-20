// <copyright file="DXEndpointTest.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using DxFeed.Graal.Net.Native.ErrorHandling;
using static DxFeed.Graal.Net.Api.DXEndpoint;
using static DxFeed.Graal.Net.Api.DXEndpoint.Role;

namespace DxFeed.Graal.Net.Tests.Api;

[TestFixture]
[SuppressMessage("Assertion", "NUnit2045:Use Assert.Multiple")]
public class DXEndpointTest
{
    private const string Port = "48756";

    [Test]
    public void ThrowExceptionWhenDisposed()
    {
        var endpoint = Create();
        endpoint.Dispose();
        Assert.Throws<JavaException>(() => endpoint.GetFeed());
        Assert.Throws<JavaException>(() => endpoint.GetPublisher());
    }

    [Test]
    public void MultipleDisposeNotThrowException()
    {
        var endpoint = Create();
        Assert.DoesNotThrow(() => endpoint.Dispose());
        Assert.DoesNotThrow(() => endpoint.Dispose());
    }

    [Test]
    public void DefaultEndpointRoleIsFeed() =>
        Assert.Multiple(() =>
        {
            Assert.That(Instance.GetRole(), Is.EqualTo(Feed));
            Assert.That(Create().GetRole(), Is.EqualTo(Feed));
        });

    [Test]
    public void CreateMethodBuildCorrectEndpointRole() =>
        Assert.Multiple(() =>
        {
            Assert.That(Create(Feed).GetRole(), Is.EqualTo(Feed));
            Assert.That(Create(OnDemandFeed).GetRole(), Is.EqualTo(OnDemandFeed));
            Assert.That(Create(StreamFeed).GetRole(), Is.EqualTo(StreamFeed));
            Assert.That(Create(Publisher).GetRole(), Is.EqualTo(Publisher));
            Assert.That(Create(StreamPublisher).GetRole(), Is.EqualTo(StreamPublisher));
            Assert.That(Create(LocalHub).GetRole(), Is.EqualTo(LocalHub));
        });

    [Test]
    public void UnsupportedRoleThrowException() =>
        Assert.Throws<JavaException>(() => Create((Role)100500));

    [Test]
    public void GetInstanceReturnsSameObject()
    {
        foreach (var role in (Role[])Enum.GetValues(typeof(Role)))
        {
            var endpoint = GetInstance(role);
            Assert.That(GetInstance(role), Is.EqualTo(endpoint));
        }
    }

    [Test]
    public void GetFeedReturnsSameObject()
    {
        var feed = Instance.GetFeed();
        Assert.That(feed, Is.EqualTo(Instance.GetFeed()));
    }

    [Test]
    public void GetGePublisherReturnsSameObject()
    {
        var publisher = Instance.GetPublisher();
        Assert.That(publisher, Is.EqualTo(Instance.GetPublisher()));
    }

    [Test]
    public void StateChangeListenerNotTerminatedWhenExceptionOccurs()
    {
        using var publisher = Create(Publisher).Connect($":{Port}");
        using var feed = Create(Feed);

        feed.AddStateChangeListener((_, _) =>
            throw new AssertionException("Not terminated"));

        feed.CloseAndAwaitTermination();
        Assert.That(feed.GetState(), Is.EqualTo(State.Closed));
    }

    [Test]
    public void SimpleCheckEndpointListenerStates()
    {
        var timeout = new TimeSpan(0, 0, 3);
        var countdownEvent = new CountdownEvent(1);

        using var publisher = Create(Publisher).Connect($":{Port}");
        using var feed = Create(Feed);

        var allExpectedState = new List<State>
        {
            // First state.
            State.NotConnected,

            // Connect.
            State.Connecting,
            State.Connected,

            // Reconnect.
            State.Connecting,
            State.Connected,

            // Disconnect.
            State.NotConnected,

            // Connect.
            State.Connecting,
            State.Connected,

            // Close.
            State.Closed
        };

        var allActualStates = new List<State>();

        // First state is not connected.
        Assert.That(feed.GetState(), Is.EqualTo(State.NotConnected));

        var expectedState = State.Connected;
        feed.AddStateChangeListener((oldSate, newState) =>
        {
            if (allActualStates.Count == 0)
            {
                allActualStates.Add(oldSate);
            }

            allActualStates.Add(newState);

            // ReSharper disable once AccessToModifiedClosure
            if (expectedState == newState)
            {
                countdownEvent.Signal();
            }
        });

        // Wait Connected state.
        countdownEvent.Reset();
        expectedState = State.Connected;
        feed.Connect($"localhost:{Port}");
        Assert.That(countdownEvent.Wait(timeout), Is.True);
        Assert.That(feed.GetState(), Is.EqualTo(expectedState));

        // Reconnect. Wait Connected state.
        countdownEvent.Reset();
        expectedState = State.Connected;
        feed.Reconnect();
        Assert.That(countdownEvent.Wait(timeout), Is.True);
        Assert.That(feed.GetState(), Is.EqualTo(expectedState));


        // Wait NotConnected state.
        countdownEvent.Reset();
        expectedState = State.NotConnected;
        feed.Disconnect();
        feed.AwaitNotConnected();
        Assert.That(countdownEvent.Wait(timeout), Is.True);
        Assert.That(feed.GetState(), Is.EqualTo(expectedState));

        // Wait Connected state.
        countdownEvent.Reset();
        expectedState = State.Connected;
        feed.Connect($"localhost:{Port}");
        Assert.That(countdownEvent.Wait(timeout), Is.True);
        Assert.That(feed.GetState(), Is.EqualTo(expectedState));

        // Wait Close state.
        countdownEvent.Reset();
        expectedState = State.Closed;
        feed.Close();
        Assert.That(countdownEvent.Wait(timeout), Is.True);
        Assert.That(feed.GetState(), Is.EqualTo(expectedState));

        // Cannot connect after close.
        countdownEvent.Reset();
        expectedState = State.Connected;
        feed.Connect($"localhost:{Port}");
        Assert.That(countdownEvent.Wait(timeout), Is.False);
        Assert.That(feed.GetState(), Is.EqualTo(State.Closed));

        // Compare expected and actually states.
        Assert.That(allExpectedState.SequenceEqual(allActualStates), Is.True);
    }
}
