// <copyright file="OptionChainsBuilder.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using DxFeed.Graal.Net.Utils;

namespace DxFeed.Graal.Net.Ipf.Options;

/// <summary>
/// Builder class for a set of option chains grouped by product or underlying symbol.
///
/// <h3>Threads and clocks</h3>
///
/// This class is <b>NOT</b> thread-safe and cannot be used from multiple threads without external synchronization.
/// </summary>
/// <typeparam name="T">The type of option instrument instances.</typeparam>
public class OptionChainsBuilder<T>
{
    private readonly OptionSeries<T> _series = new();
    private string _product = string.Empty;
    private string _underlying = string.Empty;
    private string _cfi = string.Empty;

    /// <summary>
    /// Gets or sets the product for futures and options on futures (underlying asset name).
    /// Example: "/YG".
    /// </summary>
    public string Product
    {
        get => _product;
        set => _product = string.IsNullOrEmpty(value) ? string.Empty : value;
    }

    /// <summary>
    /// Gets or sets primary underlying symbol for options.
    /// </summary>
    /// <example>"C", "/YGM9".</example>
    public string Underlying
    {
        get => _underlying;
        set => _underlying = string.IsNullOrEmpty(value) ? string.Empty : value;
    }

    /// <summary>
    /// Gets or sets day id of expiration.
    /// </summary>
    /// <example><see cref="DayUtil.GetYearMonthDayByDayId">DayUtil.GetYearMonthDayByDayId(20090117)</see>.</example>
    public int Expiration
    {
        get => _series.Expiration;
        set => _series.Expiration = value;
    }

    /// <summary>
    /// Gets or sets day id of last trading day.
    /// </summary>
    /// <example><see cref="DayUtil.GetYearMonthDayByDayId">DayUtil.GetYearMonthDayByDayId(20090117)</see>.</example>
    public int LastTrade
    {
        get => _series.LastTrade;
        set => _series.LastTrade = value;
    }

    /// <summary>
    /// Gets or sets market value multiplier.
    /// </summary>
    /// <example>"100", "33.2".</example>
    public double Multiplier
    {
        get => _series.Multiplier;
        set => _series.Multiplier = value;
    }

    /// <summary>
    /// Gets or sets shares per contract for options.
    /// </summary>
    /// <example>"1", "100".</example>
    public double SPC
    {
        get => _series.SPC;
        set => _series.SPC = value;
    }

    /// <summary>
    /// Gets or sets additional underlyings for options, including additional cash.
    /// It shall use following format:
    /// <code>
    ///     &lt;VALUE&gt; ::= &lt;empty&gt; | &lt;LIST&gt;
    ///     &lt;LIST&gt; ::= &lt;AU&gt; | &lt;AU&gt; &lt;semicolon&gt; &lt;space&gt; &lt;LIST&gt;
    ///     &lt;AU&gt; ::= &lt;UNDERLYING&gt; &lt;space&gt; &lt;SPC&gt;
    /// </code>
    /// the list shall be sorted by &lt;UNDERLYING&gt;.
    /// </summary>
    /// <example>"SE 50", "FIS 53; US$ 45.46".</example>
    public string AdditionalUnderlyings
    {
        get => _series.AdditionalUnderlyings;
        set => _series.AdditionalUnderlyings = string.IsNullOrEmpty(value) ? string.Empty : value;
    }

    /// <summary>
    /// Gets or sets maturity month-year as provided for corresponding FIX tag (200).
    /// It can use several different formats depending on data source.
    /// <ul>
    ///     <li>YYYYMM – if only year and month are specified</li>
    ///     <li>YYYYMMDD – if full date is specified</li>
    ///     <li>YYYYMMwN – if week number (within a month) is specified</li>
    /// </ul>
    /// </summary>
    public string MMY
    {
        get => _series.MMY;
        set => _series.MMY = string.IsNullOrEmpty(value) ? string.Empty : value;
    }

    /// <summary>
    /// Gets or sets type of option.
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
    public string OptionType
    {
        get => _series.OptionType;
        set => _series.OptionType = string.IsNullOrEmpty(value) ? string.Empty : value;
    }

    /// <summary>
    /// Gets or sets expiration cycle style, such as "Weeklys", "Quarterlys".
    /// </summary>
    public string ExpirationStyle
    {
        get => _series.ExpirationStyle;
        set => _series.ExpirationStyle = string.IsNullOrEmpty(value) ? string.Empty : value;
    }

    /// <summary>
    /// Gets or sets settlement price determination style, such as "Open", "Close".
    /// </summary>
    public string SettlementStyle
    {
        get => _series.SettlementStyle;
        set => _series.SettlementStyle = string.IsNullOrEmpty(value) ? string.Empty : value;
    }

    /// <summary>
    /// Gets or sets classification of Financial Instruments code.
    /// It is a mandatory field for OPTION instruments as it is the only way to distinguish Call/Put type,
    /// American/European exercise, Cash/Physical delivery.
    /// It shall use six-letter CFI code from ISO 10962 standard.
    /// It is allowed to use 'X' extensively and to omit trailing letters (assumed to be 'X').
    /// See <a href="http://en.wikipedia.org/wiki/ISO_10962">ISO 10962 on Wikipedia</a>.
    /// </summary>
    /// <example>"ESNTPB", "ESXXXX", "ES", "OPASPS".</example>
    public string CFI
    {
        get => _cfi;
        set
        {
            _cfi = string.IsNullOrEmpty(value) ? string.Empty : value;
            _series.CFI = _cfi.Length < 2 ? _cfi : _cfi[0] + "X" + _cfi.Substring(2);
        }
    }

    /// <summary>
    /// Gets or sets strike price for options.
    /// </summary>
    /// <example>"80", "22.5".</example>
    public double Strike { get; set; }

    /// <summary>
    /// Gets a view of chains created by this builder.
    /// </summary>
    public Dictionary<string, OptionChain<T>> Chains { get; } = new();

    /// <summary>
    /// Builds option chains for all options from the given collections of <see cref="InstrumentProfile"/>.
    /// </summary>
    /// <param name="instruments">Collection of instrument profiles.</param>
    /// <returns>The builder with all the options from the instruments collection.</returns>
    public static OptionChainsBuilder<InstrumentProfile> Build(IEnumerable<InstrumentProfile> instruments)
    {
        var ocb = new OptionChainsBuilder<InstrumentProfile>();
        foreach (var ip in instruments)
        {
            if (!ip.Type.Equals("OPTION", StringComparison.Ordinal))
            {
                continue;
            }

            ocb.Product = ip.Product;
            ocb.Underlying = ip.Underlying;
            ocb.Expiration = ip.Expiration;
            ocb.LastTrade = ip.LastTrade;
            ocb.Multiplier = ip.Multiplier;
            ocb.SPC = ip.SPC;
            ocb.AdditionalUnderlyings = ip.AdditionalUnderlyings;
            ocb.MMY = ip.MMY;
            ocb.OptionType = ip.OptionType;
            ocb.ExpirationStyle = ip.ExpirationStyle;
            ocb.SettlementStyle = ip.SettlementStyle;
            ocb.CFI = ip.CFI;
            ocb.Strike = ip.Strike;
            ocb.AddOption(ip);
        }

        return ocb;
    }

    /// <summary>
    /// Adds an option instrument to this builder.
    /// Option is added to chains for the currently set <see cref="Product"/> and/or
    /// <see cref="Underlying"/> to the <see cref="OptionSeries{T}"/> that correspond
    /// to all other currently set attributes. This method is safe in the sense that it ignores
    /// illegal state of the builder. It only adds an option when all the following conditions are met:
    /// <ul>
    ///   <li><see cref="CFI"/> is set and starts with either "OC" for call or "OP" for put.</li>
    ///   <li><see cref="OptionSeries{T}.Expiration"/> is set and is not zero.</li>
    ///   <li><see cref="Strike"/> is set and is not <see cref="double.NaN"/> nor <see cref="double.IsInfinity"/>.</li>
    ///   <li><see cref="Product"/> or <see cref="Underlying"/> are set.</li>
    /// </ul>
    /// All the attributes remain set as before after the call to this method, but
    /// <see cref="Chains"/> are updated correspondingly.
    /// </summary>
    /// <param name="option">Option to add.</param>
    public void AddOption(T option)
    {
        var isCall = _cfi.StartsWith("OC", StringComparison.Ordinal);
        if (!isCall && !_cfi.StartsWith("OP", StringComparison.Ordinal))
        {
            return;
        }

        if (_series.Expiration == 0)
        {
            return;
        }

        if (double.IsNaN(Strike) || double.IsInfinity(Strike))
        {
            return;
        }

        if (!string.IsNullOrEmpty(_product))
        {
            GetOrCreateChain(_product).AddOption(_series, isCall, Strike, option);
        }

        if (!string.IsNullOrEmpty(_underlying))
        {
            GetOrCreateChain(_underlying).AddOption(_series, isCall, Strike, option);
        }
    }

    private OptionChain<T> GetOrCreateChain(string symbol)
    {
        if (!Chains.TryGetValue(symbol, out var chain))
        {
            chain = new OptionChain<T>(symbol);
            Chains[symbol] = chain;
        }

        return chain;
    }
}
