// <copyright file="OrderBaseNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Events.Market;

namespace DxFeed.Graal.Net.Native.Events.Market;

/// <summary>
/// The structure contains all the fields required
/// to build an <see cref="OrderBase"/>.
/// Used to exchange data with native code.
/// Includes at the beginning of each order structure.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly struct OrderBaseNative
{
    public readonly MarketEvent MarketEvent;
    public readonly int EventFlags;
    public readonly long Index;
    public readonly long TimeSequence;
    public readonly int TimeNanoPart;
    public readonly long ActionTime;
    public readonly long OrderOd;
    public readonly long AuxOrderId;
    public readonly double Price;
    public readonly double Size;
    public readonly double ExecutedSize;
    public readonly long Count;
    public readonly int Flags;
    public readonly long TradeId;
    public readonly double TradePrice;
    public readonly double TradeSize;
}
