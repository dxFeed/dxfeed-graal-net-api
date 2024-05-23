// <copyright file="AbstractTool.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using CommandLine;
using DxFeed.Graal.Net.Api;
using DxFeed.Graal.Net.Api.Osub;
using DxFeed.Graal.Net.Tools.Attributes;
using DxFeed.Graal.Net.Tools.Help;
using DxFeed.Graal.Net.Utils;

namespace DxFeed.Graal.Net.Tools;

// File May Only Contain A Single Type. It's more readable to put the generic and non-generic versions in the same file.
#pragma warning disable SA1402

public abstract class AbstractTool
{
    private readonly Lazy<ToolInfoAttribute> _toolInfo;
    private readonly Lazy<Parser> _parser;

    protected AbstractTool()
    {
        _toolInfo = new(() =>
        {
            var type = GetType();
            return AttributeUtil.GetCustomAttribute<ToolInfoAttribute>(type) ??
                   throw new InvalidOperationException($"{type.Name} has no {nameof(ToolInfoAttribute)}");
        });

        _parser = new(() => new(settings =>
        {
            settings.CaseInsensitiveEnumValues = true;
            settings.HelpWriter = null;
            settings.AutoHelp = true;
            settings.AutoVersion = true;
            settings.ParsingCulture = CultureInfo.InvariantCulture;
            settings.GetoptMode = true;
        }));
    }

    protected ToolInfoAttribute ToolInfo =>
        _toolInfo.Value;

    protected Parser DefaultParser =>
        _parser.Value;

    public abstract void Run(ICollection<string> args);

    public abstract HelpScreen GenerateHelpScreen();

    protected static IEnumerable<Type> ParseEventTypes(string types)
    {
        types = types.Trim().Trim(',');

        return types.Equals("all", StringComparison.OrdinalIgnoreCase)
            ? DXEndpoint.GetEventTypes()
            : CmdArgsUtil.ParseTypes(types);
    }

    protected static IEnumerable<object> ParseSymbols(string symbols)
    {
        symbols = symbols.Trim().Trim(',');

        return symbols.Equals("all", StringComparison.OrdinalIgnoreCase)
            ? new[] { WildcardSymbol.All }
            : CmdArgsUtil.ParseSymbols(symbols);
    }

    protected static IReadOnlyDictionary<string, string> ParseProperties(string? properties) =>
        string.IsNullOrWhiteSpace(properties)
            ? new Dictionary<string, string>()
            : CmdArgsUtil.ParseProperties(properties);
}

public abstract class AbstractTool<T> : AbstractTool
{
    public override void Run(ICollection<string> args)
    {
        if (args.Count == 0)
        {
            DisplayHelpScreen(GenerateHelpScreen());
            return;
        }

        var parserResult = DefaultParser.ParseArguments<T>(args);
        parserResult.WithParsed(Run);
        parserResult.WithNotParsed(_ => DisplayHelpScreen(GenerateHelpScreen(parserResult)));
    }

    public abstract void Run(T args);

    public override HelpScreen GenerateHelpScreen() =>
        GenerateHelpScreen(DefaultParser.ParseArguments<T>(new[] { "--help" }));

    private HelpScreen GenerateHelpScreen(ParserResult<T> parserResult)
    {
        var helpScreen = new HelpScreen();
        if (parserResult.Errors.IsHelp())
        {
            helpScreen.TypeScreen = HelpScreen.ScreenTypes.Help;
            helpScreen.AddTitle(ToolInfo.Name);
            helpScreen.AddDescription(ToolInfo.Description);
            helpScreen.AddUsage(ToolInfo.Usage);
            helpScreen.AddPostUsage(ToolInfo.PostUsage);
            helpScreen.AddOptions(parserResult);
            helpScreen.AddPostOptionsLines(ToolInfo.PostOptions);
        }
        else if (parserResult.Errors.IsVersion())
        {
            helpScreen.TypeScreen = HelpScreen.ScreenTypes.Version;
            helpScreen.DisableDefaultHeading();
            helpScreen.EnableVersion();
        }
        else if (parserResult.Tag == ParserResultType.NotParsed)
        {
            helpScreen.TypeScreen = HelpScreen.ScreenTypes.Error;
            helpScreen.AddError(parserResult);
        }

        return helpScreen;
    }

    private void DisplayHelpScreen(HelpScreen helpScreen)
    {
        var sb = new StringBuilder();
        sb.AppendLine(helpScreen);
        switch (helpScreen.TypeScreen)
        {
            case HelpScreen.ScreenTypes.Help:
                if (HelpTool.Articles.ContainsArticle(ToolInfo.Name))
                {
                    sb.AppendLine($"""Use "Help {ToolInfo.Name}" for more detailed help.""");
                }

                break;
            case HelpScreen.ScreenTypes.Error:
                sb.AppendLine($"""Use "{ToolInfo.Name} --help" for usage info.""");
                break;
        }

        Console.WriteLine(sb.ToString().Trim());
    }
}
