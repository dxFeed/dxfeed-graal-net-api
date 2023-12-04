// <copyright file="QdsArgs.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using CommandLine;
using DxFeed.Graal.Net.Tools.Arguments;

namespace DxFeed.Graal.Net.Tools.Qds;

public class QdsArgs : IPropertyArg
{
    [Value(0, MetaName = "args", Required = true, HelpText = "Represents the arguments passed to the qds-tools.")]
    public string PassedArgs { get; set; } = string.Empty;

    public string? Properties { get; set; } = null!;
}
