// <copyright file="PriceLevelBook.cs" company="Devexperts LLC">
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
using DxFeed.Graal.Net.Models;

namespace PriceLevelBookSample;

/// <summary>
/// Represents a price level book (market by price) that aggregates individual orders (market by order).
/// For this model, use market by order sources such as <see cref="OrderSource.NTV"/>, <see cref="OrderSource.GLBX"/>, etc.
/// </summary>
/// <typeparam name="TE">The type of the order.</typeparam>
public sealed class PriceLevelBook<TE> : IDisposable
    where TE : OrderBase
{
    private static readonly Comparer<PriceLevel> BuyComparator = Comparer<PriceLevel>.Create((p1, p2) =>
    {
        if (p1.Price < p2.Price)
        {
            return 1; // desc
        }

        if (p1.Price > p2.Price)
        {
            return -1;
        }

        return 0;
    });

    private static readonly Comparer<PriceLevel> SellComparator = Comparer<PriceLevel>.Create((p1, p2) =>
    {
        if (p1.Price < p2.Price)
        {
            return -1; // asc
        }

        if (p1.Price > p2.Price)
        {
            return 1;
        }

        return 0;
    });

    private readonly object _syncRoot = new();
    private readonly Dictionary<long, TE> _ordersByIndex = new();
    private readonly SortedPriceLevelSet _buyPriceLevels = new(BuyComparator);
    private readonly SortedPriceLevelSet _sellPriceLevels = new(SellComparator);
    private readonly IndexedTxModel<TE> _txModel;
    private readonly Listener? _listener;
    private CancellationTokenSource? _cts;
    private volatile bool _taskScheduled;
    private Task? _task;
    private long _aggregationPeriodMillis;
    private int _depthLimit;

    private PriceLevelBook(Builder builder)
    {
        _depthLimit = builder.DepthLimit;
        _buyPriceLevels.DepthLimit = _depthLimit;
        _sellPriceLevels.DepthLimit = _depthLimit;
        _listener = builder.Listener;
        _aggregationPeriodMillis = builder.AggregationPeriodMillis;
        _txModel = builder.TxModelBuilder.WithListener(EventReceived).Build();
    }

    public delegate void Listener(List<PriceLevel> buy, List<PriceLevel> sell);

    /// <summary>
    /// Creates a new builder instance for constructing a PriceLevelBook.
    /// </summary>
    /// <returns>A new instance of the builder.</returns>
    public static Builder NewBuilder() =>
        new();

    /// <summary>
    /// Gets the depth limit of the price level book.
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
    /// Sets the depth limit of the price level book.
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
            _buyPriceLevels.DepthLimit = value;
            _sellPriceLevels.DepthLimit = value;
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
                _listener?.Invoke(_buyPriceLevels.ToList(), _sellPriceLevels.ToList());
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
                GetPriceLevelSetForOrder(removed).Decrease(new PriceLevel(removed));
            }

            if (ShallAdd(order))
            {
                _ordersByIndex.Add(order.Index, order);
                GetPriceLevelSetForOrder(order).Increase(new PriceLevel(order));
            }
        }

        return _buyPriceLevels.IsChanged || _sellPriceLevels.IsChanged;
    }

    private void ClearBySource(IndexedEventSource source)
    {
        _ordersByIndex
            .Where(p => p.Value.EventSource.Equals(source))
            .ToList()
            .ForEach(p => _ordersByIndex.Remove(p.Key));
        _buyPriceLevels.ClearBySource(source);
        _sellPriceLevels.ClearBySource(source);
    }

    private SortedPriceLevelSet GetPriceLevelSetForOrder(TE order) =>
        order.OrderSide == Side.Buy ? _buyPriceLevels : _sellPriceLevels;

    /// <summary>
    /// Builder class for constructing instances of PriceLevelBook.
    /// </summary>
    public class Builder
    {
        internal IndexedTxModel<TE>.Builder TxModelBuilder { get; } = IndexedTxModel<TE>.NewBuilder();

        internal Listener? Listener { get; private set; }

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
        /// <param name="listener">The transaction listener.</param>
        /// <returns>The builder instance.</returns>
        public Builder WithListener(Listener? listener)
        {
            Listener = listener;
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
        /// Builds an instance of <see cref="PriceLevelBook{TE}"/> based on the provided parameters.
        /// </summary>
        /// <returns>The created <see cref="PriceLevelBook{TE}"/>.</returns>
        public PriceLevelBook<TE> Build() =>
            new(this);
    }

    /// <summary>
    /// Represents a set of orders, sorted by a comparator.
    /// </summary>
    private sealed class SortedPriceLevelSet
    {
        private readonly List<PriceLevel> _snapshot = new();
        private readonly IComparer<PriceLevel> _comparator;
        private readonly SortedSet<PriceLevel> _priceLevels;
        private int _depthLimit;

        /// <summary>
        /// Initializes a new instance of the <see cref="SortedPriceLevelSet"/> class with specified comparator.
        /// </summary>
        /// <param name="comparator">The comparator to use for sorting orders.</param>
        public SortedPriceLevelSet(IComparer<PriceLevel> comparator)
        {
            _comparator = comparator;
            _priceLevels = new SortedSet<PriceLevel>(comparator);
        }

        /// <summary>
        /// Gets a value indicating whether this set has changed.
        /// </summary>
        public bool IsChanged { get; private set; }

        /// <summary>
        /// Gets or sets depth limit.
        /// </summary>
        public int DepthLimit
        {
            get => _depthLimit;
            set
            {
                if (_depthLimit == value)
                {
                    return;
                }

                _depthLimit = value;
                IsChanged = true;
            }
        }

        /// <summary>
        /// Increase price level size or add new price level.
        /// </summary>
        /// <param name="level">The price level.</param>
        public void Increase(PriceLevel level)
        {
            if (_priceLevels.TryGetValue(level, out var existLevel))
            {
                _priceLevels.Remove(existLevel);
                existLevel.Size += level.Size;
                _priceLevels.Add(new PriceLevel(existLevel));
                MarkAsChangedIfNeeded(existLevel);
            }
            else
            {
                _priceLevels.Add(level);
                MarkAsChangedIfNeeded(level);
            }
        }

        /// <summary>
        /// Decrease price level size or remove exist price level.
        /// </summary>
        /// <param name="level">The price level.</param>
        public void Decrease(PriceLevel level)
        {
            if (_priceLevels.TryGetValue(level, out var existLevel))
            {
                var newSize = existLevel.Size - level.Size;
                if (newSize <= 0)
                {
                    _priceLevels.Remove(existLevel);
                    MarkAsChangedIfNeeded(existLevel);
                }
                else
                {
                    _priceLevels.Remove(existLevel);
                    existLevel.Size = newSize;
                    _priceLevels.Add(new PriceLevel(existLevel));
                    MarkAsChangedIfNeeded(existLevel);
                }
            }
        }

        /// <summary>
        /// Clears price levels from the set by source.
        /// </summary>
        /// <param name="source">The source to clear price levels by.</param>
        public void ClearBySource(IndexedEventSource source) =>
            IsChanged = _priceLevels.RemoveWhere(pl => pl.EventSource.Equals(source)) > 0;

        /// <summary>
        /// Converts the set to a list.
        /// </summary>
        /// <returns>The list of price levels.</returns>
        public List<PriceLevel> ToList()
        {
            if (IsChanged)
            {
                UpdateSnapshot();
            }

            return new List<PriceLevel>(_snapshot);
        }

        private void UpdateSnapshot()
        {
            IsChanged = false;
            _snapshot.Clear();
            var limit = IsDepthLimitUnbounded() ? int.MaxValue : _depthLimit;
            var it = _priceLevels.GetEnumerator();
            for (var i = 0; i < limit && it.MoveNext(); ++i)
            {
                _snapshot.Add(it.Current);
            }
        }

        private void MarkAsChangedIfNeeded(PriceLevel level)
        {
            if (IsChanged)
            {
                return;
            }

            if (IsDepthLimitUnbounded() || IsPriceLevelCountWithinDepthLimit() || IsPriceLevelWithinDepthLimit(level))
            {
                IsChanged = true;
            }
        }

        private bool IsDepthLimitUnbounded() =>
            DepthLimit <= 0 || DepthLimit == int.MaxValue;

        private bool IsPriceLevelCountWithinDepthLimit() =>
            _priceLevels.Count <= DepthLimit;

        private bool IsPriceLevelWithinDepthLimit(PriceLevel priceLevel)
        {
            if (_snapshot.Count == 0)
            {
                return true;
            }

            var last = _snapshot.Last();
            return _comparator.Compare(last, priceLevel) >= 0;
        }
    }
}
