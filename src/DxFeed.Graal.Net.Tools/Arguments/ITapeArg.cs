// <copyright file="ITapeArg.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using CommandLine;

namespace DxFeed.Graal.Net.Tools.Arguments;

public interface ITapeArg
{
    public const char ShortName = 't';
    public const string LongName = "tape";

    public const string HelpText =
        """
        Tape all incoming data into the spcified file (see "Help Tape").
        """;

    [Option(ShortName, LongName, HelpText = HelpText, Required = false)]
    public string? Tape { get; set; }
}
