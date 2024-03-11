// <copyright file="DXTimeFormat.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using DxFeed.Graal.Net.Native.Utils;

namespace DxFeed.Graal.Net.Utils;

/// <summary>
/// Utility class for parsing and formatting dates and times in ISO-compatible format.
/// </summary>
public class DXTimeFormat
{
    private static readonly Lazy<DXTimeFormat> _default = new(() =>
        new DXTimeFormat(TimeFormatNative.Default()));

    private static readonly Lazy<DXTimeFormat> _gmt = new(() =>
        new DXTimeFormat(TimeFormatNative.GMT()));

    private readonly TimeFormatNative _timeFormatNative;
    private readonly Lazy<DXTimeFormat> _defaultWithMillis;
    private readonly Lazy<DXTimeFormat> _fullIso;

    private DXTimeFormat(TimeFormatNative timeFormatNative)
    {
        _timeFormatNative = timeFormatNative;
        _defaultWithMillis = new Lazy<DXTimeFormat>(() => new DXTimeFormat(_timeFormatNative.WithMillis()));
        _fullIso = new Lazy<DXTimeFormat>(() => new DXTimeFormat(_timeFormatNative.AsFullIso()));
    }

    /// <summary>
    /// Return DXTimeFormat instance that corresponds to default timezone.
    /// </summary>
    /// <returns>The time format.</returns>
    public static DXTimeFormat Default() => _default.Value;

    /// <summary>
    /// Return DXTimeFormat instance that corresponds to GMT timezone.
    /// </summary>
    /// <returns>The time format.</returns>
    public static DXTimeFormat GMT() => _gmt.Value;

    /// <summary>
    /// Returns DXTimeFormat instance that also includes milliseconds into string when using <see cref="Format"/> format} method.
    /// </summary>
    /// <returns>The time format.</returns>
    public DXTimeFormat WithMillis() => _defaultWithMillis.Value;

    /// <summary>
    /// Returns DXTimeFormat instance that produces full ISO8610 string of "yyyy-MM-dd'T'HH:mm:ss.SSSX".
    /// </summary>
    /// <returns>The time format.</returns>
    public DXTimeFormat AsFullIso() => _fullIso.Value;

    /// <summary>
    /// Reads Date from String.
    /// This method is designed to understand
    /// <a href="http://en.wikipedia.org/wiki/ISO_8601">ISO 8601</a> formatted date and time.
    /// </summary>
    /// <param name="value">The string value to parse.</param>
    /// <returns>The date parsed from value.</returns>
    public DateTimeOffset Parse(string value) => _timeFormatNative.Parse(value);

    /// <summary>
    /// Converts value into string according to the format like `yyyyMMdd-HHmmss`.
    /// When <see cref="WithMillis"/> was used to acquire this DXTimeFormat instance,
    /// the milliseconds are also included as `.sss`.
    /// When value == 0 this method returns string "0".
    /// </summary>
    /// <param name="value">The milliseconds since January 1, 1970, 00:00:00 GMT.</param>
    /// <returns>The string representation of data and time.</returns>
    public string Format(long value) => _timeFormatNative.Format(value);
}
