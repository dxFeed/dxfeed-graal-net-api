// <copyright file="CandleNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.Symbols.Candle;

namespace DxFeed.Graal.Net.Native.Events.Candle;

/// <summary>
/// The structure contains all the fields required
/// to build an <see cref="Candle"/>.
/// Used to exchange data with native code.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly struct CandleNative
{
    public readonly BaseEventNative Base;
    public readonly unsafe CandleSymbolNative* CandleSymbol;
    public readonly int EventFlags;
    public readonly long EventTime;
    public readonly long Index;
    public readonly long Count;
    public readonly double Open;
    public readonly double High;
    public readonly double Low;
    public readonly double Close;
    public readonly double Volume;
    public readonly double VWAP;
    public readonly double BidVolume;
    public readonly double AskVolume;
    public readonly double ImpVolatility;
    public readonly double OpenInterest;
}
