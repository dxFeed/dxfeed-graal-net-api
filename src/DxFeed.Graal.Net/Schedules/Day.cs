// <copyright file="Day.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DxFeed.Graal.Net.Native.ErrorHandling;
using DxFeed.Graal.Net.Native.Schedules;

namespace DxFeed.Graal.Net.Schedules;

/// <summary>
/// <b>Day</b> represents a continuous period of time approximately 24 hours long. The day is aligned
/// to the start and the end of business activities of a certain business entity or business process.
/// For example, the day may be aligned to a trading schedule of a particular instrument on an exchange.
/// Thus, different days may start and end at various local times depending on the related trading schedules.
/// <p/>
/// The length of the day depends on the trading schedule and other circumstances. For example, it is possible
/// that day for Monday is longer than 24 hours because it includes part of Sunday; consequently, the day for
/// Sunday will be shorter than 24 hours to avoid overlapping with Monday.
/// <p/>
/// Days do not overlap with each other - rather they form consecutive chain of adjacent periods of time that
/// cover entire time scale. The point on a border line is considered to belong to following day that starts there.
/// <p/>
/// Each day consists of sessions that cover entire duration of the day. If day contains at least one trading
/// session (i.e. session within which trading activity is allowed), then the day is considered trading day.
/// Otherwise the day is considered non-trading day (e.g. weekend or holiday).
/// <p/>
/// Day may contain sessions with zero duration - e.g. indices that post value once a day.
/// Such sessions can be of any appropriate type, trading or non-trading.
/// Day may have zero duration as well - e.g. when all time within it is transferred to other days.
/// </summary>
public class Day
{
    private readonly DayHandle handle;

    internal Day(Schedule schedule, DayHandle handle)
    {
        Schedule = schedule;
        this.handle = handle;
    }

    /// <summary>
    /// Gets schedule to which this day belongs.
    /// </summary>
    public Schedule Schedule { get; }

    /// <summary>
    /// Gets number of this day since January 1, 1970. (that day has identifier of 0
    /// and previous days have negative identifiers).
    /// </summary>
    public int DayId =>
        handle.GetDayId();

    /// <summary>
    /// Gets year, month and day numbers decimally packed in the following way:
    /// <code>YearMonthDay = year * 10000 + month * 100 + day</code>
    /// </summary>
    /// <example>
    /// September 28, 1977 has value 19770928.
    /// </example>
    public int YearMonthDay =>
        handle.GetYearMonthDay();

    /// <summary>
    /// Gets calendar year - i.e. it returns <c>1977</c> for the year <c>1977</c>.
    /// </summary>
    public int Year =>
        handle.GetYear();

    /// <summary>
    /// Gets calendar month number in the year starting with <b>1=January</b> and ending with <b>12=December</b>.
    /// </summary>
    public int MonthOfYear =>
        handle.GetMonthOfYear();

    /// <summary>
    /// Gets ordinal day number in the month starting with <b>1</b> for the first day of month.
    /// </summary>
    public int DayOfMonth =>
        handle.GetDayOfMonth();

    /// <summary>
    /// Gets ordinal day number in the week starting with <b>1=Monday</b> and ending with <b>7=Sunday</b>.
    /// </summary>
    public int DayOfWeek =>
        handle.GetDayOfWeek();

    /// <summary>
    /// Gets a value indicating whether this day is an exchange holiday.
    /// Usually there are no trading takes place on an exchange holiday.
    /// </summary>
    public bool IsHoliday =>
        handle.IsHoliday();

    /// <summary>
    /// Gets a value indicating whether this day is a short day.
    /// Usually trading stops earlier on a short day.
    /// </summary>
    public bool IsShortDay =>
        handle.IsShortDay();

    /// <summary>
    /// Gets a value indicating whether trading activity is allowed within this day.
    /// Positive result assumes that day has at least one trading session.
    /// </summary>
    public bool IsTrading =>
        handle.IsTrading();

    /// <summary>
    /// Gets start time of this day (inclusive).
    /// </summary>
    public long StartTime =>
        handle.GetStartTime();

    /// <summary>
    /// Gets end time of this day (exclusive).
    /// </summary>
    public long EndTime =>
        handle.GetEndTime();

    /// <summary>
    /// Gets reset time for this day.
    /// Reset of daily data is performed on trading days only, the result has no meaning for non-trading days.
    /// </summary>
    public long ResetTime =>
        handle.GetResetTime();

    /// <summary>
    /// Determines whether a specified time falls within the current day.
    /// </summary>
    /// <param name="time">The time to check</param>
    /// <returns>
    /// <c>true</c> if the specified time is within the start and end time of the day; otherwise, <c>false</c>.
    /// </returns>
    public bool ContainsTime(long time) =>
        handle.ContainsTime(time);

    /// <summary>
    /// Gets list of sessions that constitute this day.
    /// This method will throw <see cref="JavaException"/> if specified time
    /// falls outside of valid date range from 0001-01-02 to 9999-12-30.
    /// The list is ordered according to natural order of sessions - how they occur one after another.
    /// </summary>
    /// <returns>The list of sessions.</returns>
    public IEnumerable<Session> GetSessions()
    {
        var handles = handle.GetSessions();
        return handles.Select(h => new Session(Schedule, h)).ToList();
    }

    /// <summary>
    /// Gets session belonging to this day that contains specified time.
    /// </summary>
    /// <param name="time">The time to search for.</param>
    /// <returns>The session that contains specified time.</returns>
    /// <exception cref="JavaException">If no such session was found within this day.</exception>
    public Session GetSessionByTime(long time) =>
        new(Schedule, handle.GetSessionByTime(time));

    /// <summary>
    /// Attempts to find the first session belonging to this day accepted by specified filter.
    /// This method does not cross the day boundary.
    /// </summary>
    /// <param name="filter">The filter used to evaluate sessions.</param>
    /// <param name="session">The first session that is accepted by the filter.</param>
    /// <returns>
    /// <c>true</c> if a session meeting the filter criteria is found within the day; otherwise, <c>false</c>.
    /// </returns>
    /// <example>
    /// To find the first trading session of any type:
    /// <code>bool found = TryGetFirstSession(SessionFilter.TRADING, out Session session);</code>
    /// To find the first regular trading session:
    /// <code>bool found = TryGetFirstSession(SessionFilter.REGULAR, out Session session);</code>
    /// </example>
    public bool TryGetFirstSession(SessionFilter filter, [MaybeNullWhen(false)] out Session session)
    {
        session = null;
        var first = this.handle.FindFirstSession(filter.Handle);
        if (first == null)
        {
            return false;
        }

        session = new Session(Schedule, first);
        return true;
    }

    /// <summary>
    /// Gets the first session of the day that is accepted by the specified filter, or null if no such session is found.
    /// </summary>
    /// <param name="filter">The filter used to evaluate sessions.</param>
    /// <returns>
    /// The first session of the day that meets the filter criteria; otherwise, null.
    /// </returns>
    public Session? GetFirstSession(SessionFilter filter) =>
        TryGetFirstSession(filter, out var session) ? session : null;

    /// <summary>
    /// Attempts to find the last session belonging to this day accepted by specified filter.
    /// This method does not cross the day boundary.
    /// </summary>
    /// <param name="filter">The filter used to evaluate sessions.</param>
    /// <param name="session">The first session that is accepted by the filter.</param>
    /// <returns>
    /// <c>true</c> if a session meeting the filter criteria is found within the day; otherwise, <c>false</c>.
    /// </returns>
    /// <example>
    /// To find last trading session of any type use this code:
    /// <code>bool found = TryGetLastSession(SessionFilter.TRADING, out Session session);</code>
    /// To find last regular trading session use this code:
    /// <code>bool found = TryGetLastSession(SessionFilter.REGULAR, out Session session);</code>
    /// </example>
    public bool TryGetLastSession(SessionFilter filter, [MaybeNullWhen(false)] out Session session)
    {
        session = null;
        var first = this.handle.FindLastSession(filter.Handle);
        if (first == null)
        {
            return false;
        }

        session = new Session(Schedule, first);
        return true;
    }

    /// <summary>
    /// Gets the last session of the day that is accepted by the specified filter, or null if no such session is found.
    /// </summary>
    /// <param name="filter">The filter used to evaluate sessions.</param>
    /// <returns>
    /// The last session of the day that meets the filter criteria; otherwise, null.
    /// </returns>
    public Session? GetLastSession(SessionFilter filter) =>
        TryGetLastSession(filter, out var session) ? session : null;

    /// <summary>
    /// Attempts to find the previous day accepted by specified filter.
    /// This method looks for appropriate day up to a year back in time.
    /// </summary>
    /// <param name="filter">The filter used to evaluate each day.</param>
    /// <param name="day">The nearest previous day that is accepted by the filter.</param>
    /// <returns>
    /// <b>true</b> if a suitable day is found; otherwise, <b>false</b>.
    /// </returns>
    public bool TryGetPrevDay(DayFilter filter, [MaybeNullWhen(false)] out Day day)
    {
        day = null;
        var prev = this.handle.FindPrevDay(filter.Handle);
        if (prev == null)
        {
            return false;
        }

        day = new Day(Schedule, prev);
        return true;
    }

    /// <summary>
    /// Gets the previous day that is accepted by the specified filter, if such a day exists.
    /// </summary>
    /// <param name="filter">The filter used to evaluate each day.</param>
    /// <returns>
    /// The nearest previous day that meets the filter criteria; otherwise, <c>null</c> if no suitable day is found
    /// within the search period of up to a year back in time.
    /// </returns>
    /// <remarks>
    /// This method provides a convenient way to access the previous day without handling the output parameter directly.
    /// </remarks>
    public Day? GetPrevDay(DayFilter filter) =>
        TryGetPrevDay(filter, out var day) ? day : null;

    /// <summary>
    /// Attempts to find the following day accepted by specified filter.
    /// This method looks for appropriate day up to a year in the future.
    /// </summary>
    /// <param name="filter">The filter used to evaluate each day.</param>
    /// <param name="day">The nearest following day that is accepted by the filter.</param>
    /// <returns>
    /// <b>true</b> if a suitable day is found; otherwise, <b>false</b>.
    /// </returns>
    public bool TryGetNextDay(DayFilter filter, [MaybeNullWhen(false)] out Day day)
    {
        day = null;
        var next = this.handle.FindNextDay(filter.Handle);
        if (next == null)
        {
            return false;
        }

        day = new Day(Schedule, next);
        return true;
    }

    /// <summary>
    /// Gets the following day that is accepted by the specified filter, if such a day exists.
    /// </summary>
    /// <param name="filter">The filter used to determine the suitability of days.</param>
    /// <returns>
    /// The nearest following day that meets the filter criteria; otherwise, <c>null</c> if no suitable day is found
    /// within the search period of up to a year in the future.
    /// </returns>
    /// <remarks>
    /// This method provides a convenient way to access the next day without handling the output parameter directly.
    /// </remarks>
    public Day? GetNextDay(DayFilter filter) =>
        TryGetNextDay(filter, out var day) ? day : null;

    /// <summary>
    /// Gets a hash code value for this object.
    /// </summary>
    /// <returns>A hash code value for this object.</returns>
    public override int GetHashCode() =>
        handle.HashCode();

    /// <summary>
    /// Indicates whether some other object is "equal to" this one.
    /// </summary>
    /// <param name="obj"> The object to compare with the current object.</param>
    /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj)
    {
        if (this == obj)
        {
            return true;
        }

        if (obj is not Day day)
        {
            return false;
        }

        return handle.CheckEquals(day.handle);
    }

    /// <summary>
    /// Returns string representation of this object.
    /// </summary>
    /// <returns>The string representation.</returns>
    public override string ToString() =>
        handle.ToString();
}
