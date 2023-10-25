// <copyright file="DXEndpointTest.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using static DxFeed.Graal.Net.Api.DXEndpoint;
using static DxFeed.Graal.Net.Api.DXEndpoint.Role;

namespace DxFeed.Graal.Net.Tests.Api;

[TestFixture]
public class DXEndpointTest
{
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
            Assert.That(GetInstance().GetRole(), Is.EqualTo(Feed));
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
        Assert.Throws<ArgumentException>(() => Create((Role)100500));

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
        var feed = GetInstance().GetFeed();
        Assert.That(feed, Is.EqualTo(GetInstance().GetFeed()));
    }

    [Test]
    public void GetPublisherReturnsSameObject()
    {
        var publisher = GetInstance().GetPublisher();
        Assert.That(publisher, Is.EqualTo(GetInstance().GetPublisher()));
    }

    [Test]
    public void CheckDisposeCallClose()
    {
        var countdownEvent = new CountdownEvent(1);
        var endpoint = Create(Feed);
        var currentState = endpoint.GetState();
        endpoint.AddStateChangeListener((_, newState) =>
        {
            currentState = newState;
            countdownEvent.Signal();
        });

        endpoint.Dispose();

        Assert.Multiple(() =>
        {
            Assert.That(countdownEvent.Wait(new TimeSpan(0, 0, 3)), Is.True);
            Assert.That(currentState, Is.EqualTo(State.Closed));
        });
    }
}
