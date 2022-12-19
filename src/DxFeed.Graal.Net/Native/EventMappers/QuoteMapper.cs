// <copyright file="QuoteMapper.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Events.Market;
using DxFeed.Graal.Net.Native.Events.Market;

namespace DxFeed.Graal.Net.Native.EventMappers;

internal static class QuoteMapper
{
    public static unsafe Quote FromNative(QuoteNative* eventNative) =>
        new()
        {
            EventSymbol = Marshal.PtrToStringUTF8(eventNative->MarketEvent.EventSymbol),
            EventTime = eventNative->MarketEvent.EventTime,
            TimeMillisSequence = eventNative->TimeMillisSequence,
            TimeNanoPart = eventNative->TimeNanoPart,
            BidTime = eventNative->BidTime,
            BidExchangeCode = eventNative->BidExchangeCode,
            BidPrice = eventNative->BidPrice,
            BidSize = eventNative->BidSize,
            AskTime = eventNative->AskTime,
            AskExchangeCode = eventNative->AskExchangeCode,
            AskPrice = eventNative->AskPrice,
            AskSize = eventNative->AskSize,
        };
}
