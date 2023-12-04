// <copyright file="ConnectArgs.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Tools.Arguments;

namespace DxFeed.Graal.Net.Tools.Connect;

public class ConnectArgs :
    IAddressArgRequired, ITypesArgRequired, ISymbolsArgRequired, IFromTimeArg, ISourceArg, IPropertyArg, ITapeArg,
    IQuiteArg, IForceStreamArg
{
    public string Address { get; set; } = null!;

    public string? Types { get; set; }

    public string? Symbols { get; set; }

    public string? FromTime { get; set; }

    public string? Source { get; set; }

    public string? Properties { get; set; }

    public string? Tape { get; set; }

    public bool IsQuite { get; set; }

    public bool ForceStream { get; set; }
}
