// <copyright file="Schedule.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Collections.Generic;
using DxFeed.Graal.Net.Ipf;
using DxFeed.Graal.Net.Native.ErrorHandling;
using DxFeed.Graal.Net.Native.Schedules;

namespace DxFeed.Graal.Net.Schedules;

/// <summary>
/// <b>Schedule</b> class provides API to retrieve and explore trading schedules of different exchanges
/// and different classes of financial instruments. Each instance of schedule covers separate trading schedule
/// of some class of instruments, i.e. NYSE stock trading schedule or CME corn futures trading schedule.
/// Each schedule splits entire time scale into separate <see cref="Day"/> that are aligned to the specific
/// trading hours of covered trading schedule.
/// </summary>
public class Schedule
{
    private readonly ScheduleHandle handle;

    private Schedule(ScheduleHandle handle) =>
        this.handle = handle;

    /// <summary>
    /// Gets default schedule instance for specified instrument profile.
    /// </summary>
    /// <param name="profile">The instrument profile those schedule is requested.</param>
    /// <returns>The default schedule instance for specified instrument profile.</returns>
    public static Schedule GetInstance(InstrumentProfile profile) =>
        new(ScheduleHandle.GetInstance(profile));

    /// <summary>
    /// Gets default schedule instance for specified schedule definition.
    /// </summary>
    /// <param name="scheduleDefinition">The schedule definition of requested schedule.</param>
    /// <returns>The default schedule instance for specified schedule definition.</returns>
    public static Schedule GetInstance(string scheduleDefinition) =>
        new(ScheduleHandle.GetInstance(scheduleDefinition));

    /// <summary>
    /// Gets schedule instance for specified instrument profile and trading venue.
    /// </summary>
    /// <param name="profile">The instrument profile those schedule is requested.</param>
    /// <param name="venue">The trading venue those schedule is requested.</param>
    /// <returns>The schedule instance for specified instrument profile and trading venue.</returns>
    public static Schedule GetInstance(InstrumentProfile profile, string venue) =>
        new(ScheduleHandle.GetInstance(profile, venue));

    /// <summary>
    /// Gets trading venues for specified instrument profile.
    /// </summary>
    /// <param name="profile">The instrument profile those trading venues are requested.</param>
    /// <returns>The trading venues for specified instrument profile.</returns>
    public static List<string> GetTradingVenues(InstrumentProfile profile) =>
        ScheduleHandle.GetTradingVenues(profile);

    /// <summary>
    /// Downloads defaults using specified download config and optionally start periodic download.
    /// The specified config can be one of the following:
    /// <ul>
    /// <li>"" or null - stop periodic download</li>
    /// <li>URL   - download once from specified URL and stop periodic download</li>
    /// <li>URL,period   - start periodic download from specified URL</li>
    /// <li>"auto"   - start periodic download from default location</li>
    /// </ul>
    /// </summary>
    /// <param name="downloadConfig">The download config.</param>
    public static void DownloadDefaults(string downloadConfig) =>
        ScheduleHandle.DownloadDefaults(downloadConfig);

    /// <summary>
    /// Sets shared defaults that are used by individual schedule instances.
    /// </summary>
    /// <param name="data">The content of default data.</param>
    public static void SetDefaults(byte[] data) =>
        ScheduleHandle.SetDefaults(data);

    /// <summary>
    /// Gets session that contains specified time.
    /// This method will throw <see cref="JavaException"/> if specified time
    /// falls outside of valid date range from 0001-01-02 to 9999-12-30.
    /// </summary>
    /// <param name="time">The time to search for.</param>
    /// <returns>The session that contains specified time.</returns>
    public Session GetSessionByTime(long time) =>
        new(this, handle.GetSessionByTime(time));

    /// <summary>
    /// Gets day that contains specified time.
    /// This method will throw <see cref="JavaException"/> if specified time
    /// falls outside of valid date range from 0001-01-02 to 9999-12-30.
    /// </summary>
    /// <param name="time">The time to search for.</param>
    /// <returns>The day that contains specified time.</returns>
    public Day GetDayByTime(long time) =>
        new(this, handle.GetDayByTime(time));

    /// <summary>
    /// Gets day for specified day identifier.
    /// This method will throw <see cref="JavaException"/> if specified day identifier
    /// falls outside of valid date range from 0001-01-02 to 9999-12-30.
    /// </summary>
    /// <param name="dayId">The day identifier to search for.</param>
    /// <returns>The day for specified day identifier.</returns>
    public Day GetDayById(int dayId) =>
        new(this, handle.GetDayById(dayId));

    /// <summary>
    /// Gets day for specified year, month and day numbers.
    /// Year, month, and day numbers shall be decimally packed in the following way:
    /// <code>YearMonthDay = year * 10000 + month * 100 + day</code>
    /// For example, September 28, 1977 has value 19770928.
    /// <p/>
    /// If specified day does not exist then this method returns day with
    /// the lowest valid YearMonthDay that is greater than specified one.
    /// This method will throw <see cref="JavaException"/> if specified year, month and day numbers
    /// fall outside of valid date range from 0001-01-02 to 9999-12-30.
    /// </summary>
    /// <param name="yearMonthDay">The year, month and day numbers to search for.</param>
    /// <returns>The day for specified year, month and day numbers.</returns>
    public Day GetDayByYearMonthDay(int yearMonthDay) =>
        new(this, handle.GetDayByYearMonthDay(yearMonthDay));

    /// <summary>
    /// Tries to find the session nearest to the specified time that is accepted by the specified filter.
    /// </summary>
    /// <remarks>
    /// This method will return <c>null</c> if no sessions acceptable by the specified filter are found within one year.
    /// To find the nearest trading session of any type, use the following code:
    /// <code>
    /// bool found = TryGetNearestSessionByTime(time, SessionFilter.TRADING, out Session session);
    /// </code>
    /// To find the nearest regular trading session, use this code:
    /// <code>
    /// bool found = TryGetNearestSessionByTime(time, SessionFilter.REGULAR, out Session session);
    /// </code>
    /// </remarks>
    /// <param name="time">The time to search for, expressed as a long integer.</param>
    /// <param name="filter">The filter used to test the sessions.</param>
    /// <param name="session">When this method returns, contains the session that is nearest to the specified time and accepted by the specified filter, if such a session is found; otherwise, null.</param>
    /// <returns><c>true</c> if a session meeting the filter criteria is found; otherwise, <c>false</c>.</returns>
    /// <exception cref="JavaException">Thrown if the specified time falls outside of the valid date range from 0001-01-02 to 9999-12-30.</exception>
    public bool TryGetNearestSessionByTime(long time, SessionFilter filter, out Session session)
    {
        session = null!;
        var prev = handle.FindNearestSessionByTime(time, filter.Handle);
        if (prev == null)
        {
            return false;
        }

        session = new Session(this, prev);
        return true;
    }

    /// <summary>
    /// Gets the session nearest to the specified time that is accepted by the specified filter, or null if no such session is found.
    /// </summary>
    /// <param name="time">The time to search for, expressed as a long integer.</param>
    /// <param name="filter">The filter used to evaluate the sessions.</param>
    /// <returns>The nearest session to the specified time that meets the filter criteria; otherwise, null.</returns>
    public Session? GetNearestSessionByTime(long time, SessionFilter filter) =>
        TryGetNearestSessionByTime(time, filter, out var session) ? session : null;

    /// <summary>
    /// Gets name of this schedule.
    /// </summary>
    /// <returns>The name.</returns>
    public string GetName() =>
        handle.GetName();

    /// <summary>
    /// Gets time zone in which this schedule is defined.
    /// </summary>
    /// <returns>The time zone.</returns>
    public string GetTimeZone() =>
        handle.GetTimeZone();
}
