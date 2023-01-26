// <copyright file="CandleNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Events.Candles;
using DxFeed.Graal.Net.Native.Symbols.Candle;

namespace DxFeed.Graal.Net.Native.Events.Candles;

/// <summary>
/// The structure contains all the fields required
/// to build an <see cref="Candle"/>.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly record struct CandleNative(
    EventTypeNative Base,
    nint EventSymbol,
    int EventFlags,
    long EventTime,
    long Index,
    long Count,
    double Open,
    double High,
    double Low,
    double Close,
    double Volume,
    double VWAP,
    double BidVolume,
    double AskVolume,
    double ImpVolatility,
    double OpenInterest)
{
    public unsafe Candle ToEvent() =>
        new()
        {
            EventSymbol = ((CandleSymbolNative*)EventSymbol)->Symbol.ToString(),
            EventFlags = EventFlags,
            EventTime = EventTime,
            Index = Index,
            Count = Count,
            Open = Open,
            High = High,
            Low = Low,
            Close = Close,
            Volume = Volume,
            VWAP = VWAP,
            BidVolume = BidVolume,
            AskVolume = AskVolume,
            ImpVolatility = ImpVolatility,
            OpenInterest = OpenInterest,
        };
}
