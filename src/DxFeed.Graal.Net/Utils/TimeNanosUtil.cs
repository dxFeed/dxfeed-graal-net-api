// <copyright file="TimeNanosUtil.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace DxFeed.Graal.Net.Utils;

/// <summary>
/// A collection of static utility methods for manipulation of time measured in nanoseconds since Unix epoch.
/// <br/>
/// Porting Java class <c>com.dxfeed.event.impl.TimeNanosUtil</c>.
/// </summary>
public static class TimeNanosUtil
{
    /// <summary>
    /// Number of nanoseconds in millisecond.
    /// </summary>
    private const long NanosInMillis = 1_000_000L;

    /// <summary>
    /// Returns time measured in nanoseconds since Unix epoch from the time in milliseconds and its nano part.
    /// The result of this method is <c>timeMillis * 1_000_000 + timeNanoPart</c>.
    /// </summary>
    /// <param name="timeMillis">The time in milliseconds since Unix epoch.</param>
    /// <param name="timeNanoPart">The nanoseconds part that shall lie within [0..999999] interval.</param>
    /// <returns>The time measured in nanoseconds since Unix epoch.</returns>
    public static long GetNanosFromMillisAndNanoPart(long timeMillis, int timeNanoPart) =>
        (timeMillis * NanosInMillis) + timeNanoPart;

    /// <summary>
    /// Returns time measured in milliseconds since Unix epoch from the time in nanoseconds.
    /// Idea is that nano part of time shall be within [0..999999] interval
    /// so that the following equation always holds:
    /// <code>GetMillisFromNanos(timeNanos) * 1_000_000 + GetNanoPartFromNanos(timeNanos) == timeNanos</code>
    /// <seealso cref="GetNanoPartFromNanos(long)"/>
    /// </summary>
    /// <param name="timeNanos">The time measured in nanoseconds since Unix epoch.</param>
    /// <returns>The time measured in milliseconds since Unix epoch.</returns>
    public static long GetMillisFromNanos(long timeNanos) =>
        MathUtil.FloorDiv(timeNanos, NanosInMillis);

    /// <summary>
    /// Returns nano part of time.
    /// Idea is that nano part of time shall be within [0..999999] interval
    /// so that the following equation always holds:
    /// <code>GetMillisFromNanos(timeNanos) * 1_000_000 + GetNanoPartFromNanos(timeNanos) == timeNanos</code>
    /// <seealso cref="GetMillisFromNanos(long)"/>
    /// </summary>
    /// <param name="timeNanos">The time measured in nanoseconds since Unix epoch.</param>
    /// <returns>The time measured in milliseconds since Unix epoch.</returns>
    public static int GetNanoPartFromNanos(long timeNanos) =>
        (int)MathUtil.FloorMod(timeNanos, NanosInMillis);
}
