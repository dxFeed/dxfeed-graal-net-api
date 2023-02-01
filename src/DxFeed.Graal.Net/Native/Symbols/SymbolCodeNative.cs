// <copyright file="SymbolCodeNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Api.Osub;
using DxFeed.Graal.Net.Events.Candles;

namespace DxFeed.Graal.Net.Native.Symbols;

/// <summary>
/// A list of all symbols type, that can be passed to/from native code, represented as a numeric code.
/// </summary>
internal enum SymbolCodeNative
{
    /// <summary>
    /// Symbol as <see cref="string"/>.
    /// </summary>
    String,

    /// <summary>
    /// Symbol as <see cref="Net.Events.Candles.CandleSymbol"/>.
    /// </summary>
    CandleSymbol,

    /// <summary>
    /// Symbol as <see cref="Api.Osub.WildcardSymbol"/>.
    /// </summary>
    WildcardSymbol,

    /// <summary>
    /// Symbols as <see cref="Api.Osub.IndexedEventSubscriptionSymbol"/>.
    /// </summary>
    IndexedEventSubscriptionSymbol,

    /// <summary>
    /// Symbols as <see cref="Api.Osub.TimeSeriesSubscriptionSymbol"/>.
    /// </summary>
    TimeSeriesSubscriptionSymbol,
}
