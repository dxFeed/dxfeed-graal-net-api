// <copyright file="DumpArgs.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Tools.Arguments;

namespace DxFeed.Graal.Net.Tools.Dump;

public class DumpArgs : IAddressArgRequired, ITypesArg, ISymbolsArg, IPropertyArg, ITapeArg, IQuiteArg
{
    public string Address { get; set; } = null!;

    public string? Types { get; set; } = null!;

    public string? Symbols { get; set; } = null!;

    public string? Properties { get; set; } = null!;

    public string? Tape { get; set; } = null!;

    public bool IsQuite { get; set; }
}
