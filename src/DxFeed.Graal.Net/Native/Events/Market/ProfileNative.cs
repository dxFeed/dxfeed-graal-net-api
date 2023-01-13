// <copyright file="ProfileNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Events.Market;

namespace DxFeed.Graal.Net.Native.Events.Market;

/// <summary>
/// The structure contains all the fields required
/// to build an <see cref="Profile"/>.
/// Used to exchange data with native code.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly struct ProfileNative
{
    public readonly MarketEventNative MarketEvent;
    public readonly nint Description; // A null-terminated UTF-8 string.
    public readonly nint StatusReason; // A null-terminated UTF-8 string.
    public readonly long HaltStartTime;
    public readonly long HaltEndTime;
    public readonly double HighLimitPrice;
    public readonly double LowLimitPrice;
    public readonly double High52WeekPrice;
    public readonly double Low52WeekPrice;
    public readonly int Flags;
}
