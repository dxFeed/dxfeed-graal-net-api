// <copyright file="IForceStreamArg.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using CommandLine;

namespace DxFeed.Graal.Net.Tools.Arguments;

public interface IForceStreamArg
{
    public const string LongName = "force-stream";

    public const string HelpText =
        """
        "Enforces a streaming contract for subscription. The StreamFeed role is used instead of Feed."
        """;

    [Option(LongName, HelpText = HelpText, Required = false)]
    public bool ForceStream { get; set; }
}
