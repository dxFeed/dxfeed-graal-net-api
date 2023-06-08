// <copyright file="HelpTool.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using DxFeed.Graal.Net.Tools.Attributes;

namespace DxFeed.Graal.Net.Tools.Help;

[ToolInfo(
    "Help",
    ShortDescription = "Help tool.",
    Description = "Displays documentation pages.",
    Usage = new[] { "Help <article>" },
    PostOptions = new[]
    {
        """To see help on some topic type "Help <topic>".""",
        """To see list of all articles type "Help contents".""",
        """Use "Help all" to generate all existing help articles.""",
    })]
public sealed class HelpTool : AbstractTool<HelpArgs>
{
    private static readonly Lazy<HelpArticles> HelpArticles = new(() => new HelpArticles(Resources.HelpFile));

    private static readonly Lazy<IEnumerable<string>> AllContents = new(() =>
    {
        var contents = new SortedSet<string>();
        contents.UnionWith(Articles.ListOfAllArticles);
        contents.UnionWith(Tools.ListOfAvailableTools);
        return contents;
    });

    public static HelpArticles Articles =>
        HelpArticles.Value;

    private static IEnumerable<string> ListOfAllContents =>
        AllContents.Value;

    public override void Run(HelpArgs args)
    {
        var articleName = string.Join(' ', args.Article);
        if (articleName.Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            DisplayAll();
            return;
        }

        if (articleName.Equals("contents", StringComparison.OrdinalIgnoreCase))
        {
            DisplayContents();
            return;
        }

        var content = ListOfAllContents
            .FirstOrDefault(c => c.Equals(articleName, StringComparison.OrdinalIgnoreCase));

        if (content == null)
        {
            DisplayNotFound(articleName);
            return;
        }

        DisplayContent(content);
    }

    private static void DisplayAll()
    {
        var helpScreen = new HelpScreen();
        helpScreen.AddPageBreak();
        foreach (var content in ListOfAllContents)
        {
            var toolHelpScreen = GenerateContent(content);
            toolHelpScreen.DisableDefaultHeading();
            helpScreen.AddPostOptionsText(toolHelpScreen);
            helpScreen.AddPageBreak();
        }

        Console.Write(helpScreen);
    }

    private static void DisplayContents()
    {
        var helpScreen = new HelpScreen();
        helpScreen.AddPreOptionsLine($"{Environment.NewLine}Help articles:");
        helpScreen.AddPreOptionsIndentLines(ListOfAllContents);
        Console.WriteLine(helpScreen);
    }

    private static void DisplayNotFound(string articleName)
    {
        var helpScreen = new HelpScreen();
        helpScreen.AddError($"""No help article found for "{articleName}".""");
        Console.WriteLine(helpScreen);
    }

    private static void DisplayContent(string article) =>
        Console.Write(GenerateContent(article));

    private static HelpScreen GenerateContent(string article)
    {
        var helpScreen = new HelpScreen();
        if (Tools.TryCreateTool(article, out var tool))
        {
            helpScreen = tool.GenerateHelpScreen();
        }
        else
        {
            helpScreen.AddTitle(article);
        }

        helpScreen.AddPostOptionsText(Articles.FindArticle(article));
        return helpScreen;
    }
}
