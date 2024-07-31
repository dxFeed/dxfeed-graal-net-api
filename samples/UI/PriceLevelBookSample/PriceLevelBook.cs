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
using DxFeed.Graal.Net.Samples;

namespace PriceLevelBookSample;

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
    private readonly PriceLevelSet _buyOrders = new(BuyComparator);
    private readonly PriceLevelSet _sellOrders = new(SellComparator);
    private readonly IndexedTxModel<TE> _txModel;
    private readonly OnUpdate? _onUpdate;
    private readonly OnIncChange? _onIncChange;
    private CancellationTokenSource? _cts;
    private volatile bool _taskScheduled;
    private Task? _task;
    private long _aggregationPeriodMillis;
    private int _depthLimit;

    private PriceLevelBook(Builder builder)
    {
        _depthLimit = builder.DepthLimit;
        _buyOrders.DepthLimit = _depthLimit;
        _sellOrders.DepthLimit = _depthLimit;
        _onUpdate = builder.OnUpdate;
        _onIncChange = builder.OnIncChange;
        _aggregationPeriodMillis = builder.AggregationPeriodMillis;
        _txModel = builder.TxModelBuilder.WithListener(EventReceived).Build();
    }

    public delegate void OnUpdate(List<PriceLevel> buy, List<PriceLevel> sell);

    public delegate void OnIncChange(List<PriceLevel> add, List<PriceLevel> remove, List<PriceLevel> update);

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
            _buyOrders.DepthLimit = value;
            _sellOrders.DepthLimit = value;
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
                _buyOrders.ApplyChanges();
                _sellOrders.ApplyChanges();
                var add = _buyOrders.GetAddedLevels();
                add.AddRange(_sellOrders.GetAddedLevels());
                var remove = _buyOrders.GetRemovedLevels();
                remove.AddRange(_sellOrders.GetRemovedLevels());
                var update = _buyOrders.GetUpdatedLevels();
                update.AddRange(_sellOrders.GetUpdatedLevels());

                if (add.Any() || remove.Any() || update.Any())
                {
                    _onIncChange?.Invoke(add, remove, update);
                    _onUpdate?.Invoke(_buyOrders.GetBook(), _sellOrders.GetBook());
                }
            }
            finally
            {
                _buyOrders.ClearChanges();
                _sellOrders.ClearChanges();
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
                GetOrderSetForOrder(removed).Decrease(new PriceLevel(removed));
            }

            if (ShallAdd(order))
            {
                _ordersByIndex.Add(order.Index, order);
                GetOrderSetForOrder(order).Increase(new PriceLevel(order));
            }
        }

        return _buyOrders.IsChanged || _sellOrders.IsChanged;
    }

    private void ClearBySource(IndexedEventSource source)
    {
        _ordersByIndex
            .Where(p => p.Value.EventSource.Equals(source))
            .ToList()
            .ForEach(p => _ordersByIndex.Remove(p.Key));
        _buyOrders.ClearBySource(source);
        _sellOrders.ClearBySource(source);
    }

    private PriceLevelSet GetOrderSetForOrder(TE order) =>
        order.OrderSide == Side.Buy ? _buyOrders : _sellOrders;

    /// <summary>
    /// Builder class for constructing instances of MarketDepthModel.
    /// </summary>
    public class Builder
    {
        internal IndexedTxModel<TE>.Builder TxModelBuilder { get; } = IndexedTxModel<TE>.NewBuilder();

        internal OnUpdate? OnUpdate { get; private set; }

        internal OnIncChange? OnIncChange { get; private set; }

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
        /// <param name="onUpdate">The transaction listener.</param>
        /// <returns>The builder instance.</returns>
        public Builder WithListener(OnUpdate? onUpdate, OnIncChange? onIncChange)
        {
            OnUpdate = onUpdate;
            OnIncChange = onIncChange;
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
        public PriceLevelBook<TE> Build() =>
            new(this);
    }

    private sealed class PriceLevelSet
    {
        private readonly IComparer<PriceLevel> _comparator;
        private readonly SortedSet<PriceLevel> _priceLevels;
        private readonly List<PriceLevel> _addedLevels = new();
        private readonly List<PriceLevel> _removedLevels = new();
        private readonly List<PriceLevel> _updatedLevels = new();
        private SortedSet<PriceLevel> _snapshot;
        private int _depthLimit;

        public PriceLevelSet(IComparer<PriceLevel> comparator)
        {
            _comparator = comparator;
            _priceLevels = new SortedSet<PriceLevel>(comparator);
            _snapshot = new SortedSet<PriceLevel>(comparator);
        }

        public bool IsChanged { get; private set; }

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

        public void ClearBySource(IndexedEventSource source)
        {
            var removedLevels = _priceLevels.Where(priceLevel => priceLevel.EventSource.Equals(source)).ToList();
            foreach (var level in removedLevels)
            {
                _priceLevels.Remove(level);
                MarkAsChangedIfNeeded(level);
            }

            IsChanged = removedLevels.Count > 0;
        }

        public List<PriceLevel> GetBook() => _snapshot.ToList();

        public List<PriceLevel> GetAddedLevels() => new(_addedLevels);

        public List<PriceLevel> GetRemovedLevels() => new(_removedLevels);

        public List<PriceLevel> GetUpdatedLevels() => new(_updatedLevels);

        public void ApplyChanges()
        {
            var newSnapshot = new SortedSet<PriceLevel>(_comparator);
            var limit = IsDepthLimitUnbounded() ? int.MaxValue : DepthLimit;
            var it = _priceLevels.GetEnumerator();
            var i = 0;

            _addedLevels.Clear();
            _removedLevels.Clear();
            _updatedLevels.Clear();

            while (i < limit && it.MoveNext())
            {
                var current = new PriceLevel(it.Current);
                newSnapshot.Add(current);

                if (_snapshot.TryGetValue(current, out var oldLevel))
                {
                    if (!oldLevel.Size.Equals(current.Size))
                    {
                        _updatedLevels.Add(current);
                    }
                }
                else
                {
                    _addedLevels.Add(current);
                }

                i++;
            }

            foreach (var oldLevel in _snapshot)
            {
                if (!newSnapshot.Contains(oldLevel))
                {
                    _removedLevels.Add(oldLevel);
                }
            }

            _snapshot = newSnapshot;
        }

        public void ClearChanges()
        {
            _addedLevels.Clear();
            _removedLevels.Clear();
            _updatedLevels.Clear();
            IsChanged = false;
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
