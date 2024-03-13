// <copyright file="AnalyticOrderMapper.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Events.Market;

namespace DxFeed.Graal.Net.Native.Events.Market;

internal class OtcMarketsOrderMapper : OrderBaseMapper<OtcMarketsOrder, OtcMarketsOrderNative>
{
    private static readonly OrderMapper OrderMapper = new();

    public override unsafe IEventType FromNative(EventTypeNative* eventType) =>
        Convert((OtcMarketsOrderNative*)eventType);

    public override unsafe EventTypeNative* ToNative(IEventType eventType) =>
        (EventTypeNative*)Convert((OtcMarketsOrder)eventType);

    public override unsafe void Release(EventTypeNative* eventType) =>
        OrderMapper.Release(eventType);

    protected override unsafe OtcMarketsOrder Convert(OtcMarketsOrderNative* eventType)
    {
        var order = CreateOrderBase((OrderBaseNative*)eventType);
        order.MarketMaker = eventType->Order.MarketMaker;
        order.OtcMarketsFlags = eventType->OtcMarketsFlags;
        order.QuoteAccessPayment = eventType->QuoteAccessPayment;
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
}
