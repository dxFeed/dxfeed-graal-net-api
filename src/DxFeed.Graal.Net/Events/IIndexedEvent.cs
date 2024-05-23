// <copyright file="IIndexedEvent.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Events.Market;

namespace DxFeed.Graal.Net.Events;

/// <summary>
/// Represents an indexed collection of up-to-date information about some
/// condition or state of an external entity that updates in real-time. For example,
/// <see cref="Order"/> represents an order to buy or to sell some market instrument
/// that is currently active on a market exchange and multiple
/// orders are active for each symbol at any given moment in time.
/// <br/>
/// For more details see <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/event/IndexedEvent.html">Javadoc</a>.
/// </summary>
public interface IIndexedEvent : IEventType
{
    /// <summary>
    /// Gets source of this event.
    /// </summary>
    IndexedEventSource EventSource { get; }

    /// <summary>
    /// Gets or sets transactional event flags.
    /// </summary>
    int EventFlags { get; set; }

    /// <summary>
    /// Gets or sets unique per-symbol index of this event.
    /// </summary>
    long Index { get; set; }
}
