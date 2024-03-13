// <copyright file="OrderSource.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using DxFeed.Graal.Net.Api;
using DxFeed.Graal.Net.Api.Osub;
using Microsoft.Extensions.Caching.Memory;

// This file is marked as auto-generated in .editorconfig for disable style warning.
// Because the current fields naming is important to use.
// ReSharper disable InconsistentNaming for order source.

namespace DxFeed.Graal.Net.Events.Market;

/// <summary>
/// Identifies source of <see cref="Order"/>, <see cref="AnalyticOrder"/> and <see cref="SpreadOrder"/> events.
/// <ul>
/// <li>
/// <em>Synthetic</em> sources <see cref="COMPOSITE_BID"/>, <see cref="COMPOSITE_ASK"/>,
///     <see cref="REGIONAL_BID"/> and <see cref="REGIONAL_ASK"/> are provided for convenience of a consolidated
///     order book and are automatically generated based on the corresponding <see cref="Quote"/> events.
/// </li>
/// <li>
/// <em>Aggregate</em> sources <see cref="AGGREGATE_BID"/>  and <see cref="AGGREGATE_ASK"/>  provide
///     futures depth (aggregated by price level) and NASDAQ Level II (top of book for each market maker).
///     These source cannot be directly published to via dxFeed API.
/// </li>
/// <li>
/// <see cref="IsPublishable"/> sources <see cref="DEFAULT"/>, <see cref="NTV"/> and <see cref="ISE"/>
///     support full range of dxFeed API features.
/// </li>
/// </ul>
/// For more details see <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/event/market/OrderSource.html">Javadoc</a>.
/// </summary>
public class OrderSource : IndexedEventSource
{
    /// <summary>
    /// Local in-memory cache for <see cref="OrderSource"/> instances.
    /// The cache permanently contains "builtin" order sources.
    /// And can contain custom in limited quantity.
    /// </summary>
    private static readonly IMemoryCache CacheSource = new MemoryCache(new MemoryCacheOptions { SizeLimit = 100 });

    /// <summary>
    /// Default source for publishing custom order books.
    /// </summary>
    public static new readonly OrderSource DEFAULT = new(0, "DEFAULT",
        PubOrder | PubAnalyticOrder | PubOtcMarketdOrder | PubSpreadOrder | FullOrderBook);

    /// <summary>
    /// Bid side of a composite <see cref="Quote"/>.
    /// It is a <em>synthetic</em> source.
    /// The subscription on composite <see cref="Quote"/> event is observed when this source is subscribed to.
    /// </summary>
    public static readonly OrderSource COMPOSITE_BID = new(1, "COMPOSITE_BID", 0);

    /// <summary>
    /// Ask side of a composite <see cref="Quote"/>.
    /// It is a <em>synthetic</em> source.
    /// The subscription on composite <see cref="Quote"/> event is observed when this source is subscribed to.
    /// </summary>
    public static readonly OrderSource COMPOSITE_ASK = new(2, "COMPOSITE_ASK", 0);

    /// <summary>
    /// Bid side of a regional <see cref="Quote"/>.
    /// It is a <em>synthetic</em> source.
    /// The subscription on regional <see cref="Quote"/> event is observed when this source is subscribed to.
    /// </summary>
    public static readonly OrderSource REGIONAL_BID = new(3, "REGIONAL_BID", 0);

    /// <summary>
    /// Ask side of a regional <see cref="Quote"/>.
    /// It is a <em>synthetic</em> source.
    /// The subscription on regional <see cref="Quote"/> event is observed when this source is subscribed to.
    /// </summary>
    public static readonly OrderSource REGIONAL_ASK = new(4, "REGIONAL_ASK", 0);

    /// <summary>
    /// Bid side of an aggregate order book (futures depth and NASDAQ Level II).
    /// This source cannot be directly published via dxFeed API, but otherwise it is fully operational.
    /// </summary>
    public static readonly OrderSource AGGREGATE_BID = new(5, "AGGREGATE_BID", 0);

    /// <summary>
    /// Ask side of an aggregate order book (futures depth and NASDAQ Level II).
    /// This source cannot be directly published via dxFeed API, but otherwise it is fully operational.
    /// </summary>
    public static readonly OrderSource AGGREGATE_ASK = new(6, "AGGREGATE_ASK", 0);

    /// <summary>
    /// NASDAQ Total View.
    /// </summary>
    public static readonly OrderSource NTV = new("NTV", PubOrder | FullOrderBook);

    /// <summary>
    /// NASDAQ Total View. Record for price level book.
    /// </summary>
    public static readonly OrderSource ntv = new("ntv", PubOrder);

    /// <summary>
    /// NASDAQ Futures Exchange.
    /// </summary>
    public static readonly OrderSource NFX = new("NFX", PubOrder);

    /// <summary>
    /// NASDAQ eSpeed.
    /// </summary>
    public static readonly OrderSource ESPD = new("ESPD", PubOrder);

    /// <summary>
    /// NASDAQ Fixed Income.
    /// </summary>
    public static readonly OrderSource XNFI = new("XNFI", PubOrder);

    /// <summary>
    /// Intercontinental Exchange.
    /// </summary>
    public static readonly OrderSource ICE = new("ICE", PubOrder);

    /// <summary>
    /// International Securities Exchange.
    /// </summary>
    public static readonly OrderSource ISE = new("ISE", PubOrder | PubSpreadOrder);

    /// <summary>
    /// Direct-Edge EDGA Exchange.
    /// </summary>
    public static readonly OrderSource DEA = new("DEA", PubOrder);

    /// <summary>
    /// Direct-Edge EDGX Exchange.
    /// </summary>
    public static readonly OrderSource DEX = new("DEX", PubOrder);

    /// <summary>
    /// Bats BYX Exchange.
    /// </summary>
    public static readonly OrderSource BYX = new("BYX", PubOrder);

    /// <summary>
    /// Bats BZX Exchange.
    /// </summary>
    public static readonly OrderSource BZX = new("BZX", PubOrder);

    /// <summary>
    /// Bats Europe BXE Exchange.
    /// </summary>
    public static readonly OrderSource BATE = new("BATE", PubOrder);

    /// <summary>
    /// Bats Europe CXE Exchange.
    /// </summary>
    public static readonly OrderSource CHIX = new("CHIX", PubOrder);

    /// <summary>
    /// Bats Europe DXE Exchange.
    /// </summary>
    public static readonly OrderSource CEUX = new("CEUX", PubOrder);

    /// <summary>
    /// Bats Europe TRF.
    /// </summary>
    public static readonly OrderSource BXTR = new("BXTR", PubOrder);

    /// <summary>
    /// Borsa Istanbul Exchange.
    /// </summary>
    public static readonly OrderSource IST = new("IST", PubOrder);

    /// <summary>
    /// Borsa Istanbul Exchange. Record for particular top 20 order book.
    /// </summary>
    public static readonly OrderSource BI20 = new("BI20", PubOrder);

    /// <summary>
    /// ABE (abe.io) exchange.
    /// </summary>
    public static readonly OrderSource ABE = new("ABE", PubOrder);

    /// <summary>
    /// FAIR (FairX) exchange.
    /// </summary>
    public static readonly OrderSource FAIR = new("FAIR", PubOrder);

    /// <summary>
    /// CME Globex.
    /// </summary>
    public static readonly OrderSource GLBX = new("GLBX", PubOrder | PubAnalyticOrder);

    /// <summary>
    /// CME Globex. Record for price level book.
    /// </summary>
    public static readonly OrderSource glbx = new("glbx", PubOrder);

    /// <summary>
    /// Eris Exchange group of companies.
    /// </summary>
    public static readonly OrderSource ERIS = new("ERIS", PubOrder);

    /// <summary>
    /// Eurex Exchange.
    /// </summary>
    public static readonly OrderSource XEUR = new("XEUR", PubOrder);

    /// <summary>
    /// Eurex Exchange. Record for price level book.
    /// </summary>
    public static readonly OrderSource xeur = new("xeur", PubOrder);

    /// <summary>
    /// CBOE Futures Exchange.
    /// </summary>
    public static readonly OrderSource CFE = new("CFE", PubOrder);

    /// <summary>
    /// CBOE Options C2 Exchange.
    /// </summary>
    public static readonly OrderSource C2OX = new("C2OX", PubOrder);

    /// <summary>
    /// Small Exchange.
    /// </summary>
    public static readonly OrderSource SMFE = new("SMFE", PubOrder);

    /// <summary>
    /// Small Exchange. Record for price level book.
    /// </summary>
    public static readonly OrderSource smfe = new("smfe", PubOrder);

    /// <summary>
    /// Investors exchange. Record for price level book.
    /// </summary>
    public static readonly OrderSource iex = new("iex", PubOrder);

    /// <summary>
    /// Members Exchange.
    /// </summary>
    public static readonly OrderSource MEMX = new("MEMX", PubOrder);

    /// <summary>
    /// Members Exchange. Record for price level book.
    /// </summary>
    public static readonly OrderSource memx = new("memx", PubOrder);

    /// <summary>
    /// Pink Sheets. Record for price level book.
    /// Pink sheets are listings for stocks that trade over-the-counter (OTC).
    /// </summary>
    public static readonly OrderSource pink = new("pink", PubOrder | PubOtcMarketdOrder);

    /// <summary>
    /// The binary flags representing <see cref="Order"/>.
    /// </summary>
    private const int PubOrder = 0x0001;

    /// <summary>
    /// The binary flags representing <see cref="AnalyticOrder"/>.
    /// </summary>
    private const int PubAnalyticOrder = 0x0002;

    /// <summary>
    /// The binary flags representing <see cref="OtcMarketsOrder"/>.
    /// </summary>
    private const int PubOtcMarketdOrder = 0x0004;

    /// <summary>
    /// The binary flags representing <see cref="SpreadOrder"/>.
    /// </summary>
    private const int PubSpreadOrder = 0x0008;

    /// <summary>
    /// The binary flags representing Full Order Book.
    /// </summary>
    private const int FullOrderBook = 0x0010;

    /// <summary>
    /// Set of binary flags for current <see cref="OrderSource"/>.
    /// </summary>
    private readonly int _pubFlags;

    /// <summary>
    /// Flag indicating that the order source is "builtin".
    /// And was not created by the user.
    /// </summary>
    private readonly bool _isBuiltin;

    private OrderSource(string name, int pubFlags)
        : this(ComposeId(name), name, pubFlags)
    {
    }

    private OrderSource(int id, string name, int pubFlags)
        : base(id, name)
    {
        _pubFlags = pubFlags;
        _isBuiltin = true;

        // Below are sanity and integrity checks for special and builtin pre-defined sources.
        // They also guard against uncoordinated changes of id/name with other methods.
        switch (id)
        {
            case < 0:
                throw new ArgumentException("Id is negative", nameof(id));
            case > 0 and < 0x20 when !IsSpecialSourceId(id):
                throw new ArgumentException("Id is not marked as special", nameof(id));
            case >= 0x20 when id != ComposeId(name) || !name.Equals(DecodeName(id), StringComparison.Ordinal):
                throw new ArgumentException("Id does not match name", nameof(id));
        }

        // Flag FullOrderBook requires that source must be publishable.
        if ((pubFlags & FullOrderBook) != 0 && (pubFlags & (PubOrder | PubAnalyticOrder | PubOtcMarketdOrder | PubSpreadOrder)) == 0)
        {
            throw new ArgumentException("Unpublishable full order book order", nameof(pubFlags));
        }

        if (!TryAddToCacheOrderSource(id, this))
        {
            throw new ArgumentException("Duplicate id", nameof(id));
        }

        if (!TryAddToCacheOrderSource(name, this))
        {
            throw new ArgumentException("Duplicate name", nameof(name));
        }
    }

    private OrderSource(int id, string name)
        : base(id, name)
    {
        _pubFlags = 0;
        _isBuiltin = false;
    }

    /// <summary>
    /// Gets a value indicating whether this source supports Full Order Book.
    /// </summary>
    public bool IsFullOrderBook =>
        (_pubFlags & FullOrderBook) != 0;

    /// <summary>
    /// Determines whether specified source identifier refers to special order source.
    /// Special order sources are used for wrapping non-order events into order events.
    /// </summary>
    /// <param name="sourceId">The source identifier.</param>
    /// <returns>Returns <c>true</c> if it is a special source identifier.</returns>
    public static bool IsSpecialSourceId(int sourceId) =>
        sourceId is >= 1 and <= 6;

    /// <summary>
    /// Returns order source for the specified source identifier.
    /// </summary>
    /// <param name="sourceId">The source identifier.</param>
    /// <returns>Return <see cref="OrderSource"/>.</returns>
    /// <exception cref="ArgumentException">If sourceId is negative or zero.</exception>
    public static OrderSource ValueOf(int sourceId) =>
        GetOrCreateAndCacheOrderSource(sourceId, () => new OrderSource(sourceId, DecodeName(sourceId)));

    /// <summary>
    /// Returns order source for the specified source name.
    /// The name must be either predefined, or contain at most 4 alphanumeric characters.
    /// </summary>
    /// <param name="name">The name of the source.</param>
    /// <returns>Return <see cref="OrderSource"/>.</returns>
    /// <exception cref="ArgumentException">If name is malformed..</exception>
    public static OrderSource ValueOf(string name) =>
        GetOrCreateAndCacheOrderSource(name, () => new OrderSource(ComposeId(name), name));

    /// <summary>
    /// Gets type mask by specified event type.
    /// </summary>
    /// <param name="eventType">The type of event.</param>
    /// <returns>Returns type mask.</returns>
    /// <exception cref="ArgumentException">
    /// If the eventType does not inherit OrderBase. <see cref="OrderBase"/>.
    /// </exception>
    public static int GetEventTypeMask(Type eventType)
    {
        if (eventType == typeof(Order))
        {
            return PubOrder;
        }

        if (eventType == typeof(AnalyticOrder))
        {
            return PubAnalyticOrder;
        }

        if (eventType == typeof(OtcMarketsOrder))
        {
            return PubOtcMarketdOrder;
        }
        if (eventType == typeof(SpreadOrder))
        {
            return PubSpreadOrder;
        }

        throw new ArgumentException($"Invalid order event type: {eventType}");
    }

    /// <summary>
    /// Gets a value indicating whether the given event type can be directly published with this source.
    /// Subscription on such sources can be observed directly via <see cref="DXPublisher"/>.
    /// Subscription on such sources is observed via instances of <see cref="IndexedEventSubscriptionSymbol{T}"/> class.
    /// </summary>
    /// <param name="eventType">
    /// Typeof <see cref="Order"/> or <see cref="AnalyticOrder"/> or <see cref="SpreadOrder"/>.
    /// </param>
    /// <returns>
    /// <c>true</c> if <see cref="Order"/> , <see cref="AnalyticOrder"/>  and <see cref="SpreadOrder"/>
    /// events can be directly published with this source.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// If eventType differs from <see cref="Order"/> , <see cref="AnalyticOrder"/>, <see cref="SpreadOrder"/>.
    /// </exception>
    public bool IsPublishable(Type eventType) =>
        (_pubFlags & GetEventTypeMask(eventType)) != 0;

    /// <summary>
    /// Checks that char is an alphanumeric character.
    /// </summary>
    /// <param name="c">The char.</param>
    /// <exception cref="ArgumentException">If char is not alphanumeric characters.</exception>
    private static void CheckChar(char c)
    {
        if (c is (>= 'A' and <= 'Z') or (>= 'a' and <= 'z') or (>= '0' and <= '9'))
        {
            return;
        }

        throw new ArgumentException("Source name must contain only alphanumeric characters", nameof(c));
    }

    private static int ComposeId(string name)
    {
        var sourceId = 0;
        var n = name.Length;
        if (n is 0 or > 4)
        {
            throw new ArgumentException("Source name must contain from 1 to 4 characters", nameof(name));
        }

        for (var i = 0; i < n; ++i)
        {
            var c = name[i];
            CheckChar(c);
            sourceId = (sourceId << 8) | c;
        }

        return sourceId;
    }

    private static string DecodeName(int id)
    {
        if (id == 0)
        {
            throw new ArgumentException("Source name must contain from 1 to 4 characters", nameof(id));
        }

        var name = new char[4];
        var n = 0;
        for (var i = 24; i >= 0; i -= 8)
        {
            if (id >> i == 0) // Skip highest contiguous zeros.
            {
                continue;
            }

            var c = (char)((id >> i) & 0xff);
            CheckChar(c);
            name[n++] = c;
        }

        return new string(name, 0, n);
    }

    /// <summary>
    /// Adds a new <see cref="OrderSource"/> instance to the cache by key.
    /// If the added instance is "builtin" source, the cache item priority sets
    /// to <see cref="CacheItemPriority.NeverRemove"/> the user order source
    /// is set to <see cref="CacheItemPriority.Low"/>.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="orderSource">The instance of <see cref="OrderSource"/>.</param>
    private static void AddToCacheOrderSource(object key, OrderSource orderSource)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSize(1)
            .SetPriority(orderSource._isBuiltin ? CacheItemPriority.NeverRemove : CacheItemPriority.Low);
        CacheSource.Set(key, orderSource, cacheEntryOptions);
    }

    /// <summary>
    /// Tries to add new instance of <see cref="OrderSource"/> to cache by key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="orderSource">The instance of <see cref="OrderSource"/>.</param>
    /// <returns>
    /// <c>true</c> if the key/value pair was added to the cache successfully; <c>false</c> if the key already exists.
    /// </returns>
    private static bool TryAddToCacheOrderSource(object key, OrderSource orderSource)
    {
        if (!CacheSource.TryGetValue(key, out _))
        {
            AddToCacheOrderSource(key, orderSource);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Gets an <see cref="OrderSource"/> by key from cache
    /// or creates a new instance using a value factory and places it in the cache if key not exist.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="valueFactory">The fabric method for create new instance <see cref="OrderSource"/>.</param>
    /// <returns>Returns <see cref="OrderSource"/>.</returns>
    private static OrderSource GetOrCreateAndCacheOrderSource(object key, Func<OrderSource> valueFactory)
    {
        if (!CacheSource.TryGetValue(key, out OrderSource orderSource))
        {
            orderSource = valueFactory();
            AddToCacheOrderSource(key, orderSource);
        }

        return orderSource!;
    }
}
