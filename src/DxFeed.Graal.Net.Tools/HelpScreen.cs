// <copyright file="HelpScreen.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using CommandLine.Text;

namespace DxFeed.Graal.Net.Tools;

public class HelpScreen : HelpText
{
    private readonly Comparison<ComparableOption> _orderOnShortNameAndValuesBeforeOptions = (x, y) =>
    {
        switch (x.IsValue)
        {
            case true when y.IsValue:
            {
                switch (x.Required)
                {
                    case true when !y.Required:
                        return -1;
                    case false when y.Required:
                        return 1;
                }

                if (string.IsNullOrEmpty(x.ShortName) && !string.IsNullOrEmpty(y.ShortName))
                {
                    return 1;
                }

                if (!string.IsNullOrEmpty(x.ShortName) && string.IsNullOrEmpty(y.ShortName))
                {
                    return -1;
                }

                return string.Compare(x.ShortName, y.ShortName, StringComparison.Ordinal);
            }

            case true when y.IsOption:
                return -1;
            default:
                return 1;
        }
    };

    public HelpScreen()
    {
        EnableDefaultHeading();
        AddNewLineBetweenHelpSections = true;
        AdditionalNewLineAfterOption = false;
        AddDashesToOption = true;
        OptionComparison = _orderOnShortNameAndValuesBeforeOptions;
        TypeScreen = ScreenTypes.Plain;
        Indent = 2;
    }

    public enum ScreenTypes
    {
        Plain,
        Help,
        Version,
        Error,
    }

    public ScreenTypes TypeScreen { get; set; }

    public int Indent { get; set; }

    public static implicit operator string(HelpScreen screen) =>
        screen.ToString();

    public void EnableDefaultHeading()
    {
        EnableVersion();
        EnableCopyright();
    }

    public void DisableDefaultHeading()
    {
        DisableVersion();
        DisableCopyright();
    }

    public void EnableVersion() =>
        Heading = HeadingInfo.Default;

    public void DisableVersion() =>
        Heading = string.Empty;

    public void EnableCopyright() =>
        Copyright = CopyrightInfo.Default;

    public void DisableCopyright() =>
        Copyright = string.Empty;

    public void AddPageBreak()
    {
        AddPostOptionsLine(new string('-', MaximumDisplayWidth));
        AddPostOptionsLine(Environment.NewLine);
    }

    public void AddTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return;
        }

        AddPreOptionsLine($"{Environment.NewLine}{title}");
        AddPreOptionsLine(new string('=', title.Length));
    }

    public void AddDescription(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return;
        }

        AddPreOptionsLine($"{Environment.NewLine}{description}");
    }

    public void AddUsage(IEnumerable<string>? usage)
    {
        if (usage == null)
        {
            return;
        }

        AddPreOptionsLine($"{Environment.NewLine}Usage:");
        AddPreOptionsIndentLines(usage);
    }

    public void AddPostUsage(string? postUsage)
    {
        if (string.IsNullOrWhiteSpace(postUsage))
        {
            return;
        }

        AddPreOptionsLine($"{Environment.NewLine}{postUsage}");
    }

    public new HelpText AddVerbs(params Type[] types) =>
        types.Length == 0 ? this : base.AddVerbs(types);

    public void AddError<T>(ParserResult<T> parserResult)
    {
        DefaultParsingErrorsHandler(parserResult, this);
        AddPreOptionsLine(TextWrapper.WrapAndIndentText(Environment.NewLine, 0, MaximumDisplayWidth));
    }

    public void AddError(params string[] errors) =>
        AddError(errors.ToList());

    public void AddError(IEnumerable<string> errors)
    {
        AddPreOptionsLine($"{Environment.NewLine}{SentenceBuilder.ErrorsHeadingText()}");
        AddPreOptionsIndentLines(errors);
    }

    public void AddPreOptionsIndentLines(IEnumerable<string> lines)
    {
        foreach (var line in lines)
        {
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            AddPreOptionsLine(TextWrapper.WrapAndIndentText(line, Indent, MaximumDisplayWidth));
        }
    }

    public new void AddPostOptionsText(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        base.AddPostOptionsText(value);
    }

    public new void AddPostOptionsLines(IEnumerable<string>? lines)
    {
        if (lines == null)
        {
            return;
        }

        base.AddPostOptionsLines(lines);
        AddPostOptionsLine(Environment.NewLine);
    }

    public new string ToString() =>
        base.ToString().TrimStart();
}
