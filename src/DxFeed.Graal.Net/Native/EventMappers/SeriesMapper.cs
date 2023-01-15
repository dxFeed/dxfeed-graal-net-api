// <copyright file="SeriesMapper.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Events.Options;
using DxFeed.Graal.Net.Native.Events.Options;

namespace DxFeed.Graal.Net.Native.EventMappers;

internal static class SeriesMapper
{
    public static unsafe Series FromNative(SeriesNative* eventNative) =>
        new()
        {
            EventSymbol = Marshal.PtrToStringUTF8(eventNative->MarketEvent.EventSymbol),
            EventTime = eventNative->MarketEvent.EventTime,
            EventFlags = eventNative->EventFlags,
            Index = eventNative->Index,
            TimeSequence = eventNative->TimeSequence,
            Expiration = eventNative->Expiration,
            Volatility = eventNative->Volatility,
            CallVolume = eventNative->CallVolume,
            PutVolume = eventNative->PutVolume,
            PutCallRatio = eventNative->PutCallRatio,
            ForwardPrice = eventNative->ForwardPrice,
            Dividend = eventNative->Dividend,
            Interest = eventNative->Interest,
        };
}
