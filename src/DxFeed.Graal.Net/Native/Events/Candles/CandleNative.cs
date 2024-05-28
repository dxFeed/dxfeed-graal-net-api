// <copyright file="CandleNative.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Events.Candles;

namespace DxFeed.Graal.Net.Native.Events.Candles;

/// <summary>
/// The structure contains all the fields required
/// to build an <see cref="Candle"/>.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal record struct CandleNative(
    EventTypeNative EventType,
    int EventFlags,
    long Index,
    long Count,
    double Open,
    double High,
    double Low,
    double Close,
    double Volume,
    double VWAP,
    double BidVolume,
    double AskVolume,
    double ImpVolatility,
    double OpenInterest);
