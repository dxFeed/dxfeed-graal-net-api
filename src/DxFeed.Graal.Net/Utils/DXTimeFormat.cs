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
    private TimeFormatNative _timeFormatNative;

    public static DXTimeFormat Default() => new() { _timeFormatNative = TimeFormatNative.Default() };

    public static DXTimeFormat GMT() => new() { _timeFormatNative = TimeFormatNative.GMT() };

    public static DXTimeFormat WithTimeZone(string timeZone) =>
        new() { _timeFormatNative = TimeFormatNative.WithTimeZone(timeZone) };

    public DateTimeOffset Parse(string value)
    {
        return _timeFormatNative.Parse(value);
    }
}
