// <copyright file="DXTimePeriod.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using DxFeed.Graal.Net.Native.Utils;

namespace DxFeed.Graal.Net.Utils;

public class DXTimePeriod
{
    private static readonly Lazy<DXTimePeriod> _zero = new(() =>
        new DXTimePeriod(TimePeriodNative.Zero()));

    private static readonly Lazy<DXTimePeriod> _unlimited = new(() =>
        new DXTimePeriod(TimePeriodNative.Unlimited()));

    private TimePeriodNative _timePeriodNative;

    private DXTimePeriod(TimePeriodNative timePeriodNative) => _timePeriodNative = timePeriodNative;

    public static DXTimePeriod Zero() => _zero.Value;

    public static DXTimePeriod Unlimited() => _unlimited.Value;

    public static DXTimePeriod ValueOf(long value) => new(TimePeriodNative.ValueOf(value));
    //from millis to timepspan
    public static DXTimePeriod ValueOf(string value) => new(TimePeriodNative.ValueOf(value));

    public long GetTime() => _timePeriodNative.GetTime();

    public int GetSeconds() => _timePeriodNative.GetSeconds();

    public long GetNanos() => _timePeriodNative.GetNanos();
}
