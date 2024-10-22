// <copyright file="SeriesMapper.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Events.Options;

namespace DxFeed.Graal.Net.Native.Events.Options;

internal sealed class SeriesMapper : EventTypeMapper<Series, SeriesNative>
{
    public override unsafe IEventType FromNative(EventTypeNative* eventType) =>
        Convert((SeriesNative*)eventType);

    public override unsafe EventTypeNative* ToNative(IEventType eventType) =>
        (EventTypeNative*)Convert((Series)eventType);

    public override unsafe IEventType FillFromNative(EventTypeNative* nativeEventType, IEventType eventType) =>
        Fill((SeriesNative*)nativeEventType, (Series)eventType);

    public override unsafe void Release(EventTypeNative* eventType) =>
        ReleaseEventType(eventType);

    protected override unsafe Series Convert(SeriesNative* eventType)
    {
        var series = new Series();
        Fill(eventType, series);
        return series;
    }

    protected override unsafe SeriesNative* Convert(Series eventType)
    {
        var ptr = AllocEventType();
        *ptr = new()
        {
            EventType = CreateEventType(eventType),
            EventFlags = eventType.EventFlags,
            Index = eventType.Index,
            TimeSequence = eventType.TimeSequence,
            Expiration = eventType.Expiration,
            Volatility = eventType.Volatility,
            CallVolume = eventType.CallVolume,
            PutVolume = eventType.PutVolume,
            PutCallRatio = eventType.PutCallRatio,
            ForwardPrice = eventType.ForwardPrice,
            Dividend = eventType.Dividend,
            Interest = eventType.Interest,
        };
        return ptr;
    }

    private static unsafe Series Fill(SeriesNative* eventType, Series series)
    {
        AssignEventType((EventTypeNative*)eventType, series);
        series.EventFlags = eventType->EventFlags;
        series.Index = eventType->Index;
        series.TimeSequence = eventType->TimeSequence;
        series.Expiration = eventType->Expiration;
        series.Volatility = eventType->Volatility;
        series.CallVolume = eventType->CallVolume;
        series.PutVolume = eventType->PutVolume;
        series.PutCallRatio = eventType->PutCallRatio;
        series.ForwardPrice = eventType->ForwardPrice;
        series.Dividend = eventType->Dividend;
        series.Interest = eventType->Interest;
        return series;
    }
}
