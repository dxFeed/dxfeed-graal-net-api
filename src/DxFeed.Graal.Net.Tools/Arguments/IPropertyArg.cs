// <copyright file="IPropertyArg.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using CommandLine;

namespace DxFeed.Graal.Net.Tools.Arguments;

public interface IPropertyArg
{
    public const char ShortName = 'p';
    public const string LongName = "properties";

    public const string HelpText =
        """
        Comma-separated list of properties (key-value pair separated by an equals sign).
        """;

    [Option(ShortName, LongName, HelpText = HelpText, Required = false)]
    public string? Properties { get; set; }
}
