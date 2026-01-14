// <copyright file="SymbolsByOrderSourceIdMapEntryNative.cs" company="Devexperts LLC">
// Copyright © 2025 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.Interop;

namespace DxFeed.Graal.Net.Native.Orcs;

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct SymbolsByOrderSourceIdMapEntryNative
{
    public int OrderSourceId;
    public ListNative<StringNative>* Symbols;
}
