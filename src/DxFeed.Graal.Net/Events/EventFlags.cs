// <copyright file="EventFlags.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace DxFeed.Graal.Net.Events;

/// <summary>
/// Provides constants and utility methods for handling event flags.
/// </summary>
public static class EventFlags
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
    /// Determines if the given event marks the beginning of a snapshot.
    /// </summary>
    /// <param name="e">The event to be checked.</param>
    /// <returns><c>true</c> if the event flags indicate the beginning of a snapshot, <c>false</c> otherwise.</returns>
    public static bool IsSnapshotBegin(IIndexedEvent e) =>
        (e.EventFlags & SnapshotBegin) != 0;

    /// <summary>
    /// Determines if the given event marks the end of a snapshot.
    /// </summary>
    /// <param name="e">The event to be checked.</param>
    /// <returns><c>true</c> if the event flags indicate the end of a snapshot, <c>false</c> otherwise.</returns>
    public static bool IsSnapshotEnd(IIndexedEvent e) =>
        (e.EventFlags & SnapshotEnd) != 0;

    /// <summary>
    /// Determines if the given event is marked as a snapshot snip.
    /// </summary>
    /// <param name="e">The event to be checked.</param>
    /// <returns><c>true</c> if the event flags indicate a snapshot snip, <c>false</c> otherwise.</returns>
    public static bool IsSnapshotSnip(IIndexedEvent e) =>
        (e.EventFlags & SnapshotSnip) != 0;

    /// <summary>
    /// Determines if the given event marks the end of a snapshot or a snapshot snip.
    /// </summary>
    /// <param name="e">The event to be checked.</param>
    /// <returns><c>true</c> if the event flags indicate the end or snip of a snapshot, <c>false</c> otherwise.</returns>
    public static bool IsSnapshotEndOrSnip(IIndexedEvent e) =>
        IsSnapshotEnd(e) || IsSnapshotSnip(e);

    /// <summary>
    /// Determines if the given event is in a pending state.
    /// </summary>
    /// <param name="e">The event to be checked.</param>
    /// <returns><c>true</c> if the event flags indicate a pending transaction, <c>false</c> otherwise.</returns>
    public static bool IsPending(IIndexedEvent e) =>
        (e.EventFlags & TxPending) != 0;

    /// <summary>
    /// Determines if the given event is marked for removal.
    /// </summary>
    /// <param name="e">The event to be checked.</param>
    /// <returns><c>true</c> if the event flags indicate a remove action, <c>false</c> otherwise.</returns>
    public static bool IsRemove(IIndexedEvent e) =>
        (e.EventFlags & RemoveEvent) != 0;
}
