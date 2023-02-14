// <copyright file="EventMapper.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Native.Events.Candles;
using DxFeed.Graal.Net.Native.Events.Market;
using DxFeed.Graal.Net.Native.Events.Options;
using DxFeed.Graal.Net.Native.Interop;

namespace DxFeed.Graal.Net.Native.Events;

/// <summary>
/// A collection of static methods for mapping unmanaged native events (<see cref="EventTypeNative"/>)
/// to an <see cref="IEventType"/>.
/// </summary>
internal static class EventMapper
{
    private static readonly QuoteMapper QuoteMapper = new();
    private static readonly ProfileMapper ProfileMapper = new();
    private static readonly SummaryMapper SummaryMapper = new();
    private static readonly GreeksMapper GreeksMapper = new();
    private static readonly CandleMapper CandleMapper = new();
    private static readonly UnderlyingMapper UnderlyingMapper = new();
    private static readonly TheoPriceMapper TheoPriceMapper = new();
    private static readonly TradeMapper TradeMapper = new();
    private static readonly TradeETHMapper TradeETHMapper = new();
    private static readonly TimeAndSaleMapper TimeAndSaleMapper = new();
    private static readonly OrderMapper OrderMapper = new();
    private static readonly AnalyticOrderMapper AnalyticOrderMapper = new();
    private static readonly SpreadOrderMapper SpreadOrderMapper = new();
    private static readonly SeriesMapper SeriesMapper = new();
    private static readonly OptionSaleMapper OptionSaleMapper = new();

    /// <summary>
    /// Creates a specific collection of <see cref="IEventType"/> from a <see cref="ListNative{T}"/> unsafe pointer.
    /// </summary>
    /// <param name="list">The unsafe pointer to <see cref="ListNative{T}"/>.</param>
    /// <returns>The collection of <see cref="IEventType"/>.</returns>
    /// <exception cref="NotImplementedException">
    /// If specific <see cref="EventTypeNative.EventCode"/> inside <see cref="ListNative{T}"/> not implement.
    /// </exception>
    public static unsafe ICollection<IEventType> FromNative(ListNative<EventTypeNative>* list) =>
        FromNative(list->Elements, list->Size);

    /// <summary>
    /// Creates a specific collection of <see cref="IEventType"/> from a <see cref="EventTypeNative"/> unsafe pointers.
    /// </summary>
    /// <param name="eventTypeNative">The unsafe pointer-to-pointer to <see cref="EventTypeNative"/>.</param>
    /// <param name="size">The count of event.</param>
    /// <returns>The collection of <see cref="IEventType"/>.</returns>
    /// <exception cref="NotImplementedException">
    /// If specific <see cref="EventTypeNative.EventCode"/> inside <see cref="EventTypeNative"/> not implement.
    /// </exception>
    public static unsafe ICollection<IEventType> FromNative(EventTypeNative** eventTypeNative, int size)
    {
        var eventList = new List<IEventType>(size);
        for (var i = 0; i < size; ++i)
        {
            eventList.Add(FromNative(eventTypeNative[i]));
        }

        return eventList;
    }

    /// <summary>
    /// Creates a specific <see cref="IEventType"/> from a <see cref="EventTypeNative"/> unsafe pointer.
    /// The specific type of <see cref="IEventType"/> define by <see cref="EventTypeNative.EventCode"/>.
    /// </summary>
    /// <param name="eventTypeNative">The unsafe pointer to <see cref="EventTypeNative"/>.</param>
    /// <returns>The created <see cref="IEventType"/>.</returns>
    /// <exception cref="NotImplementedException">
    /// If specific <see cref="EventTypeNative.EventCode"/> not implement.
    /// </exception>
    public static unsafe IEventType FromNative(EventTypeNative* eventTypeNative) =>
        GetEventMapperByEventCode(eventTypeNative->EventCode).FromNative(eventTypeNative);

    public static unsafe EventTypeNative* ToNative(IEventType eventType) =>
        GetEventMapperByEventCode(eventType).ToNative(eventType);

    public static unsafe ListNative<EventTypeNative>* ToNative(IEnumerable<IEventType> eventType) =>
        ListNative<EventTypeNative>.Create(eventType, e => (nint)ToNative(e));

    public static unsafe void Release(EventTypeNative* eventType) =>
        GetEventMapperByEventCode(eventType->EventCode).Release(eventType);

    public static unsafe void Release(ListNative<EventTypeNative>* eventType) =>
        ListNative<EventTypeNative>.Release(eventType, e => Release((EventTypeNative*)e));

    public static IEventMapper GetEventMapperByEventCode(IEventType eventType) =>
        GetEventMapperByEventCode(EventCodeAttribute.GetEventCode(eventType.GetType()));

    public static IEventMapper GetEventMapperByEventCode(EventCodeNative eventCode) =>
        eventCode switch
        {
            EventCodeNative.Quote => QuoteMapper,
            EventCodeNative.Profile => ProfileMapper,
            EventCodeNative.Summary => SummaryMapper,
            EventCodeNative.Greeks => GreeksMapper,
            EventCodeNative.Candle => CandleMapper,
            EventCodeNative.Underlying => UnderlyingMapper,
            EventCodeNative.TheoPrice => TheoPriceMapper,
            EventCodeNative.Trade => TradeMapper,
            EventCodeNative.TradeETH => TradeETHMapper,
            EventCodeNative.TimeAndSale => TimeAndSaleMapper,
            EventCodeNative.Order => OrderMapper,
            EventCodeNative.AnalyticOrder => AnalyticOrderMapper,
            EventCodeNative.SpreadOrder => SpreadOrderMapper,
            EventCodeNative.Series => SeriesMapper,
            EventCodeNative.OptionSale => OptionSaleMapper,
            EventCodeNative.DailyCandle => throw new NotImplementedException(),
            EventCodeNative.OrderBase => throw new NotImplementedException(),
            EventCodeNative.Configuration => throw new NotImplementedException(),
            EventCodeNative.Message => throw new NotImplementedException(),
            _ => throw new ArgumentException($"Unknown {nameof(EventCodeNative)}: {eventCode}"),
        };
}
