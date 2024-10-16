// <copyright file="Session.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using DxFeed.Graal.Net.Native.Schedules;

namespace DxFeed.Graal.Net.Schedules;

/// <summary>
/// <b>Session</b> represents a continuous period of time during which apply same rules regarding trading activity.
/// For example, <code>regular trading session</code> is a period of time consisting of one day of business activities
/// in a financial market, from the opening bell to the closing bell, when regular trading occurs.
/// <p/>
/// Sessions can be either <b>trading</b> or <b>non-trading</b>, with different sets of rules and reasons to exist.
/// Sessions do not overlap with each other - rather they form consecutive chain of adjacent periods of time that
/// cover entire time scale. The point on a border line is considered to belong to following session that starts there.
/// Each session completely fits inside a certain day. Day may contain sessions with zero duration - e.g. indices
/// that post value once a day. Such sessions can be of any appropriate type, trading or non-trading.
/// </summary>
public class Session
{
    private readonly Schedule schedule;
    private readonly SessionHandle handle;
    private readonly Lazy<Day> day;

    internal Session(Schedule schedule, SessionHandle handle)
    {
        this.schedule = schedule;
        this.handle = handle;
        day = new(() => new(schedule, handle.GetDay()));
    }

    /// <summary>
    /// Gets day to which this session belongs.
    /// </summary>
    public Day Day =>
        day.Value;

    /// <summary>
    /// Gets type of this session.
    /// </summary>
    public SessionType Type =>
        handle.GetSessionType();

    /// <summary>
    /// Gets a value indicating whether trading activity is allowed within this session.
    /// Some sessions may have zero duration - e.g. indices that post value once a day.
    /// Such sessions can be of any appropriate type, trading or non-trading.
    /// </summary>
    public bool IsTrading =>
        handle.IsTrading();

    /// <summary>
    /// Gets a value indicating whether this session has zero duration.
    /// Empty sessions can be used for indices that post value once a day or for convenience.
    /// Such sessions can be of any appropriate type, trading or non-trading.
    /// </summary>
    public bool IsEmpty =>
        handle.IsEmpty();

    /// <summary>
    /// Gets start time of this session (inclusive).
    /// For normal sessions the start time is less than the end time, for empty sessions they are equal.
    /// </summary>
    public long StartTime =>
        handle.GetStartTime();

    /// <summary>
    /// Gets end time of this session (exclusive).
    /// For normal sessions the end time is greater than the start time, for empty sessions they are equal.
    /// </summary>
    public long EndTime =>
        handle.GetEndTime();

    /// <summary>
    /// Determines whether a specified time belongs to this session.
    /// </summary>
    /// <param name="time">The time to check</param>
    /// <returns>
    /// <c>true</c> if the specified time is belongs to this session; otherwise, <c>false</c>.
    /// </returns>
    public bool ContainsTime(long time) =>
        handle.ContainsTime(time);

    /// <summary>
    /// Attempts to find the previous session accepted by specified filter.
    /// This method may cross the day boundary and return appropriate session from
    /// previous days - up to a year back in time.
    /// </summary>
    /// <param name="filter">The filter used to evaluate sessions.</param>
    /// <param name="session">The nearest previous session that is accepted by the filter.</param>
    /// <returns>
    /// <c>true</c> if a session meeting the filter criteria is found; otherwise, <c>false</c>.
    /// </returns>
    /// <example>
    /// To find previous trading session of any type use this code:
    /// <code>bool found = TryGetPrevSession(SessionFilter.TRADING, out Session session);</code>
    /// To find previous regular trading session use this code:
    /// <code>bool found = TryGetPrevSession(SessionFilter.REGULAR, out Session session);</code>
    /// </example>
    public bool TryGetPrevSession(SessionFilter filter, out Session session)
    {
        session = null!;
        var prev = handle.FindPrevSession(filter.Handle);
        if (prev == null || prev.IsInvalid)
        {
            return false;
        }

        session = new Session(schedule, prev);
        return true;
    }

    /// <summary>
    /// Gets the previous session that is accepted by the specified filter, if such a session exists.
    /// </summary>
    /// <param name="filter">The filter used to determine the suitability of sessions.</param>
    /// <returns>
    /// The nearest previous session that meets the filter criteria; otherwise, <c>null</c> if no suitable session
    /// is found within the search period of up to a year back in time.
    /// </returns>
    public Session? GetPrevSession(SessionFilter filter) =>
        TryGetPrevSession(filter, out var session) ? session : null;

    /// <summary>
    /// Attempts to find the following session accepted by specified filter.
    /// This method may cross the day boundary and return appropriate session from
    /// following days - up to a year in the future.
    /// </summary>
    /// <param name="filter">The filter used to evaluate sessions.</param>
    /// <param name="session">The nearest following session that is accepted by the filter.</param>
    /// <returns>
    /// <c>true</c> if a session meeting the filter criteria is found; otherwise, <c>false</c>.
    /// </returns>
    /// <example>
    /// To find following trading session of any type use this code:
    /// <code>bool found = TryGetNextSession(SessionFilter.TRADING, out Session session);</code>
    /// To find following regular trading session use this code:
    /// <code>bool found = TryGetNextSession(SessionFilter.REGULAR, out Session session);</code>
    /// </example>
    public bool TryGetNextSession(SessionFilter filter, out Session session)
    {
        session = null!;
        var next = handle.FindNextSession(filter.Handle);
        if (next == null || next.IsInvalid)
        {
            return false;
        }

        session = new Session(schedule, next);
        return true;
    }

    /// <summary>
    /// Retrieves the following session that is accepted by the specified filter, if such a session exists.
    /// </summary>
    /// <param name="filter">The filter used to determine the suitability of sessions.</param>
    /// <returns>
    /// The nearest following session that meets the filter criteria; otherwise, <c>null</c> if no suitable session
    /// is found within the search period of up to a year in the future.
    /// </returns>
    public Session? GetNextSession(SessionFilter filter) =>
        TryGetNextSession(filter, out var session) ? session : null;

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

        if (obj is not Session session)
        {
            return false;
        }

        return handle.CheckEquals(session.handle);
    }

    /// <summary>
    /// Returns string representation of this object.
    /// </summary>
    /// <returns>The string representation.</returns>
    public override string ToString() =>
        handle.ToString();
}
