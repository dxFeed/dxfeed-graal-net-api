// <copyright file="PerfTestArgs.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using CommandLine;
using DxFeed.Graal.Net.Tools.Arguments;

namespace DxFeed.Graal.Net.Tools.PerfTest;

public class PerfTestArgs : IAddressArgRequired, ITypesArgRequired, ISymbolsArgRequired, IPropertyArg, IForceStreamArg
{
    public string Address { get; set; } = null!;

    public string? Types { get; set; } = null!;

    public string? Symbols { get; set; } = null!;

    public string? Properties { get; set; } = null!;

    public bool ForceStream { get; set; }

    [Option("cpu-usage-by-core", Required = false, HelpText = "Show CPU usage by core (where 1 core = 100%).")]
    public bool ShowCpuUsageByCore { get; set; } = false;

    [Option("detach-listener", Required = false, HelpText = "Don't attach a listener. Used for debugging purposes.")]
    public bool DetachListener { get; set; } = false;
}
