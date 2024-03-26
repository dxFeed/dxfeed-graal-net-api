// <copyright file="CandleMapper.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Events.Candles;

namespace DxFeed.Graal.Net.Native.Events.Candles;

internal sealed class CandleMapper : EventTypeMapper<Candle, CandleNative>
{
    public override unsafe IEventType FromNative(EventTypeNative* eventType) =>
        Convert((CandleNative*)eventType);

    public override unsafe EventTypeNative* ToNative(IEventType eventType) =>
        (EventTypeNative*)Convert((Candle)eventType);

    public override unsafe void Release(EventTypeNative* eventType) =>
        ReleaseEventType(eventType);

    protected override unsafe Candle Convert(CandleNative* eventType)
    {
        var candle = CreateEventType(eventType);
        candle.EventFlags = eventType->EventFlags;
        candle.Index = eventType->Index;
        candle.Count = eventType->Count;
        candle.Open = eventType->Open;
        candle.High = eventType->High;
        candle.Low = eventType->Low;
        candle.Close = eventType->Close;
        candle.Volume = eventType->Volume;
        candle.VWAP = eventType->VWAP;
        candle.BidVolume = eventType->BidVolume;
        candle.AskVolume = eventType->AskVolume;
        candle.ImpVolatility = eventType->ImpVolatility;
        candle.OpenInterest = eventType->OpenInterest;
        return candle;
    }

    protected override unsafe CandleNative* Convert(Candle eventType)
    {
        var ptr = AllocEventType();
        *ptr = new()
        {
            EventType = CreateEventType(eventType),
            EventFlags = eventType.EventFlags,
            Index = eventType.Index,
            Count = eventType.Count,
            Open = eventType.Open,
            High = eventType.High,
            Low = eventType.Low,
            Close = eventType.Close,
            Volume = eventType.Volume,
            VWAP = eventType.VWAP,
            BidVolume = eventType.BidVolume,
            AskVolume = eventType.AskVolume,
            ImpVolatility = eventType.ImpVolatility,
            OpenInterest = eventType.OpenInterest,
        };
        return ptr;
    }
}
