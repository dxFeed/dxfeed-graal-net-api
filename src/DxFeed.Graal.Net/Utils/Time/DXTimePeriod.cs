// <copyright file="DXTimePeriod.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Native.Utils;

namespace DxFeed.Graal.Net.Utils;

public class DXTimePeriod
{
    private TimePeriodNative _timePeriodNative;

    public static DXTimePeriod Zero() => new() { _timePeriodNative = TimePeriodNative.Zero() };

    public static DXTimePeriod Unlimited() => new() { _timePeriodNative = TimePeriodNative.Unlimited() };

    public static DXTimePeriod ValueOf(long value) => new() { _timePeriodNative = TimePeriodNative.ValueOf(value) };

    public static DXTimePeriod ValueOf(string value) => new() { _timePeriodNative = TimePeriodNative.ValueOf(value) };

    public long GetTime() => _timePeriodNative.GetTime();

    public int GetSeconds() => _timePeriodNative.GetSeconds();

    public long GetNanos() => _timePeriodNative.GetNanos();
}
