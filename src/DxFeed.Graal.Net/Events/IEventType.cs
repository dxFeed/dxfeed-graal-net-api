// <copyright file="IEventType.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using DxFeed.Graal.Net.Api;

namespace DxFeed.Graal.Net.Events;

/// <summary>
/// Marks all event types that can be received via dxFeed API.
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

    /// <summary>
    /// Gets all types that implement this interface.
    /// </summary>
    /// <returns>Returns a collection of event types.</returns>
    static IEnumerable<Type> GetEventTypes() =>
        AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(t => typeof(IEventType).IsAssignableFrom(t))
            .Where(t => !t.IsAbstract)
            .Where(t => t.IsClass);
}

/// <summary>
/// Marks all event types that can be received via dxFeed API.
/// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/event/EventType.html">Javadoc</a>.
/// </summary>
/// <typeparam name="T">The type of <see cref="EventSymbol"/>.</typeparam>
// ToDo Avoid generic type for IEventType.
public interface IEventType<out T> : IEventType
{
    /// <summary>
    /// Gets event symbol that identifies this event type <see cref="DXFeedSubscription"/>.
    /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/event/EventType.html#getEventSymbol--">Javadoc</a>.
    /// </summary>
    new T? EventSymbol { get; }
}
