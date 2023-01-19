// <copyright file="CandlePrice.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Concurrent;
using System.Linq;
using DxFeed.Graal.Net.Events.Market;

namespace DxFeed.Graal.Net.Events.Candle;

/// <summary>
/// Price type attribute of <see cref="CandleSymbol"/> defines price that is used to build the candles.
/// <br/>
/// For more details see <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/event/candle/CandlePrice.html">Javadoc</a>.
/// </summary>
public class CandlePrice : ICandleSymbolProperty
{
    // Elements Must Be Ordered By Access. Ignored, it is necessary to observe the order of initialization.
    // For avoid creating static ctor.
#pragma warning disable SA1202
    /// <summary>
    /// The attribute key that is used to store the value of <see cref="CandlePrice"/> in
    /// a symbol string using methods of <see cref="MarketEventSymbols"/> class.
    /// The value of this constant is "price".
    /// The value that this key shall be set to is equal to
    /// the corresponding <see cref="ToString"/>.
    /// </summary>
    public const string AttributeKey = "price";

    /// <summary>
    /// A dictionary containing the matching string representation
    /// of the candle price type attribute (<see cref="ToString"/>) and the <see cref="CandlePrice"/> instance.
    /// </summary>
    private static readonly ConcurrentDictionary<string, CandlePrice> ByValue = new();

    /// <summary>
    /// A dictionary containing the matching id
    /// of the candle price type (<see cref="Id"/>) and the <see cref="CandlePrice"/> instance.
    /// </summary>
    private static readonly ConcurrentDictionary<CandlePriceId, CandlePrice> ById = new();

    /// <summary>
    /// Last trading price.
    /// </summary>
    public static readonly CandlePrice Last = new(CandlePriceId.Last, "last");

    /// <summary>
    /// Quote bid price.
    /// </summary>
    public static readonly CandlePrice Bid = new(CandlePriceId.Bid, "bid");

    /// <summary>
    /// Quote ask price.
    /// </summary>
    public static readonly CandlePrice Ask = new(CandlePriceId.Ask, "ask");

    /// <summary>
    /// Market price defined as average between quote bid and ask prices.
    /// </summary>
    public static readonly CandlePrice Mark = new(CandlePriceId.Mark, "mark");

    /// <summary>
    /// Official settlement price that is defined by exchange or last trading price otherwise.
    /// It updates based on all <see cref="PriceType"/> values:
    /// <see cref="PriceType"/>.<see cref="PriceType.Indicative"/>,
    /// <see cref="PriceType"/>.<see cref="PriceType.Preliminary"/>,
    /// and <see cref="PriceType"/>.<see cref="PriceType.Final"/>.
    /// </summary>
    public static readonly CandlePrice Settlement = new CandlePrice(CandlePriceId.Settlement, "s");

    /// <summary>
    /// Default price type is <see cref="Last"/>.
    /// </summary>
    public static readonly CandlePrice Default = Last;
#pragma warning restore SA1202

    private CandlePrice(CandlePriceId id, string value)
    {
        Id = id;
        Name = id.ToString();
        Value = value;

        if (!ByValue.TryAdd(value, this))
        {
            throw new ArgumentException($"Duplicate value: {value}", nameof(value));
        }

        if (!ById.TryAdd(id, this))
        {
            throw new ArgumentException($"Duplicate id: {id}", nameof(id));
        }
    }

    /// <summary>
    /// List of ids <see cref="CandlePriceId"/>.
    /// </summary>
    public enum CandlePriceId
    {
        /// <summary>
        /// Id associated with
        /// <see cref="CandlePrice"/>.<see cref="CandlePrice.Last"/>.
        /// </summary>
        Last,

        /// <summary>
        /// Id associated with
        /// <see cref="CandlePrice"/>.<see cref="CandlePrice.Bid"/>.
        /// </summary>
        Bid,

        /// <summary>
        /// Id associated with
        /// <see cref="CandlePrice"/>.<see cref="CandlePrice.Ask"/>.
        /// </summary>
        Ask,

        /// <summary>
        /// Id associated with
        /// <see cref="CandlePrice"/>.<see cref="CandlePrice.Mark"/>.
        /// </summary>
        Mark,

        /// <summary>
        /// Id associated with
        /// <see cref="CandlePrice"/>.<see cref="CandlePrice.Settlement"/>.
        /// </summary>
        Settlement,
    }

    /// <summary>
    /// Gets <see cref="CandlePriceId"/> associated with this instance.
    /// </summary>
    public CandlePriceId Id { get; }

    /// <summary>
    /// Gets full name this <see cref="CandlePrice"/> instance.
    /// For example,
    /// <see cref="Last"/> returns <c>"Last"</c>,
    /// <see cref="Bid"/> returns <c>"Bid"</c>.
    /// </summary>
    public string Name { get; }

    /// <inheritdoc cref="ToString"/>
    public string Value { get; }

    /// <summary>
    /// Gets <see cref="CandlePrice"/> associated with the specified <see cref="CandlePriceId"/>.
    /// </summary>
    /// <param name="id">The candle price type id.</param>
    /// <returns>The price type type.</returns>
    /// <exception cref="ArgumentException">If candle type id not exist.</exception>
    public static CandlePrice GetById(CandlePriceId id)
    {
        if (ById.TryGetValue(id, out var price))
        {
            return price;
        }

        throw new ArgumentException($"Unknown candle price type id: {id}", nameof(id));
    }

    /// <summary>
    /// Parses string representation of candle price type into object.
    /// Any string that was returned by <see cref="ToString"/> can be parsed
    /// and case is ignored for parsing.
    /// </summary>
    /// <param name="s">The string representation of candle price.</param>
    /// <returns>The candle price.</returns>
    /// <exception cref="ArgumentException">If the string representation is invalid.</exception>
    public static CandlePrice Parse(string s)
    {
        var n = s.Length;
        if (n == 0)
        {
            throw new ArgumentException("Missing candle price", nameof(s));
        }

        // Fast path to reverse toString result.
        if (ByValue.TryGetValue(s, out var result))
        {
            return result;
        }

        // Slow path for different case.
        try
        {
            return ByValue.Values.First(price =>
            {
                var ps = price.ToString();
                return ps.Length >= n && ps[..n].Equals(s, StringComparison.OrdinalIgnoreCase);
            });
        }
        catch
        {
            throw new ArgumentException($"Unknown candle price: {s}", nameof(s));
        }
    }

    /// <summary>
    /// Normalizes candle symbol string with representation of the candle price type attribute.
    /// </summary>
    /// <param name="symbol">The candle symbol string.</param>
    /// <returns>
    /// Returns candle symbol string with the normalized representation of the candle price type attribute.
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
            if (other == Default)
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
    /// Gets candle price type of the given candle symbol string.
    /// The result is <see cref="Default"/> if the symbol does not have candle price type attribute.
    /// </summary>
    /// <param name="symbol">The candle symbol string.</param>
    /// <returns>The candle price of the given candle symbol string.</returns>
    public static CandlePrice GetAttributeForSymbol(string? symbol)
    {
        var s = MarketEventSymbols.GetAttributeStringByKey(symbol, AttributeKey);
        return s == null ? Default : Parse(s);
    }

    /// <summary>
    /// Returns candle event symbol string with this candle price type set.
    /// </summary>
    /// <param name="symbol">The original candle event symbol.</param>
    /// <returns>The candle event symbol string with this candle price type set.</returns>
    public string? ChangeAttributeForSymbol(string? symbol) =>
        this == Default
            ? MarketEventSymbols.RemoveAttributeStringByKey(symbol, AttributeKey)
            : MarketEventSymbols.ChangeAttributeStringByKey(symbol, AttributeKey, ToString());

    /// <summary>
    /// Internal method that initializes attribute in the candle symbol.
    /// </summary>
    /// <param name="candleSymbol">The candle symbol.</param>
    /// <exception cref="InvalidOperationException">If used outside of internal initialization logic.</exception>
    public void CheckInAttributeCore(CandleSymbol candleSymbol)
    {
        if (candleSymbol.Price != null)
        {
            throw new InvalidOperationException("Already initialized");
        }

        candleSymbol.Price = this;
    }

    /// <summary>
    /// Returns string representation of this candle price type.
    /// The string representation of candle price type is a lower case string
    /// that corresponds to its name.
    /// For example, <see cref="Last"/> is represented as "last".
    /// </summary>
    /// <returns>The string representation.</returns>
    public override string ToString() =>
        Value;

    /// <summary>
    /// Returns full string representation of this candle price type.
    /// It is contains attribute key and its value.
    /// For example, the full string representation of <see cref="Last"/> is "price=last".
    /// </summary>
    /// <returns>The string representation.</returns>
    public string ToFullString() =>
        $"{AttributeKey}={Value}";
}
