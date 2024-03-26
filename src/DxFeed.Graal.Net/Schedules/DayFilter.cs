// <copyright file="DayFilter.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using DxFeed.Graal.Net.Native.Schedules;

namespace DxFeed.Graal.Net.Schedules;

/// <summary>
/// A filter for days used by various search methods.
/// This class provides predefined filters for certain Day attributes,
/// although users can create their own filters to suit their needs.
/// <p/>
/// Please note that days can be either trading or non-trading, and this distinction can be
/// either based on rules (e.g. weekends) or dictated by special occasions (e.g. holidays).
/// Different filters treat this distinction differently - some accept only trading days,
/// some only non-trading, and some ignore type of day altogether.
/// </summary>
public class DayFilter
{
    /// <summary>
    /// Accepts any day - useful for pure calendar navigation.
    /// </summary>
    public static readonly DayFilter ANY = new(0);

    /// <summary>
    /// Accepts trading days only - those with <code>({@link Day#isTrading()} == true)</code>.
    /// </summary>
    public static readonly DayFilter TRADING = new(1);

    /// <summary>
    /// Accepts non-trading days only - those with <see cref="Day"/>.<see cref="Day.IsTrading"/> == <c>false</c>.
    /// </summary>
    public static readonly DayFilter NON_TRADING = new(2);

    /// <summary>
    /// Accepts holidays only - those with <see cref="Day"/>.<see cref="Day.IsHoliday"/> == <c>false</c>.
    /// </summary>
    public static readonly DayFilter HOLIDAY = new(3);

    /// <summary>
    ///  Accepts short days only - those with <see cref="Day"/>.<see cref="Day.IsShortDay"/> == <c>false</c>.
    /// </summary>
    public static readonly DayFilter SHORT_DAY = new(4);

    /// <summary>
    /// Accepts Mondays only - those with <see cref="Day"/>.<see cref="Day.DayOfWeek"/> == <c>1</c>.
    /// </summary>
    public static readonly DayFilter MONDAY = new(5);

    /// <summary>
    /// Accepts Tuesdays only - those with <see cref="Day"/>.<see cref="Day.DayOfWeek"/> == <c>2</c>.
    /// </summary>
    public static readonly DayFilter TUESDAY = new(6);

    /// <summary>
    /// Accepts Wednesdays only - those with <see cref="Day"/>.<see cref="Day.DayOfWeek"/> == <c>3</c>.
    /// </summary>
    public static readonly DayFilter WEDNESDAY = new(7);

    /// <summary>
    /// Accepts Thursdays only - those with <see cref="Day"/>.<see cref="Day.DayOfWeek"/> == <c>4</c>.
    /// </summary>
    public static readonly DayFilter THURSDAY = new(8);

    /// <summary>
    /// Accepts Fridays only - those with <see cref="Day"/>.<see cref="Day.DayOfWeek"/> == <c>5</c>.
    /// </summary>
    public static readonly DayFilter FRIDAY = new(9);

    /// <summary>
    /// Accepts Saturdays only - those with <see cref="Day"/>.<see cref="Day.DayOfWeek"/> == <c>6</c>.
    /// </summary>
    public static readonly DayFilter SATURDAY = new(10);

    /// <summary>
    /// Accepts Sundays only - those with <see cref="Day"/>.<see cref="Day.DayOfWeek"/> == <c>7</c>.
    /// </summary>
    public static readonly DayFilter SUNDAY = new(11);

    /// <summary>
    /// Accepts week-days only - those with <see cref="Day"/>.<see cref="Day.DayOfWeek"/> &lt;= <c>5</c>.
    /// </summary>
    public static readonly DayFilter WEEK_DAY = new(12);

    /// <summary>
    /// Accepts weekends only - those with <see cref="Day"/>.<see cref="Day.DayOfWeek"/>  &gt;= <c>6</c>.
    /// </summary>
    public static readonly DayFilter WEEK_END = new(13);

    private DayFilter(int id) =>
        Handle = DayFilterHandle.GetInstance(id);

    internal DayFilterHandle Handle { get; }
}
