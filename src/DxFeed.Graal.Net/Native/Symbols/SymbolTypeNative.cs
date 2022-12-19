// <copyright file="SymbolTypeNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Api.Osub;

namespace DxFeed.Graal.Net.Native.Symbols;

/// <summary>
/// List of all symbols type, that can be passed to/from native code, represented as a numeric code.
/// </summary>
internal enum SymbolTypeNative
{
    /// <summary>
    /// Symbol as <see cref="string"/>.
    /// </summary>
    String,

    /// <summary>
    /// Symbol as CandleSymbol.
    /// Not implemented.
    /// </summary>
    Candle,

    /// <summary>
    /// Symbol as <see cref="WildcardSymbol"/>.
    /// </summary>
    Wildcard,

    /// <summary>
    /// Symbols as <see cref="IndexedEventSubscriptionSymbol"/>.
    /// </summary>
    IndexedEventSymbol,

    /// <summary>
    /// Symbols as <see cref="TimeSeriesSubscriptionSymbol"/>.
    /// </summary>
    TimeSeriesSymbol,
}
