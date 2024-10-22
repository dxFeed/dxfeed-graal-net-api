// <copyright file="GreeksMapper.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Events.Options;

namespace DxFeed.Graal.Net.Native.Events.Options;

internal sealed class GreeksMapper : EventTypeMapper<Greeks, GreeksNative>
{
    public override unsafe IEventType FromNative(EventTypeNative* eventType) =>
        Convert((GreeksNative*)eventType);

    public override unsafe EventTypeNative* ToNative(IEventType eventType) =>
        (EventTypeNative*)Convert((Greeks)eventType);

    public override unsafe IEventType FillFromNative(EventTypeNative* nativeEventType, IEventType eventType) =>
        Fill((GreeksNative*)nativeEventType, (Greeks)eventType);

    public override unsafe void Release(EventTypeNative* eventType) =>
        ReleaseEventType(eventType);

    protected override unsafe Greeks Convert(GreeksNative* eventType)
    {
        var greeks = new Greeks();
        Fill(eventType, greeks);
        return greeks;
    }

    protected override unsafe GreeksNative* Convert(Greeks eventType)
    {
        var ptr = AllocEventType();
        *ptr = new()
        {
            EventType = CreateEventType(eventType),
            EventFlags = eventType.EventFlags,
            Index = eventType.Index,
            Price = eventType.Price,
            Volatility = eventType.Volatility,
            Delta = eventType.Delta,
            Gamma = eventType.Gamma,
            Theta = eventType.Theta,
            Rho = eventType.Rho,
            Vega = eventType.Vega,
        };
        return ptr;
    }

    private static unsafe Greeks Fill(GreeksNative* eventType, Greeks greeks)
    {
        AssignEventType((EventTypeNative*)eventType, greeks);
        greeks.EventFlags = eventType->EventFlags;
        greeks.Index = eventType->Index;
        greeks.Price = eventType->Price;
        greeks.Volatility = eventType->Volatility;
        greeks.Delta = eventType->Delta;
        greeks.Gamma = eventType->Gamma;
        greeks.Theta = eventType->Theta;
        greeks.Rho = eventType->Rho;
        greeks.Vega = eventType->Vega;
        return greeks;
    }
}
