// <copyright file="BaseArgs.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Collections.Generic;
using CommandLine;

namespace DxFeed.Graal.Net.Tools.Arguments;

public abstract class BaseArgs<T>
{
    public T? ParseArgs(IEnumerable<string> args)
    {
        var parser = new Parser(config =>
        {
            config.MaximumDisplayWidth = 120;
            config.AutoHelp = true;
            config.AutoVersion = true;
            config.EnableDashDash = true;
            config.HelpWriter = null;
            config.CaseInsensitiveEnumValues = true;
            config.IgnoreUnknownArguments = true;
        });
        var result = parser.ParseArguments<T>(args);
        result.WithNotParsed(_ => DisplayHelpText(result));
        return result.Value;
    }

    protected abstract void DisplayHelpText(ParserResult<T> parserResult);
}
