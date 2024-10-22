// <copyright file="OtcMarketsOrderMapper.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Events.Market;

namespace DxFeed.Graal.Net.Native.Events.Market;

internal sealed class OtcMarketsOrderMapper : OrderBaseMapper<OtcMarketsOrder, OtcMarketsOrderNative>
{
    private static readonly OrderMapper OrderMapper = new();

    public override unsafe IEventType FromNative(EventTypeNative* eventType) =>
        Convert((OtcMarketsOrderNative*)eventType);

    public override unsafe EventTypeNative* ToNative(IEventType eventType) =>
        (EventTypeNative*)Convert((OtcMarketsOrder)eventType);

    public override unsafe IEventType FillFromNative(EventTypeNative* nativeEventType, IEventType eventType) =>
        Fill((OtcMarketsOrderNative*)nativeEventType, (OtcMarketsOrder)eventType);

    public override unsafe void Release(EventTypeNative* eventType) =>
        OrderMapper.Release(eventType);

    protected override unsafe OtcMarketsOrder Convert(OtcMarketsOrderNative* eventType)
    {
        var order = new OtcMarketsOrder();
        Fill(eventType, order);
        return order;
    }

    protected override unsafe OtcMarketsOrderNative* Convert(OtcMarketsOrder eventType)
    {
        var ptr = AllocEventType();
        *ptr = new OtcMarketsOrderNative
        {
            Order = new() { OrderBase = OrderMapper.CreateOrderBase(eventType), MarketMaker = eventType.MarketMaker, },
            QuoteAccessPayment = eventType.QuoteAccessPayment,
            OtcMarketsFlags = eventType.OtcMarketsFlags,
        };
        return ptr;
    }

    private static unsafe OtcMarketsOrder Fill(OtcMarketsOrderNative* eventType, OtcMarketsOrder order)
    {
        AssignOrderBase((OrderBaseNative*)eventType, order);
        order.MarketMaker = eventType->Order.MarketMaker;
        order.OtcMarketsFlags = eventType->OtcMarketsFlags;
        order.QuoteAccessPayment = eventType->QuoteAccessPayment;
        return order;
    }
}
