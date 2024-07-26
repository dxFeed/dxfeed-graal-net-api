// <copyright file="MarketDepthModel.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DxFeed.Graal.Net.Api;
using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Events.Market;

namespace DxFeed.Graal.Net.Models;

/// <summary>
/// Represents a model for market depth, tracking buy and sell orders and notifies listener
/// of changes in the order book.
///
/// <p>This model can set depth limit and aggregation period. This model notifies
/// the user of received transactions through an installed <see cref="MarketDepthModelListener{TE}"/>.</p>
///
/// <p>The depth limit specifies the maximum number of buy or sell orders to maintain in the order book.
/// For example, if the depth limit is set to 10, the model will only keep track of the top 10 buy orders
/// and the top 10 sell orders. This helps in managing the size of the order book.</p>
///
/// <p>The aggregation period, specified in milliseconds, determines the frequency at which the model aggregates
/// and notifies changes in the order book to the listeners. For instance, if the aggregation period is
/// set to 1000 milliseconds the model will aggregate changes and notify listeners every second.
/// A value of 0 means that changes are notified immediately.</p>
///
/// <h3>Configuration</h3>
///
/// <p>This model must be configured using the <see cref="Builder"/> class, as most configuration
/// settings cannot be changed once the model is built. This model requires configuration
/// <see cref="Builder.WithSymbol"/> and it must be <see cref="Builder.WithFeed"/> attached to a <see cref="DXFeed"/>
/// instance to begin operation.</p>
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
/// <see cref="Dispose()"/> method.
/// Closed model becomes permanently detached from all feeds, removes all its listeners and is guaranteed
/// to be reclaimable by the garbage collector as soon as all external references to it are cleared.</p>
///
/// <h3>Threads and locks</h3>
///
/// <p>This class is thread-safe and can be used concurrently from multiple threads without external synchronization.</p>
/// The corresponding <see cref="MarketDepthModelListener{TE}"/> to never be concurrent.
/// </summary>
/// <typeparam name="TE">The type of order derived from <see cref="OrderBase"/>.</typeparam>
public sealed class MarketDepthModel<TE> : IDisposable
    where TE : OrderBase
{
    private static readonly Comparer<TE> OrderComparator = Comparer<TE>.Create((o1, o2) =>
    {
        var ind1 = o1.Scope == Scope.Order;
        var ind2 = o2.Scope == Scope.Order;
        if (ind1 && ind2)
        {
            // Both orders are individual orders.
            var c = o1.TimeSequence.CompareTo(o2.TimeSequence); // asc
            if (c != 0)
            {
                return c;
            }

            c = o1.Index.CompareTo(o2.Index); // asc
            return c;
        }
        else
        {
            if (ind1)
            {
                // First order is individual, second is not.
                return 1;
            }

            if (ind2)
            {
                // Second order is individual, first is not.
                return -1;
            }

            // Both orders are non-individual orders.
            var c = o2.Size.CompareTo(o1.Size); // desc
            if (c != 0)
            {
                return c;
            }

            c = o1.TimeSequence.CompareTo(o2.TimeSequence); // asc
            if (c != 0)
            {
                return c;
            }

            c = o1.Scope.CompareTo(o2.Scope); // asc
            if (c != 0)
            {
                return c;
            }

            c = o1.ExchangeCode.CompareTo(o2.ExchangeCode); // asc
            if (c != 0)
            {
                return c;
            }

            if (o1 is Order order1 && o2 is Order order2)
            {
                c = string.Compare(order1.MarketMaker, order2.MarketMaker, StringComparison.Ordinal); // asc
                if (c != 0)
                {
                    return c;
                }
            }

            c = o1.Index.CompareTo(o2.Index); // asc
            return c;
        }
    });

    private static readonly Comparer<TE> BuyComparator = Comparer<TE>.Create((o1, o2) =>
    {
        if (o1.Price < o2.Price)
        {
            return 1; // desc
        }

        if (o1.Price > o2.Price)
        {
            return -1;
        }

        return OrderComparator.Compare(o1, o2);
    });

    private static readonly Comparer<TE> SellComparator = Comparer<TE>.Create((o1, o2) =>
    {
        if (o1.Price < o2.Price)
        {
            return -1; // asc
        }

        if (o1.Price > o2.Price)
        {
            return 1;
        }

        return OrderComparator.Compare(o1, o2);
    });

    private readonly object _syncRoot = new();
    private readonly Dictionary<long, TE> _ordersByIndex = new();
    private readonly SortedOrderSet _buyOrders = new(BuyComparator);
    private readonly SortedOrderSet _sellOrders = new(SellComparator);
    private readonly IndexedTxModel<TE> _txModel;
    private readonly MarketDepthModelListener<TE>? _listener;
    private CancellationTokenSource? _cts;
    private volatile bool _taskScheduled;
    private Task? _task;
    private long _aggregationPeriodMillis;
    private int _depthLimit;

    private MarketDepthModel(Builder builder)
    {
        _depthLimit = builder.DepthLimit;
        _buyOrders.SetDepthLimit(_depthLimit);
        _sellOrders.SetDepthLimit(_depthLimit);
        _listener = builder.Listener;
        _aggregationPeriodMillis = builder.AggregationPeriodMillis;
        _txModel = builder.TxModelBuilder.WithListener(EventReceived).Build();
    }

    /// <summary>
    /// Creates a new builder instance for constructing a MarketDepthModel.
    /// </summary>
    /// <returns>A new instance of the builder.</returns>
    public static Builder NewBuilder() =>
        new();

    /// <summary>
    /// Gets the depth limit of the order book.
    /// </summary>
    /// <returns>The current depth limit.</returns>
    public int GetDepthLimit()
    {
        lock (_syncRoot)
        {
            return _depthLimit;
        }
    }

    /// <summary>
    /// Sets the depth limit of the order book.
    /// </summary>
    /// <param name="value">The new depth limit value.</param>
    public void SetDepthLimit(int value)
    {
        lock (_syncRoot)
        {
            if (value < 0)
            {
                value = 0;
            }

            if (value == _depthLimit)
            {
                return;
            }

            _depthLimit = value;
            _buyOrders.SetDepthLimit(value);
            _sellOrders.SetDepthLimit(value);
            TryCancelTask();
            NotifyListeners();
        }
    }

    /// <summary>
    /// Gets the aggregation period in milliseconds.
    /// </summary>
    /// <returns>The current aggregation period.</returns>
    public long GetAggregationPeriod()
    {
        lock (_syncRoot)
        {
            return _aggregationPeriodMillis;
        }
    }

    /// <summary>
    /// Sets the aggregation period in milliseconds.
    /// </summary>
    /// <param name="value">The new aggregation period value.</param>
    public void SetAggregationPeriod(long value)
    {
        lock (_syncRoot)
        {
            if (value < 0)
            {
                value = 0;
            }

            if (value == _aggregationPeriodMillis)
            {
                return;
            }

            _aggregationPeriodMillis = value;
            RescheduleTaskIfNeeded(_aggregationPeriodMillis);
        }
    }

    /// <summary>
    /// Closes this model and makes it <i>permanently detached</i>.
    /// </summary>
    public void Dispose() =>
        _txModel.Dispose();

    private static bool ShallAdd(TE order) =>
        order.HasSize && (order.EventFlags & EventFlags.RemoveEvent) == 0;

    private void EventReceived(IndexedEventSource source, IEnumerable<TE> events, bool isSnapshot)
    {
        lock (_syncRoot)
        {
            if (!Update(source, events, isSnapshot))
            {
                return;
            }

            if (isSnapshot || _aggregationPeriodMillis == 0)
            {
                TryCancelTask();
                NotifyListeners();
            }
            else
            {
                ScheduleTaskIfNeeded(_aggregationPeriodMillis);
            }
        }
    }

    private void NotifyListeners()
    {
        lock (_syncRoot)
        {
            try
            {
                _listener?.Invoke(GetBuyOrders(), GetSellOrders());
            }
            finally
            {
                _taskScheduled = false;
            }
        }
    }

    private void ScheduleTaskIfNeeded(long delay)
    {
        lock (_syncRoot)
        {
            if (!_taskScheduled)
            {
                _taskScheduled = true;
                _cts = new CancellationTokenSource();
                _task = Task.Delay(TimeSpan.FromMilliseconds(delay), _cts.Token)
                    .ContinueWith(_ => NotifyListeners(), TaskScheduler.Default);
            }
        }
    }

    private void RescheduleTaskIfNeeded(long delay)
    {
        lock (_syncRoot)
        {
            if (TryCancelTask() && delay != 0)
            {
                ScheduleTaskIfNeeded(delay);
            }
        }
    }

    private bool TryCancelTask()
    {
        lock (_syncRoot)
        {
            if (_taskScheduled && _task is { IsCompleted: false })
            {
                _cts?.Cancel();
                _cts?.Dispose();
                _taskScheduled = false;
                return true;
            }

            return false;
        }
    }

    private bool Update(IndexedEventSource source, IEnumerable<TE> events, bool isSnapshot)
    {
        if (isSnapshot)
        {
            ClearBySource(source);
        }

        foreach (var order in events)
        {
            if (_ordersByIndex.TryGetValue(order.Index, out var removed))
            {
                _ordersByIndex.Remove(order.Index);
                GetOrderSetForOrder(removed).Remove(removed);
            }

            if (ShallAdd(order))
            {
                _ordersByIndex.Add(order.Index, order);
                GetOrderSetForOrder(order).Add(order);
            }
        }

        return _buyOrders.IsChanged || _sellOrders.IsChanged;
    }

    private List<TE> GetBuyOrders() =>
        _buyOrders.ToList();

    private List<TE> GetSellOrders() =>
        _sellOrders.ToList();

    private void ClearBySource(IndexedEventSource source)
    {
        _ordersByIndex
            .Where(p => p.Value.EventSource.Equals(source))
            .ToList()
            .ForEach(p => _ordersByIndex.Remove(p.Key));
        _buyOrders.ClearBySource(source);
        _sellOrders.ClearBySource(source);
    }

    private SortedOrderSet GetOrderSetForOrder(TE order) =>
        order.OrderSide == Side.Buy ? _buyOrders : _sellOrders;

    /// <summary>
    /// Builder class for constructing instances of MarketDepthModel.
    /// </summary>
    public class Builder
    {
        internal IndexedTxModel<TE>.Builder TxModelBuilder { get; } = IndexedTxModel<TE>.NewBuilder();

        internal MarketDepthModelListener<TE>? Listener { get; private set; }

        internal long AggregationPeriodMillis { get; private set; }

        internal int DepthLimit { get; private set; }

        /// <summary>
        /// Sets the <see cref="DXFeed"/> for the model being created.
        /// The feed cannot be attached after the model has been built.
        /// </summary>
        /// <param name="feed">The <see cref="DXFeed"/>.</param>
        /// <returns>The builder instance.</returns>
        public Builder WithFeed(DXFeed feed)
        {
            TxModelBuilder.WithFeed(feed);
            return this;
        }

        /// <summary>
        /// Sets the subscription symbol for the model being created.
        /// The symbol cannot be added or changed after the model has been built.
        /// </summary>
        /// <param name="symbol">The subscription symbol.</param>
        /// <returns>The builder instance.</returns>
        public Builder WithSymbol(object symbol)
        {
            TxModelBuilder.WithSymbol(symbol);
            return this;
        }

        /// <summary>
        /// Sets the listener for transaction notifications.
        /// The listener cannot be changed or added once the model has been built.
        /// </summary>
        /// <param name="modelListener">The transaction listener.</param>
        /// <returns>The builder instance.</returns>
        public Builder WithListener(MarketDepthModelListener<TE> modelListener)
        {
            Listener = modelListener;
            return this;
        }

        /// <summary>
        /// Sets the sources from which to subscribe for indexed events.
        /// If no sources have been set, subscriptions will default to all possible sources.
        /// </summary>
        /// <remarks>
        /// The default value for this source is an empty set,
        /// which means that this model subscribes to all available sources.
        /// </remarks>
        /// <param name="sources">The specified sources.</param>
        /// <returns><c>this</c> builder.</returns>
        public Builder WithSources(params IndexedEventSource[] sources)
        {
            TxModelBuilder.WithSources(sources);
            return this;
        }

        /// <summary>
        /// Sets the sources from which to subscribe for indexed events.
        /// If no sources have been set, subscriptions will default to all possible sources.
        /// </summary>
        /// <remarks>
        /// The default value for this source is an empty set,
        /// which means that this model subscribes to all available sources.
        /// </remarks>
        /// <param name="sources">The specified sources.</param>
        /// <returns><c>this</c> builder.</returns>
        public Builder WithSources(ICollection<IndexedEventSource> sources)
        {
            TxModelBuilder.WithSources(sources);
            return this;
        }

        /// <summary>
        /// Sets the aggregation period.
        /// </summary>
        /// <param name="aggregationPeriodMillis">The aggregation period in milliseconds.</param>
        /// <returns><c>this</c> builder.</returns>
        public Builder WithAggregationPeriod(long aggregationPeriodMillis)
        {
            if (aggregationPeriodMillis < 0)
            {
                aggregationPeriodMillis = 0;
            }

            AggregationPeriodMillis = aggregationPeriodMillis;
            return this;
        }

        /// <summary>
        /// Sets the depth limit.
        /// </summary>
        /// <param name="depthLimit">The depth limit.</param>
        /// <returns><c>this</c> builder.</returns>
        public Builder WithDepthLimit(int depthLimit)
        {
            if (depthLimit < 0)
            {
                depthLimit = 0;
            }

            DepthLimit = depthLimit;
            return this;
        }

        /// <summary>
        /// Builds an instance of <see cref="MarketDepthModel{TE}"/> based on the provided parameters.
        /// </summary>
        /// <returns>The created <see cref="MarketDepthModel{TE}"/>.</returns>
        public MarketDepthModel<TE> Build() =>
            new(this);
    }

    /// <summary>
    /// Represents a set of orders, sorted by a comparator.
    /// </summary>
    private sealed class SortedOrderSet
    {
        private readonly List<TE> _snapshot = new();
        private readonly IComparer<TE> _comparator;
        private readonly SortedSet<TE> _orders;
        private int _depthLimit;

        /// <summary>
        /// Initializes a new instance of the <see cref="SortedOrderSet"/> class with specified comparator.
        /// </summary>
        /// <param name="comparator">The comparator to use for sorting orders.</param>
        public SortedOrderSet(IComparer<TE> comparator)
        {
            _comparator = comparator;
            _orders = new SortedSet<TE>(comparator);
        }

        /// <summary>
        /// Gets a value indicating whether this set has changed.
        /// </summary>
        public bool IsChanged { get; private set; }

        /// <summary>
        /// Sets the depth limit.
        /// </summary>
        /// <param name="depthLimit">The new depth limit.</param>
        public void SetDepthLimit(int depthLimit)
        {
            if (_depthLimit == depthLimit)
            {
                return;
            }

            _depthLimit = depthLimit;
            IsChanged = true;
        }

        /// <summary>
        /// Adds an order to the set.
        /// </summary>
        /// <param name="order">The order to add.</param>
        public void Add(TE order)
        {
            if (_orders.Add(order))
            {
                MarkAsChangedIfNeeded(order);
            }
        }

        /// <summary>
        /// Removes an order from the set.
        /// </summary>
        /// <param name="order">The order to remove.</param>
        public void Remove(TE order)
        {
            if (_orders.Remove(order))
            {
                MarkAsChangedIfNeeded(order);
            }
        }

        /// <summary>
        /// Clears orders from the set by source.
        /// </summary>
        /// <param name="source">The source to clear orders by.</param>
        public void ClearBySource(IndexedEventSource source) =>
            IsChanged = _orders.RemoveWhere(order => order.EventSource.Equals(source)) > 0;

        /// <summary>
        /// Converts the set to a list.
        /// </summary>
        /// <returns>The list of orders.</returns>
        public List<TE> ToList()
        {
            if (IsChanged)
            {
                UpdateSnapshot();
            }

            return new List<TE>(_snapshot);
        }

        private void UpdateSnapshot()
        {
            IsChanged = false;
            _snapshot.Clear();
            var limit = IsDepthLimitUnbounded() ? int.MaxValue : _depthLimit;
            var it = _orders.GetEnumerator();
            for (var i = 0; i < limit && it.MoveNext(); ++i)
            {
                _snapshot.Add(it.Current);
            }
        }

        private void MarkAsChangedIfNeeded(TE order)
        {
            if (IsChanged)
            {
                return;
            }

            if (IsDepthLimitUnbounded() || IsOrderCountWithinDepthLimit() || IsOrderWithinDepthLimit(order))
            {
                IsChanged = true;
            }
        }

        private bool IsDepthLimitUnbounded() =>
            _depthLimit <= 0 || _depthLimit == int.MaxValue;

        private bool IsOrderCountWithinDepthLimit() =>
            _orders.Count <= _depthLimit;

        private bool IsOrderWithinDepthLimit(TE order)
        {
            if (_snapshot.Count == 0)
            {
                return true;
            }

            var last = _snapshot[_snapshot.Count - 1];
            return _comparator.Compare(last, order) >= 0;
        }
    }
}
