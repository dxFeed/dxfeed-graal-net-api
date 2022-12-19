// <copyright file="ISymbolsArg.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using CommandLine;

namespace DxFeed.Graal.Net.Tools.Arguments;

public interface ISymbolsArg
{
    public const int Index = 2;
    public const string MetaName = "symbols";
    public const bool IsRequired = true;

    public const string HelpText = @"
Comma-separated list of symbol names to get events for (e.g. ""IBM,AAPL,MSFT"").
Use ""all"" for wildcard subscription.";

    [Value(Index, MetaName = MetaName, Required = IsRequired, HelpText = HelpText)]
    public string? Symbols { get; set; }
}
