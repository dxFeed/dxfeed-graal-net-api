// <copyright file="TimeFormat.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Globalization;

namespace DxFeed.Graal.Net.Utils;

/// <summary>
/// Provides functionality to convert UTC time to a string with the specified format.
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
    /// <c>True</c> if the time converts to Local Time Zone; otherwise is UTC.
    /// </summary>
    private readonly bool _isLocalTime;

    private bool _withMillis;
    private bool _withTimeZone;
    private bool _isOnlyDate;
    private bool _asFullIso;

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeFormat"/> class.
    /// </summary>
    /// <param name="isLocalTime"><c>True</c> if the time converts to Local Time Zone; otherwise is UTC.</param>
    // ToDo Avoid creating new TimeFormat instances for same format.
    private TimeFormat(bool isLocalTime) =>
        _isLocalTime = isLocalTime;

    /// <summary>
    /// Gets new instance <see cref="TimeFormat"/> with UTC.
    /// </summary>
    // ToDo Avoid creating new TimeFormat instances for same format.
    public static TimeFormat Utc =>
        new(isLocalTime: false);

    /// <summary>
    /// Gets new instance <see cref="TimeFormat"/> with Local Time Zone.
    /// </summary>
    public static TimeFormat LocalTime =>
        new(isLocalTime: true);

    /// <summary>
    /// Adds milliseconds to the current format string.
    /// Example: 19700101-000000.000.
    /// </summary>
    /// <returns>Returns <see cref="TimeFormat"/>.</returns>
    public TimeFormat WithMillis()
    {
        _withMillis = true;
        return this;
    }

    /// <summary>
    /// Adds Time Zone to the current format string.
    /// Example: 19700101-000000+00:00.
    /// </summary>
    /// <returns>Returns <see cref="TimeFormat"/>.</returns>
    public TimeFormat WithTimeZone()
    {
        _withTimeZone = true;
        return this;
    }

    /// <summary>
    /// Sets the current format as Only Date.
    /// Example: 19700101.
    /// </summary>
    /// <returns>Returns <see cref="TimeFormat"/>.</returns>
    public TimeFormat OnlyDate()
    {
        _isOnlyDate = true;
        return this;
    }

    /// <summary>
    /// Sets the current format as Full Iso.
    /// Example: 1970-01-01T00:00:00.0000000+00:00.
    /// </summary>
    /// <returns>Returns <see cref="TimeFormat"/>.</returns>
    public TimeFormat AsFullIso()
    {
        _asFullIso = true;
        return this;
    }

    /// <summary>
    /// Converts the value in days since Unix epoch
    /// to its equivalent string representation using the current format <see cref="GetFormat()"/>,
    /// current time zone (UTC or Local Time Zone) and <see cref="CultureInfo.InvariantCulture"/>.
    /// </summary>
    /// <param name="dayId">A number of whole days since Unix epoch of January 1, 1970.</param>
    /// <returns>The string representation of the date.</returns>
    public string FromDayId(int dayId) =>
        Format(DateTimeOffset.UtcNow.AddDays(dayId));

    /// <summary>
    /// Converts the value in seconds since Unix epoch
    /// to its equivalent string representation using the current format <see cref="GetFormat()"/>,
    /// current time zone (UTC or Local Time Zone) and <see cref="CultureInfo.InvariantCulture"/>.
    /// </summary>
    /// <param name="timeSeconds">The time measured in seconds since Unix epoch.</param>
    /// <returns>The string representation of the date, or "0" if timeSeconds is 0.</returns>
    public string FromSeconds(long timeSeconds) =>
        FromMillis(timeSeconds * 1000);

    /// <summary>
    /// Converts the value in milliseconds since Unix epoch
    /// to its equivalent string representation using the current format <see cref="GetFormat()"/>,
    /// current time zone (UTC or Local Time Zone) and <see cref="CultureInfo.InvariantCulture"/>.
    /// </summary>
    /// <param name="timeMillis">The time measured in milliseconds since Unix epoch.</param>
    /// <returns>The string representation of the date, or "0" if timeMillis is 0.</returns>
    public string FromMillis(long timeMillis)
    {
        if (timeMillis == 0)
        {
            return "0";
        }

        var dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(timeMillis);
        dateTimeOffset = _isLocalTime ? dateTimeOffset.ToLocalTime() : dateTimeOffset;
        return Format(dateTimeOffset);
    }

    /// <summary>
    /// Converts the value of the specified <see cref="DateTimeOffset"/> object
    /// to its equivalent string representation using the current format <see cref="GetFormat()"/>,
    /// current time zone (UTC or Local Time Zone) and <see cref="CultureInfo.InvariantCulture"/>.
    /// </summary>
    /// <param name="dateTimeOffset">The <see cref="DateTimeOffset"/> object.</param>
    /// <returns>The string representation.</returns>
    private string Format(DateTimeOffset dateTimeOffset) =>
        dateTimeOffset.ToString(GetFormat(), CultureInfo.InvariantCulture);

    /// <summary>
    /// Gets current format string.
    /// </summary>
    /// <returns>The current format string.</returns>
    private string GetFormat()
    {
        if (_asFullIso)
        {
            return FullIsoFormat;
        }

        if (_isOnlyDate)
        {
            return OnlyDateFormat;
        }

        var withMillis = _withMillis ? WithMillisFormat : string.Empty;
        var withTimeZone = _withTimeZone ? WithTimeZoneFormat : string.Empty;
        return $"{DefaultFormat}{withMillis}{withTimeZone}";
    }
}
