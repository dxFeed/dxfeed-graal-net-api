// <copyright file="CandleSession.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Concurrent;
using System.Linq;
using DxFeed.Graal.Net.Events.Market;

namespace DxFeed.Graal.Net.Events.Candles;

/// <summary>
/// Session attribute of <see cref="CandleSymbol"/> defines trading that is used to build the candles.
/// <br/>
/// For more details see <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/event/candle/CandleSession.html">Javadoc</a>.
/// </summary>
public class CandleSession : ICandleSymbolProperty
{
    // Elements Must Be Ordered By Access. Ignored, it is necessary to observe the order of initialization.
    // For avoid creating static ctor.
#pragma warning disable SA1202

    /// <summary>
    /// A dictionary containing the matching string representation
    /// of the candle session attribute (<see cref="ToString"/>) and the <see cref="CandleSession"/> instance.
    /// </summary>
    private static readonly ConcurrentDictionary<string, CandleSession> ByValue = new();

    /// <summary>
    /// A dictionary containing the matching id
    /// of the candle session attribute (<see cref="Id"/>) and the <see cref="CandleSession"/> instance.
    /// </summary>
    private static readonly ConcurrentDictionary<CandleSessionId, CandleSession> ById = new();

    /// <summary>
    /// The attribute key that is used to store the value of <see cref="CandleSession"/> in
    /// a symbol string using methods of <see cref="MarketEventSymbols"/> class.
    /// class.
    /// The value of this constant is "tho", which is an abbreviation for "trading hours only".
    /// The value that this key shall be set to is equal to
    /// the corresponding <see cref="ToString"/>.
    /// </summary>
    public const string AttributeKey = "tho";

    /// <summary>
    /// All trading sessions are used to build candles.
    /// </summary>
    public static readonly CandleSession Any = new(CandleSessionId.Any, "false");

    /// <summary>
    /// Only regular trading session data is used to build candles.
    /// </summary>
    public static readonly CandleSession Regular = new(CandleSessionId.Regular, "true");

    /// <summary>
    /// Default trading session is <see cref="Any"/>.
    /// </summary>
    public static readonly CandleSession Default = Any;
#pragma warning restore SA1202

    private CandleSession(CandleSessionId id, string value)
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
    /// List of ids <see cref="CandleSession"/>.
    /// </summary>
    public enum CandleSessionId
    {
        /// <summary>
        /// Id associated with
        /// <see cref="CandleSession"/>.<see cref="CandleSession.Any"/>.
        /// </summary>
        Any,

        /// <summary>
        /// Id associated with
        /// <see cref="CandleSession"/>.<see cref="CandleSession.Regular"/>.
        /// </summary>
        Regular,
    }

    /// <summary>
    /// Gets <see cref="CandleSessionId"/> associated with this instance.
    /// </summary>
    public CandleSessionId Id { get; }

    /// <summary>
    /// Gets full name this <see cref="CandleSession"/> instance.
    /// For example,
    /// <see cref="Any"/> returns <c>"Any"</c>,
    /// <see cref="Regular"/> returns <c>"Regular"</c>.
    /// </summary>
    public string Name { get; }

    /// <inheritdoc cref="ToString"/>
    public string Value { get; }

    /// <summary>
    /// Parses string representation of candle session attribute into object.
    /// Any string that was returned by <see cref="ToString"/> can be parsed
    /// and case is ignored for parsing.
    /// </summary>
    /// <param name="s">The string representation of candle price.</param>
    /// <returns>Returns instance of <see cref="CandleSession"/>.</returns>
    /// <exception cref="ArgumentException">If the string representation is invalid.</exception>
    public static CandleSession Parse(string s)
    {
        var n = s.Length;
        if (n == 0)
        {
            throw new ArgumentException("Missing candle session", nameof(s));
        }

        try
        {
            return ByValue.Values.First(price =>
            {
                var ss = price.ToString();
                return ss.Length >= n && ss[..n].Equals(s, StringComparison.OrdinalIgnoreCase);
            });
        }
        catch
        {
            throw new ArgumentException($"Unknown candle session: {s}", nameof(s));
        }
    }

    /// <summary>
    /// Normalizes candle symbol string with representation of the candle session attribute.
    /// </summary>
    /// <param name="symbol">The candle symbol string.</param>
    /// <returns>
    /// Returns candle symbol string with the normalized representation of the candle session attribute.
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
            var b = bool.Parse(a);
            switch (b)
            {
                case false:
                    MarketEventSymbols.RemoveAttributeStringByKey(symbol, AttributeKey);
                    break;
                case true when !a.Equals(Regular.ToString(), StringComparison.Ordinal):
                    return MarketEventSymbols.ChangeAttributeStringByKey(symbol, AttributeKey, Regular.ToString());
            }

            return symbol;
        }
        catch (ArgumentException)
        {
            return symbol;
        }
    }

    /// <summary>
    /// Gets candle session of the given candle symbol string.
    /// The result is <see cref="Default"/> if the symbol does not have candle session attribute.
    /// </summary>
    /// <param name="symbol">The candle symbol string.</param>
    /// <returns>The candle session attribute of the given candle symbol string.</returns>
    public static CandleSession GetAttributeForSymbol(string? symbol)
    {
        var s = MarketEventSymbols.GetAttributeStringByKey(symbol, AttributeKey);
        return s != null && bool.Parse(s) ? Regular : Default;
    }

    /// <summary>
    /// Returns candle event symbol string with this session attribute set.
    /// </summary>
    /// <param name="symbol">The original candle event symbol.</param>
    /// <returns>The candle event symbol string with this session attribute set.</returns>
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
        if (candleSymbol.Session != null)
        {
            throw new InvalidOperationException("Already initialized");
        }

        candleSymbol.Session = this;
    }

    /// <summary>
    /// Returns string representation of this candle session attribute.
    /// The string representation of candle session attribute is a lower case string
    /// that corresponds to its type name. For example,
    /// <see cref="Any"/> is represented as "false".
    /// </summary>
    /// <returns>The string representation.</returns>
    public override string ToString() =>
        Value;

    /// <summary>
    /// Returns full string representation of this candle session attribute.
    /// It is contains attribute key and its value.
    /// For example, the full string representation of <see cref="Any"/> is "tho=false".
    /// </summary>
    /// <returns>The string representation.</returns>
    public string ToFullString() =>
        $"{AttributeKey}={Value}";
}
