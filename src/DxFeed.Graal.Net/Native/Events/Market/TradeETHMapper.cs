// <copyright file="TradeETHMapper.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Events.Market;

// Disable pascal case naming rules.
#pragma warning disable S101

namespace DxFeed.Graal.Net.Native.Events.Market;

internal class TradeETHMapper : TradeBaseMapper<TradeETH, TradeETHNative>
{
    public override unsafe IEventType FromNative(EventTypeNative* eventType) =>
        Convert((TradeETHNative*)eventType);

    public override unsafe EventTypeNative* ToNative(IEventType eventType) =>
        (EventTypeNative*)Convert((TradeETH)eventType);

    public override unsafe void Release(EventTypeNative* eventType) =>
        ReleaseTradeBase(eventType);

    protected override unsafe TradeETH Convert(TradeETHNative* eventType) =>
        CreateTradeBase((TradeBaseNative*)eventType);

    protected override unsafe TradeETHNative* Convert(TradeETH eventType)
    {
        var ptr = AllocEventType();
        *ptr = new() { TradeBase = CreateTradeBase(eventType) };
        return ptr;
    }
}
