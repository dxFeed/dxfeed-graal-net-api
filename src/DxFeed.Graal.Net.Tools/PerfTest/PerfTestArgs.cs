// <copyright file="PerfTestArgs.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using CommandLine;
using CommandLine.Text;
using DxFeed.Graal.Net.Tools.Arguments;

namespace DxFeed.Graal.Net.Tools.PerfTest;

public class PerfTestArgs :
    AbstractParser<PerfTestArgs>, IAddressArg, ITypesArg, ISymbolsArg, IPropertyArg
{
    public string Address { get; set; } = null!;

    public string? Types { get; set; } = null!;

    public string? Symbols { get; set; } = null!;

    public string? Properties { get; set; } = null!;

    [Option("force-stream", Required = false,
        HelpText = "Enforces a streaming contract for subscription. The StreamFeed role is used instead of Feed.")]
    public bool ForceStream { get; set; } = false;

    [Option("cpu-usage-by-core", Required = false, HelpText = "Show CPU usage by core (where 1 core = 100%).")]
    public bool ShowCpuUsageByCore { get; set; } = false;

    [Option("detach-listener", Required = false, HelpText = "Don't attach a listener. Used for debugging purposes.")]
    public bool DetachListener { get; set; } = false;

    protected override void DisplayHelpText(ParserResult<PerfTestArgs> parserResult)
    {
        var helpText = HelpText.AutoBuild(parserResult, h =>
        {
            h.MaximumDisplayWidth = 120;
            h.AdditionalNewLineAfterOption = true;

            h.AddPreOptionsLine(@"
PerfTest:
     Connects to the specified address(es) and calculates performance counters (events per second, memory usage, cpu usage, etc).");

            h.AddPreOptionsLine(@"
Usage:
    PerfTest <address> <types> <symbols> [<options>]");

            h.AddPostOptionsLine(@"
Examples:
    Connects to the local endpoint, subscribes to TimeAndSale event for ETH/USD:GDAX symbol
    and print performance counters:
       perftest 127.0.0.1:7777 TimeAndSale ETH/USD:GDAX

    Connects to the tape file (on max speed and cyclically), subscribes for all events and all symbols
    and print performance counters:
        perftest file:tape.csv[speed=mac,cycle] feed all -force-stream");

            return HelpText.DefaultParsingErrorsHandler(parserResult, h);
        }, e => e);
        Console.WriteLine(helpText);
    }
}
