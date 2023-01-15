// <copyright file="SeriesNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.Events.Market;

namespace DxFeed.Graal.Net.Native.Events.Options;

/// <summary>
/// The structure contains all the fields required
/// to build an <see cref="SeriesNative"/>.
/// Used to exchange data with native code.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly struct SeriesNative
{
    public readonly MarketEventNative MarketEvent;
    public readonly int EventFlags;
    public readonly long Index;
    public readonly long TimeSequence;
    public readonly int Expiration;
    public readonly double Volatility;
    public readonly double CallVolume;
    public readonly double PutVolume;
    public readonly double PutCallRatio;
    public readonly double ForwardPrice;
    public readonly double Dividend;
    public readonly double Interest;
}
