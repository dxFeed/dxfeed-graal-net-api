// <copyright file="IQuiteArg.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using CommandLine;

namespace DxFeed.Graal.Net.Tools.Arguments;

public interface IQuiteArg
{
    public const char ShortName = 'q';
    public const string LongName = "quite";

    public const string HelpText =
        """
        Be quiet, event printing is disabled.
        """;

    [Option(ShortName, LongName, HelpText = HelpText, Required = false)]
    public bool IsQuite { get; set; }
}
