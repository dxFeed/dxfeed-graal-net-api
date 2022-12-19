// <copyright file="EventMapper.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Native.Events;
using DxFeed.Graal.Net.Native.Events.Market;

namespace DxFeed.Graal.Net.Native.EventMappers;

/// <summary>
/// A collection of static methods for mapping unmanaged native events to an <see cref="IEventType"/>.
/// </summary>
internal static class EventMapper
{
    /// <summary>
    /// Creates a specific collection of <see cref="IEventType"/> from a <see cref="ListNative{T}"/> unsafe pointers.
    /// </summary>
    /// <param name="list">The unsafe pointer to <see cref="ListNative{T}"/>.</param>
    /// <returns>The collection of <see cref="IEventType"/>.</returns>
    /// <exception cref="NotImplementedException">
    /// If specific <see cref="BaseEventNative.EventCode"/> not implement.
    /// </exception>
    public static unsafe ICollection<IEventType> FromNative(ListNative<BaseEventNative>* list) =>
        FromNative(list->Elements, list->Size);

    /// <summary>
    /// Creates a specific collection of <see cref="IEventType"/> from a <see cref="BaseEventNative"/> unsafe pointers.
    /// </summary>
    /// <param name="baseEvent">The unsafe pointer to base event.</param>
    /// <param name="size">The count of event.</param>
    /// <returns>The collection of <see cref="IEventType"/>.</returns>
    /// <exception cref="NotImplementedException">
    /// If specific <see cref="BaseEventNative.EventCode"/> not implement.
    /// </exception>
    public static unsafe ICollection<IEventType> FromNative(BaseEventNative** baseEvent, int size)
    {
        var eventList = new List<IEventType>(size);
        for (var i = 0; i < size; ++i)
        {
            eventList.Add(FromNative(baseEvent[i]));
        }

        return eventList;
    }

    /// <summary>
    /// Creates a specific <see cref="IEventType"/> from a <see cref="BaseEventNative"/> unsafe pointer.
    /// The specific type of <see cref="IEventType"/> define by <see cref="BaseEventNative.EventCode"/>.
    /// </summary>
    /// <param name="baseEvent">The unsafe pointer to base event.</param>
    /// <returns>The created <see cref="IEventType"/>.</returns>
    /// <exception cref="NotImplementedException">
    /// If specific <see cref="BaseEventNative.EventCode"/> not implement.
    /// </exception>
    public static unsafe IEventType FromNative(BaseEventNative* baseEvent) =>
        baseEvent->EventCode switch
        {
            EventCodeNative.Quote => QuoteMapper.FromNative((QuoteNative*)baseEvent),
            EventCodeNative.TimeAndSale => TimeAndSaleMapper.FromNative((TimeAndSaleNative*)baseEvent),
            EventCodeNative.Trade => TradeMapper.FromNative((TradeNative*)baseEvent),
            EventCodeNative.TradeETH => TradeMapper.FromNative((TradeETHNative*)baseEvent),
            EventCodeNative.Profile => throw new NotImplementedException(),
            EventCodeNative.Summary => throw new NotImplementedException(),
            EventCodeNative.Greeks => throw new NotImplementedException(),
            EventCodeNative.Candle => throw new NotImplementedException(),
            EventCodeNative.DailyCandle => throw new NotImplementedException(),
            EventCodeNative.Underlying => throw new NotImplementedException(),
            EventCodeNative.TheoPrice => throw new NotImplementedException(),
            EventCodeNative.Configuration => throw new NotImplementedException(),
            EventCodeNative.Message => throw new NotImplementedException(),
            EventCodeNative.OrderBase => throw new NotImplementedException(),
            EventCodeNative.Order => OrderMapper.FromNative((OrderNative*)baseEvent),
            EventCodeNative.AnalyticOrder => OrderMapper.FromNative((AnalyticOrderNative*)baseEvent),
            EventCodeNative.SpreadOrder => OrderMapper.FromNative((SpreadOrderNative*)baseEvent),
            EventCodeNative.Series => throw new NotImplementedException(),
            _ => throw new ArgumentException($"Unknown {nameof(EventCodeNative)}:{baseEvent->EventCode}"),
        };
}
