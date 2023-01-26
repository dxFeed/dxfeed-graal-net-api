// <copyright file="SummaryNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Events.Market;

namespace DxFeed.Graal.Net.Native.Events.Market;

/// <summary>
/// The structure contains all the fields required
/// to build an <see cref="Summary"/>.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly record struct SummaryNative(
        EventTypeNative EventType,
        int DayId,
        double DayOpenPrice,
        double DayHighPrice,
        double DayLowPrice,
        double DayClosePrice,
        int PrevDayId,
        double PrevDayClosePrice,
        double PrevDayVolume,
        long OpenInterest,
        int Flags)
    : IEventTypeNative<Summary>
{
    /// <inheritdoc/>
    public Summary ToEventType()
    {
        var summary = EventType.ToEventType<Summary>();
        summary.DayId = DayId;
        summary.DayOpenPrice = DayOpenPrice;
        summary.DayHighPrice = DayHighPrice;
        summary.DayLowPrice = DayLowPrice;
        summary.DayClosePrice = DayClosePrice;
        summary.PrevDayId = PrevDayId;
        summary.PrevDayClosePrice = PrevDayClosePrice;
        summary.PrevDayVolume = PrevDayVolume;
        summary.OpenInterest = OpenInterest;
        summary.Flags = Flags;
        return summary;
    }
}
