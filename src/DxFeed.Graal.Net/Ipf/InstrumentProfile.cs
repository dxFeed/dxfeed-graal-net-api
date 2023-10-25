// <copyright file="InstrumentProfile.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Utils;

// Disable auto-property
#pragma warning disable S2292

namespace DxFeed.Graal.Net.Ipf;

/// <summary>
/// Represents basic profile information about market instrument.
/// Please see <a href="http://www.dxfeed.com/downloads/documentation/dxFeed_Instrument_Profile_Format.pdf">Instrument Profile Format documentation</a>
/// for complete description.
/// </summary>
public class InstrumentProfile
{
    private string type = string.Empty;
    private string symbol = string.Empty;
    private string description = string.Empty;
    private string localSymbol = string.Empty;
    private string localDescription = string.Empty;
    private string country = string.Empty;
    private string opol = string.Empty;
    private string exchangeData = string.Empty;
    private string exchanges = string.Empty;
    private string currency = string.Empty;
    private string baseCurrency = string.Empty;
    private string cfi = string.Empty;
    private string isin = string.Empty;
    private string sedol = string.Empty;
    private string cusip = string.Empty;
    private int icb;
    private int sic;
    private double multiplier;
    private string product = string.Empty;
    private string underlying = string.Empty;
    private double spc;
    private string additionalUnderlyings = string.Empty;
    private string mmy = string.Empty;
    private int expiration;
    private int lastTrade;
    private double strike;
    private string optionType = string.Empty;
    private string expirationStyle = string.Empty;
    private string settlementStyle = string.Empty;
    private string priceIncrements = string.Empty;
    private string tradingHours = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="InstrumentProfile"/> class.
    /// </summary>
    public InstrumentProfile()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InstrumentProfile"/> class,
    /// creating a deep copy of the provided instrument profile.
    /// </summary>
    /// <param name="ip">The <see cref="InstrumentProfile"/> instance to copy.</param>
    /// <remarks>
    /// This constructor is used for creating a new instance of the <see cref="InstrumentProfile"/>
    /// class with the same properties as the given instance.
    /// It performs a deep copy, ensuring that the new instance does not share references with the original,
    /// making it safe from modifications to the original instance.
    /// </remarks>
    public InstrumentProfile(InstrumentProfile ip)
    {
        type = ip.type;
        symbol = ip.symbol;
        description = ip.description;
        localSymbol = ip.localSymbol;
        localDescription = ip.localDescription;
        country = ip.country;
        opol = ip.opol;
        exchangeData = ip.exchangeData;
        exchanges = ip.exchanges;
        currency = ip.currency;
        baseCurrency = ip.baseCurrency;
        cfi = ip.cfi;
        isin = ip.isin;
        sedol = ip.sedol;
        cusip = ip.cusip;
        icb = ip.icb;
        sic = ip.sic;
        multiplier = ip.multiplier;
        product = ip.product;
        underlying = ip.underlying;
        spc = ip.spc;
        additionalUnderlyings = ip.additionalUnderlyings;
        mmy = ip.mmy;
        expiration = ip.expiration;
        lastTrade = ip.lastTrade;
        strike = ip.strike;
        optionType = ip.optionType;
        expirationStyle = ip.expirationStyle;
        settlementStyle = ip.settlementStyle;
        priceIncrements = ip.priceIncrements;
        tradingHours = ip.tradingHours;
    }

    /// <summary>
    /// Gets or sets type of instrument.
    /// It takes precedence in conflict cases with other fields.
    /// It is a mandatory field. It may not be empty.
    /// </summary>
    /// <example>"STOCK", "FUTURE", "OPTION".</example>
    public string Type
    {
        get => type;
        set => type = string.IsNullOrEmpty(value) ? string.Empty : value;
    }

    /// <summary>
    /// Gets or sets identifier of instrument,
    /// preferable an international one in Latin alphabet.
    /// It is a mandatory field. It may not be empty.
    /// </summary>
    /// <example>"GOOG", "/YGM9", ".ZYEAD".</example>
    public string Symbol
    {
        get => symbol;
        set => symbol = string.IsNullOrEmpty(value) ? string.Empty : value;
    }

    /// <summary>
    /// Gets or sets description of instrument,
    /// preferable an international one in Latin alphabet.
    /// </summary>
    /// <example>"Google Inc.", "Mini Gold Futures,Jun-2009,ETH".</example>
    public string Description
    {
        get => description;
        set => description = string.IsNullOrEmpty(value) ? string.Empty : value;
    }

    /// <summary>
    /// Gets or sets identifier of instrument in national language.
    /// It shall be empty if same as <see cref="Symbol"/>.
    /// </summary>
    public string LocalSymbol
    {
        get => localSymbol;
        set => localSymbol = string.IsNullOrEmpty(value) ? string.Empty : value;
    }

    /// <summary>
    /// Gets or sets description of instrument in national language.
    /// It shall be empty if same as <see cref="Description"/>.
    /// </summary>
    public string LocalDescription
    {
        get => localDescription;
        set => localDescription = string.IsNullOrEmpty(value) ? string.Empty : value;
    }

    /// <summary>
    /// Gets or sets country of origin (incorporation) of corresponding company or parent entity.
    /// It shall use two-letter country code from ISO 3166-1 standard.
    /// See <a href="http://en.wikipedia.org/wiki/ISO_3166-1">ISO 3166-1 on Wikipedia</a>.
    /// </summary>
    /// <example>"US", "RU".</example>
    public string Country
    {
        get => country;
        set => country = string.IsNullOrEmpty(value) ? string.Empty : value;
    }

    /// <summary>
    /// Gets or sets official Place Of Listing, the organization that have listed this instrument.
    /// Instruments with multiple listings shall use separate profiles for each listing.
    /// It shall use Market Identifier Code (MIC) from ISO 10383 standard.
    /// See <a href="http://en.wikipedia.org/wiki/ISO_10383">ISO 10383 on Wikipedia</a>
    /// or <a href="http://www.iso15022.org/MIC/homepageMIC.htm">MIC homepage</a>.
    /// </summary>
    /// <example>"XNAS", "RTSX".</example>
    public string OPOL
    {
        get => opol;
        set => opol = string.IsNullOrEmpty(value) ? string.Empty : value;
    }

    /// <summary>
    /// Gets or sets exchange-specific data required to properly identify instrument when communicating with exchange.
    /// It uses exchange-specific format.
    /// </summary>
    public string ExchangeData
    {
        get => exchangeData;
        set => exchangeData = string.IsNullOrEmpty(value) ? string.Empty : value;
    }

    /// <summary>
    /// Gets or sets list of exchanges where instrument is quoted or traded.
    /// Its shall use the following format:
    /// <code>
    ///     &lt;VALUE&gt; ::= &lt;empty&gt; | &lt;LIST&gt;
    ///     &lt;IST&gt; ::= &lt;MIC&gt; | &lt;MIC&gt; &lt;semicolon&gt;
    /// </code>
    /// &lt;LIST&gt; the list shall be sorted by MIC.
    /// </summary>
    /// <example>"ARCX;CBSX;XNAS;XNYS".</example>
    public string Exchanges
    {
        get => exchanges;
        set => exchanges = string.IsNullOrEmpty(value) ? string.Empty : value;
    }

    /// <summary>
    /// Gets or sets currency of quotation, pricing and trading.
    /// It shall use three-letter currency code from ISO 4217 standard.
    /// See <a href="http://en.wikipedia.org/wiki/ISO_4217">ISO 4217 on Wikipedia</a>.
    /// </summary>
    /// <example>"USD", "RUB".</example>
    public string Currency
    {
        get => currency;
        set => currency = string.IsNullOrEmpty(value) ? string.Empty : value;
    }

    /// <summary>
    /// Gets or sets base currency of currency pair (FOREX instruments).
    /// It shall use three-letter currency code similarly to <see cref="Currency"/>.
    /// </summary>
    public string BaseCurrency
    {
        get => baseCurrency;
        set => baseCurrency = string.IsNullOrEmpty(value) ? string.Empty : value;
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
        get => cfi;
        set => cfi = string.IsNullOrEmpty(value) ? string.Empty : value;
    }

    /// <summary>
    /// Gets or sets international Securities Identifying Number.
    /// It shall use twelve-letter code from ISO 6166 standard.
    /// See <a href="http://en.wikipedia.org/wiki/ISO_6166">ISO 6166 on Wikipedia</a>
    /// or <a href="http://en.wikipedia.org/wiki/International_Securities_Identifying_Number">ISIN on Wikipedia</a>.
    /// </summary>
    /// <example>"DE0007100000", "US38259P5089".</example>
    public string ISIN
    {
        get => isin;
        set => isin = string.IsNullOrEmpty(value) ? string.Empty : value;
    }

    /// <summary>
    /// Gets or sets stock Exchange Daily Official List.
    /// It shall use seven-letter code assigned by London Stock Exchange.
    /// See <a href="http://en.wikipedia.org/wiki/SEDOL">SEDOL on Wikipedia</a> or
    /// <a href="http://www.londonstockexchange.com/en-gb/products/informationproducts/sedol/">SEDOL on LSE</a>.
    /// </summary>
    /// <example>"2310967", "5766857".</example>
    public string SEDOL
    {
        get => sedol;
        set => sedol = string.IsNullOrEmpty(value) ? string.Empty : value;
    }

    /// <summary>
    /// Gets or sets committee on Uniform Security Identification Procedures code.
    /// It shall use nine-letter code assigned by CUSIP Services Bureau.
    /// See <a href="http://en.wikipedia.org/wiki/CUSIP">CUSIP on Wikipedia</a>.
    /// </summary>
    /// <example>"38259P508".</example>
    public string CUSIP
    {
        get => cusip;
        set => cusip = string.IsNullOrEmpty(value) ? string.Empty : value;
    }

    /// <summary>
    /// Gets or sets industry Classification Benchmark.
    /// It shall use four-digit number from ICB catalog.
    /// See <a href="http://en.wikipedia.org/wiki/Industry_Classification_Benchmark">ICB on Wikipedia</a>
    /// or <a href="http://www.icbenchmark.com/">ICB homepage</a>.
    /// </summary>
    /// <example>"9535".</example>
    public int ICB
    {
        get => icb;
        set => icb = value;
    }

    /// <summary>
    /// Gets or sets standard Industrial Classification.
    /// It shall use four-digit number from SIC catalog.
    /// See <a href="http://en.wikipedia.org/wiki/Standard_Industrial_Classification">SIC on Wikipedia</a>
    /// or <a href="https://www.osha.gov/pls/imis/sic_manual.html">SIC structure</a>.
    /// </summary>
    /// <example>"7371".</example>
    public int SIC
    {
        get => sic;
        set => sic = value;
    }

    /// <summary>
    /// Gets or sets market value multiplier.
    /// </summary>
    /// <example>"100", "33.2".</example>
    public double Multiplier
    {
        get => multiplier;
        set => multiplier = value;
    }

    /// <summary>
    /// Gets or sets product for futures and options on futures (underlying asset name).
    /// </summary>
    /// <example>"/YG".</example>
    public string Product
    {
        get => product;
        set => product = string.IsNullOrEmpty(value) ? string.Empty : value;
    }

    /// <summary>
    /// Gets or sets primary underlying symbol for options.
    /// </summary>
    /// <example>"C", "/YGM9".</example>
    public string Underlying
    {
        get => underlying;
        set => underlying = string.IsNullOrEmpty(value) ? string.Empty : value;
    }

    /// <summary>
    /// Gets or sets shares per contract for options.
    /// </summary>
    /// <example>"1", "100".</example>
    public double SPC
    {
        get => spc;
        set => spc = value;
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
        get => additionalUnderlyings;
        set => additionalUnderlyings = string.IsNullOrEmpty(value) ? string.Empty : value;
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
        get => mmy;
        set => mmy = string.IsNullOrEmpty(value) ? string.Empty : value;
    }

    /// <summary>
    /// Gets or sets day id of expiration.
    /// </summary>
    /// <example><see cref="DayUtil.GetYearMonthDayByDayId">DayUtil.GetYearMonthDayByDayId(20090117)</see>.</example>
    public int Expiration
    {
        get => expiration;
        set => expiration = value;
    }

    /// <summary>
    /// Gets or sets day id of last trading day.
    /// </summary>
    /// <example><see cref="DayUtil.GetYearMonthDayByDayId">DayUtil.GetYearMonthDayByDayId(20090117)</see>.</example>
    public int LastTrade
    {
        get => lastTrade;
        set => lastTrade = value;
    }

    /// <summary>
    /// Gets or sets strike price for options.
    /// </summary>
    /// <example>"80", "22.5".</example>
    public double Strike
    {
        get => strike;
        set => strike = value;
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
        get => optionType;
        set => optionType = string.IsNullOrEmpty(value) ? string.Empty : value;
    }

    /// <summary>
    /// Gets or sets expiration cycle style, such as "Weeklys", "Quarterlys".
    /// </summary>
    public string ExpirationStyle
    {
        get => expirationStyle;
        set => expirationStyle = string.IsNullOrEmpty(value) ? string.Empty : value;
    }

    /// <summary>
    /// Gets or sets settlement price determination style, such as "Open", "Close".
    /// </summary>
    public string SettlementStyle
    {
        get => settlementStyle;
        set => settlementStyle = string.IsNullOrEmpty(value) ? string.Empty : value;
    }

    /// <summary>
    /// Gets or sets minimum allowed price increments with corresponding price ranges.
    /// It shall use following format:
    /// <code>
    ///     &lt;VALUE&gt; ::= &lt;empty&gt; | &lt;LIST&gt;
    ///     &lt;LIST&gt; ::= &lt;INCREMENT&gt; | &lt;RANGE&gt; &lt;semicolon&gt; &lt;space&gt; &lt;LIST&gt;
    ///     &lt;RANGE&gt; ::= &lt;INCREMENT&gt; &lt;space&gt; &lt;UPPER_LIMIT&gt;
    /// </code>
    /// the list shall be sorted by &lt;UPPER_LIMIT&gt;.
    /// </summary>
    /// <example>"0.25", "0.01 3; 0.05".</example>
    public string PriceIncrements
    {
        get => priceIncrements;
        set => priceIncrements = string.IsNullOrEmpty(value) ? string.Empty : value;
    }

    /// <summary>
    /// Gets or sets trading hours specification.
    /// </summary>
    public string TradingHours
    {
        get => tradingHours;
        set => tradingHours = string.IsNullOrEmpty(value) ? string.Empty : value;
    }

    /// <summary>
    /// Returns a string representation of the instrument profile.
    /// </summary>
    /// <returns>The string representation.</returns>
    public override string ToString() =>
        $"{Type} {Symbol}";
}
