// <copyright file="IFromTimeArg.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using CommandLine;

namespace DxFeed.Graal.Net.Tools.Arguments;

public interface IFromTimeArg
{
    public const char ShortName = 't';
    public const string LongName = "from-time";
    public const bool IsRequired = false;

    public const string HelpText = @"
From-time for history subscription in standard formats.
Dates and times can be parsed from one of the following forms:

    <value-in-milliseconds>
This is a standart java representation of time as a single long number. Value of msecs is measured from 1970-01-01. Here
the value should be positive and have at least 9 digits (otherwise it could not be distinguished from date in format
'yyyyMMdd'). Each date since 1970-01-03 can be represented in this form.

    <date>[<time>][<timezone>]
This is the most formal way to represent a date-time. If time is missing it is supposed to be '00:00:00'.
If no time zone is specified in the parsed string, the string is assumed to denote a local time.

Here <date> is one of:
    yyyy-MM-dd
    yyyyMMdd

<time> is one of:
    HH:mm:ss[.sss]
    HHmmss[.sss]

<timezone> is one of:
    [+-]HH:mm
    [+-]HHmm
    Z for UTC.

Examples of valid date-times:
    20070101-123456
    20070101-123456.123
    2005-12-31 21:00:00
    2005-12-31 21:00:00.123+03:00
    2005-12-31 21:00:00.123+0400
    2007-11-02Z
    123456789 // value-in-milliseconds
";

    [Option(ShortName, LongName, Required = IsRequired, HelpText = HelpText)]
    public string? FromTime { get; set; }
}
