// <copyright file="IEventType.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Api;

namespace DxFeed.Graal.Net.Events;

/// <summary>
/// Marks all event types that can be received via dxFeed API.
/// Events are considered instantaneous, non-persistent, and unconflateable
/// (each event is individually delivered) unless they implement one of interfaces
/// defined in this package to further refine their meaning.
/// <br/>
/// For more details see <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/event/EventType.html">Javadoc</a>.
/// </summary>
public interface IEventType
{
    /// <summary>
    /// Gets or sets event symbol that identifies this event type <see cref="DXFeedSubscription"/>.
    /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/event/EventType.html#getEventSymbol--">Javadoc</a>.
    /// </summary>
    string? EventSymbol { get; set; }

    /// <summary>
    /// Gets or sets time when event was created or zero when time is not available.
    /// The difference, measured in milliseconds, between the event creation time and midnight,
    /// January 1, 1970 UTC or zero when time is not available.
    /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/event/EventType.html#getEventTime--">Javadoc</a>.
    /// </summary>
    long EventTime { get; set; }
}
