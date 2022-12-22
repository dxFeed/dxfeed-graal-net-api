// <copyright file="ProgramArgs.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace DxFeed.Graal.Net.Tools;

public enum Tools
{
    Connect,
    Dump
}

public class ProgramArgs : AbstractParser<ProgramArgs>
{
    [Value(0, MetaName = "tool", Required = true, HelpText = "The name of tool.")]
    public Tools? Tool { get; set; } = null!;

    public override ProgramArgs? ParseArgs(IEnumerable<string> args)
    {
        // Change default settings for parser.
        void ParserSettings(ParserSettings settings)
        {
            GetDefaultParserSettings()(settings);
            settings.IgnoreUnknownArguments = true;
        }

        return ParseArgs(args, ParserSettings);
    }

    protected override void DisplayHelpText(ParserResult<ProgramArgs> parserResult)
    {
        var helpText = HelpText.AutoBuild(parserResult, h =>
        {
            h.MaximumDisplayWidth = 120;
            h.AdditionalNewLineAfterOption = true;

            h.AddPreOptionsLine(@"
Usage:
  <tool> [...]
Where <tool> is one of:
    Connect - Connects to specified address(es).
    Dump    - Dumps all events received from address.");

            h.AddPostOptionsLine("To get detailed help on some tool use <tool> --help.");

            return HelpText.DefaultParsingErrorsHandler(parserResult, h);
        }, e => e);
        Console.WriteLine(helpText);
    }
}
