// <copyright file="TimeSeriesSubscriptionSymbol.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Utils;

namespace DxFeed.Graal.Net.Api.Osub;

// File May Only Contain A Single Type. It's more readable to put the generic and non-generic versions in the same file.
#pragma warning disable SA1402

/// <summary>
/// Represents subscription to time-series of events.
/// Instances of this class can be used with <see cref="DXFeedSubscription"/> to specify subscription
/// for time series events from a specific time. By default, subscribing to time-series events by
/// their event symbol object, the subscription is performed to a stream of new events as they happen only.
/// <br/>
/// For more details see <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/osub/TimeSeriesSubscriptionSymbol.html">Javadoc</a>.
/// </summary>
/// <typeparam name="T">The type of event symbol.</typeparam>
public class TimeSeriesSubscriptionSymbol<T> : IndexedEventSubscriptionSymbol<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TimeSeriesSubscriptionSymbol{T}"/> class
    /// with a specified event symbol and from time in milliseconds since Unix epoch.
    /// </summary>
    /// <param name="eventSymbol">The event symbol.</param>
    /// <param name="fromTime">
    /// The from time is measured in milliseconds between the current time and midnight, January 1, 1970 UTC.
    /// </param>
    public TimeSeriesSubscriptionSymbol(T eventSymbol, long fromTime)
        : base(eventSymbol, IndexedEventSource.DEFAULT) =>
        FromTime = fromTime;

    /// <inheritdoc cref="TimeSeriesSubscriptionSymbol{T}(T,long)"/>
    public TimeSeriesSubscriptionSymbol(T eventSymbol, DateTimeOffset fromTime)
        : this(eventSymbol, fromTime.ToUnixTimeMilliseconds())
    {
    }

    /// <summary>
    /// Gets subscription time in milliseconds since Unix epoch.
    /// </summary>
    public long FromTime { get; }

    /// <inheritdoc/>
    public override string ToString() =>
        $"{EventSymbol}{{fromTime={TimeFormat.Local.WithMillis().WithTimeZone().FormatFromMillis(FromTime)}}}";

    /// <summary>
    /// Indicates whether some other time series event subscription symbol
    /// has the same <see cref="IndexedEventSubscriptionSymbol{T}.EventSymbol"/>.
    /// </summary>
    /// <param name="obj"> The object to compare with the current object.</param>
    /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj)
    {
        if (this == obj)
        {
            return true;
        }

        if (obj is not TimeSeriesSubscriptionSymbol<T> that)
        {
            return false;
        }

        return EventSymbol!.Equals(that.EventSymbol);
    }

    /// <summary>
    /// Returns a hash code value for this object.
    /// </summary>
    /// <returns>A hash code value for this object.</returns>
    public override int GetHashCode() =>
        EventSymbol!.GetHashCode();
}

/// Non-generic version, for erasing a generic type.
/// <inheritdoc/>
public class TimeSeriesSubscriptionSymbol : TimeSeriesSubscriptionSymbol<object>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TimeSeriesSubscriptionSymbol"/> class
    /// with a specified event symbol and from time in milliseconds since Unix epoch.
    /// </summary>
    /// <param name="eventSymbol">The event symbol.</param>
    /// <param name="fromTime">
    /// The from time is measured in milliseconds between the current time and midnight, January 1, 1970 UTC.
    /// </param>
    public TimeSeriesSubscriptionSymbol(object eventSymbol, long fromTime)
        : base(eventSymbol, fromTime)
    {
    }

    /// <inheritdoc cref="TimeSeriesSubscriptionSymbol(object,long)"/>
    public TimeSeriesSubscriptionSymbol(object eventSymbol, DateTimeOffset fromTime)
        : this(eventSymbol, fromTime.ToUnixTimeMilliseconds())
    {
    }
}
