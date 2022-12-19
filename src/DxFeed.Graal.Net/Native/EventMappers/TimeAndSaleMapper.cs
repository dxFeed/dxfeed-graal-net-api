// <copyright file="TimeAndSaleMapper.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Events.Market;
using DxFeed.Graal.Net.Native.Events.Market;

namespace DxFeed.Graal.Net.Native.EventMappers;

internal static class TimeAndSaleMapper
{
    public static unsafe TimeAndSale FromNative(TimeAndSaleNative* eventNative) =>
        new()
        {
            EventSymbol = Marshal.PtrToStringUTF8(eventNative->MarketEvent.EventSymbol),
            EventTime = eventNative->MarketEvent.EventTime,
            EventFlags = eventNative->EventFlags,
            Index = eventNative->Index,
            TimeNanoPart = eventNative->TimeNanoPart,
            ExchangeCode = eventNative->ExchangeCode,
            Price = eventNative->Price,
            Size = eventNative->Size,
            BidPrice = eventNative->BidPrice,
            AskPrice = eventNative->AskPrice,
            ExchangeSaleConditions = Marshal.PtrToStringUTF8(eventNative->ExchangeSaleConditions),
            Flags = eventNative->Flags,
            Buyer = Marshal.PtrToStringUTF8(eventNative->Buyer),
            Seller = Marshal.PtrToStringUTF8(eventNative->Seller),
        };
}
