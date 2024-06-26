// <copyright file="TimePeriod.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using DxFeed.Graal.Net.Native.Utils;

namespace DxFeed.Graal.Net.Utils;

/// <summary>
/// A collection of utility methods for creation Timespan with support for ISO8601 duration format.
/// </summary>
public static class TimePeriod
{
    /// <summary>
    /// Returns <see cref="TimeSpan"/> with represented with a given string.
    /// Allowable format is ISO8601 duration, but there are some simplifications and modifications available:
    /// <ul>
    ///     <li> Letters are case insensitive.</li>
    ///     <li> Letters "P" and "T" can be omitted.</li>
    ///     <li> Letter "S" can be also omitted. In this case last number will be supposed to be seconds.</li>
    ///     <li> Number of seconds can be fractional. So it is possible to define duration accurate within milliseconds.</li>
    ///     <li> Every part can be omitted. It is supposed that it's value is zero then.</li>
    ///     <li> String "inf" recognized as unlimited period.</li>
    /// </ul>
    /// </summary>
    /// <param name="value">The string representation.</param>
    /// <returns>The time span that represented with a given string.</returns>
    public static TimeSpan ValueOf(string value)
    {
        const long MaxMilliSeconds = long.MaxValue / TimeSpan.TicksPerMillisecond;
        const long MinMilliSeconds = long.MinValue / TimeSpan.TicksPerMillisecond;
        var millis = TimePeriodNative.ValueOf(value).GetTime();
        return millis switch
        {
            >= MaxMilliSeconds => TimeSpan.MaxValue,
            <= MinMilliSeconds => TimeSpan.MinValue,
            _ => TimeSpan.FromMilliseconds(millis),
        };
    }
}
