// <copyright file="AbstractTxModel.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using DxFeed.Graal.Net.Api;
using DxFeed.Graal.Net.Events;

namespace DxFeed.Graal.Net.Models;

/// <summary>
/// Abstract base class for models that handle transactions of <see cref="IIndexedEvent"/>.
/// This model manages all snapshot and transaction logic, subscription handling, and listener notifications.
///
/// <p>This model is designed to handle incremental transactions. Users of this model only see the list
/// of events in a consistent state. This model delays incoming events that are part of an incomplete snapshot
/// or ongoing transaction until the snapshot is complete or the transaction has ended.</p>
///
/// <h3>Configuration</h3>
///
/// <p>This model must be configured using the <see cref="Builder"/>. Specific implementations can add additional
/// configuration options as needed. This model requires a call to the <see cref="SetSymbols"/> method
/// (all inheritors must call this method) for subscription.</p>
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
/// The corresponding <see cref="TxModelListener{TE}"/> to never be concurrent
/// </summary>
/// <typeparam name="TE">The type of indexed events processed by this model.</typeparam>
public abstract class AbstractTxModel<TE> : IDisposable
    where TE : class, IIndexedEvent
{
    private readonly ConcurrentDictionary<IndexedEventSource, TxEventProcessor<TE>> _processorsBySource = new();
    private readonly HashSet<TxEventProcessor<TE>> _readyProcessors = new();
    private readonly DXFeedSubscription _subscription;
    private readonly object _symbol;
    private readonly TxModelListener<TE>? _listener;

    private protected AbstractTxModel(Builder builder)
    {
        _symbol = builder.Symbol ?? throw new InvalidOperationException("The 'symbol' must not be null.");
        var feed = builder.Feed ?? throw new InvalidOperationException("The 'feed' must not be null.");
        IsBatchProcessing = builder.IsBatchProcessing;
        IsSnapshotProcessing = builder.IsSnapshotProcessing;
        _listener = builder.Listener;
        _subscription = feed.CreateSubscription(typeof(TE));
        _subscription.AddEventListener(ProcessEvents);
    }

    /// <summary>
    /// Gets a value indicating whether if batch processing is enabled.
    /// See <see cref="Builder{TB,TM}.WithBatchProcessing(bool)"/>.
    /// </summary>
    /// <returns>
    /// <c>true</c> if batch processing is enabled; otherwise, <c>false</c>.
    /// </returns>
    public bool IsBatchProcessing { get; }

    /// <summary>
    /// Gets a value indicating whether if snapshot processing is enabled.
    /// See <see cref="Builder{TB,TM}.WithSnapshotProcessing(bool)"/>.
    /// </summary>
    /// <returns>
    /// <c>true</c> if snapshot processing is enabled; otherwise, <c>false</c>.
    /// </returns>
    public bool IsSnapshotProcessing { get; }

    /// <summary>
    /// Closes this model and makes it <i>permanently detached</i>.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Processes a list of events, updating the relevant processors and handling batch processing.
    /// </summary>
    /// <param name="events">The list of events to process.</param>
    internal void ProcessEvents(IEnumerable<IEventType> events)
    {
        try
        {
            IndexedEventSource? source = null;
            TxEventProcessor<TE>? currentProcessor = null;
            foreach (var e in events.OfType<TE>())
            {
                if (source == null || source.Id != e.EventSource.Id)
                {
                    source = e.EventSource;
                    currentProcessor = GetOrCreateProcessor(source);
                }

                if (currentProcessor!.ProcessEvent(e))
                {
                    _readyProcessors.Add(currentProcessor);
                }
            }

            foreach (var processor in _readyProcessors)
            {
                processor.ReceiveAllEventsInBatch();
            }
        }
        catch (Exception e)
        {
            // ToDo Add log entry.
            Console.Error.WriteLine($"Exception in {GetType().Name} event listener: {e}");
        }
        finally
        {
            _readyProcessors.Clear();
        }
    }

    /// <summary>
    /// Gets the undecorated symbol associated with the model.
    /// </summary>
    /// <returns>The undecorated symbol associated with the model.</returns>
    protected object GetUndecoratedSymbol() =>
        _symbol;

    /// <summary>
    /// Sets the subscription symbols for the model.
    /// </summary>
    /// <param name="symbols">The set of symbols to subscribe to.</param>
    protected void SetSymbols(HashSet<object> symbols) =>
        _subscription.SetSymbols(symbols);

    /// <inheritdoc cref="Dispose()"/>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _subscription.Dispose();
        }
    }

    private TxEventProcessor<TE> GetOrCreateProcessor(IndexedEventSource source) =>
        _processorsBySource.GetOrAdd(source, src => new(
            IsBatchProcessing,
            IsSnapshotProcessing,
            (transaction, isSnapshot) =>
            {
                _listener?.Invoke(src, new List<TE>(transaction), isSnapshot);
            }));

    /// <summary>
    /// Non-generic version, for erasing a generic type.
    /// </summary>
    public abstract class Builder
    {
        internal bool IsBatchProcessing { get; set; } = true;

        internal bool IsSnapshotProcessing { get; set; }

        internal DXFeed? Feed { get; set; }

        internal object? Symbol { get; set; }

        internal TxModelListener<TE>? Listener { get; set; }
    }

    /// <summary>
    /// Abstract builder for building models inherited from <see cref="AbstractTxModel{TE}"/>.
    /// Specific implementations can add additional configuration options to this builder.
    ///
    /// <p>Inheritors of this class must override the abstract method <see cref="Build"/> to build a specific model.</p>
    /// </summary>
    /// <typeparam name="TB">The type of the builder subclass.</typeparam>
    /// <typeparam name="TM">The type of the model subclass.</typeparam>
    public abstract class Builder<TB, TM> : Builder
        where TB : Builder<TB, TM>
        where TM : AbstractTxModel<TE>
    {
        /// <summary>
        /// Enables or disables batch processing.
        /// <b>This is enabled by default</b>.
        ///
        /// <p>If batch processing is disabled, the model will notify the listener
        /// <b>separately for each transaction</b> (even if it is represented by a single event);
        /// otherwise, transactions can be combined in a single listener call.</p>
        ///
        /// <p>A transaction may represent either a snapshot or update events that are received after a snapshot.
        /// Whether this flag is set or not, the model will always notify listeners that a snapshot has been received
        /// and will not combine multiple snapshots or a snapshot with another transaction
        /// into a single listener notification.</p>
        /// </summary>
        /// <param name="isBatchProcessing"><c>true</c> to enable batch processing; <c>false</c> otherwise.</param>
        /// <returns>The builder instance.</returns>
        public TB WithBatchProcessing(bool isBatchProcessing)
        {
            IsBatchProcessing = isBatchProcessing;
            return (TB)this;
        }

        /// <summary>
        /// Enables or disables snapshot processing.
        /// <b>This is disabled by default</b>.
        ///
        /// <p>If snapshot processing is enabled, transactions representing a snapshot will be processed as follows:
        /// events that are marked for removal will be removed, repeated indexes will be merged, and
        /// event flags of events are set to zero; otherwise, the user will see the snapshot in raw form,
        /// with possible repeated indexes, events marked for removal, and event flags unchanged.</p>
        ///
        /// <p>Whether this flag is set or not, in transactions that are not a snapshot, events that are marked
        /// for removal will not be removed, repeated indexes will not be merged, and
        /// event flags of events will not be changed.
        /// This flag only affects the processing of transactions that are a snapshot.</p>
        /// </summary>
        /// <param name="isSnapshotProcessing"><c>true</c> to enable snapshot processing; <c>false</c> otherwise.</param>
        /// <returns>The builder instance.</returns>
        public TB WithSnapshotProcessing(bool isSnapshotProcessing)
        {
            IsSnapshotProcessing = isSnapshotProcessing;
            return (TB)this;
        }

        /// <summary>
        /// Sets the <see cref="DXFeed"/> for the model being created.
        /// The feed cannot be attached after the model has been built.
        /// </summary>
        /// <param name="feed">The <see cref="DXFeed"/>.</param>
        /// <returns>The builder instance.</returns>
        public TB WithFeed(DXFeed feed)
        {
            Feed = feed;
            return (TB)this;
        }

        /// <summary>
        /// Sets the subscription symbol for the model being created.
        /// The symbol cannot be added or changed after the model has been built.
        /// </summary>
        /// <param name="symbol">The subscription symbol.</param>
        /// <returns>The builder instance.</returns>
        public TB WithSymbol(object symbol)
        {
            Symbol = symbol;
            return (TB)this;
        }

        /// <summary>
        /// Sets the listener for transaction notifications.
        /// The listener cannot be changed or added once the model has been built.
        /// </summary>
        /// <param name="listener">The transaction listener.</param>
        /// <returns>The builder instance.</returns>
        public TB WithListener(TxModelListener<TE> listener)
        {
            Listener = listener;
            return (TB)this;
        }

        /// <summary>
        /// Builds an instance of the model based on the provided parameters.
        /// </summary>
        /// <returns>The created model.</returns>
        public abstract TM Build();
    }
}
