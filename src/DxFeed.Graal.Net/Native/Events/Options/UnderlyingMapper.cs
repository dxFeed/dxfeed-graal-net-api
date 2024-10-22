// <copyright file="UnderlyingMapper.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Events.Options;

namespace DxFeed.Graal.Net.Native.Events.Options;

internal sealed class UnderlyingMapper : EventTypeMapper<Underlying, UnderlyingNative>
{
    public override unsafe IEventType FromNative(EventTypeNative* eventType) =>
        Convert((UnderlyingNative*)eventType);

    public override unsafe EventTypeNative* ToNative(IEventType eventType) =>
        (EventTypeNative*)Convert((Underlying)eventType);

    public override unsafe IEventType FillFromNative(EventTypeNative* nativeEventType, IEventType eventType) =>
        Fill((UnderlyingNative*)nativeEventType, (Underlying)eventType);

    public override unsafe void Release(EventTypeNative* eventType) =>
        ReleaseEventType(eventType);

    protected override unsafe Underlying Convert(UnderlyingNative* eventType)
    {
        var underlying = new Underlying();
        Fill(eventType, underlying);
        return underlying;
    }

    protected override unsafe UnderlyingNative* Convert(Underlying eventType)
    {
        var ptr = AllocEventType();
        *ptr = new()
        {
            EventType = CreateEventType(eventType),
            EventFlags = eventType.EventFlags,
            Index = eventType.Index,
            Volatility = eventType.Volatility,
            FrontVolatility = eventType.FrontVolatility,
            BackVolatility = eventType.BackVolatility,
            CallVolume = eventType.CallVolume,
            PutVolume = eventType.PutVolume,
            PutCallRatio = eventType.PutCallRatio,
        };
        return ptr;
    }

    private static unsafe Underlying Fill(UnderlyingNative* eventType, Underlying underlying)
    {
        AssignEventType((EventTypeNative*)eventType, underlying);
        underlying.EventFlags = eventType->EventFlags;
        underlying.Index = eventType->Index;
        underlying.Volatility = eventType->Volatility;
        underlying.FrontVolatility = eventType->FrontVolatility;
        underlying.BackVolatility = eventType->BackVolatility;
        underlying.CallVolume = eventType->CallVolume;
        underlying.PutVolume = eventType->PutVolume;
        underlying.PutCallRatio = eventType->PutCallRatio;
        return underlying;
    }
}
