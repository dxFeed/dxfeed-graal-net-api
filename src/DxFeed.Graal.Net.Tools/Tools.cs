// <copyright file="Tools.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using DxFeed.Graal.Net.Tools.Attributes;

namespace DxFeed.Graal.Net.Tools;

public static class Tools
{
    private static readonly Dictionary<string, Type> AvailableTools =
        Assembly.GetAssembly(typeof(AbstractTool))?.GetTypes()
            .Where(typeof(AbstractTool).IsAssignableFrom)
            .Where(t => t is { IsAbstract: false, IsClass: true })
            .Where(t => t.GetCustomAttributes().OfType<ToolInfoAttribute>().Any(a => !a.Hidden))
            .ToDictionary(
                kvp => kvp.GetCustomAttribute<ToolInfoAttribute>()!.Name,
                kvp => kvp,
                StringComparer.OrdinalIgnoreCase)
        ?? new();

    public static IEnumerable<string> ListOfAvailableTools =>
        AvailableTools.Keys;

    public static bool TryCreateTool(string toolName, [MaybeNullWhen(false)] out AbstractTool tool)
    {
        tool = null;
        if (AvailableTools.TryGetValue(toolName, out var toolType))
        {
            tool = (AbstractTool?)Activator.CreateInstance(toolType);
        }

        return tool != null;
    }

    public static void Run(string[] args)
    {
        var helpScreen = new HelpScreen { AutoHelp = false, AutoVersion = false, AddDashesToOption = false, };

        if (args.Length == 0 || args[0].Equals("--help"))
        {
            helpScreen.TypeScreen = HelpScreen.ScreenTypes.Help;
            helpScreen.AddUsage(new[] { "<tool> [...]" });
            helpScreen.AddPostUsage("Where <tool> is one of:");
            helpScreen.AddVerbs(AvailableTools.Values.ToArray());
            helpScreen.AddPostOptionsText("""To get detailed help on some tool use "Help <tool>".""");
            Console.WriteLine(helpScreen);
            return;
        }

        if (args[0].Equals("--version"))
        {
            helpScreen.TypeScreen = HelpScreen.ScreenTypes.Version;
            helpScreen.DisableDefaultHeading();
            helpScreen.EnableVersion();
            Console.WriteLine(helpScreen);
            return;
        }

        if (!TryCreateTool(args[0], out var tool))
        {
            helpScreen.TypeScreen = HelpScreen.ScreenTypes.Error;
            helpScreen.AddError($"""Unknown tool "{args[0]}".""");
            Console.WriteLine(helpScreen);
            return;
        }

        tool.Run(args.AsSpan()[1..].ToArray());
    }
}
