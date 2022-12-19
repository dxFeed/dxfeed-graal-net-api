// <copyright file="OrderMapper.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Events.Market;
using DxFeed.Graal.Net.Native.Events.Market;

namespace DxFeed.Graal.Net.Native.EventMappers;

internal static class OrderMapper
{
    public static unsafe Order FromNative(OrderNative* orderNative)
    {
        var order = FillOrderBase(new Order(), &orderNative->OrderBase);
        order.MarketMaker = Marshal.PtrToStringUTF8(orderNative->MarketMaker);
        return order;
    }

    public static unsafe SpreadOrder FromNative(SpreadOrderNative* spreadOrderNative)
    {
        var spreadOrder = FillOrderBase(new SpreadOrder(), &spreadOrderNative->OrderBase);
        spreadOrder.SpreadSymbol = Marshal.PtrToStringUTF8(spreadOrderNative->SpreadSymbol);
        return spreadOrder;
    }

    public static unsafe AnalyticOrder FromNative(AnalyticOrderNative* analyticOrderNative)
    {
        var analyticOrder = FillOrderBase(new AnalyticOrder(), &analyticOrderNative->OrderBase);
        analyticOrder.IcebergPeakSize = analyticOrderNative->IcebergPeakSize;
        analyticOrder.IcebergHiddenSize = analyticOrderNative->IcebergHiddenSize;
        analyticOrder.IcebergExecutedSize = analyticOrderNative->IcebergExecutedSize;
        analyticOrder.IcebergFlags = analyticOrderNative->IcebergFlags;
        return analyticOrder;
    }

    private static unsafe T FillOrderBase<T>(T orderBase, OrderBaseNative* orderBaseNative)
        where T : OrderBase
    {
        orderBase.EventSymbol = Marshal.PtrToStringUTF8(orderBaseNative->MarketEvent.EventSymbol);
        orderBase.EventTime = orderBaseNative->MarketEvent.EventTime;
        orderBase.EventFlags = orderBaseNative->EventFlags;
        orderBase.Index = orderBaseNative->Index;
        orderBase.TimeSequence = orderBaseNative->TimeSequence;
        orderBase.TimeNanoPart = orderBaseNative->TimeNanoPart;
        orderBase.ActionTime = orderBaseNative->ActionTime;
        orderBase.OrderId = orderBaseNative->OrderOd;
        orderBase.AuxOrderId = orderBaseNative->AuxOrderId;
        orderBase.Price = orderBaseNative->Price;
        orderBase.Size = orderBaseNative->Size;
        orderBase.ExecutedSize = orderBaseNative->ExecutedSize;
        orderBase.Count = orderBaseNative->Count;
        orderBase.Flags = orderBaseNative->Flags;
        orderBase.TradeId = orderBaseNative->TradeId;
        orderBase.TradePrice = orderBaseNative->TradePrice;
        orderBase.TradeSize = orderBaseNative->TradeSize;
        return orderBase;
    }
}
