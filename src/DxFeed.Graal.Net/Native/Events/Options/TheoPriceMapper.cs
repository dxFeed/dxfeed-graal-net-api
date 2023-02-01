// <copyright file="TheoPriceMapper.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Events.Options;

namespace DxFeed.Graal.Net.Native.Events.Options;

internal class TheoPriceMapper : EventTypeMapper<TheoPrice, TheoPriceNative>
{
    public override unsafe IEventType FromNative(EventTypeNative* eventType) =>
        Convert((TheoPriceNative*)eventType);

    public override unsafe EventTypeNative* ToNative(IEventType eventType) =>
        (EventTypeNative*)Convert((TheoPrice)eventType);

    public override unsafe void Release(EventTypeNative* eventType) =>
        ReleaseEventType(eventType);

    protected override unsafe TheoPrice Convert(TheoPriceNative* eventType)
    {
        var theoPrice = CreateEventType(eventType);
        theoPrice.EventFlags = eventType->EventFlags;
        theoPrice.Index = eventType->Index;
        theoPrice.Price = eventType->Price;
        theoPrice.UnderlyingPrice = eventType->UnderlyingPrice;
        theoPrice.Delta = eventType->Delta;
        theoPrice.Gamma = eventType->Gamma;
        theoPrice.Dividend = eventType->Dividend;
        theoPrice.Interest = eventType->Interest;
        return theoPrice;
    }

    protected override unsafe TheoPriceNative* Convert(TheoPrice eventType)
    {
        var ptr = AllocEventType();
        *ptr = new()
        {
            EventType = CreateEventType(eventType),
            EventFlags = eventType.EventFlags,
            Index = eventType.Index,
            Price = eventType.Price,
            UnderlyingPrice = eventType.UnderlyingPrice,
            Delta = eventType.Delta,
            Gamma = eventType.Gamma,
            Dividend = eventType.Dividend,
            Interest = eventType.Interest,
        };
        return ptr;
    }
}
