// <copyright file="TradeMapper.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Events.Market;
using DxFeed.Graal.Net.Native.Events.Market;

namespace DxFeed.Graal.Net.Native.EventMappers;

internal static class TradeMapper
{
    public static unsafe Trade FromNative(TradeNative* tradeNative) =>
        FillTradeBase(new Trade(), &tradeNative->TradeBase);

    public static unsafe TradeETH FromNative(TradeETHNative* tradeETHNative) =>
        FillTradeBase(new TradeETH(), &tradeETHNative->TradeBase);

    private static unsafe T FillTradeBase<T>(T tradeBase, TradeBaseNative* tradeBaseNative)
        where T : TradeBase
    {
        tradeBase.EventSymbol = Marshal.PtrToStringUTF8(tradeBaseNative->MarketEvent.EventSymbol);
        tradeBase.EventTime = tradeBaseNative->MarketEvent.EventTime;
        tradeBase.TimeSequence = tradeBaseNative->TimeSequence;
        tradeBase.TimeNanos = tradeBaseNative->TimeNanoPart;
        tradeBase.ExchangeCode = tradeBaseNative->ExchangeCode;
        tradeBase.Price = tradeBaseNative->Price;
        tradeBase.Change = tradeBaseNative->Change;
        tradeBase.Size = tradeBaseNative->Size;
        tradeBase.DayId = tradeBaseNative->DayId;
        tradeBase.DayVolume = tradeBaseNative->DayVolume;
        tradeBase.DayTurnover = tradeBaseNative->DayTurnover;
        tradeBase.Flags = tradeBaseNative->Flags;
        return tradeBase;
    }
}
