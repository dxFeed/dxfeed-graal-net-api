// <copyright file="CandleSymbol.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Linq;
using DxFeed.Graal.Net.Api;
using DxFeed.Graal.Net.Events.Market;
using DxFeed.Graal.Net.Utils;

namespace DxFeed.Graal.Net.Events.Candle;

/// <summary>
/// Symbol that should be used with <see cref="DXFeedSubscription"/> class
/// to subscribe for <see cref="Candle"/> events. <see cref="DXFeedSubscription"/> also accepts a string
/// representation of the candle symbol for subscription.
/// <br/>
/// For more details see <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/event/candle/CandleSymbol.html">Javadoc</a>.
/// </summary>
public class CandleSymbol
{
    private CandleSymbol(string? symbol)
    {
        Symbol = Normalize(symbol);
        InitInternal();
    }

    private CandleSymbol(string? symbol, params ICandleSymbolProperty[] attributes)
    {
        Symbol = Normalize(ChangeAttributes(symbol, attributes));
        foreach (var attribute in attributes)
        {
            attribute.CheckInAttributeCore(this);
        }
    }

    /// <inheritdoc cref="ToString"/>
    public string? Symbol { get; }

    /// <summary>
    /// Gets base market symbol without attributes.
    /// </summary>
    public string? BaseSymbol { get; private set; }

    /// <summary>
    /// Gets exchange attribute of this symbol.
    /// </summary>
    public CandleExchange? Exchange { get; internal set; }

    /// <summary>
    /// Gets price type attribute of this symbol.
    /// </summary>
    public CandlePrice? Price { get; internal set; }

    /// <summary>
    /// Gets session attribute of this symbol.
    /// </summary>
    public CandleSession? Session { get; internal set; }

    /// <summary>
    /// Gets aggregation period of this symbol.
    /// </summary>
    public CandlePeriod? Period { get; internal set; }

    /// <summary>
    /// Gets alignment attribute of this symbol.
    /// </summary>
    public CandleAlignment? Alignment { get; internal set; }

    /// <summary>
    /// Gets price level attribute of this symbol.
    /// </summary>
    public CandlePriceLevel? PriceLevel { get; internal set; }

    /// <summary>
    /// Converts the given string symbol into the candle symbol object.
    /// </summary>
    /// <param name="symbol">The string symbol.</param>
    /// <returns>The candle symbol object.</returns>
    /// <exception cref="ArgumentException">If the string does not represent a valid symbol.</exception>
    public static CandleSymbol ValueOf(string? symbol) =>
        new(symbol);

    /// <summary>
    /// Converts the given string symbol into the candle symbol object with the specified attribute set.
    /// </summary>
    /// <param name="symbol">The string symbol.</param>
    /// <param name="attributes">The attributes to set.</param>
    /// <returns>The candle symbol object.</returns>
    /// <exception cref="ArgumentException">If the string does not represent a valid symbol.</exception>
    public static CandleSymbol ValueOf(string? symbol, params ICandleSymbolProperty[] attributes) =>
        new(symbol, attributes);

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj)
    {
        if (this == obj)
        {
            return true;
        }

        return obj is CandleSymbol candleSymbol && Symbol!.Equals(candleSymbol.Symbol, StringComparison.Ordinal);
    }

    /// <summary>
    /// Returns a hash code value for this symbol.
    /// </summary>
    /// <returns>A hash code value for this symbol.</returns>
    public override int GetHashCode() =>
        Symbol!.GetHashCode();

    /// <summary>
    /// Returns string representation of this symbol.
    /// The string representation can be transformed back into symbol object
    /// using <see cref="ValueOf(string)"/> method.
    /// </summary>
    /// <returns>The string representation.</returns>
    public override string ToString() =>
        StringUtil.EncodeNullableString(Symbol);

    private static string? ChangeAttributes(string? symbol, params ICandleSymbolProperty[] attributes) =>
        attributes.Aggregate(symbol, ChangeAttribute);

    private static string? ChangeAttribute(string? symbol, ICandleSymbolProperty property) =>
        property.ChangeAttributeForSymbol(symbol);

    private static string? Normalize(string? symbol)
    {
        symbol = CandlePrice.NormalizeAttributeForSymbol(symbol);
        symbol = CandleSession.NormalizeAttributeForSymbol(symbol);
        symbol = CandlePeriod.NormalizeAttributeForSymbol(symbol);
        symbol = CandleAlignment.NormalizeAttributeForSymbol(symbol);
        symbol = CandlePriceLevel.NormalizeAttributeForSymbol(symbol);
        return symbol;
    }

    private void InitInternal()
    {
        BaseSymbol = MarketEventSymbols.GetBaseSymbol(Symbol);
        Exchange ??= CandleExchange.GetAttributeForSymbol(Symbol);
        Price ??= CandlePrice.GetAttributeForSymbol(Symbol);
        Session ??= CandleSession.GetAttributeForSymbol(Symbol);
        Period ??= CandlePeriod.GetAttributeForSymbol(Symbol);
        Alignment ??= CandleAlignment.GetAttributeForSymbol(Symbol);
        PriceLevel ??= CandlePriceLevel.GetAttributeForSymbol(Symbol);
    }
}
