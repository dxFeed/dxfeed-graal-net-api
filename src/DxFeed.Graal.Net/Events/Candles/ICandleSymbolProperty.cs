// <copyright file="ICandleSymbolProperty.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;

namespace DxFeed.Graal.Net.Events.Candles;

/// <summary>
/// Property of the <see cref="CandleSymbol"/>.
/// </summary>
public interface ICandleSymbolProperty
{
    /// <summary>
    /// Change candle event symbol string with this attribute set
    /// and returns new candle event symbol string.
    /// </summary>
    /// <param name="symbol">The original candle event symbol.</param>
    /// <returns>Returns candle event symbol string with this attribute set.</returns>
    string? ChangeAttributeForSymbol(string? symbol);

    /// <summary>
    /// Internal method that initializes attribute in the candle symbol.
    /// </summary>
    /// <param name="candleSymbol">The candle symbol.</param>
    /// <exception cref="InvalidOperationException">If used outside of internal initialization logic.</exception>
    void CheckInAttributeCore(CandleSymbol candleSymbol);
}
