// <copyright file="TradeBaseNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Events.Market;

namespace DxFeed.Graal.Net.Native.Events.Market;

/// <summary>
/// The structure contains all the fields required
/// to build an <see cref="TradeBase"/>.
/// Includes at the beginning of each trade structure.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly record struct TradeBaseNative(
    EventTypeNative EventType,
    long TimeSequence,
    int TimeNanoPart,
    char ExchangeCode,
    double Price,
    double Change,
    double Size,
    int DayId,
    double DayVolume,
    double DayTurnover,
    int Flags)
{
    /// <summary>
    /// Converts a native event to the specified <see cref="TradeBase"/>.
    /// This method fills only <see cref="TradeBase"/> properties.
    /// </summary>
    /// <typeparam name="T">The specified <see cref="TradeBase"/>.</typeparam>
    /// <returns>The <see cref="TradeBase"/>.</returns>
    public T ToEventType<T>()
        where T : TradeBase, new()
    {
        var tradeBase = EventType.ToEventType<T>();
        tradeBase.TimeSequence = TimeSequence;
        tradeBase.TimeNanoPart = TimeNanoPart;
        tradeBase.ExchangeCode = ExchangeCode;
        tradeBase.Price = Price;
        tradeBase.Change = Change;
        tradeBase.Size = Size;
        tradeBase.DayId = DayId;
        tradeBase.DayVolume = DayVolume;
        tradeBase.DayTurnover = DayTurnover;
        tradeBase.Flags = Flags;
        return tradeBase;
    }
}
