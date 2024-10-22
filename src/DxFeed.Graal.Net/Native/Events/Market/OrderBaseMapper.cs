// <copyright file="OrderBaseMapper.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Events.Market;

namespace DxFeed.Graal.Net.Native.Events.Market;

internal abstract class OrderBaseMapper<TOrderBase, TOrderBaseNative> : EventTypeMapper<TOrderBase, TOrderBaseNative>
    where TOrderBase : OrderBase, new()
    where TOrderBaseNative : unmanaged
{
    protected static unsafe TOrderBase AssignOrderBase(OrderBaseNative* eventType, TOrderBase orderBase)
    {
        orderBase.EventSymbol = eventType->EventType.EventSymbol;
        orderBase.EventTime = eventType->EventType.EventTime;
        orderBase.EventFlags = eventType->EventFlags;
        orderBase.Index = eventType->Index;
        orderBase.TimeSequence = eventType->TimeSequence;
        orderBase.TimeNanoPart = eventType->TimeNanoPart;
        orderBase.ActionTime = eventType->ActionTime;
        orderBase.OrderId = eventType->OrderId;
        orderBase.AuxOrderId = eventType->AuxOrderId;
        orderBase.Price = eventType->Price;
        orderBase.Size = eventType->Size;
        orderBase.ExecutedSize = eventType->ExecutedSize;
        orderBase.Count = eventType->Count;
        orderBase.Flags = eventType->Flags;
        orderBase.TradeId = eventType->TradeId;
        orderBase.TradePrice = eventType->TradePrice;
        orderBase.TradeSize = eventType->TradeSize;
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
