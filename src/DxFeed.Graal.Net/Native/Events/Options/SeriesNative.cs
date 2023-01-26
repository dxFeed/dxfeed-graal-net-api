// <copyright file="SeriesNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Events.Options;

namespace DxFeed.Graal.Net.Native.Events.Options;

/// <summary>
/// The structure contains all the fields required
/// to build an <see cref="Series"/>.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly record struct SeriesNative(
        EventTypeNative EventType,
        int EventFlags,
        long Index,
        long TimeSequence,
        int Expiration,
        double Volatility,
        double CallVolume,
        double PutVolume,
        double PutCallRatio,
        double ForwardPrice,
        double Dividend,
        double Interest)
    : IEventTypeNative<Series>
{
    /// <inheritdoc/>
    public Series ToEventType()
    {
        var series = EventType.ToEventType<Series>();
        series.EventFlags = EventFlags;
        series.Index = Index;
        series.TimeSequence = TimeSequence;
        series.Expiration = Expiration;
        series.Volatility = Volatility;
        series.CallVolume = CallVolume;
        series.PutVolume = PutVolume;
        series.PutCallRatio = PutCallRatio;
        series.ForwardPrice = ForwardPrice;
        series.Dividend = Dividend;
        series.Interest = Interest;
        return series;
    }
}
