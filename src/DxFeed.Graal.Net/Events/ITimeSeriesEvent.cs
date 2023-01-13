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
public interface ITimeSeriesEvent : IIndexedEvent
{
    /// <summary>
    /// Gets a source identifier for this event,
    /// which is always <see cref="IndexedEventSource.DEFAULT"/> for time-series events.
    /// </summary>
    new IndexedEventSource EventSource { get; }

    /// <summary>
    /// Gets or sets unique per-symbol index of this event.
    /// Event indices are unique within event symbol.
    /// Typically, event index for a time series event includes <see cref="Time"/> inside.
    /// <h3>Implementation notes</h3>
    /// The most common scheme for event indices is to set highest 32 bits of event index
    /// to event timestamp in seconds. The lowest 32 bits are then split as follows.
    /// Bits 22 to 31 encode milliseconds of time stamp, and bits 0 to 21 encode some kind of a sequence number.
    /// <br/>
    /// Ultimately, the scheme for event indices is specific for each even type.
    /// The actual classes for specific event types perform the corresponding encoding.
    /// </summary>
    new long Index { get; set; }

    /// <summary>
    /// Gets or sets timestamp of the event.
    /// The timestamp is in milliseconds from midnight, January 1, 1970 UTC.
    /// </summary>
    long Time { get; set; }
}
