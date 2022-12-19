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
From-time for the time-series subscription.
From-time is measured in milliseconds between the current time and midnight on January 1, 1970 UTC.";

    [Option(ShortName, LongName, Required = IsRequired, HelpText = HelpText)]
    public long? FromTime { get; set; }
}
