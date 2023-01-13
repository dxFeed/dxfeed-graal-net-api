// <copyright file="ProfileMapper.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Events.Market;
using DxFeed.Graal.Net.Native.Events.Market;

namespace DxFeed.Graal.Net.Native.EventMappers;

internal static class ProfileMapper
{
    public static unsafe Profile FromNative(ProfileNative* eventNative) =>
        new()
        {
            EventSymbol = Marshal.PtrToStringUTF8(eventNative->MarketEvent.EventSymbol),
            EventTime = eventNative->MarketEvent.EventTime,
            Description = Marshal.PtrToStringUTF8(eventNative->Description),
            StatusReason = Marshal.PtrToStringUTF8(eventNative->StatusReason),
            HaltStartTime = eventNative->HaltStartTime,
            HaltEndTime = eventNative->HaltEndTime,
            HighLimitPrice = eventNative->HighLimitPrice,
            LowLimitPrice = eventNative->LowLimitPrice,
            High52WeekPrice = eventNative->High52WeekPrice,
            Low52WeekPrice = eventNative->Low52WeekPrice,
            Flags = eventNative->Flags,
        };
}
