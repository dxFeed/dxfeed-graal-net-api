// <copyright file="DXEndpointBuilderTest.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Native.ErrorHandling;
using static DxFeed.Graal.Net.Api.DXEndpoint;
using static DxFeed.Graal.Net.Api.DXEndpoint.Role;

namespace DxFeed.Graal.Net.Tests.Api;

[TestFixture]
public class DXEndpointBuilderTest
{
    [Test]
    public void DefaultEndpointRoleIsFeed()
    {
        using var endpoint = NewBuilder().Build();
        Assert.That(endpoint.GetRole(), Is.EqualTo(Feed));
    }

    [Test]
    public void WithRoleMethodBuildCorrectEndpointRole() =>
        Assert.Multiple(() =>
        {
            Assert.That(NewBuilder().WithRole(Feed).Build().GetRole(), Is.EqualTo(Feed));
            Assert.That(NewBuilder().WithRole(OnDemandFeed).Build().GetRole(), Is.EqualTo(OnDemandFeed));
            Assert.That(NewBuilder().WithRole(StreamFeed).Build().GetRole(), Is.EqualTo(StreamFeed));
            Assert.That(NewBuilder().WithRole(Publisher).Build().GetRole(), Is.EqualTo(Publisher));
            Assert.That(NewBuilder().WithRole(StreamPublisher).Build().GetRole(), Is.EqualTo(StreamPublisher));
            Assert.That(NewBuilder().WithRole(LocalHub).Build().GetRole(), Is.EqualTo(LocalHub));
        });

    [Test]
    public void UnsupportedRoleThrowException() =>
        Assert.Throws<ArgumentException>(() => NewBuilder().WithRole((Role)100500));

    [Test]
    public void CheckSupportsProperty()
    {
        var builder = NewBuilder();
        Assert.Multiple(() =>
        {
            // Supported property returns true.
            Assert.That(builder.SupportsProperty(NameProperty), Is.True);
            Assert.That(builder.SupportsProperty(DXFeedPropertiesProperty), Is.True);
            Assert.That(builder.SupportsProperty(DXFeedAddressProperty), Is.True);
            Assert.That(builder.SupportsProperty(DXFeedUserProperty), Is.True);
            Assert.That(builder.SupportsProperty(DXFeedPasswordProperty), Is.True);
            Assert.That(builder.SupportsProperty(DXFeedThreadPoolSizeProperty), Is.True);
            Assert.That(builder.SupportsProperty(DXFeedAggregationPeriodProperty), Is.True);
            Assert.That(builder.SupportsProperty(DXFeedWildcardEnableProperty), Is.True);
            Assert.That(builder.SupportsProperty(DXPublisherPropertiesProperty), Is.True);
            Assert.That(builder.SupportsProperty(DXPublisherAddressProperty), Is.True);
            Assert.That(builder.SupportsProperty(DXPublisherThreadPoolSizeProperty), Is.True);
            Assert.That(builder.SupportsProperty(DXEndpointEventTimeProperty), Is.True);
            Assert.That(builder.SupportsProperty(DXEndpointStoreEverythingProperty), Is.True);
            Assert.That(builder.SupportsProperty(DXSchemeNanoTimeProperty), Is.True);
            Assert.That(builder.SupportsProperty(DXSchemeEnabledPropertyPrefix), Is.True);
            // Unsupported property returns false.
            Assert.That(builder.SupportsProperty("unsupported-property"), Is.False);
            // Null-key throws JavaException.
            Assert.Throws<ArgumentNullException>(() => builder.SupportsProperty(null!));
        });
    }

    [Test]
    public void SetUnsupportedPropertyNotThrowException()
    {
        var builder = NewBuilder();
        Assert.DoesNotThrow(() => builder.WithProperty("unsupported-property", "unsupported-value"));
        Assert.DoesNotThrow(() => builder.Build());
    }

    [Test]
    public void CheckWithName() =>
        Assert.Multiple(() =>
        {
            var defaultName = NewBuilder().Build().GetName();
            Assert.That(defaultName, Does.Contain("qdnet"));
            Assert.That(NewBuilder().WithName("Test").Build().GetName(), Is.EqualTo("Test"));
        });

    [Test]
    public void CheckWithPropertyOverloads()
    {
        var str = "TestName1";
        var kvp = new KeyValuePair<string, string>(NameProperty, "TestName2");
        var dic = new Dictionary<string, string> { { NameProperty, "TestName3" } };
        Assert.Multiple(() =>
        {
            Assert.That(NewBuilder().WithProperty(NameProperty, str).Build().GetName(), Is.EqualTo(str));
            Assert.That(NewBuilder().WithProperty(kvp).Build().GetName(), Is.EqualTo(kvp.Value));
            Assert.That(NewBuilder().WithProperties(dic).Build().GetName(), Is.EqualTo(dic[NameProperty]));
        });
    }

    [Test]
    public void OneBuilderInstanceCanCreateMoreThanOneEndpoint()
    {
        var builder = NewBuilder();
        Assert.Multiple(() =>
        {
            Assert.DoesNotThrow(() => builder.Build());
            Assert.DoesNotThrow(() => builder.Build());
        });
    }

    [Test]
    public void WithRoleMethodOverrideOldRole() =>
        Assert.That(NewBuilder().WithRole(Feed).WithRole(Publisher).Build().GetRole(), Is.EqualTo(Publisher));
}
