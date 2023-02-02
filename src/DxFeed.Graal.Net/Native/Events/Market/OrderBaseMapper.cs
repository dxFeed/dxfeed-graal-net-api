// <copyright file="OrderBaseMapper.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Events.Market;

namespace DxFeed.Graal.Net.Native.Events.Market;

internal abstract class OrderBaseMapper<TOrderBase, TOrderBaseNative> : EventTypeMapper<TOrderBase, TOrderBaseNative>
    where TOrderBase : OrderBase, new()
    where TOrderBaseNative : unmanaged
{
    protected static unsafe TOrderBase CreateOrderBase(OrderBaseNative* native)
    {
        var orderBase = CreateEventType((EventTypeNative*)native);
        orderBase.EventFlags = native->EventFlags;
        orderBase.Index = native->Index;
        orderBase.TimeSequence = native->TimeSequence;
        orderBase.TimeNanoPart = native->TimeNanoPart;
        orderBase.ActionTime = native->ActionTime;
        orderBase.OrderId = native->OrderId;
        orderBase.AuxOrderId = native->AuxOrderId;
        orderBase.Price = native->Price;
        orderBase.Size = native->Size;
        orderBase.ExecutedSize = native->ExecutedSize;
        orderBase.Count = native->Count;
        orderBase.Flags = native->Flags;
        orderBase.TradeId = native->TradeId;
        orderBase.TradePrice = native->TradePrice;
        orderBase.TradeSize = native->TradeSize;
        return orderBase;
    }

    protected static OrderBaseNative CreateOrderBase(TOrderBase eventType) =>
        new()
        {
            EventType = CreateEventType(eventType),
            EventFlags = eventType.EventFlags,
            Index = eventType.Index,
            TimeSequence = eventType.TimeSequence,
            TimeNanoPart = eventType.TimeNanoPart,
            ActionTime = eventType.ActionTime,
            OrderId = eventType.OrderId,
            AuxOrderId = eventType.AuxOrderId,
            Price = eventType.Price,
            Size = eventType.Size,
            ExecutedSize = eventType.ExecutedSize,
            Count = eventType.Count,
            Flags = eventType.Flags,
            TradeId = eventType.TradeId,
            TradePrice = eventType.TradePrice,
            TradeSize = eventType.TradeSize,
        };

    protected unsafe void ReleaseOrderBase(EventTypeNative* native) =>
        ReleaseEventType(native);
}
