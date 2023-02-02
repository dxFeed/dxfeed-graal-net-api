// <copyright file="CandleMapper.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Events.Candles;
using DxFeed.Graal.Net.Native.Symbols.Candle;

namespace DxFeed.Graal.Net.Native.Events.Candles;

internal class CandleMapper : IEventMapper
{
    public unsafe IEventType FromNative(EventTypeNative* eventType) =>
        ToEventType((CandleNative*)eventType);

    public unsafe EventTypeNative* ToNative(IEventType eventType) => throw new System.NotImplementedException();

    public unsafe void Release(EventTypeNative* eventType) => throw new System.NotImplementedException();

    private static unsafe Candle ToEventType(CandleNative* native) =>
        new()
        {
            EventSymbol = ((CandleSymbolNative*)native->CandleSymbol)->Symbol.ToString(),
            EventFlags = native->EventFlags,
            EventTime = native->EventTime,
            Index = native->Index,
            Count = native->Count,
            Open = native->Open,
            High = native->High,
            Low = native->Low,
            Close = native->Close,
            Volume = native->Volume,
            VWAP = native->VWAP,
            BidVolume = native->BidVolume,
            AskVolume = native->AskVolume,
            ImpVolatility = native->ImpVolatility,
            OpenInterest = native->OpenInterest,
        };
}
