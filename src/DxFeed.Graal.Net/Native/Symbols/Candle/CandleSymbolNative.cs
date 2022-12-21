// <copyright file="CandleSymbolNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;

namespace DxFeed.Graal.Net.Native.Symbols.Candle;

/// <summary>
/// Represents symbol as CandleSymbol.
/// Not implemented.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct CandleSymbolNative
{
    public BaseSymbolNative BaseSymbol;
    public nint Symbol; // A null-terminated UTF-8 string.
}