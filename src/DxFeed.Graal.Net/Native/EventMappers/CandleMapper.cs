// <copyright file="CandleMapper.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Events.Candle;
using DxFeed.Graal.Net.Native.Events.Candle;

namespace DxFeed.Graal.Net.Native.EventMappers;

internal static class CandleMapper
{
    public static unsafe Candle FromNative(CandleNative* eventNative) =>
        new()
        {
            CandleSymbol = CandleSymbol.ValueOf(Marshal.PtrToStringUTF8(eventNative->CandleSymbol->Symbol)),
            EventFlags = eventNative->EventFlags,
            EventTime = eventNative->EventTime,
            Index = eventNative->Index,
            Count = eventNative->Count,
            Open = eventNative->Open,
            High = eventNative->High,
            Low = eventNative->Low,
            Close = eventNative->Close,
            Volume = eventNative->Volume,
            VWAP = eventNative->VWAP,
            BidVolume = eventNative->BidVolume,
            AskVolume = eventNative->AskVolume,
            ImpVolatility = eventNative->ImpVolatility,
            OpenInterest = eventNative->Volume,
        };
}
