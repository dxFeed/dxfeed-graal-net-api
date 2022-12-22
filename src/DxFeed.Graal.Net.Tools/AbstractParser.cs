// <copyright file="AbstractParser.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using CommandLine;

namespace DxFeed.Graal.Net.Tools;

public abstract class AbstractParser<T>
{
    public virtual T? ParseArgs(IEnumerable<string> args) =>
        ParseArgs(args, GetDefaultParserSettings());

    protected T? ParseArgs(IEnumerable<string> args, Action<ParserSettings> settings)
    {
        var parser = new Parser(settings);
        var result = parser.ParseArguments<T>(args);
        result.WithNotParsed(_ => DisplayHelpText(result));
        return result.Value;
    }

    protected Action<ParserSettings> GetDefaultParserSettings() =>
        settings =>
        {
            settings.MaximumDisplayWidth = 120;
            settings.AutoHelp = true;
            settings.AutoVersion = true;
            settings.EnableDashDash = true;
            settings.HelpWriter = null;
            settings.CaseInsensitiveEnumValues = true;
        };

    protected abstract void DisplayHelpText(ParserResult<T> parserResult);
}
