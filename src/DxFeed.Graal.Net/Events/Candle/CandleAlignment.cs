// <copyright file="CandleAlignment.cs" company="Devexperts LLC">
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
/// Candle alignment attribute of <see cref="CandleSymbol"/> defines how candle are aligned with respect to time.
/// <br/>
/// For more details see <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/event/candle/CandleAlignment.html">Javadoc</a>.
/// </summary>
public class CandleAlignment : ICandleSymbolProperty
{
    // Elements Must Be Ordered By Access. Ignored, it is necessary to observe the order of initialization.
    // For avoid creating static ctor.
#pragma warning disable SA1202
    /// <summary>
    /// The attribute key that is used to store the value of <see cref="CandleAlignment"/> in
    /// a symbol string using methods of <see cref="MarketEventSymbols"/> class.
    /// The value of this constant is "a".
    /// The value that this key shall be set to is equal to
    /// the corresponding <see cref="ToString"/>.
    /// </summary>
    public const string AttributeKey = "a";

    /// <summary>
    /// A dictionary containing the matching string representation
    /// of the candle alignment attribute (<see cref="ToString"/>) and the <see cref="CandleAlignment"/> instance.
    /// </summary>
    private static readonly ConcurrentDictionary<string, CandleAlignment> ByValue = new();

    /// <summary>
    /// A dictionary containing the matching id
    /// of the candle alignment attribute (<see cref="Id"/>) and the <see cref="CandleAlignment"/> instance.
    /// </summary>
    private static readonly ConcurrentDictionary<CandleAlignmentId, CandleAlignment> ById = new();

    /// <summary>
    /// Align candles on midnight.
    /// </summary>
    public static readonly CandleAlignment Midnight = new(CandleAlignmentId.Midnight, "m");

    /// <summary>
    /// Align candles on trading sessions.
    /// </summary>
    public static readonly CandleAlignment Session = new(CandleAlignmentId.Session, "s");

    /// <summary>
    /// Default alignment is <see cref="Midnight"/>.
    /// </summary>
    public static readonly CandleAlignment Default = Midnight;
#pragma warning restore SA1202

    private CandleAlignment(CandleAlignmentId id, string value)
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
    /// List of ids <see cref="CandleAlignment"/>.
    /// </summary>
    public enum CandleAlignmentId
    {
        /// <summary>
        /// Id associated with
        /// <see cref="CandleAlignment"/>.<see cref="CandleAlignment.Midnight"/>.
        /// </summary>
        Midnight,

        /// <summary>
        /// Id associated with
        /// <see cref="CandleAlignment"/>.<see cref="CandleAlignment.Session"/>.
        /// </summary>
        Session,
    }

    /// <summary>
    /// Gets <see cref="CandleAlignmentId"/> associated with this instance.
    /// </summary>
    public CandleAlignmentId Id { get; }

    /// <summary>
    /// Gets full name this <see cref="CandleAlignment"/> instance.
    /// For example,
    /// <see cref="Midnight"/> returns <c>"Midnight"</c>,
    /// <see cref="Session"/> returns <c>"Session"</c>.
    /// </summary>
    public string Name { get; }

    /// <inheritdoc cref="ToString"/>
    public string Value { get; }

    /// <summary>
    /// Gets <see cref="CandleAlignment"/> associated with the specified <see cref="CandleAlignmentId"/>.
    /// </summary>
    /// <param name="id">The candle alignment id.</param>
    /// <returns>The candle alignment.</returns>
    /// <exception cref="ArgumentException">If candle type id not exist.</exception>
    public static CandleAlignment GetById(CandleAlignmentId id)
    {
        if (ById.TryGetValue(id, out var align))
        {
            return align;
        }

        throw new ArgumentException($"Unknown candle alignment id: {id}", nameof(id));
    }

    /// <summary>
    /// Parses string representation of candle alignment into object.
    /// Any string that was returned by <see cref="ToString"/> can be parsed
    /// and case is ignored for parsing.
    /// </summary>
    /// <param name="s">The string representation of candle alignment.</param>
    /// <returns>The candle alignment.</returns>
    /// <exception cref="ArgumentException">If the string representation is invalid.</exception>
    public static CandleAlignment Parse(string s)
    {
        // Fast path to reverse toString result.
        if (ByValue.TryGetValue(s, out var result))
        {
            return result;
        }

        // Slow path for different case.
        try
        {
            return ByValue.Values.First(align =>
                align.ToString().Equals(s, StringComparison.OrdinalIgnoreCase));
        }
        catch
        {
            throw new ArgumentException($"Unknown candle alignment: {s}", nameof(s));
        }
    }

    /// <summary>
    /// Normalizes candle symbol string with representation of the candle alignment attribute.
    /// </summary>
    /// <param name="symbol">The candle symbol string.</param>
    /// <returns>
    /// Returns candle symbol string with the normalized representation of the candle alignment attribute.
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
    /// Gets candle alignment of the given candle symbol string.
    /// The result is <see cref="Default"/> if the symbol does not have candle alignment attribute.
    /// </summary>
    /// <param name="symbol">The candle symbol string.</param>
    /// <returns>The candle alignment of the given candle symbol string.</returns>
    public static CandleAlignment GetAttributeForSymbol(string? symbol)
    {
        var s = MarketEventSymbols.GetAttributeStringByKey(symbol, AttributeKey);
        return s == null ? Default : Parse(s);
    }

    /// <summary>
    /// Returns candle event symbol string with this candle alignment set.
    /// </summary>
    /// <param name="symbol">The original candle event symbol.</param>
    /// <returns>The candle event symbol string with this candle alignment set.</returns>
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
        if (candleSymbol.Alignment != null)
        {
            throw new InvalidOperationException("Already initialized");
        }

        candleSymbol.Alignment = this;
    }

    /// <summary>
    /// Returns string representation of this candle alignment.
    /// The string representation of candle alignment "m" for <see cref="Midnight"/>
    /// and "s" for <see cref="Session"/>.
    /// </summary>
    /// <returns>The string representation.</returns>
    public override string ToString() =>
        Value;

    /// <summary>
    /// Returns full string representation of this candle alignment.
    /// It is contains attribute key and its value.
    /// For example, the full string representation of <see cref="Midnight"/> is "a=m".
    /// </summary>
    /// <returns>The string representation.</returns>
    public string ToFullString() =>
        $"{AttributeKey}={Value}";
}
