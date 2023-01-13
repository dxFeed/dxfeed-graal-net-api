// <copyright file="SummaryNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;

namespace DxFeed.Graal.Net.Native.Events.Market;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct SummaryNative
{
    public readonly MarketEventNative MarketEvent;
    public readonly int DayId;
    public readonly double DayOpenPrice;
    public readonly double DayHighPrice;
    public readonly double DayLowPrice;
    public readonly double DayClosePrice;
    public readonly int PrevDayId;
    public readonly double PrevDayClosePrice;
    public readonly double PrevDayVolume;
    public readonly long OpenInterest;
    public readonly int Flags;
}
