// <copyright file="CandlePriceLevel.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Globalization;
using DxFeed.Graal.Net.Events.Market;
using DxFeed.Graal.Net.Utils;

namespace DxFeed.Graal.Net.Events.Candle;

/// <summary>
/// Candle price level attribute of <see cref="CandleSymbol"/> defines how candles shall be aggregated in respect to
/// price interval. The negative or infinite values of price interval are treated as exceptional.
/// <ul>
/// <li>Price interval may be equal to zero. It means every unique price creates a particular candle
/// to aggregate all events with this price for the chosen <see cref="CandlePeriod"/>.</li>
/// <li>Non-zero price level creates sequence of intervals starting from 0:
/// ...,[-pl;0),[0;pl),[pl;2*pl),...,[n*pl,n*pl+pl).
/// Events aggregated by chosen <see cref="CandlePeriod"/> and price intervals.</li>
/// </ul>
/// <br/>
/// For more details see <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/event/candle/CandlePriceLevel.html">Javadoc</a>.
/// </summary>
public class CandlePriceLevel : ICandleSymbolProperty
{
    /// <summary>
    /// The attribute key that is used to store the value of <see cref="CandlePriceLevel"/> in
    /// a symbol string using methods of <see cref="MarketEventSymbols"/> class.
    /// The value of this constant is "pl".
    /// The value that this key shall be set to is equal to
    /// the corresponding <see cref="ToString"/>.
    /// </summary>
    public const string AttributeKey = "pl";

    /// <summary>
    /// Default candle price level <see cref="double"/>.<see cref="double.NaN"/>.
    /// </summary>
    public static readonly CandlePriceLevel Default = new(double.NaN);

    /// <summary>
    /// The cached value of the string representation of this price level.
    /// </summary>
    private readonly Lazy<string> _stringRepresentation;

    private CandlePriceLevel(double value)
    {
        if (double.IsInfinity(value) || MathUtil.IsNegativeZero(value)) // Reject -0.0.
        {
            throw new ArgumentException($"Incorrect candle price level: {value}", nameof(value));
        }

        Value = value;
        _stringRepresentation = new(() =>
        {
            return Value.Equals((long)Value)
                ? $"{(long)Value}"
                : $"{Value.ToString(CultureInfo.InvariantCulture)}";
        });
    }

    /// <summary>
    /// Gets a price level value.
    /// For example, the value of <c>1</c> represents [0;1), [1;2) and so on intervals to build candles.
    /// </summary>
    public double Value { get; }

    /// <summary>
    /// Returns candle price level object that corresponds to the specified value.
    /// </summary>
    /// <param name="value">The candle price level value.</param>
    /// <returns>The candle price level with the given value and type.</returns>
    public static CandlePriceLevel ValueOf(double value) =>
        double.IsNaN(value) ? Default : new CandlePriceLevel(value);

    /// <summary>
    /// Parses string representation of candle price level into object.
    /// Any string that was returned by <see cref="ToString"/> can be parsed.
    /// and case is ignored for parsing.
    /// </summary>
    /// <param name="s">The string representation of candle candle price level attribute.</param>
    /// <returns>The candle price level attribute.</returns>
    public static CandlePriceLevel Parse(string s)
    {
        var value = double.Parse(s, CultureInfo.InvariantCulture);
        return ValueOf(value);
    }

    /// <summary>
    /// Normalizes candle symbol string with representation of the candle price level attribute.
    /// </summary>
    /// <param name="symbol">The candle symbol string.</param>
    /// <returns>
    /// Returns candle symbol string with the normalized representation of the candle price level attribute.
    /// </returns>
    public static string? NormalizeAttributeForSymbol(string? symbol)
    {
        var a = MarketEventSymbols.GetAttributeStringByKey(symbol, AttributeKey);
        if (a == null)
        {
            return symbol;
        }

        try
        {
            var other = Parse(a);
            if (ReferenceEquals(other, Default))
            {
                MarketEventSymbols.RemoveAttributeStringByKey(symbol, AttributeKey);
            }

            if (!a.Equals(other.ToString(), StringComparison.Ordinal))
            {
                return MarketEventSymbols.ChangeAttributeStringByKey(symbol, AttributeKey, other.ToString());
            }

            return symbol;
        }
        catch (ArgumentException)
        {
            return symbol;
        }
    }

    /// <summary>
    /// Gets candle price level of the given candle symbol string.
    /// The result is <see cref="Default"/> if the symbol does not have candle price level attribute.
    /// </summary>
    /// <param name="symbol">The candle symbol string.</param>
    /// <returns>The candle price level of the given candle symbol string.</returns>
    public static CandlePriceLevel GetAttributeForSymbol(string? symbol)
    {
        var s = MarketEventSymbols.GetAttributeStringByKey(symbol, AttributeKey);
        return s == null ? Default : Parse(s);
    }

    /// <summary>
    /// Returns candle event symbol string with this candle price level set.
    /// </summary>
    /// <param name="symbol">The original candle event symbol.</param>
    /// <returns>The candle event symbol string with this candle price level set.</returns>
    public string? ChangeAttributeForSymbol(string? symbol) =>
        ReferenceEquals(this, Default)
            ? MarketEventSymbols.RemoveAttributeStringByKey(symbol, AttributeKey)
            : MarketEventSymbols.ChangeAttributeStringByKey(symbol, AttributeKey, ToString());

    /// <summary>
    /// Internal method that initializes attribute in the candle symbol.
    /// </summary>
    /// <param name="candleSymbol">The candle symbol.</param>
    /// <exception cref="InvalidOperationException">If used outside of internal initialization logic.</exception>
    public void CheckInAttributeCore(CandleSymbol candleSymbol)
    {
        if (candleSymbol.PriceLevel != null)
        {
            throw new InvalidOperationException("Already initialized");
        }

        candleSymbol.PriceLevel = this;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// The same price level has the same <see cref="Value"/>.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj)
    {
        if (this == obj)
        {
            return true;
        }

        if (obj is not CandlePriceLevel that)
        {
            return false;
        }

        return Value.CompareTo(that.Value) == 0;
    }

    /// <summary>
    /// Returns a hash code value for this object.
    /// </summary>
    /// <returns>A hash code value for this object.</returns>
    public override int GetHashCode() =>
        Value.GetHashCode();

    /// <summary>
    /// Returns string representation of this candle price level attribute.
    /// The string representation is composed of value.
    /// This string representation can be converted back into object
    /// with <see cref="Parse"/> method.
    /// </summary>
    /// <returns>The string representation.</returns>
    public override string ToString() =>
        _stringRepresentation.Value;

    /// <summary>
    /// Returns full string representation of this candle price level attribute.
    /// It is contains attribute key and its value.
    /// For example, the  full string representation of price level = 0.5 is "pl=0.5".
    /// </summary>
    /// <returns>The full string representation of a candle price level attribute.</returns>
    public string ToFullString() =>
        $"{AttributeKey}={ToString()}";
}
