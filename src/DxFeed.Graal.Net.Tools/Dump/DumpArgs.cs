// <copyright file="DumpArgs.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using CommandLine;
using CommandLine.Text;
using DxFeed.Graal.Net.Tools.Arguments;

namespace DxFeed.Graal.Net.Tools.Dump;

public class DumpArgs :
    AbstractParser<DumpArgs>, IAddressArg, ITypesArg, ISymbolsArg, IPropertyArg, ITapeArg, IQuiteArg
{
    public string Address { get; set; } = null!;

    [Value(ITypesArg.Index, MetaName = ITypesArg.MetaName, Required = false, HelpText = ITypesArg.HelpText)]
    public string? Types { get; set; } = null!;

    [Value(ISymbolsArg.Index, MetaName = ISymbolsArg.MetaName, Required = false, HelpText = ISymbolsArg.HelpText)]
    public string? Symbols { get; set; } = null!;

    public string? Properties { get; set; } = null!;

    public string? Tape { get; set; } = null!;

    public bool IsQuite { get; set; }

    protected override void DisplayHelpText(ParserResult<DumpArgs> parserResult)
    {
        var helpText = HelpText.AutoBuild(parserResult, h =>
        {
            h.MaximumDisplayWidth = 120;
            h.AdditionalNewLineAfterOption = true;

            h.AddPreOptionsLine(@"
Dump:
    Dumps all events received from address.
    Enforces a streaming contract for subscription. A wildcard enabled by default.
    This was designed to receive data from a file.
    If <types> is not specified, creates a subscription for all available event types.
    If <symbol> is not specified, the wildcard symbol is used.");

            h.AddPreOptionsLine(@"
Usage:
    Dump <address>
    Dump <address> <types> <symbols> [<options>]");

            h.AddPostOptionsLine(@"
Examples:
  Dump all events and all symbols from the specified file:
      dump tape.csv

  Dump all events and all symbols from the specified file at maximum speed:
      dump tape.csv[speed=max]

  Dump all events and all symbols from the specified file at maximum speed, cyclically:
      dump tape.csv[speed=max,cycle]

  Dump only Quote event for all symbols from the specified file:
      dump tape.csv Quote

  Dump only Quote event for AAPL symbol from the specified file:
      dump tape.csv Quote AAPL

  Dump only Quote event for AAPL symbol from the specified address in a stream contract:
      dump demo.dxfeed.com:7300 Quote AAPL

  Tape only Quote event for AAPL symbol from the specified address in a stream contract into the specified tape file:
      dump demo.dxfeed.com:7300 Quote AAPL -q -t tape.bin");

            return HelpText.DefaultParsingErrorsHandler(parserResult, h);
        }, e => e);
        Console.WriteLine(helpText);
    }
}
