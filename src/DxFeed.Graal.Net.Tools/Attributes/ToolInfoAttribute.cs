// <copyright file="ToolInfoAttribute.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using CommandLine;

namespace DxFeed.Graal.Net.Tools.Attributes;

public class ToolInfoAttribute : VerbAttribute
{
    public ToolInfoAttribute(string name)
        : base(name)
    {
    }

    public string ShortDescription
    {
        get => HelpText;
        set => HelpText = value;
    }

    public string? Description { get; set; }

    public string[]? Usage { get; set; }

    public string? PostUsage { get; set; } = "Where:";

    public string[]? PostOptions { get; set; }
}
