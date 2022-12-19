// <copyright file="IIndexedEvent.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace DxFeed.Graal.Net.Events;

/// <summary>
/// Represents an indexed collection of up-to-date information about some
/// condition or state of an external entity that updates in real-time.
/// <br/>
/// For more details see <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/event/IndexedEvent.html">Javadoc</a>.
/// </summary>
/// <typeparam name="T">Type of the event symbol for this event type.</typeparam>
public interface IIndexedEvent<out T> : IEventType<T>
{
    /// <summary>
    /// Indicates a pending transactional update. When <see cref="TxPending"/> is 1,
    /// it means that an ongoing transaction update, that spans multiple events, is in process.
    /// </summary>
    public const int TxPending = 0x01;

    /// <summary>
    /// Indicates that the event with the corresponding index has to be removed.
    /// </summary>
    public const int RemoveEvent = 0x02;

    /// <summary>
    /// Indicates when the loading of a snapshot starts.
    /// </summary>
    public const int SnapshotBegin = 0x04;

    /// <summary>
    /// <see cref="SnapshotEnd"/> or <see cref="SnapshotSnip"/> indicates the end of a snapshot.
    /// The difference between <see cref="SnapshotEnd"/>  and <see cref="SnapshotSnip"/> is the following:
    /// <see cref="SnapshotEnd"/> indicates that the data source sent all the data pertaining to
    /// the subscription for the corresponding indexed event,
    /// while <see cref="SnapshotSnip"/> indicates that some limit on the amount of data was reached
    /// and while there still might be more data available, it will not be provided.
    /// </summary>
    public const int SnapshotEnd = 0x08;

    /// <summary>
    /// <see cref="SnapshotEnd"/> or <see cref="SnapshotSnip"/> indicates the end of a snapshot.
    /// The difference between <see cref="SnapshotEnd"/>  and <see cref="SnapshotSnip"/> is the following:
    /// <see cref="SnapshotEnd"/> indicates that the data source sent all the data pertaining to
    /// the subscription for the corresponding indexed event,
    /// while <see cref="SnapshotSnip"/> indicates that some limit on the amount of data was reached
    /// and while there still might be more data available, it will not be provided.
    /// </summary>
    public const int SnapshotSnip = 0x10;

    /// <summary>
    /// Is used to instruct dxFeed to use snapshot mode.
    /// It is intended to be used only for publishing to activate (if not yet activated) snapshot mode.
    /// The difference from <see cref="SnapshotBegin"/> flag is that <see cref="SnapShotMode"/>
    /// only switches on snapshot mode without starting snapshot synchronization protocol.
    /// </summary>
    public const int SnapShotMode = 0x40;

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
