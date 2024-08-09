// <copyright file="MultipleMarketDepthModel.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using DxFeed.Graal.Net.Api;
using DxFeed.Graal.Net.Api.Osub;
using DxFeed.Graal.Net.Events.Market;
using DxFeed.Graal.Net.Models;

namespace DxFeed.Graal.Net.Samples;

/// <summary>
/// This class manages multiple market depth models for different symbols.
/// It provides methods to add and remove symbols and to retrieve the current order book for a symbol.
/// </summary>
/// <typeparam name="T">The type of order.</typeparam>
public class MultipleMarketDepthModel<T>
    where T : OrderBase
{
    private readonly object _syncRoot = new();
    private readonly Dictionary<IndexedEventSubscriptionSymbol, MarketDepthModel<T>> _models = new();
    private readonly Dictionary<IndexedEventSubscriptionSymbol, OrderBook<T>> _books = new();
    private readonly MarketDepthModel<T>.Builder _builder;
    private readonly Listener? _listener;

    /// <summary>
    /// A delegate type for handling updates to the order book.
    /// </summary>
    /// <param name="symbol">The symbol for which the order book is updated.</param>
    /// <param name="book">The updated order book.</param>
    public delegate void Listener(IndexedEventSubscriptionSymbol symbol, OrderBook<T> book);

    private MultipleMarketDepthModel(Builder builder)
    {
        _builder = builder._builder;
        _listener = builder._listener;
    }

    /// <summary>
    /// Adds a symbol to the model.
    /// </summary>
    /// <param name="symbol">The symbol to add.</param>
    public void AddSymbol(IndexedEventSubscriptionSymbol symbol)
    {
        lock (_syncRoot)
        {
            if (_models.ContainsKey(symbol))
            {
                return;
            }
            _models.Add(symbol, CreateModel(symbol));
        }
    }

    /// <summary>
    /// Removes a symbol from the model.
    /// </summary>
    /// <param name="symbol">The symbol to remove.</param>
    public void RemoveSymbol(IndexedEventSubscriptionSymbol symbol)
    {
        lock (_syncRoot)
        {
            if (_models.TryGetValue(symbol, out var model))
            {
                model.Dispose();
                _models.Remove(symbol);
                _books.Remove(symbol);
            }
        }
    }

    /// <summary>
    /// Tries to get the order book for a given symbol.
    /// </summary>
    /// <param name="symbol">The symbol for which to get the order book.</param>
    /// <param name="book">The order book, if found.</param>
    /// <returns><c>true</c> if the order book is found; otherwise, <c>false</c>.</returns>
    public bool TryGetBook(IndexedEventSubscriptionSymbol symbol, [MaybeNullWhen(false)] out OrderBook<T> book)
    {
        lock (_syncRoot)
        {
            return _books.TryGetValue(symbol, out book);
        }
    }

    /// <summary>
    /// Creates a market depth model for a given symbol.
    /// </summary>
    /// <param name="symbol">The symbol for which to create the model.</param>
    /// <returns>The created market depth model.</returns>
    private MarketDepthModel<T> CreateModel(IndexedEventSubscriptionSymbol symbol) =>
        _builder
            .WithSymbol(symbol.EventSymbol)
            .WithSources(symbol.Source)
            .WithListener((buy, sell) =>
            {
                lock (_syncRoot)
                {
                    if (!_models.ContainsKey(symbol))
                    {
                        return;
                    }
                    if (_books.TryGetValue(symbol, out var book))
                    {
                        book.Buy = buy;
                        book.Sell = sell;
                    }
                    else
                    {
                        book = new OrderBook<T> { Buy = buy, Sell = sell };
                        _books.Add(symbol, book);
                    }

                    _listener?.Invoke(symbol, book);
                }
            })
            .Build();

    /// <summary>
    /// A builder class for creating instances of <see cref="MultipleMarketDepthModel{T}"/>.
    /// </summary>
    public class Builder
    {
        internal readonly MarketDepthModel<T>.Builder _builder = new();
        internal Listener? _listener;

        /// <summary>
        /// Sets the feed for the market depth model.
        /// </summary>
        /// <param name="feed">The feed to use.</param>
        /// <returns>The builder instance.</returns>
        public Builder WithFeed(DXFeed feed)
        {
            _builder.WithFeed(feed);
            return this;
        }

        /// <summary>
        /// Sets the depth limit for the market depth model.
        /// </summary>
        /// <param name="depthLimit">The depth limit.</param>
        /// <returns>The builder instance.</returns>
        public Builder WithDepthLimit(int depthLimit)
        {
            _builder.WithDepthLimit(depthLimit);
            return this;
        }

        /// <summary>
        /// Sets the aggregation period for the market depth model.
        /// </summary>
        /// <param name="aggregationPeriod">The aggregation period.</param>
        /// <returns>The builder instance.</returns>
        public Builder WithAggregationPeriod(TimeSpan aggregationPeriod)
        {
            _builder.WithAggregationPeriod((long)aggregationPeriod.TotalMilliseconds);
            return this;
        }

        /// <summary>
        /// Sets the listener for updates to the order book.
        /// </summary>
        /// <param name="listener">The listener to set.</param>
        /// <returns>The builder instance.</returns>
        public Builder WithListener(Listener listener)
        {
            _listener = listener;
            return this;
        }

        /// <summary>
        /// Builds and returns an instance of <see cref="MultipleMarketDepthModel{T}"/>.
        /// </summary>
        /// <returns>A new instance of <see cref="MultipleMarketDepthModel{T}"/>.</returns>
        public MultipleMarketDepthModel<T> Build() =>
            new(this);
    }
}
