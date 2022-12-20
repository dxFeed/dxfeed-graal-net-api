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
    public void CheckSupportsProperty()
    {
        using var builder = NewBuilder();
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
            Assert.Throws<JavaException>(() => builder.SupportsProperty(null!));
        });
    }

    [Test]
    public void SetUnsupportedPropertyNotThrowException()
    {
        using var builder = NewBuilder();
        Assert.DoesNotThrow(() => builder.WithProperty("unsupported-property", "unsupported-value"));
        Assert.DoesNotThrow(() => builder.Build());
    }

    [Test]
    public void ThrowExceptionWhenDisposed()
    {
        var builder = NewBuilder();
        builder.Dispose();
        Assert.Throws<JavaException>(() => builder.Build());
        Assert.Throws<JavaException>(() => builder.Build());
    }

    [Test]
    public void MultipleDisposeNotThrowException()
    {
        using var builder = NewBuilder();
        Assert.DoesNotThrow(() => builder.Dispose());
        Assert.DoesNotThrow(() => builder.Dispose());
    }

    [Test]
    public void UnsupportedRoleThrowException()
    {
        using var builder = NewBuilder();
        Assert.Throws<JavaException>(() => builder.WithRole((Role)100500));
    }

    [Test]
    public void OneBuilderInstanceCannotCreateMoreThanOneEndpoint()
    {
        using var builder = NewBuilder();
        Assert.Multiple(() =>
        {
            Assert.DoesNotThrow(() => builder.Build());
            Assert.Throws<JavaException>(() => builder.Build());
        });
    }

    [Test]
    public void DefaultEndpointRoleIsFeed() =>
        Assert.That(NewBuilder().Build().GetRole(), Is.EqualTo(Feed));

    [Test]
    public void WithRoleMethodBuildCorrectEndpointRole() =>
        Assert.Multiple(() =>
        {
            Assert.That(new Builder().WithRole(Feed).Build().GetRole(), Is.EqualTo(Feed));
            Assert.That(new Builder().WithRole(OnDemandFeed).Build().GetRole(), Is.EqualTo(OnDemandFeed));
            Assert.That(new Builder().WithRole(StreamFeed).Build().GetRole(), Is.EqualTo(StreamFeed));
            Assert.That(new Builder().WithRole(Publisher).Build().GetRole(), Is.EqualTo(Publisher));
            Assert.That(new Builder().WithRole(StreamPublisher).Build().GetRole(), Is.EqualTo(StreamPublisher));
            Assert.That(new Builder().WithRole(LocalHub).Build().GetRole(), Is.EqualTo(LocalHub));
        });

    [Test]
    public void WithRoleMethodOverrideOldRole() =>
        Assert.That(NewBuilder().WithRole(Feed).WithRole(Publisher).Build().GetRole(), Is.EqualTo(Publisher));
}
