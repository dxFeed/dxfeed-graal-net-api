// <copyright file="DayUtil.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace DxFeed.Graal.Net.Utils;

/// <summary>
/// A collection of static utility methods for manipulation of <c>int</c> day id,
/// that is the number of days since Unix epoch of January 1, 1970.
/// <br/>
/// Porting Java class <c>com.devexperts.util.DayUtil</c>.
/// </summary>
public static class DayUtil
{
    /// <summary>
    /// Gets <c>yyyymmdd</c> integer in Gregorian calendar for a specified day identifier.
    /// The day identifier is defined as the number of days since Unix epoch of January 1, 1970.
    /// The result is equal to:
    /// <code>yearSign * (abs(year) * 10000 + month * 100 + day)</code>
    /// where year, month, and day are in Gregorian calendar,
    /// month is between 1 and 12 inclusive, and day is counted from 1.
    /// </summary>
    /// <param name="dayId">A number of whole days since Unix epoch of January 1, 1970.</param>
    /// <returns>The <c>yyyymmdd</c> integer in Gregorian calendar.</returns>
    /// <example>
    /// <code>
    /// DayUtil.GetYearMonthDayByDayId(-1) == 19691231
    /// DayUtil.GetYearMonthDayByDayId(0)  == 19700101
    /// DayUtil.GetYearMonthDayByDayId(1)  == 19700102
    /// </code>
    /// </example>
    public static int GetYearMonthDayByDayId(int dayId)
    {
        // This shifts the epoch back to astronomical year -4800.
        var j = dayId + 2472632;
        var g = MathUtil.Div(j, 146097);
        var dg = j - (g * 146097);
        var c = ((dg / 36524) + 1) * 3 / 4;
        var dc = dg - (c * 36524);
        var b = dc / 1461;
        var db = dc - (b * 1461);
        var a = ((db / 365) + 1) * 3 / 4;
        var da = db - (a * 365);

        // This is the integer number of full years elapsed since March 1, 4801 BC at 00:00 UTC.
        var y = (g * 400) + (c * 100) + (b * 4) + a;

        // This is the integer number of full months elapsed since the last March 1 at 00:00 UTC.
        var m = (((da * 5) + 308) / 153) - 2;

        // This is the number of days elapsed since day 1 of the month at 00:00 UTC.
        var d = da - ((m + 4) * 153 / 5) + 122;
        var yyyy = y - 4800 + ((m + 2) / 12);
        var mm = ((m + 2) % 12) + 1;
        var dd = d + 1;
        var yyyymmdd = (MathUtil.Abs(yyyy) * 10000) + (mm * 100) + dd;

        return yyyy >= 0 ? yyyymmdd : -yyyymmdd;
    }
}
