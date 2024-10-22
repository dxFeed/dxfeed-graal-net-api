// <copyright file="SummaryMapper.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Events.Market;

namespace DxFeed.Graal.Net.Native.Events.Market;

internal sealed class SummaryMapper : EventTypeMapper<Summary, SummaryNative>
{
    public override unsafe IEventType FromNative(EventTypeNative* eventType) =>
        Convert((SummaryNative*)eventType);

    public override unsafe EventTypeNative* ToNative(IEventType eventType) =>
        (EventTypeNative*)Convert((Summary)eventType);

    public override unsafe IEventType FillFromNative(EventTypeNative* nativeEventType, IEventType eventType) =>
        Fill((SummaryNative*)nativeEventType, (Summary)eventType);

    public override unsafe void Release(EventTypeNative* eventType) =>
        ReleaseEventType(eventType);

    protected override unsafe Summary Convert(SummaryNative* eventType)
    {
        var summary = new Summary();
        Fill(eventType, summary);
        return summary;
    }

    protected override unsafe SummaryNative* Convert(Summary eventType)
    {
        var ptr = AllocEventType();
        *ptr = new()
        {
            EventType = CreateEventType(eventType),
            DayId = eventType.DayId,
            DayOpenPrice = eventType.DayOpenPrice,
            DayHighPrice = eventType.DayHighPrice,
            DayLowPrice = eventType.DayLowPrice,
            DayClosePrice = eventType.DayClosePrice,
            PrevDayId = eventType.PrevDayId,
            PrevDayClosePrice = eventType.PrevDayClosePrice,
            PrevDayVolume = eventType.PrevDayVolume,
            OpenInterest = eventType.OpenInterest,
            Flags = eventType.Flags,
        };
        return ptr;
    }

    private static unsafe Summary Fill(SummaryNative* eventType, Summary summary)
    {
        AssignEventType((EventTypeNative*)eventType, summary);
        summary.DayId = eventType->DayId;
        summary.DayOpenPrice = eventType->DayOpenPrice;
        summary.DayHighPrice = eventType->DayHighPrice;
        summary.DayLowPrice = eventType->DayLowPrice;
        summary.DayClosePrice = eventType->DayClosePrice;
        summary.PrevDayId = eventType->PrevDayId;
        summary.PrevDayClosePrice = eventType->PrevDayClosePrice;
        summary.PrevDayVolume = eventType->PrevDayVolume;
        summary.OpenInterest = eventType->OpenInterest;
        summary.Flags = eventType->Flags;
        return summary;
    }
}
