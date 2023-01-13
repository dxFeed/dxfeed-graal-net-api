// <copyright file="TimeFormat.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Globalization;

namespace DxFeed.Graal.Net.Utils;

/// <summary>
/// Utility class for parsing and formatting dates and times in ISO-compatible format.
/// </summary>
public class TimeFormat
{
    /// <summary>
    /// Default format string.
    /// </summary>
    private const string DefaultFormat = "yyyyMMdd-HHmmss";

    /// <summary>
    /// Format string for only date representation.
    /// </summary>
    private const string OnlyDateFormat = "yyyyMMdd";

    /// <summary>
    /// Format string with milliseconds.
    /// </summary>
    private const string WithMillisFormat = ".fff";

    /// <summary>
    /// Format string with TimeZone.
    /// </summary>
    private const string WithTimeZoneFormat = "zzz";

    /// <summary>
    /// Full ISO format string.
    /// </summary>
    private const string FullIsoFormat = "o";

    /// <summary>
    /// Lazy initialization of the <see cref="TimeFormat"/> with Local Time Zone.
    /// </summary>
    private static readonly Lazy<TimeFormat> LocalTimeZoneFormat = new(() =>
        Create(() => TimeZoneInfo.Local));

    /// <summary>
    /// Lazy initialization of the <see cref="TimeFormat"/> with UTC Time Zone.
    /// For UTC we can pass null for better performance because no conversion is required.
    /// </summary>
    private static readonly Lazy<TimeFormat> UtcTimeZoneFormat = new(() =>
        Create(null));

    // The field is made as a func because, you should always access
    // the local time zone through the TimeZoneInfo.Local and TimeZoneInfo.Utc property rather
    // than assigning the local time zone to a TimeZoneInfo object variable.
    // This prevents the TimeZoneInfo object variable from being invalidated
    // by a call to the ClearCachedData method.
    private readonly Func<TimeZoneInfo>? _timeZone;
    private readonly TimeFormat _withMillis;
    private readonly TimeFormat _withTimeZone;
    private readonly TimeFormat _asOnlyDate;
    private readonly TimeFormat _asFullIso;
    private readonly string _formatString;

    private TimeFormat(
        Func<TimeZoneInfo>? timeZone,
        TimeFormat? withMillis,
        TimeFormat? withTimeZone,
        TimeFormat? asOnlyDate,
        TimeFormat? asFullIso)
    {
        _timeZone = timeZone;
        _withMillis = withMillis ?? this;
        _withTimeZone = withTimeZone ?? this;
        _asOnlyDate = asOnlyDate ?? this;
        _asFullIso = asFullIso ?? this;
        _formatString = CreateFormatString();
    }

    /// <summary>
    /// Gets instance <see cref="TimeFormat"/> with <see cref="TimeZoneInfo"/>.<see cref="TimeZoneInfo.Local"/>.
    /// </summary>
    public static TimeFormat LocalTime =>
        LocalTimeZoneFormat.Value;

    /// <summary>
    /// Gets instance <see cref="TimeFormat"/> with <see cref="TimeZoneInfo"/>.<see cref="TimeZoneInfo.Utc"/>.
    /// </summary>
    public static TimeFormat Utc =>
        UtcTimeZoneFormat.Value;

    /// <summary>
    /// Creates a new instance of <see cref="TimeFormat"/>
    /// with a specified <see cref="TimeZoneInfo"/> and <see cref="DefaultFormat"/>.
    /// </summary>
    /// <param name="timeZone">
    /// The specified <see cref="TimeZoneInfo"/>, or <c>null</c> if no conversions are required (for the UTC Time Zone).
    /// </param>
    /// <returns>Returns new instance <see cref="TimeFormat"/>.</returns>
    public static TimeFormat Create(Func<TimeZoneInfo>? timeZone)
    {
        var fullIso = new TimeFormat(timeZone, null, null, null, null);
        var onlyDate = new TimeFormat(timeZone, null, null, null, fullIso);
        var millisTimezone = new TimeFormat(timeZone, null, null, onlyDate, fullIso);
        var timezone = new TimeFormat(timeZone, millisTimezone, null, onlyDate, fullIso);
        var millis = new TimeFormat(timeZone, null, millisTimezone, onlyDate, fullIso);
        return new(timeZone, millis, timezone, onlyDate, fullIso);
    }

    /// <summary>
    /// Adds milliseconds to the current format string.
    /// </summary>
    /// <returns>Returns <see cref="TimeFormat"/>.</returns>
    /// <example>19700101-000000.000.</example>
    public TimeFormat WithMillis() =>
        _withMillis;

    /// <summary>
    /// Adds Time Zone to the current format string.
    /// </summary>
    /// <returns>Returns <see cref="TimeFormat"/>.</returns>
    /// <example>19700101-000000+00:00.</example>
    public TimeFormat WithTimeZone() =>
        _withTimeZone;

    /// <summary>
    /// Sets the current format as Only Date.
    /// </summary>
    /// <returns>Returns <see cref="TimeFormat"/>.</returns>
    /// <example>19700101.</example>
    public TimeFormat AsOnlyDate() =>
        _asOnlyDate;

    /// <summary>
    /// Sets the current format as Full Iso.
    /// </summary>
    /// <returns>Returns <see cref="TimeFormat"/>.</returns>
    /// <example>1970-01-01T00:00:00.0000000+00:00.</example>
    public TimeFormat AsFullIso() =>
        _asFullIso;

    /// <summary>
    /// Converts the value in days since Unix epoch
    /// to its equivalent string representation using the current format <see cref="CreateFormatString()"/>,
    /// current <see cref="TimeZoneInfo"/> and <see cref="CultureInfo.InvariantCulture"/>.
    /// </summary>
    /// <param name="dayId">A number of whole days since Unix epoch of January 1, 1970.</param>
    /// <returns>The string representation of the date.</returns>
    public string FromDayId(int dayId) =>
        Format(DateTimeOffset.UnixEpoch.AddDays(dayId));

    /// <summary>
    /// Converts the value in seconds since Unix epoch
    /// to its equivalent string representation using the current format <see cref="CreateFormatString()"/>,
    /// current <see cref="TimeZoneInfo"/> and <see cref="CultureInfo.InvariantCulture"/>.
    /// </summary>
    /// <param name="timeSeconds">The time measured in seconds since Unix epoch.</param>
    /// <returns>The string representation of the date, or <c>"0"</c> if timeSeconds is <c>0</c>.</returns>
    public string FromSeconds(long timeSeconds) =>
        FromMillis(timeSeconds * 1000);

    /// <summary>
    /// Converts the value in milliseconds since Unix epoch
    /// to its equivalent string representation using the current format <see cref="CreateFormatString()"/>,
    /// current <see cref="TimeZoneInfo"/> and <see cref="CultureInfo.InvariantCulture"/>.
    /// </summary>
    /// <param name="timeMillis">The time measured in milliseconds since Unix epoch.</param>
    /// <returns>The string representation of the date, or <c>"0"</c> if timeMillis is <c>0</c>.</returns>
    public string FromMillis(long timeMillis)
    {
        if (timeMillis == 0)
        {
            return "0";
        }

        var dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(timeMillis);
        return Format(dateTimeOffset);
    }

    /// <summary>
    /// Converts the value of the specified <see cref="DateTimeOffset"/> object
    /// to its equivalent string representation using the current format <see cref="CreateFormatString()"/>,
    /// current <see cref="TimeZoneInfo"/> and <see cref="CultureInfo.InvariantCulture"/>.
    /// </summary>
    /// <param name="dateTimeOffset">The <see cref="DateTimeOffset"/> object.</param>
    /// <returns>The string representation.</returns>
    private string Format(DateTimeOffset dateTimeOffset)
    {
        if (_timeZone != null)
        {
            // If _timeZone not null, we need convert time.
            dateTimeOffset = TimeZoneInfo.ConvertTime(dateTimeOffset, _timeZone());
        }

        // Otherwise, is UTC Time Zone, no conversions required.
        return dateTimeOffset.ToString(_formatString, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Creates a format string for the <see cref="DateTimeOffset"/>.<see cref="DateTimeOffset.ToString()"/> method.
    /// </summary>
    /// <returns>Returns format string.</returns>
    private string CreateFormatString()
    {
        if (_asFullIso == this)
        {
            return FullIsoFormat;
        }

        if (_asOnlyDate == this)
        {
            return OnlyDateFormat;
        }

        var withMillis = _withMillis == this ? WithMillisFormat : string.Empty;
        var withTimeZone = _withTimeZone == this ? WithTimeZoneFormat : string.Empty;
        return $"{DefaultFormat}{withMillis}{withTimeZone}";
    }
}
