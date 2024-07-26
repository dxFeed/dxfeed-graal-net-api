// <copyright file="TxModelListener.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Collections.Generic;
using DxFeed.Graal.Net.Events;

namespace DxFeed.Graal.Net.Models;

/// <summary>
/// Invoked when a complete transaction (one or more) is received. This behavior can be changed when building
/// the model; see <see cref="AbstractTxModel{TE}.Builder{TB,TM}.WithBatchProcessing(bool)"/>.
///
/// <p>Only events that have the same <see cref="IndexedEventSource"/> and <see cref="IEventType.EventSymbol"/>
/// can be in the same listener call and cannot be mixed within a single call. If there are multiple sources,
/// listener notifications will happen separately for each source.</p>
///
/// <p>A transaction can also be a snapshot. In such cases, the <paramref name="isSnapshot"/> flag is set to <c>true</c>,
/// indicating that all state based on previously received events for the corresponding
/// <see cref="IndexedEventSource"/> should be cleared. A snapshot can also be post-processed or raw;
/// see <see cref="AbstractTxModel{TE}.Builder{TB,TM}.WithSnapshotProcessing(bool)"/>.
/// If <see cref="AbstractTxModel{TE}.Builder{TB,TM}.WithSnapshotProcessing(bool)"/> is <c>true</c>,
/// the transaction containing the snapshot can be empty (<c>events.Count == 0</c>),
/// meaning that an empty snapshot was received.</p>
/// </summary>
/// <param name="source">The source of the indexed events.</param>
/// <param name="events">The list of received events representing one or more transactions.</param>
/// <param name="isSnapshot">Indicates if the events form a snapshot.</param>
/// <typeparam name="TE">The type of indexed events passed to this listener.</typeparam>
public delegate void TxModelListener<in TE>(IndexedEventSource source, IEnumerable<TE> events, bool isSnapshot)
    where TE : IIndexedEvent;
