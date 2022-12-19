// <copyright file="TimeUtil.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;

namespace DxFeed.Graal.Net.Utils;

/// <summary>
/// A collection of static utility methods for manipulation of time measured in milliseconds since Unix epoch.
/// <br/>
/// Porting Java class <c>com.devexperts.util.TimeUtil</c>.
/// </summary>
public static class TimeUtil
{
    /// <summary>
    /// Number of milliseconds in a second.
    /// </summary>
    public const long Second = 1000;

    /// <summary>
    /// Number of milliseconds in a minute.
    /// </summary>
    public const long Minute = 60 * Second;

    /// <summary>
    /// Number of milliseconds in an hour.
    /// </summary>
    public const long Hour = 60 * Minute;

    /// <summary>
    /// Number of milliseconds in a day.
    /// </summary>
    public const long Day = 24 * Hour;

    /// <summary>
    /// Returns correct number of seconds with proper handling negative values and overflows.
    /// Idea is that number of milliseconds shall be within [0..999] interval
    /// so that the following equation always holds:
    /// <c>GetSecondsFromTime(timeMillis) * 1000L + GetMillisFromTime(timeMillis) == timeMillis</c>
    /// as as long the time in seconds fits into <b>int</b>.
    /// <seealso cref="GetMillisFromTime(long)"/>
    /// </summary>
    /// <param name="timeMillis">The time measured in milliseconds since Unix epoch.</param>
    /// <returns>The number of seconds.</returns>
    public static int GetSecondsFromTime(long timeMillis) =>
        timeMillis >= 0
            ? (int)Math.Min(timeMillis / Second, int.MaxValue)
            : (int)Math.Max(((timeMillis + 1) / Second) - 1, int.MinValue);

    /// <summary>
    /// Returns correct number of milliseconds with proper handling negative values.
    /// Idea is that number of milliseconds shall be within [0..999] interval
    /// so that the following equation always holds:
    /// <c>GetSecondsFromTime(timeMillis) * 1000L + GetMillisFromTime(timeMillis) == timeMillis</c>
    /// as as long the time in seconds fits into <b>int</b>.
    /// <seealso cref="GetSecondsFromTime(long)"/>
    /// </summary>
    /// <param name="timeMillis">The time measured in milliseconds since Unix epoch.</param>
    /// <returns>The number of milliseconds.</returns>
    public static int GetMillisFromTime(long timeMillis) =>
        (int)MathUtil.FloorMod(timeMillis, Second);
}
