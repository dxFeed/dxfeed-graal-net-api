// <copyright file="ITimeSeriesEvent.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Events.Market;

namespace DxFeed.Graal.Net.Events;

/// <summary>
/// Represents time-series snapshots of some
/// process that is evolving in time or actual events in some external system
/// that have an associated time stamp and can be uniquely identified.
/// For example, <see cref="TimeAndSale"/> events represent the actual sales that happen
/// on a market exchange at specific time moments, while Candle events represent snapshots
/// of aggregate information about trading over a specific time period.
/// <br/>
/// For more details see <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/event/TimeSeriesEvent.html">Javadoc</a>.
/// </summary>
/// <typeparam name="T">Type of the event symbol for this event type.</typeparam>
public interface ITimeSeriesEvent<out T> : IIndexedEvent<T>
{
    /// <summary>
    /// Gets or sets timestamp of the event.
    /// The timestamp is in milliseconds from midnight, January 1, 1970 UTC.
    /// </summary>
    long Time { get; set; }
}
