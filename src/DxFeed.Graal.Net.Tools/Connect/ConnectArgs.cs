// <copyright file="ConnectArgs.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using CommandLine;
using CommandLine.Text;
using DxFeed.Graal.Net.Tools.Arguments;

namespace DxFeed.Graal.Net.Tools.Connect;

public class ConnectArgs :
    AbstractParser<ConnectArgs>, IAddressArg, ITypesArg, ISymbolsArg, IFromTimeArg, ISourceArg, IPropertyArg, ITapeArg,
    IQuiteArg
{
    public string Address { get; set; } = null!;

    public string? Types { get; set; } = null!;

    public string? Symbols { get; set; } = null!;

    public string? FromTime { get; set; }

    public string? Source { get; set; }

    public string? Properties { get; set; } = null!;

    public string? Tape { get; set; } = null!;

    public bool IsQuite { get; set; }

    protected override void DisplayHelpText(ParserResult<ConnectArgs> parserResult)
    {
        var helpText = HelpText.AutoBuild(parserResult, h =>
        {
            h.MaximumDisplayWidth = 120;
            h.AdditionalNewLineAfterOption = true;

            h.AddPreOptionsLine(@"
Connect:
    Connects to the specified address(es) and subscribe to the specified events with the specified symbol.");

            h.AddPreOptionsLine(@"
Usage:
    Connect <address> <types> <symbols> [<options>]");

            h.AddPostOptionsLine(@"
Examples:
   Connects to the demo-endpoint and subscribes to Quote event for AAPL,IBM,MSFT symbols:
       connect demo.dxfeed.com:7300 Quote AAPL,IBM,MSFT

   Connects to the demo-endpoint and subscribes to Quote,Trade events for AAPL,MSFT symbols
   with aggregation period 5s:
       connect demo.dxfeed.com:7300 Quote,Trade AAPL,MSFT -p dxfeed.aggregationPeriod=5s

   Connects to the demo-endpoint and subscribes to Order event for AAPL symbol
   with order source NTV (NASDAQ Total View):
       connect demo.dxfeed.com:7300 Order AAPL -s NTV

   Connects to the demo-endpoint and subscribes to TimeAndSale event for AAPL symbol
   from starting January 1, 1970:
       connect demo.dxfeed.com:7300 TimeAndSale AAPL -t 0

   Connects to the tape file and subscribes for all events and all symbols:
       connect file:tape.csv feed all -p dxfeed.wildcard.enable=true");

            return HelpText.DefaultParsingErrorsHandler(parserResult, h);
        }, e => e);
        Console.WriteLine(helpText);
    }
}
