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
        eventTypeNative->EventCode switch
        {
            EventCodeNative.Quote => ((QuoteNative*)eventTypeNative)->ToEventType(),
            EventCodeNative.Profile => ((ProfileNative*)eventTypeNative)->ToEventType(),
            EventCodeNative.Summary => ((SummaryNative*)eventTypeNative)->ToEventType(),
            EventCodeNative.Greeks => ((GreeksNative*)eventTypeNative)->ToEventType(),
            EventCodeNative.Candle => ((CandleNative*)eventTypeNative)->ToEvent(),
            EventCodeNative.Underlying => ((UnderlyingNative*)eventTypeNative)->ToEventType(),
            EventCodeNative.TheoPrice => ((TheoPriceNative*)eventTypeNative)->ToEventType(),
            EventCodeNative.Trade => ((TradeNative*)eventTypeNative)->ToEventType(),
            EventCodeNative.TradeETH => ((TradeETHNative*)eventTypeNative)->ToEventType(),
            EventCodeNative.TimeAndSale => ((TimeAndSaleNative*)eventTypeNative)->ToEventType(),
            EventCodeNative.Order => ((OrderNative*)eventTypeNative)->ToEventType(),
            EventCodeNative.AnalyticOrder => ((AnalyticOrderNative*)eventTypeNative)->ToEventType(),
            EventCodeNative.SpreadOrder => ((SpreadOrderNative*)eventTypeNative)->ToEventType(),
            EventCodeNative.Series => ((SeriesNative*)eventTypeNative)->ToEventType(),
            EventCodeNative.DailyCandle => throw new NotImplementedException(),
            EventCodeNative.OrderBase => throw new NotImplementedException(),
            EventCodeNative.Configuration => throw new NotImplementedException(),
            EventCodeNative.Message => throw new NotImplementedException(),
            _ => throw new ArgumentException($"Unknown {nameof(EventCodeNative)}: {eventTypeNative->EventCode}"),
        };
}
