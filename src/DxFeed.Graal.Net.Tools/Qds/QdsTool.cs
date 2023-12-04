// <copyright file="QdsTool.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.CommandLine.Parsing;
using DxFeed.Graal.Net.Tools.Attributes;
using DxFeed.Graal.Net.Utils;

namespace DxFeed.Graal.Net.Tools.Qds;

[ToolInfo(
    "Qds",
    ShortDescription = "A collection of tools ported from the Java qds-tools.",
    Usage = new[] { "Qds \"<arg>\" [<options>]" })]
public class QdsTool : AbstractTool<QdsArgs>
{
    public override void Run(QdsArgs args)
    {
        SystemProperty.SetProperties(ParseProperties(args.Properties));
        QdsUtil.RunTool(CommandLineStringSplitter.Instance.Split(args.PassedArgs));
    }
}
