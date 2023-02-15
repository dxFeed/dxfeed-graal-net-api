// <copyright file="IPropertyArg.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Collections.Generic;
using CommandLine;

namespace DxFeed.Graal.Net.Tools.Arguments;

public interface IPropertyArg
{
    public const char ShortName = 'p';
    public const string LongName = "properties";
    public const bool IsRequired = false;

    public const string HelpText = @"
Comma-separated list of properties.
Examples:
    -p dxfeed.aggregationPeriod=5s - to set the aggregation period.
    -p dxfeed.wildcard.enable=true - to enable the wildcard symbol.";

    [Option(ShortName, LongName, Required = IsRequired, HelpText = HelpText)]
    public string? Properties { get; set; }
}
