// <copyright file="ISourceArg.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using CommandLine;

namespace DxFeed.Graal.Net.Tools.Arguments;

public interface ISourceArg
{
    public const char ShortName = 's';
    public const string LongName = "source";

    public const string HelpText =
        """
        Order source for the indexed subscription (e.g. NTV, ntv).
        """;

    [Option(ShortName, LongName, HelpText = HelpText, Required = false)]
    string? Source { get; set; }
}
