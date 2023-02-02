// <copyright file="TradeBaseMapper.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Events.Market;

namespace DxFeed.Graal.Net.Native.Events.Market;

internal abstract class TradeBaseMapper<TTradeBase, TTradeBaseNative> : EventTypeMapper<TTradeBase, TTradeBaseNative>
    where TTradeBase : TradeBase, new()
    where TTradeBaseNative : unmanaged
{
    protected static unsafe TTradeBase CreateTradeBase(TradeBaseNative* eventType)
    {
        var tradeBase = CreateEventType((EventTypeNative*)eventType);
        tradeBase.TimeSequence = eventType->TimeSequence;
        tradeBase.TimeNanoPart = eventType->TimeNanoPart;
        tradeBase.ExchangeCode = eventType->ExchangeCode;
        tradeBase.Price = eventType->Price;
        tradeBase.Change = eventType->Change;
        tradeBase.Size = eventType->Size;
        tradeBase.DayId = eventType->DayId;
        tradeBase.DayVolume = eventType->DayVolume;
        tradeBase.DayTurnover = eventType->DayTurnover;
        tradeBase.Flags = eventType->Flags;
        return tradeBase;
    }

    protected static TradeBaseNative CreateTradeBase(TTradeBase eventType) =>
        new()
        {
            EventType = CreateEventType(eventType),
            TimeSequence = eventType.TimeSequence,
            TimeNanoPart = eventType.TimeNanoPart,
            ExchangeCode = eventType.ExchangeCode,
            Price = eventType.Price,
            Change = eventType.Change,
            Size = eventType.Size,
            DayId = eventType.DayId,
            DayVolume = eventType.DayVolume,
            DayTurnover = eventType.DayTurnover,
            Flags = eventType.Flags,
        };

    protected unsafe void ReleaseTradeBase(EventTypeNative* native) =>
        ReleaseEventType(native);
}
