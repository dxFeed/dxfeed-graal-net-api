// <copyright file="IndexedTxModel.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Collections.Generic;
using DxFeed.Graal.Net.Api;
using DxFeed.Graal.Net.Api.Osub;
using DxFeed.Graal.Net.Events;

namespace DxFeed.Graal.Net.Models;

/// <summary>
/// An incremental model for indexed events.
/// This model manages all snapshot and transaction logic, subscription handling, and listener notifications.
///
/// <p>This model is designed to handle incremental transactions. Users of this model only see the list
/// of events in a consistent state. This model delays incoming events that are part of an incomplete snapshot
/// or ongoing transaction until the snapshot is complete or the transaction has ended. This model notifies
/// the user of received transactions through an installed <see cref="TxModelListener{TE}"/>.</p>
///
/// <h3>Configuration</h3>
///
/// <p>This model must be configured using the <see cref="Builder"/> class, as most configuration
/// settings cannot be changed once the model is built. This model requires configuration
/// <see cref="AbstractTxModel{TE}.Builder{TB,TM}.WithSymbol(object)"/> and
/// <see cref="Builder.WithSources(DxFeed.Graal.Net.Events.IndexedEventSource[])"/> for subscription,
/// and it must be <see cref="AbstractTxModel{TE}.Builder{TB,TM}.WithFeed(DXFeed)"/> attached to a <see cref="DXFeed"/>
/// instance to begin operation.
/// For ease of use, some of these configurations can be changed after the model is built,
/// see <see cref="SetSources(DxFeed.Graal.Net.Events.IndexedEventSource[])"/>.</p>
///
/// <p>This model only supports single symbol subscriptions; multiple symbols cannot be configured.</p>
///
/// <h3>Resource management and closed models</h3>
///
/// <p>Attached model is a potential memory leak. If the pointer to attached model is lost, then there is no way
/// to detach this model from the feed and the model will not be reclaimed by the garbage collector as long as the
/// corresponding feed is still used. Detached model can be reclaimed by the garbage collector, but detaching model
/// requires knowing the pointer to the feed at the place of the call, which is not always convenient.</p>
///
/// <p>The convenient way to detach model from the feed is to call its
/// <see cref="AbstractTxModel{TE}.Dispose()"/> method.
/// Closed model becomes permanently detached from all feeds, removes all its listeners and is guaranteed
/// to be reclaimable by the garbage collector as soon as all external references to it are cleared.</p>
///
/// <h3>Threads and locks</h3>
///
/// <p>This class is thread-safe and can be used concurrently from multiple threads without external synchronization.</p>
/// The corresponding <see cref="TxModelListener{TE}"/> to never be concurrent.
/// </summary>
/// <typeparam name="TE">The type of indexed events processed by this model.</typeparam>
public sealed class IndexedTxModel<TE> : AbstractTxModel<TE>
    where TE : class, IIndexedEvent
{
    private readonly object _syncRoot = new();
    private HashSet<IndexedEventSource> _sources;

    private IndexedTxModel(Builder builder)
        : base(builder)
    {
        _sources = new HashSet<IndexedEventSource>(builder.Sources);
        UpdateSubscription(GetUndecoratedSymbol(), _sources);
    }

    /// <summary>
    /// Factory method to create a new builder for this model.
    /// </summary>
    /// <returns>A new <see cref="Builder"/> instance.</returns>
    public static Builder NewBuilder() =>
        new();

    /// <summary>
    /// Gets the current set of sources.
    /// If no sources have been set, an empty set is returned,
    /// indicating that all possible sources have been subscribed to.
    /// </summary>
    /// <returns>The set of current sources.</returns>
    public HashSet<IndexedEventSource> GetSources()
    {
        lock (_syncRoot)
        {
            return new HashSet<IndexedEventSource>(_sources);
        }
    }

    /// <summary>
    /// Sets the sources from which to subscribe for indexed events.
    /// If an empty list is provided, subscriptions will default to all available sources.
    /// If these sources have already been set, nothing happens.
    /// </summary>
    /// <param name="sources">The specified sources.</param>
    public void SetSources(params IndexedEventSource[] sources) =>
        SetSources(new HashSet<IndexedEventSource>(sources));

    /// <summary>
    /// Sets the sources from which to subscribe for indexed events.
    /// If an empty set is provided, subscriptions will default to all available sources.
    /// If these sources have already been set, nothing happens.
    /// </summary>
    /// <param name="sources">The specified sources.</param>
    public void SetSources(ICollection<IndexedEventSource> sources)
    {
        lock (_syncRoot)
        {
            if (_sources.SetEquals(sources))
            {
                return;
            }

            _sources = new HashSet<IndexedEventSource>(sources);
            UpdateSubscription(GetUndecoratedSymbol(), _sources);
        }
    }

    private static HashSet<object> DecorateSymbol(object symbol, HashSet<IndexedEventSource> sources)
    {
        if (sources.Count == 0)
        {
            return new HashSet<object> { symbol };
        }

        var symbols = new HashSet<object>();
        foreach (var source in sources)
        {
            symbols.Add(new IndexedEventSubscriptionSymbol(symbol, source));
        }

        return symbols;
    }

    private void UpdateSubscription(object symbol, HashSet<IndexedEventSource> sources) =>
        SetSymbols(DecorateSymbol(symbol, sources));

    /// <summary>
    /// A builder class for creating an instance of <see cref="IndexedTxModel{TE}"/>.
    /// </summary>
    public new class Builder : Builder<Builder, IndexedTxModel<TE>>
    {
        internal HashSet<IndexedEventSource> Sources { get; private set; } = new();

        /// <summary>
        /// Sets the sources from which to subscribe for indexed events.
        /// If no sources have been set, subscriptions will default to all possible sources.
        ///
        /// <p>The default value for this source is an empty set,
        /// which means that this model subscribes to all available sources.
        /// These sources can be changed later, after the model has been created,
        /// by calling <see cref="SetSources(IndexedEventSource[])"/>.</p>
        /// </summary>
        /// <param name="sources">The specified sources.</param>
        /// <returns><c>this</c> builder.</returns>
        public Builder WithSources(params IndexedEventSource[] sources)
        {
            Sources = new HashSet<IndexedEventSource>(sources);
            return this;
        }

        /// <summary>
        /// Sets the sources from which to subscribe for indexed events.
        /// If no sources have been set, subscriptions will default to all possible sources.
        ///
        /// <p>The default value for this source is an empty set,
        /// which means that this model subscribes to all available sources.
        /// These sources can be changed later, after the model has been created,
        /// by calling <see cref="SetSources(IndexedEventSource[])"/>.</p>
        /// </summary>
        /// <param name="sources">The specified sources.</param>
        /// <returns><c>this</c> builder.</returns>
        public Builder WithSources(ICollection<IndexedEventSource> sources)
        {
            Sources = new HashSet<IndexedEventSource>(sources);
            return this;
        }

        /// <summary>
        /// Builds an instance of <see cref="IndexedTxModel{TE}"/> based on the provided parameters.
        /// </summary>
        /// <returns>The created <see cref="IndexedTxModel{TE}"/>.</returns>
        public override IndexedTxModel<TE> Build() =>
            new(this);
    }
}
