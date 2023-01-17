// <copyright file="TimeFormat.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using static System.Globalization.CultureInfo;

namespace DxFeed.Graal.Net.Utils;

/// <summary>
/// Utility class for parsing and formatting dates and times in ISO-compatible format.
/// </summary>
public class TimeFormat
{
    /// <summary>
    /// Default format string.
    /// Example:
    /// 20090615-134530.
    /// </summary>
    private const string DefaultFormat = "yyyyMMdd-HHmmss";

    /// <summary>
    /// Format string for only date representation.
    /// Example:
    /// 20090615.
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
    /// Example:
    /// 2009-06-15T13:45:30.0000000Z.
    /// </summary>
    private const string FullIsoFormat = "o";

    /// <summary>
    /// Sortable date/time format string.
    /// Example:
    /// 2009-06-15T13:45:30.
    /// </summary>
    private const string SortableFormat = "s";

    /// <summary>
    /// Universal format string.
    /// Example:
    /// 2009-06-15 13:45:30Z.
    /// </summary>
    private const string UniversalFormat = "u";

    /// <summary>
    /// List all available format.
    /// </summary>
    private static readonly List<string> AvailableFormats = new()
    {
        DefaultFormat,
        $"{DefaultFormat}{WithMillisFormat}",
        $"{DefaultFormat}{WithTimeZoneFormat}",
        $"{DefaultFormat}Z",
        $"{DefaultFormat}{WithMillisFormat}{WithTimeZoneFormat}",
        $"{DefaultFormat}{WithMillisFormat}Z",
        OnlyDateFormat,
        $"{OnlyDateFormat}{WithTimeZoneFormat}",
        $"{OnlyDateFormat}Z",
        FullIsoFormat,
        SortableFormat,
        UniversalFormat,
    };

    /// <summary>
    /// Lazy initialization of the <see cref="TimeFormat"/> with Local Time Zone.
    /// </summary>
    private static readonly Lazy<TimeFormat> LocalTimeZoneFormat = new(() =>
        Create(() => TimeZoneInfo.Local));

    /// <summary>
    /// Lazy initialization of the <see cref="TimeFormat"/> with UTC Time Zone.
    /// </summary>
    private static readonly Lazy<TimeFormat> UtcTimeZoneFormat = new(() =>
        Create(() => TimeZoneInfo.Utc));

    // The field is made as a func because, you should always access
    // the local time zone through the TimeZoneInfo.Local (same for TimeZoneInfo.Utc) property rather
    // than assigning the local time zone to a TimeZoneInfo object variable.
    // This prevents the TimeZoneInfo object variable from being invalidated
    // by a call to the ClearCachedData method.
    private readonly Func<TimeZoneInfo> _timeZone;
    private readonly TimeFormat _withMillis;
    private readonly TimeFormat _withTimeZone;
    private readonly TimeFormat _asOnlyDate;
    private readonly TimeFormat _asFullIso;
    private readonly string _formatString;

    private TimeFormat(
        Func<TimeZoneInfo> timeZone,
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
    public static TimeFormat Local =>
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
    /// <param name="timeZone"> The specified <see cref="TimeZoneInfo"/>. </param>
    /// <returns>Returns new instance <see cref="TimeFormat"/>.</returns>
    public static TimeFormat Create(TimeZoneInfo timeZone) =>
        Create(() => timeZone);

    /// <summary>
    /// Creates a new instance of <see cref="TimeFormat"/>
    /// with a specified <see cref="TimeZoneInfo"/> encapsulates
    /// in <see cref="Func{TResult}"/> and <see cref="DefaultFormat"/>.
    /// The <see cref="TimeZoneInfo"/>  is made as a func because, you should always access
    /// the local time zone through the TimeZoneInfo.Local (same for TimeZoneInfo.Utc) property rather
    /// than assigning the local time zone to a TimeZoneInfo object variable.
    /// </summary>
    /// <param name="timeZone">
    /// The specified <see cref="TimeZoneInfo"/> encapsulates in <see cref="Func{TResult}"/>.
    /// </param>
    /// <returns>Returns new instance <see cref="TimeFormat"/>.</returns>
    public static TimeFormat Create(Func<TimeZoneInfo> timeZone)
    {
        // The idea is that the different formats are already created in advance
        // and the corresponding "c" methods return ready-made objects.
        var fullIso = new TimeFormat(timeZone, null, null, null, null);
        var onlyDate = new TimeFormat(timeZone, null, null, null, fullIso);
        var millisTimezone = new TimeFormat(timeZone, null, null, onlyDate, fullIso);
        var timezone = new TimeFormat(timeZone, millisTimezone, null, onlyDate, fullIso);
        var millis = new TimeFormat(timeZone, null, millisTimezone, onlyDate, fullIso);
        return new(timeZone, millis, timezone, onlyDate, fullIso);
    }

    /// <summary>
    /// Converts the specified string representation of a date and time
    /// to its <see cref="DateTimeOffset"/> in current <see cref="TimeZoneInfo"/>
    /// and <see cref="InvariantCulture"/>.
    /// If no time zone is specified in the parsed string, the string is assumed to denote a local time,
    /// and converted to current <see cref="TimeZoneInfo"/>.
    /// <br/>
    /// It accepts the following formats.
    /// <br/>
    /// <ul>
    /// <li>
    /// <b><tt>0</tt></b> is parsed as zero time in UTC.
    /// </li>
    /// <li>
    /// <b><tt>&lt;long-value-in-milliseconds&gt;</tt></b>
    /// The value in milliseconds since Unix epoch since Unix epoch.
    /// It should be positive and have at least 9 digits
    /// (otherwise it could not be distinguished from date in format <tt>'yyyymmdd'</tt>).
    /// Each date since 1970-01-03 can be represented in this form.
    /// </li>
    /// <li>
    /// <b><tt>&lt;date&gt;[&lt;time&gt;][&lt;timezone&gt;]</tt></b>
    /// If time is missing it is supposed to be <tt>'00:00:00'</tt>.
    /// </li>
    /// </ul>
    /// <ul>
    /// <li>
    /// <b>&lt;date&gt;</b> is one of:
    /// <ul>
    ///     <li><b><tt>yyyy-MM-dd</tt></b></li>
    ///     <li><b><tt>yyyyMMdd</tt></b></li>
    /// </ul>
    /// </li>
    /// <li>
    /// <b>&lt;time&gt;</b> is one of:
    /// <ul>
    ///     <li><b><tt>HH:mm:ss[.sss]</tt></b></li>
    ///     <li><b><tt>HHmmss[.sss]</tt></b></li>
    /// </ul>
    /// </li>
    /// <li>
    /// <b>&lt;timezone&gt;</b> is one of:
    /// <ul>
    ///     <li><b><tt>[+-]HH:mm</tt></b></li>
    ///     <li><b><tt>[+-]HHmm</tt></b></li>
    ///     <li><b><tt>Z</tt></b> for UTC.</li>
    /// </ul>
    /// </li>
    /// </ul>
    /// </summary>
    /// <param name="value">The input value for parse.</param>
    /// <returns>Returns <see cref="DateTimeOffset"/> parsed from input value.</returns>
    /// <exception cref="ArgumentException">If input value has wrong format.</exception>
    public DateTimeOffset Parse(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException("Cannot parse date-time from empty or null string", nameof(value));
        }

        value = value.Trim();

        // Fast path for 0 ms since Unix epoch.
        if (value.Equals("0", StringComparison.Ordinal))
        {
            return ConvertDateTimeToCurrentTimeZone(DateTimeOffset.FromUnixTimeMilliseconds(0));
        }

        var styles = DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal;
        DateTimeOffset dateTimeOffset;

        // Try to parse for all available format strings.
        foreach (var format in AvailableFormats)
        {
            if (DateTimeOffset.TryParseExact(value, format, CurrentCulture, styles, out dateTimeOffset))
            {
                return ConvertDateTimeToCurrentTimeZone(dateTimeOffset);
            }
        }

        // Try to parse for builtin formats string.
        if (DateTimeOffset.TryParse(value, CurrentCulture, styles, out dateTimeOffset))
        {
            return ConvertDateTimeToCurrentTimeZone(dateTimeOffset);
        }

        // Try parse as milliseconds since Unix epoch.
        if (long.TryParse(value, out var milliseconds))
        {
            return ConvertDateTimeToCurrentTimeZone(DateTimeOffset.FromUnixTimeMilliseconds(milliseconds));
        }

        throw new ArgumentException($"Cannot parse date-time from input string: \"{value}\"");
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
    /// Converts the value in seconds since Unix epoch
    /// to its equivalent string representation using the current format <see cref="CreateFormatString()"/>,
    /// current <see cref="TimeZoneInfo"/> and <see cref="InvariantCulture"/>.
    /// </summary>
    /// <param name="timeSeconds">The time measured in seconds since Unix epoch.</param>
    /// <returns>The string representation of the date, or <c>"0"</c> if timeSeconds is <c>0</c>.</returns>
    public string FormatFromSeconds(long timeSeconds) =>
        FormatFromMillis(timeSeconds * 1000);

    /// <summary>
    /// Converts the value in milliseconds since Unix epoch
    /// to its equivalent string representation using the current format <see cref="CreateFormatString()"/>,
    /// current <see cref="TimeZoneInfo"/> and <see cref="InvariantCulture"/>.
    /// </summary>
    /// <param name="timeMillis">The time measured in milliseconds since Unix epoch.</param>
    /// <returns>The string representation of the date, or <c>"0"</c> if timeMillis is <c>0</c>.</returns>
    public string FormatFromMillis(long timeMillis)
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
    /// current <see cref="TimeZoneInfo"/> and <see cref="InvariantCulture"/>.
    /// </summary>
    /// <param name="dateTimeOffset">The <see cref="DateTimeOffset"/> object.</param>
    /// <returns>The string representation.</returns>
    public string Format(DateTimeOffset dateTimeOffset)
    {
        dateTimeOffset = ConvertDateTimeToCurrentTimeZone(dateTimeOffset);
        return dateTimeOffset.ToString(_formatString, InvariantCulture);
    }

    /// <summary>
    /// Converts specified <see cref="DateTimeOffset"/> to <see cref="DateTimeOffset"/> in current Time Zone.
    /// </summary>
    /// <param name="dateTimeOffset">The specified <see cref="DateTimeOffset"/>.</param>
    /// <returns>Returns <see cref="DateTimeOffset"/> in current Time Zone.</returns>
    private DateTimeOffset ConvertDateTimeToCurrentTimeZone(DateTimeOffset dateTimeOffset) =>
        TimeZoneInfo.ConvertTime(dateTimeOffset, _timeZone());

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
