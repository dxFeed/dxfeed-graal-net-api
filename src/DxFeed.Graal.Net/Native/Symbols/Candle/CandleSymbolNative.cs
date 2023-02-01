// <copyright file="CandleSymbolNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.Interop;

namespace DxFeed.Graal.Net.Native.Symbols.Candle;

/// <summary>
/// Represents symbol as CandleSymbol.
/// Not implemented.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct CandleSymbolNative
{
    public SymbolNative SymbolNative;
    public StringNative Symbol; // A null-terminated UTF-8 string.
}
