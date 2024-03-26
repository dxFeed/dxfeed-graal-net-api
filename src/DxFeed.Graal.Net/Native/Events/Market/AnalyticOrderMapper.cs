// <copyright file="AnalyticOrderMapper.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Events.Market;

namespace DxFeed.Graal.Net.Native.Events.Market;

internal sealed class AnalyticOrderMapper : OrderBaseMapper<AnalyticOrder, AnalyticOrderNative>
{
    private static readonly OrderMapper OrderMapper = new();

    public override unsafe IEventType FromNative(EventTypeNative* eventType) =>
        Convert((AnalyticOrderNative*)eventType);

    public override unsafe EventTypeNative* ToNative(IEventType eventType) =>
        (EventTypeNative*)Convert((AnalyticOrder)eventType);

    public override unsafe void Release(EventTypeNative* eventType) =>
        OrderMapper.Release(eventType);

    protected override unsafe AnalyticOrder Convert(AnalyticOrderNative* eventType)
    {
        var analyticOrder = CreateOrderBase((OrderBaseNative*)eventType);
        analyticOrder.MarketMaker = eventType->Order.MarketMaker;
        analyticOrder.IcebergPeakSize = eventType->IcebergPeakSize;
        analyticOrder.IcebergHiddenSize = eventType->IcebergHiddenSize;
        analyticOrder.IcebergExecutedSize = eventType->IcebergExecutedSize;
        analyticOrder.IcebergFlags = eventType->IcebergFlags;
        return analyticOrder;
    }

    protected override unsafe AnalyticOrderNative* Convert(AnalyticOrder eventType)
    {
        var ptr = AllocEventType();
        *ptr = new AnalyticOrderNative
        {
            Order = new() { OrderBase = OrderMapper.CreateOrderBase(eventType), MarketMaker = eventType.MarketMaker, },
            IcebergPeakSize = eventType.IcebergPeakSize,
            IcebergHiddenSize = eventType.IcebergHiddenSize,
            IcebergExecutedSize = eventType.IcebergExecutedSize,
            IcebergFlags = eventType.IcebergFlags,
        };
        return ptr;
    }
}
