// <copyright file="CandleExchange.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using DxFeed.Graal.Net.Events.Market;

namespace DxFeed.Graal.Net.Events.Candles;

/// <summary>
/// Exchange attribute of <see cref="CandleSymbol"/> defines exchange identifier where data is
/// taken from to build the candles.
/// <br/>
/// For more details see <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/event/candle/CandleExchange.html">Javadoc</a>.
/// </summary>
public class CandleExchange : ICandleSymbolProperty
{
    /// <summary>
    /// Composite exchange where data is taken from all exchanges.
    /// </summary>
    public static readonly CandleExchange Composite = new('\0');

    /// <summary>
    /// Default exchange is <see cref="Composite"/>.
    /// </summary>
    public static readonly CandleExchange Default = Composite;

    private CandleExchange(char exchangeCode) =>
        ExchangeCode = exchangeCode;

    /// <summary>
    /// Gets exchange code.
    /// It is <c>'\0'</c> for <see cref="Composite"/> exchange.
    /// </summary>
    public char ExchangeCode { get; }

    /// <summary>
    /// Returns exchange attribute object that corresponds to the specified exchange code character.
    /// </summary>
    /// <param name="exchangeCode">The exchange code character.</param>
    /// <returns>The exchange attribute object.</returns>
    public static CandleExchange ValueOf(char exchangeCode) =>
        exchangeCode == '\0' ? Composite : new CandleExchange(exchangeCode);

    /// <summary>
    /// Gets exchange attribute object of the given candle symbol string.
    /// The result is <see cref="Default"/> if the symbol does not have exchange attribute.
    /// </summary>
    /// <param name="symbol">The candle symbol string.</param>
    /// <returns>The exchange attribute object of the given candle symbol string.</returns>
    public static CandleExchange GetAttributeForSymbol(string? symbol) =>
        ValueOf(MarketEventSymbols.GetExchangeCode(symbol));

    /// <summary>
    /// Returns candle event symbol string with this candle exchange set.
    /// </summary>
    /// <param name="symbol">The original candle event symbol.</param>
    /// <returns>The candle event symbol string with this candle exchange set.</returns>
    public string? ChangeAttributeForSymbol(string? symbol) =>
        MarketEventSymbols.ChangeExchangeCode(symbol, ExchangeCode);

    /// <summary>
    /// Internal method that initializes attribute in the candle symbol.
    /// </summary>
    /// <param name="candleSymbol">The candle symbol.</param>
    /// <exception cref="InvalidOperationException">If used outside of internal initialization logic.</exception>
    public void CheckInAttribute(CandleSymbol candleSymbol)
    {
        if (candleSymbol.Exchange != null)
        {
            throw new InvalidOperationException("Already initialized");
        }

        candleSymbol.Exchange = this;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj) =>
        this == obj || (obj is CandleExchange exchange && ExchangeCode == exchange.ExchangeCode);

    /// <summary>
    /// Returns a hash code value for this object.
    /// </summary>
    /// <returns>A hash code value for this object.</returns>
    public override int GetHashCode() =>
        ExchangeCode;

    /// <summary>
    /// Returns string representation of this exchange.
    /// It is the string "COMPOSITE" for <see cref="Composite"/> exchange or
    /// exchange character otherwise.
    /// </summary>
    /// <returns>The string representation.</returns>
    public override string ToString() =>
        ExchangeCode == '\0' ? "COMPOSITE" : $"{ExchangeCode}";
}
