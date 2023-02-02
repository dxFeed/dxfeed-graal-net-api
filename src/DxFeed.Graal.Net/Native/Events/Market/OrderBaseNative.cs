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
/// Includes at the beginning of each order structure.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly record struct OrderBaseNative(
    EventTypeNative EventType,
    int EventFlags,
    long Index,
    long TimeSequence,
    int TimeNanoPart,
    long ActionTime,
    long OrderId,
    long AuxOrderId,
    double Price,
    double Size,
    double ExecutedSize,
    long Count,
    int Flags,
    long TradeId,
    double TradePrice,
    double TradeSize);
