// <copyright file="DXEndpointListenerTest.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using static DxFeed.Graal.Net.Api.DXEndpoint;
using static DxFeed.Graal.Net.Api.DXEndpoint.Role;

namespace DxFeed.Graal.Net.Tests.Api;

[TestFixture]
public class DXEndpointListenerTest
{
    private readonly TimeSpan _timeoutStateChange = new(0, 0, 3);

    [Test]
    public void StateChangeListenerNotTerminatedWhenExceptionOccurs()
    {
        using var endpoint = Create(Feed);

        endpoint.AddStateChangeListener((_, _) =>
            throw new AssertionException("Not terminated"));

        endpoint.CloseAndAwaitTermination();
        Assert.That(endpoint.IsClosed(), Is.True);
    }

    [Test]
    public void CheckAddMultipleStateChangeListener()
    {
        var countdownEvent1 = new CountdownEvent(1);
        var countdownEvent2 = new CountdownEvent(1);
        using var endpoint = Create(Feed);

        var allActualStates1 = new List<State>();

        void Listener1(State oldState, State newState)
        {
            allActualStates1.Add(oldState);
            allActualStates1.Add(newState);
            if (newState == State.Closed)
            {
                countdownEvent1.Signal();
            }
        }

        var allActualStates2 = new List<State>();

        void Listener2(State oldState, State newState)
        {
            allActualStates2.Add(oldState);
            allActualStates2.Add(newState);
            if (newState == State.Closed)
            {
                countdownEvent2.Signal();
            }
        }

        endpoint.AddStateChangeListener(Listener1);
        endpoint.AddStateChangeListener(Listener2);
        endpoint.CloseAndAwaitTermination();

        Assert.Multiple(() =>
        {
            // Wait for all listeners to fire.
            Assert.That(countdownEvent1.Wait(_timeoutStateChange), Is.True);
            Assert.That(countdownEvent2.Wait(_timeoutStateChange), Is.True);

            // Compare all states of both listeners.
            Assert.That(allActualStates1.SequenceEqual(allActualStates2), Is.True);
        });
    }

    [Test]
    public void CheckAddOneStateChangeListenerMultipleTime()
    {
        var countdownEvent = new CountdownEvent(2);
        using var endpoint = Create(Feed);

        var allActualStates = new List<State>();

        void Listener(State oldState, State newState)
        {
            allActualStates.Add(newState);
            countdownEvent.Signal();
        }

        endpoint.AddStateChangeListener(Listener);
        endpoint.AddStateChangeListener(Listener);
        endpoint.CloseAndAwaitTermination();

        Assert.Multiple(() =>
        {
            // Wait for listener to fire twice.
            Assert.That(countdownEvent.Wait(_timeoutStateChange), Is.True);

            // Listener calls twice.
            Assert.That(allActualStates.SequenceEqual(new[] { State.Closed, State.Closed }), Is.True);
        });
    }

    [Test]
    public void CheckRemoveSateChangeListenerInsideListener()
    {
        var countdownEvent = new CountdownEvent(1);
        using var endpoint = Create(Publisher);
        var countCallListener = 0;

        void Listener(State oldState, State newState)
        {
            ++countCallListener;
            endpoint.RemoveStateChangeListener(Listener);
            countdownEvent.Signal();
        }

        endpoint.AddStateChangeListener(Listener);
        endpoint.Connect(":0");
        Assert.That(countdownEvent.Wait(_timeoutStateChange), Is.True);
        endpoint.Disconnect();
        endpoint.AwaitNotConnected();
        endpoint.CloseAndAwaitTermination();

        Assert.Multiple(() =>
        {
            // The connection was closed.
            Assert.That(endpoint.IsClosed, Is.True);

            // Listener was only called once.
            Assert.That(countCallListener, Is.EqualTo(1));
        });
    }

    [Test]
    public void CheckDisconnectFromListener()
    {
        var endpoint = Create(Publisher);
        endpoint.AddStateChangeListener((_, newState) =>
        {
            if (newState == State.Connected)
            {
                endpoint.Disconnect();
            }
        });
        endpoint.Connect(":0");
        endpoint.AwaitNotConnected();
        Assert.That(endpoint.GetState(), Is.EqualTo(State.NotConnected));
        endpoint.Close();
    }

    [Test]
    public void CheckCloseFromListener()
    {
        var endpoint = Create(Publisher);
        endpoint.AddStateChangeListener((_, newState) =>
        {
            if (newState == State.Connected)
            {
                endpoint.Close();
            }
        });
        endpoint.Connect(":0");
        endpoint.AwaitNotConnected();
        Assert.That(endpoint.GetState(), Is.EqualTo(State.Closed));
    }

    [Test]
    public void CheckDisposeCallClose()
    {
        var endpoint = Create(Publisher);
        var lastState = endpoint.GetState();
        endpoint.AddStateChangeListener((_, newState) =>
        {
            if (newState == State.Connected)
            {
                endpoint.Dispose();
            }

            lastState = newState;
        });
        endpoint.Connect(":0");
        endpoint.AwaitNotConnected();
        Assert.Multiple(() =>
        {
            Assert.That(endpoint.GetState(), Is.EqualTo(State.Closed));
            Assert.That(lastState, Is.EqualTo(State.Closed));
        });
    }

    [Test]
    public void SimpleCheckEndpointListenerStates()
    {
        const string address = ":0";
        var countdownEvent = new CountdownEvent(1);
        using var endpoint = Create(Publisher);

        var allExpectedState = new List<State>
        {
            // First state.
            State.NotConnected,
            // Connect.
            State.Connected,
            // Disconnect.
            State.NotConnected,
            // Connect.
            State.Connected,
            // Close.
            State.Closed
        };

        var allActualStates = new List<State>();

        // First state is not connected.
        Assert.That(endpoint.GetState(), Is.EqualTo(State.NotConnected));

        var expectedState = State.Connected;
        endpoint.AddStateChangeListener((oldSate, newState) =>
        {
            // Adds NotConnected first time.
            if (allActualStates.Count == 0)
            {
                allActualStates.Add(oldSate);
            }

            // ReSharper disable once AccessToModifiedClosure
            if (expectedState == newState)
            {
                allActualStates.Add(newState);
                countdownEvent.Signal();
            }
        });

        // Wait Connected state.
        expectedState = State.Connected;
        countdownEvent.Reset();
        endpoint.Connect(address);
        Assert.Multiple(() =>
        {
            Assert.That(countdownEvent.Wait(_timeoutStateChange), Is.True);
            Assert.That(endpoint.GetState(), Is.EqualTo(expectedState));
        });

        // Wait NotConnected state.
        expectedState = State.NotConnected;
        countdownEvent.Reset();
        endpoint.Disconnect();
        endpoint.AwaitNotConnected();
        Assert.Multiple(() =>
        {
            Assert.That(countdownEvent.Wait(_timeoutStateChange), Is.True);
            Assert.That(endpoint.GetState(), Is.EqualTo(expectedState));
        });

        // Wait Connected state.
        expectedState = State.Connected;
        countdownEvent.Reset();
        endpoint.Connect(address);
        Assert.Multiple(() =>
        {
            Assert.That(countdownEvent.Wait(_timeoutStateChange), Is.True);
            Assert.That(endpoint.GetState(), Is.EqualTo(expectedState));
        });

        // Wait Close state.
        expectedState = State.Closed;
        countdownEvent.Reset();
        endpoint.Close();
        Assert.Multiple(() =>
        {
            Assert.That(countdownEvent.Wait(_timeoutStateChange), Is.True);
            Assert.That(endpoint.GetState(), Is.EqualTo(expectedState));
        });

        // Cannot connect after close.
        expectedState = State.Connected;
        countdownEvent.Reset();
        endpoint.Connect(address);
        Assert.Multiple(() =>
        {
            Assert.That(countdownEvent.Wait(_timeoutStateChange), Is.False);
            Assert.That(endpoint.GetState(), Is.EqualTo(State.Closed));
        });

        // Compare expected and actually states.
        Assert.That(allExpectedState.SequenceEqual(allActualStates), Is.True);
    }
}
