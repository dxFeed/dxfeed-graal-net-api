// <copyright file="DayUtil.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Diagnostics.CodeAnalysis;

namespace DxFeed.Graal.Net.Utils;

/// <summary>
/// A collection of static utility methods for manipulation of <see cref="int"/> day id,
/// that is the number of days since Unix epoch of January 1, 1970.
/// <br/>
/// Ports the Java class <c>com.devexperts.util.DayUtil</c>.
/// </summary>
public static class DayUtil
{
    /// <summary>
    /// Represent the cumulative number of days that have elapsed in a year up to the beginning of each month,
    /// considering a non-leap year.
    /// <ul>
    ///     <li>[0] - a placeholder since month indices usually start from 1 for January.</li>
    ///     <li>[1] - January 1st, 0 days have passed before January.</li>
    ///     <li>[2] - February 1st, 31 days (January) have passed.</li>
    ///     <li>[3] - March 1st, 59 days (January + February) have passed.</li>
    ///     <li>...</li>
    ///     <li>[13] - represents the total number of days in a non-leap year up to the end of December.</li>
    /// </ul>
    /// </summary>
    private static readonly int[] DAY_OF_YEAR = { 0, 0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334, 365 };

    /// <summary>
    /// Gets the day identifier for specified year, month and day in Gregorian calendar.
    /// The day identifier is defined as the number of days since Unix epoch of January 1, 1970.
    /// Month must be between 1 and 12 inclusive.
    /// Year and day might take arbitrary values assuming proleptic Gregorian calendar.
    /// The value returned by this method for an arbitrary day value always satisfies the following equality:
    /// <code>
    /// GetDayIdByYearMonthDay(year, month, day) == GetDayIdByYearMonthDay(year, month, 0) + day
    /// </code>
    /// </summary>
    /// <param name="year">The year.</param>
    /// <param name="month">The month between 1 and 12 inclusive.</param>
    /// <param name="day">The day.</param>
    /// <returns>The day id.</returns>
    /// <exception cref="ArgumentException">If the month is less than 1 or greater than 12.</exception>
    public static int GetDayIdByYearMonthDay(int year, int month, int day)
    {
        if (month is < 1 or > 12)
        {
            throw new ArgumentException($"Invalid month {month}", nameof(month));
        }

        var dayOfYear = DAY_OF_YEAR[month] + day - 1;
        if (month > 2 && year % 4 == 0 && (year % 100 != 0 || year % 400 == 0))
        {
            // Leap year.
            dayOfYear++;
        }

        return (year * 365) +
            MathUtil.Div(year - 1, 4) -
            MathUtil.Div(year - 1, 100) +
            MathUtil.Div(year - 1, 400) +
            dayOfYear - 719527;
    }

    /// <summary>
    /// Gets the day identifier for specified <c>yyyymmdd</c>  integer in Gregorian calendar.
    /// The day identifier is defined as the number of days since Unix epoch of January 1, 1970.
    /// The <c>yyyymmdd</c>  integer is equal to <c>yearSign * (abs(year) * 10000 + month * 100 + day)</c>,
    /// where year, month, and day are in Gregorian calendar,
    /// month is between 1 and 12 inclusive, and day is counted from 1.
    /// </summary>
    /// <param name="yyyymmdd">The <c>yyyymmdd</c> integer in Gregorian calendar.</param>
    /// <returns>The day id.</returns>
    /// <seealso cref="GetDayIdByYearMonthDay(int,int,int)"/>
    /// <example>
    /// <code>
    /// DayUtil.GetDayIdByYearMonthDay(19691231) == -1
    /// DayUtil.GetDayIdByYearMonthDay(19700101) ==  0
    /// DayUtil.GetDayIdByYearMonthDay(19700102) ==  1
    /// </code>
    /// </example>
    /// <exception cref="ArgumentException">If the month is less than 1 or greater than 12.</exception>
    [SuppressMessage("ReSharper", "ArrangeRedundantParentheses", Justification = "Readability")]
    public static int GetDayIdByYearMonthDay(int yyyymmdd) =>
        yyyymmdd >= 0
            ? GetDayIdByYearMonthDay(yyyymmdd / 10000, (yyyymmdd / 100) % 100, yyyymmdd % 100)
            : GetDayIdByYearMonthDay(-(-yyyymmdd / 10000), (-yyyymmdd / 100) % 100, -yyyymmdd % 100);

    /// <summary>
    /// Gets the integer <c>yyyymmdd</c> in Gregorian calendar for a specified day identifier.
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
    [SuppressMessage("ReSharper", "SuggestVarOrType_BuiltInTypes", Justification = "Readability")]
    public static int GetYearMonthDayByDayId(int dayId)
    {
        // This shifts the epoch back to astronomical year -4800.
        int j = dayId + 2472632;
        int g = MathUtil.Div(j, 146097);
        int dg = j - (g * 146097);
        int c = ((dg / 36524) + 1) * 3 / 4;
        int dc = dg - (c * 36524);
        int b = dc / 1461;
        int db = dc - (b * 1461);
        int a = ((db / 365) + 1) * 3 / 4;
        int da = db - (a * 365);

        // This is the integer number of full years elapsed since March 1, 4801 BC at 00:00 UTC.
        int y = (g * 400) + (c * 100) + (b * 4) + a;

        // This is the integer number of full months elapsed since the last March 1 at 00:00 UTC.
        int m = (((da * 5) + 308) / 153) - 2;

        // This is the number of days elapsed since day 1 of the month at 00:00 UTC.
        int d = da - ((m + 4) * 153 / 5) + 122;
        int yyyy = y - 4800 + ((m + 2) / 12);
        int mm = ((m + 2) % 12) + 1;
        int dd = d + 1;
        int yyyymmdd = (MathUtil.Abs(yyyy) * 10000) + (mm * 100) + dd;

        return yyyy >= 0 ? yyyymmdd : -yyyymmdd;
    }
}
