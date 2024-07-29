// <copyright file="OptionSeries.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using DxFeed.Graal.Net.Utils;

namespace DxFeed.Graal.Net.Ipf.Options;

/// <summary>
/// Represents a series of call and put options with different strike sharing the same attributes
/// such as expiration, last trading day, SPC, multiplier, etc.
///
/// <h3>Threads and locks</h3>
///
/// This class is <b>NOT</b> thread-safe and cannot be used from multiple threads without external synchronization.
/// </summary>
/// <typeparam name="T">The type of option instrument instances.</typeparam>
public sealed class OptionSeries<T> : ICloneable, IComparable<OptionSeries<T>>
{
    private List<double>? _strikes;

    internal OptionSeries()
    {
    }

    internal OptionSeries(OptionSeries<T> other)
    {
        Expiration = other.Expiration;
        LastTrade = other.LastTrade;
        Multiplier = other.Multiplier;
        SPC = other.SPC;
        AdditionalUnderlyings = other.AdditionalUnderlyings;
        MMY = other.MMY;
        OptionType = other.OptionType;
        ExpirationStyle = other.ExpirationStyle;
        SettlementStyle = other.SettlementStyle;
        CFI = other.CFI;
    }

    /// <summary>
    /// Gets the day id of expiration.
    /// </summary>
    /// <example><see cref="DayUtil.GetYearMonthDayByDayId">DayUtil.GetYearMonthDayByDayId(20090117)</see>.</example>
    public int Expiration { get; internal set; }

    /// <summary>
    /// Gets the day id of the last trading day.
    /// </summary>
    /// <example><see cref="DayUtil.GetYearMonthDayByDayId">DayUtil.GetYearMonthDayByDayId(20090117)</see>.</example>
    public int LastTrade { get; internal set; }

    /// <summary>
    /// Gets the market value multiplier.
    /// </summary>
    /// <example>"100", "33.2".</example>
    public double Multiplier { get; internal set; }

    /// <summary>
    /// Gets the shares per contract for options.
    /// </summary>
    /// <example>"1", "100".</example>
    public double SPC { get; internal set; }

    /// <summary>
    /// Gets additional underlyings for options, including additional cash.
    /// It shall use following format:
    /// <code>
    ///     &lt;VALUE&gt; ::= &lt;empty&gt; | &lt;LIST&gt;
    ///     &lt;LIST&gt; ::= &lt;AU&gt; | &lt;AU&gt; &lt;semicolon&gt; &lt;space&gt; &lt;LIST&gt;
    ///     &lt;AU&gt; ::= &lt;UNDERLYING&gt; &lt;space&gt; &lt;SPC&gt;
    /// </code>
    /// the list shall be sorted by &lt;UNDERLYING&gt;.
    /// </summary>
    /// <example>"SE 50", "FIS 53; US$ 45.46".</example>
    public string AdditionalUnderlyings { get; internal set; } = string.Empty;

    /// <summary>
    /// Gets maturity month-year as provided for corresponding FIX tag (200).
    /// It can use several different formats depending on data source.
    /// <ul>
    ///     <li>YYYYMM – if only year and month are specified</li>
    ///     <li>YYYYMMDD – if full date is specified</li>
    ///     <li>YYYYMMwN – if week number (within a month) is specified</li>
    /// </ul>
    /// </summary>
    public string MMY { get; internal set; } = string.Empty;

    /// <summary>
    /// Gets type of option.
    /// It shall use one of following values.
    /// <ul>
    ///     <li>STAN = Standard Options</li>
    ///     <li>LEAP = Long-term Equity AnticiPation Securities</li>
    ///     <li>SDO = Special Dated Options</li>
    ///     <li>BINY = Binary Options</li>
    ///     <li>FLEX = FLexible EXchange Options</li>
    ///     <li>VSO = Variable Start Options</li>
    ///     <li>RNGE = Range</li>
    /// </ul>
    /// </summary>
    public string OptionType { get; internal set; } = string.Empty;

    /// <summary>
    /// Gets expiration cycle style, such as "Weeklys", "Quarterlys".
    /// </summary>
    public string ExpirationStyle { get; internal set; } = string.Empty;

    /// <summary>
    /// Gets settlement price determination style, such as "Open", "Close".
    /// </summary>
    public string SettlementStyle { get; internal set; } = string.Empty;

    /// <summary>
    /// Gets classification of Financial Instruments code.
    /// It is a mandatory field for OPTION instruments as it is the only way to distinguish Call/Put type,
    /// American/European exercise, Cash/Physical delivery.
    /// It shall use six-letter CFI code from ISO 10962 standard.
    /// It is allowed to use 'X' extensively and to omit trailing letters (assumed to be 'X').
    /// See <a href="http://en.wikipedia.org/wiki/ISO_10962">ISO 10962 on Wikipedia</a>.
    /// </summary>
    /// <example>"ESNTPB", "ESXXXX", "ES", "OPASPS".</example>
    public string CFI { get; internal set; } = string.Empty;

    /// <summary>
    /// Gets a sorted map of all calls from strike to a corresponding option instrument.
    /// </summary>
    public SortedDictionary<double, T> Calls { get; } = new();

    /// <summary>
    /// Gets a sorted map of all puts from strike to a corresponding option instrument.
    /// </summary>
    public SortedDictionary<double, T> Puts { get; } = new();

    /// <summary>
    /// Gets a list of all strikes in ascending order.
    /// </summary>
    public List<double> Strikes
    {
        get
        {
            if (_strikes == null)
            {
                var strikesSet = new SortedSet<double>(Calls.Keys);
                strikesSet.UnionWith(Puts.Keys);
                _strikes = strikesSet.ToList();
            }

            return _strikes;
        }
    }

    /// <summary>
    /// Determines whether two specified instances of <see cref="OptionSeries{T}"/> are equal.
    /// </summary>
    /// <param name="left">The first instance to compare.</param>
    /// <param name="right">The second instance to compare.</param>
    /// <returns><c>true</c> if the instances are equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(OptionSeries<T> left, OptionSeries<T> right) =>
        Equals(left, right);

    /// <summary>
    /// Determines whether two specified instances of <see cref="OptionSeries{T}"/> are not equal.
    /// </summary>
    /// <param name="left">The first instance to compare.</param>
    /// <param name="right">The second instance to compare.</param>
    /// <returns><c>true</c> if the instances are not equal; otherwise, <c>false</c>.</returns>
    public static bool operator !=(OptionSeries<T> left, OptionSeries<T> right) =>
        !Equals(left, right);

    /// <summary>
    /// Determines whether one specified <see cref="OptionSeries{T}"/> is less than another.
    /// </summary>
    /// <param name="left">The first instance to compare.</param>
    /// <param name="right">The second instance to compare.</param>
    /// <returns><c>true</c> if the first instance is less than the second instance; otherwise, <c>false</c>.</returns>
    public static bool operator <(OptionSeries<T> left, OptionSeries<T> right) =>
        left.CompareTo(right) < 0;

    /// <summary>
    /// Determines whether one specified <see cref="OptionSeries{T}"/> is less than or equal to another.
    /// </summary>
    /// <param name="left">The first instance to compare.</param>
    /// <param name="right">The second instance to compare.</param>
    /// <returns>
    /// <c>true</c> if the first instance is less than or equal to the second instance; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator <=(OptionSeries<T> left, OptionSeries<T> right) =>
        left.CompareTo(right) <= 0;

    /// <summary>
    /// Determines whether one specified <see cref="OptionSeries{T}"/> is greater than another.
    /// </summary>
    /// <param name="left">The first instance to compare.</param>
    /// <param name="right">The second instance to compare.</param>
    /// <returns>
    /// <c>true</c> if the first instance is greater than the second instance; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator >(OptionSeries<T> left, OptionSeries<T> right) =>
        left.CompareTo(right) > 0;

    /// <summary>
    /// Determines whether one specified <see cref="OptionSeries{T}"/> is greater than or equal to another.
    /// </summary>
    /// <param name="left">The first instance to compare.</param>
    /// <param name="right">The second instance to compare.</param>
    /// <returns>
    /// <c>true</c> if the first instance is greater than or equal to the second instance; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator >=(OptionSeries<T> left, OptionSeries<T> right) =>
        left.CompareTo(right) >= 0;

    /// <summary>
    /// Gets n strikes centered around a specified strike value.
    /// </summary>
    /// <param name="n">The maximum number of strikes to return.</param>
    /// <param name="strike">The center strike.</param>
    /// <returns>A list of n strikes centered around the specified strike value.</returns>
    /// <exception cref="ArgumentException">If n &lt; 0.</exception>
    public List<double> GetNStrikesAround(int n, double strike)
    {
        if (n < 0)
        {
            throw new ArgumentException("Must not be less than zero.", nameof(n));
        }

        var strikes = Strikes;
        var i = strikes.BinarySearch(strike);
        if (i < 0)
        {
            i = ~i;
        }

        var from = Math.Max(0, i - (n / 2));
        var to = Math.Min(strikes.Count, from + n);
        return strikes.GetRange(from, to - from);
    }

    /// <summary>
    /// Creates a shallow copy of this option series.
    /// Collections of calls and puts are copied, but option instrument instances are shared with the original.
    /// </summary>
    /// <returns>A shallow copy of this option series.</returns>
    public object Clone()
    {
        var clone = new OptionSeries<T>(this);
        foreach (var kvp in Calls)
        {
            clone.Calls.Add(kvp.Key, kvp.Value);
        }

        foreach (var kvp in Puts)
        {
            clone.Puts.Add(kvp.Key, kvp.Value);
        }

        return clone;
    }

    /// <summary>
    /// Compares this option series to another one by its attributes.
    /// Expiration takes precedence in comparison.
    /// </summary>
    /// <param name="other">The other option series to compare with.</param>
    /// <returns>The result of the comparison.</returns>
    public int CompareTo(OptionSeries<T>? other)
    {
        if (other is null)
        {
            return 1;
        }

        if (Expiration < other.Expiration)
        {
            return -1;
        }

        if (Expiration > other.Expiration)
        {
            return 1;
        }

        if (LastTrade < other.LastTrade)
        {
            return -1;
        }

        if (LastTrade > other.LastTrade)
        {
            return 1;
        }

        var i = Multiplier.CompareTo(other.Multiplier);
        if (i != 0)
        {
            return i;
        }

        i = SPC.CompareTo(other.SPC);
        if (i != 0)
        {
            return i;
        }

        i = string.Compare(AdditionalUnderlyings, other.AdditionalUnderlyings, StringComparison.Ordinal);
        if (i != 0)
        {
            return i;
        }

        i = string.Compare(MMY, other.MMY, StringComparison.Ordinal);
        if (i != 0)
        {
            return i;
        }

        i = string.Compare(OptionType, other.OptionType, StringComparison.Ordinal);
        if (i != 0)
        {
            return i;
        }

        i = string.Compare(ExpirationStyle, other.ExpirationStyle, StringComparison.Ordinal);
        if (i != 0)
        {
            return i;
        }

        i = string.Compare(SettlementStyle, other.SettlementStyle, StringComparison.Ordinal);
        if (i != 0)
        {
            return i;
        }

        return string.Compare(CFI, other.CFI, StringComparison.Ordinal);
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj is not OptionSeries<T> other)
        {
            return false;
        }

        return Expiration == other.Expiration &&
               LastTrade == other.LastTrade &&
               Multiplier.Equals(other.Multiplier) &&
               SPC.Equals(other.SPC) &&
               AdditionalUnderlyings.Equals(other.AdditionalUnderlyings, StringComparison.Ordinal) &&
               ExpirationStyle.Equals(other.ExpirationStyle, StringComparison.Ordinal) &&
               MMY.Equals(other.MMY, StringComparison.Ordinal) &&
               OptionType.Equals(other.OptionType, StringComparison.Ordinal) &&
               CFI.Equals(other.CFI, StringComparison.Ordinal) &&
               SettlementStyle.Equals(other.SettlementStyle, StringComparison.Ordinal);
    }

    /// <summary>
    /// Returns a hash code value for this object.
    /// </summary>
    /// <returns>A hash code value for this object.</returns>
    [SuppressMessage(
        "ReSharper",
        "NonReadonlyMemberInGetHashCode",
        Justification = "Setters are only called in the OptionChainsBuilder")]
    public override int GetHashCode()
    {
        var result = Expiration;
        result = (31 * result) + LastTrade;
        var temp = !Multiplier.Equals(0) ? BitConverter.DoubleToInt64Bits(Multiplier) : 0L;
        result = (31 * result) + temp.GetHashCode();
        temp = !SPC.Equals(0) ? BitConverter.DoubleToInt64Bits(SPC) : 0L;
        result = (31 * result) + temp.GetHashCode();
        result = (31 * result) + AdditionalUnderlyings.GetHashCode();
        result = (31 * result) + MMY.GetHashCode();
        result = (31 * result) + OptionType.GetHashCode();
        result = (31 * result) + ExpirationStyle.GetHashCode();
        result = (31 * result) + SettlementStyle.GetHashCode();
        result = (31 * result) + CFI.GetHashCode();
        return result;
    }

    /// <summary>
    /// Returns a string representation of this series.
    /// </summary>
    /// <returns>The string representation of this series.</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("expiration=").Append(DayUtil.GetYearMonthDayByDayId(Expiration));
        if (LastTrade != 0)
        {
            sb.Append(", lastTrade=").Append(DayUtil.GetYearMonthDayByDayId(LastTrade));
        }

        if (!Multiplier.Equals(0))
        {
            sb.Append(", multiplier=").Append(Multiplier);
        }

        if (!SPC.Equals(0))
        {
            sb.Append(", spc=").Append(SPC);
        }

        if (!string.IsNullOrEmpty(AdditionalUnderlyings))
        {
            sb.Append(", additionalUnderlyings=").Append(AdditionalUnderlyings);
        }

        if (!string.IsNullOrEmpty(MMY))
        {
            sb.Append(", mmy=").Append(MMY);
        }

        if (!string.IsNullOrEmpty(OptionType))
        {
            sb.Append(", optionType=").Append(OptionType);
        }

        if (!string.IsNullOrEmpty(ExpirationStyle))
        {
            sb.Append(", expirationStyle=").Append(ExpirationStyle);
        }

        if (!string.IsNullOrEmpty(SettlementStyle))
        {
            sb.Append(", settlementStyle=").Append(SettlementStyle);
        }

        sb.Append(", cfi=").Append(CFI);
        return sb.ToString();
    }

    /// <summary>
    /// Adds an option to the series.
    /// </summary>
    /// <param name="isCall">Indicates whether the option is a call.</param>
    /// <param name="strike">The strike price of the option.</param>
    /// <param name="option">The option to add.</param>
    internal void AddOption(bool isCall, double strike, T option)
    {
        var map = isCall ? Calls : Puts;
        if (!map.ContainsKey(strike))
        {
            _strikes = null; // Clear cached strikes list.
        }

        map[strike] = option;
    }
}
