// <copyright file="ITypesArg.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using CommandLine;

namespace DxFeed.Graal.Net.Tools.Arguments;

public interface ITypesArg
{
    public const int Index = 1;
    public const string MetaName = "types";

    public const string HelpText =
        """
        Comma-separated list of dxfeed event types (e.g. Quote, TimeAndSale).
        Use "all" for all available event types.
        """;

    [Value(Index, MetaName = MetaName, HelpText = HelpText, Required = false)]
    public string? Types { get; set; }
}

public interface ITypesArgRequired : ITypesArg
{
    [Value(Index, MetaName = MetaName, HelpText = HelpText, Required = true)]
    public new string? Types { get; set; }
}
