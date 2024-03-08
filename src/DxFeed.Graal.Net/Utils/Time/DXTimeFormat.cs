// <copyright file="DXTimeFormat.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using DxFeed.Graal.Net.Native.Utils;

namespace DxFeed.Graal.Net.Utils;

public class DXTimeFormat
{
    private static readonly Lazy<DXTimeFormat> _defaultWithMillis = new(() => DXTimeFormat.Default().WithMillis());

    private static readonly Lazy<DXTimeFormat> _default = new(() =>
        new DXTimeFormat { _timeFormatNative = TimeFormatNative.Default() });

    private static readonly Lazy<DXTimeFormat> _gmt = new(() =>
        new DXTimeFormat { _timeFormatNative = TimeFormatNative.GMT() });

    private TimeFormatNative _timeFormatNative;

    public static DXTimeFormat DefaultWithMillis() => _defaultWithMillis.Value;

    public static DXTimeFormat Default() => _default.Value;

    public static DXTimeFormat GMT() => _gmt.Value;

    public static DXTimeFormat WithTimeZone(string timeZone) =>
        new() { _timeFormatNative = TimeFormatNative.WithTimeZone(timeZone) };

    public DXTimeFormat WithMillis()
    {
        var existingTimeFormat = _timeFormatNative;
        return new DXTimeFormat() { _timeFormatNative = existingTimeFormat.WithMillis() };
    }

    public DXTimeFormat AsFullIso()
    {
        var existingTimeFormat = _timeFormatNative;
        return new DXTimeFormat { _timeFormatNative = existingTimeFormat.AsFullIso() };
    }

    public DateTimeOffset Parse(string value) => _timeFormatNative.Parse(value);

    public string Format(long value) => _timeFormatNative.Format(value);
}
