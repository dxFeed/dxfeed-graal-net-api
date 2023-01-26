// <copyright file="UnderlyingNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Events.Options;

namespace DxFeed.Graal.Net.Native.Events.Options;

/// <summary>
/// The structure contains all the fields required
/// to build an <see cref="Underlying"/>.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly record struct UnderlyingNative(
        EventTypeNative EventType,
        int EventFlags,
        long Index,
        double Volatility,
        double FrontVolatility,
        double BackVolatility,
        double CallVolume,
        double PutVolume,
        double PutCallRatio)
    : IEventTypeNative<Underlying>
{
    /// <inheritdoc/>
    public Underlying ToEventType()
    {
        var underlying = EventType.ToEventType<Underlying>();
        underlying.EventFlags = EventFlags;
        underlying.Index = Index;
        underlying.Volatility = Volatility;
        underlying.FrontVolatility = FrontVolatility;
        underlying.BackVolatility = BackVolatility;
        underlying.CallVolume = CallVolume;
        underlying.PutVolume = PutVolume;
        underlying.PutCallRatio = PutCallRatio;
        return underlying;
    }
}
