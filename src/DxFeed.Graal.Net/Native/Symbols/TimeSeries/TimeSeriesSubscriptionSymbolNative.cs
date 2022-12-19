// <copyright file="TimeSeriesSubscriptionSymbolNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Api.Osub;

namespace DxFeed.Graal.Net.Native.Symbols.TimeSeries;

/// <summary>
/// Represents <see cref="TimeSeriesSubscriptionSymbol{T}"/>.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal unsafe struct TimeSeriesSubscriptionSymbolNative
{
    public BaseSymbolNative BaseSymbol;
    public BaseSymbolNative* Symbol; // Can be any allowed symbol (String, Wildcard, etc.).
    public long FromTime;
}
