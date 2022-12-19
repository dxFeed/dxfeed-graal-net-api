// <copyright file="QuoteNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Events.Market;

namespace DxFeed.Graal.Net.Native.Events.Market;

/// <summary>
/// The structure contains all the fields required
/// to build an <see cref="Quote"/>.
/// Used to exchange data with native code.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly struct QuoteNative
{
    public readonly MarketEvent MarketEvent;
    public readonly int TimeMillisSequence;
    public readonly int TimeNanoPart;
    public readonly long BidTime;
    public readonly char BidExchangeCode;
    public readonly double BidPrice;
    public readonly double BidSize;
    public readonly long AskTime;
    public readonly char AskExchangeCode;
    public readonly double AskPrice;
    public readonly double AskSize;
}
