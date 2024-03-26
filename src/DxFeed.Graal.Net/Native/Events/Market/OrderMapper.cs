// <copyright file="OrderMapper.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Events.Market;

namespace DxFeed.Graal.Net.Native.Events.Market;

internal sealed class OrderMapper : OrderBaseMapper<Order, OrderNative>
{
    public override unsafe IEventType FromNative(EventTypeNative* eventType) =>
        Convert((OrderNative*)eventType);

    public override unsafe EventTypeNative* ToNative(IEventType eventType) =>
        (EventTypeNative*)Convert((Order)eventType);

    public override unsafe void Release(EventTypeNative* eventType)
    {
        if (eventType == (EventTypeNative*)0)
        {
            return;
        }

        var orderNative = (OrderNative*)eventType;
        orderNative->MarketMaker.Release();
        ReleaseOrderBase(eventType);
    }

    protected override unsafe Order Convert(OrderNative* eventType)
    {
        var order = CreateOrderBase((OrderBaseNative*)eventType);
        order.MarketMaker = eventType->MarketMaker;
        return order;
    }

    protected override unsafe OrderNative* Convert(Order eventType)
    {
        var ptr = AllocEventType();
        *ptr = new() { OrderBase = CreateOrderBase(eventType), MarketMaker = eventType.MarketMaker, };
        return ptr;
    }
}
