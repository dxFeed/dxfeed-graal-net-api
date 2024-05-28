// <copyright file="ProfileNative.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Events.Market;
using DxFeed.Graal.Net.Native.Interop;

namespace DxFeed.Graal.Net.Native.Events.Market;

/// <summary>
/// The structure contains all the fields required
/// to build an <see cref="Profile"/>.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal record struct ProfileNative(
    EventTypeNative EventType,
    StringNative Description,
    StringNative StatusReason,
    long HaltStartTime,
    long HaltEndTime,
    double HighLimitPrice,
    double LowLimitPrice,
    double High52WeekPrice,
    double Low52WeekPrice,
    double Beta,
    double EarningsPerShare,
    double DividendFrequency,
    double ExDividendAmount,
    int ExDividendDayId,
    double Shares,
    double FreeFloat,
    int Flags);
