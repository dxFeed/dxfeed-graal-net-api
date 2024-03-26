// <copyright file="SpreadOrderMapper.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Events.Market;

namespace DxFeed.Graal.Net.Native.Events.Market;

internal sealed class SpreadOrderMapper : OrderBaseMapper<SpreadOrder, SpreadOrderNative>
{
    public override unsafe IEventType FromNative(EventTypeNative* eventType) =>
        Convert((SpreadOrderNative*)eventType);

    public override unsafe EventTypeNative* ToNative(IEventType eventType) =>
        (EventTypeNative*)Convert((SpreadOrder)eventType);

    public override unsafe void Release(EventTypeNative* eventType)
    {
        if (eventType == (EventTypeNative*)0)
        {
            return;
        }

        var spreadOrderNative = (SpreadOrderNative*)eventType;
        spreadOrderNative->SpreadSymbol.Release();
        ReleaseOrderBase(eventType);
    }

    protected override unsafe SpreadOrder Convert(SpreadOrderNative* eventType)
    {
        var spreadOrder = CreateOrderBase((OrderBaseNative*)eventType);
        spreadOrder.SpreadSymbol = eventType->SpreadSymbol;
        return spreadOrder;
    }

    protected override unsafe SpreadOrderNative* Convert(SpreadOrder eventType)
    {
        var ptr = AllocEventType();
        *ptr = new() { OrderBase = CreateOrderBase(eventType), SpreadSymbol = eventType.SpreadSymbol, };
        return ptr;
    }
}
