// <copyright file="TradeMapper.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Events.Market;

namespace DxFeed.Graal.Net.Native.Events.Market;

internal sealed class TradeMapper : TradeBaseMapper<Trade, TradeNative>
{
    public override unsafe IEventType FromNative(EventTypeNative* eventType) =>
        Convert((TradeNative*)eventType);

    public override unsafe EventTypeNative* ToNative(IEventType eventType) =>
        (EventTypeNative*)Convert((Trade)eventType);

    public override unsafe IEventType FillFromNative(EventTypeNative* nativeEventType, IEventType eventType) =>
        Fill((TradeNative*)nativeEventType, (Trade)eventType);

    public override unsafe void Release(EventTypeNative* eventType) =>
        ReleaseTradeBase(eventType);

    protected override unsafe Trade Convert(TradeNative* eventType)
    {
        var trade = new Trade();
        Fill(eventType, trade);
        return trade;
    }

    protected override unsafe TradeNative* Convert(Trade eventType)
    {
        var ptr = AllocEventType();
        *ptr = new() { TradeBase = CreateTradeBase(eventType) };
        return ptr;
    }

    private static unsafe Trade Fill(TradeNative* eventType, Trade trade)
    {
        AssignTradeBase((TradeBaseNative*)eventType, trade);
        return trade;
    }
}
