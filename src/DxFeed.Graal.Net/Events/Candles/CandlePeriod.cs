// <copyright file="CandlePeriod.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Globalization;
using DxFeed.Graal.Net.Events.Market;

namespace DxFeed.Graal.Net.Events.Candles;

/// <summary>
/// Period attribute of <see cref="CandleSymbol"/> defines aggregation period of the candles.
/// Aggregation period is defined as pair of a <see cref="Value"/> and <see cref="Type"/>.
/// <br/>
/// For more details see <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/event/candle/CandlePeriod.html">Javadoc</a>.
/// </summary>
public class CandlePeriod : ICandleSymbolProperty
{
    /// <summary>
    /// The attribute key that is used to store the value of <see cref="CandlePeriod"/> in
    /// a symbol string using methods of <see cref="MarketEventSymbols"/> class.
    /// The value of this constant is an empty string, because this is the
    /// main attribute that every <see cref="CandleSymbol"/> must have.
    /// The value that this key shall be set to is equal to
    /// the corresponding <see cref="ToString"/>.
    /// </summary>
    public const string AttributeKey = ""; // Empty string as attribute key is allowed!

    /// <summary>
    /// Tick aggregation where each candle represents an individual tick.
    /// </summary>
    public static readonly CandlePeriod Tick = new(DefaultPeriodValue, CandleType.Tick);

    /// <summary>
    /// Day aggregation where each candle represents a day.
    /// </summary>
    public static readonly CandlePeriod Day = new(DefaultPeriodValue, CandleType.Day);

    /// <summary>
    /// Default period is <see cref="Tick"/>.
    /// </summary>
    public static readonly CandlePeriod Default = Tick;

    /// <summary>
    /// The number represents default period value.
    /// </summary>
    private const int DefaultPeriodValue = 1;

    /// <summary>
    /// The cached value of the string representation of this candle period.
    /// </summary>
    private readonly Lazy<string> _stringRepresentation;

    private CandlePeriod(double value, CandleType type)
    {
        Value = value;
        Type = type;
        PeriodIntervalMillis = (long)(Type.PeriodIntervalMillis * Value);

        _stringRepresentation = new(() =>
        {
            if (Value.Equals(DefaultPeriodValue))
            {
                return type.ToString();
            }

            return Value.Equals((long)Value)
                ? $"{(long)Value}{type}"
                : $"{Value.ToString(CultureInfo.InvariantCulture)}{type}";
        });
    }

    /// <summary>
    /// Gets aggregation period value.
    /// For example, the value of <c>5</c> with
    /// the candle type of <see cref="CandleType.Minute"/> represents 5 minute
    /// aggregation period.
    /// </summary>
    public double Value { get; }

    /// <summary>
    /// Gets aggregation period type.
    /// </summary>
    public CandleType Type { get; }

    /// <summary>
    /// Gets aggregation period in milliseconds as closely as possible.
    /// Certain aggregation types like <see cref="CandleType.Second"/> and
    /// <see cref="CandleType.Day"/> span a specific number of milliseconds.
    /// <see cref="CandleType.Month"/>, <see cref="CandleType.OptExp"/> and <see cref="CandleType.Year"/>
    /// are approximate. Candle period of
    /// <see cref="CandleType.Tick"/>, <see cref="CandleType.Volume"/>, <see cref="CandleType.Price"/>,
    /// <see cref="CandleType.PriceMomentum"/> and <see cref="CandleType.PriceRenko"/>
    /// is not defined and this method returns <c>0</c>.
    /// The result of this method is equal to:
    /// <code> (long) (this.Type.PeriodIntervalMillis * this.Value) </code>
    /// </summary>
    public long PeriodIntervalMillis { get; }

    /// <summary>
    /// Returns candle period with the given value and type.
    /// </summary>
    /// <param name="value">The value candle period value.</param>
    /// <param name="type">The candle period type.</param>
    /// <returns>The candle period with the given value and type.</returns>
    public static CandlePeriod ValueOf(double value, CandleType type) =>
        value switch
        {
            DefaultPeriodValue when type == CandleType.Day => Day,
            DefaultPeriodValue when type == CandleType.Tick => Tick,
            _ => new CandlePeriod(value, type),
        };

    /// <summary>
    /// Parses string representation of aggregation period into object.
    /// Any string that was returned by <see cref="ToString"/> can be parsed.
    /// This method is flexible in the way candle types can be specified.
    /// See <see cref="Parse"/> for details.
    /// </summary>
    /// <param name="s">string representation of aggregation period.</param>
    /// <returns>aggregation period object.</returns>
    /// <exception cref="ArgumentNullException">If input string is null.</exception>
    /// <exception cref="FormatException">If input string does not represent a number in a valid format.</exception>
    /// <exception cref="OverflowException">
    /// If input string represents a number that is less than
    /// <see cref="double.MinValue"/> or greater than <see cref="double.MaxValue"/>.
    /// </exception>
    public static CandlePeriod Parse(string s)
    {
        if (s.Equals(CandleType.Day.ToString(), StringComparison.Ordinal))
        {
            return Day;
        }

        if (s.Equals(CandleType.Tick.ToString(), StringComparison.Ordinal))
        {
            return Tick;
        }

        var i = 0;
        for (; i < s.Length; i++)
        {
            var c = s[i];
            if (c is (< '0' or > '9') and not '.' and not '-' and not '+' and not 'e' and not 'E')
            {
                break;
            }
        }

        var value = s[..i];
        var type = s[i..];

        return string.IsNullOrEmpty(value)
            ? ValueOf(1, CandleType.Parse(type))
            : ValueOf(double.Parse(value, CultureInfo.InvariantCulture), CandleType.Parse(type));
    }

    /// <summary>
    /// Returns candle symbol string with the normalized representation of the candle period attribute.
    /// </summary>
    /// <param name="symbol">The candle symbol string.</param>
    /// <returns>
    /// The candle symbol string with the normalized representation of the the candle period attribute.
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
            if (other.Equals(Default))
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
    /// Gets candle period of the given candle symbol string.
    /// The result is <see cref="Default"/> if the symbol does not have candle period attribute.
    /// </summary>
    /// <param name="symbol">The candle symbol string.</param>
    /// <returns>The candle period of the given candle symbol string.</returns>
    public static CandlePeriod GetAttributeForSymbol(string? symbol)
    {
        var s = MarketEventSymbols.GetAttributeStringByKey(symbol, AttributeKey);
        return s == null ? Default : Parse(s);
    }

    /// <summary>
    /// Returns candle event symbol string with this aggregation period set.
    /// </summary>
    /// <param name="symbol">The original candle event symbol.</param>
    /// <returns>The candle event symbol string with this aggregation period set.</returns>
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
        if (candleSymbol.Period != null)
        {
            throw new InvalidOperationException("Already initialized");
        }

        candleSymbol.Period = this;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// The same aggregation period has the same <see cref="Value"/>} and
    /// <see cref="Type"/>.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj)
    {
        if (this == obj)
        {
            return true;
        }

        if (obj is not CandlePeriod that)
        {
            return false;
        }

        return Value.CompareTo(that.Value) == 0 && Type == that.Type;
    }

    /// <summary>
    /// Returns a hash code value for this object.
    /// </summary>
    /// <returns>A hash code value for this object.</returns>
    public override int GetHashCode() =>
        (31 * Value.GetHashCode()) + Type.GetHashCode();

    /// <summary>
    /// Returns string representation of this aggregation period.
    /// The string representation is composed of value and type string.
    /// For example, 5 minute aggregation is represented as <c>"5m"</c>.
    /// The value of <c>1</c> is omitted in the string representation, so
    /// <see cref="Day"/> (one day) is represented as <c>"d"</c>.
    /// This string representation can be converted back into object
    /// with <see cref="Parse"/> method.
    /// </summary>
    /// <returns>The string representation.</returns>
    public override string ToString() =>
        _stringRepresentation.Value;
}
