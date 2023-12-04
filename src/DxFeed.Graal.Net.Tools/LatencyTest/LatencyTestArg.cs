// <copyright file="LatencyTestArg.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using CommandLine;
using DxFeed.Graal.Net.Tools.Arguments;

namespace DxFeed.Graal.Net.Tools.LatencyTest;

public class LatencyTestArgs : IAddressArg, ITypesArg, ISymbolsArg, IPropertyArg, IForceStreamArg
{
    public string Address { get; set; } = null!;

    public string? Types { get; set; } = null!;

    public string? Symbols { get; set; } = null!;

    public string? Properties { get; set; } = null!;

    public bool ForceStream { get; set; }

    [Option("interval", Required = false, HelpText = "Measurement interval in seconds.")]
    public int Interval { get; set; } = 2;

    [Option("ignore-exchanges", Required = false, HelpText = "Ignoring next exchanges.")]
    public string? IgnoreExchanges { get; set; } = null!;
}
