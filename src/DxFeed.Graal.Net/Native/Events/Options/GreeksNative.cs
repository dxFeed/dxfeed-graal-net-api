// <copyright file="GreeksNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.Events.Market;

namespace DxFeed.Graal.Net.Native.Events.Options;

/// <summary>
/// The structure contains all the fields required
/// to build an <see cref="GreeksNative"/>.
/// Used to exchange data with native code.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly struct GreeksNative
{
    public readonly MarketEventNative MarketEvent;
    public readonly int EventFlags;
    public readonly long Index;
    public readonly double Price;
    public readonly double Volatility;
    public readonly double Delta;
    public readonly double Gamma;
    public readonly double Theta;
    public readonly double Rho;
    public readonly double Vega;
}
