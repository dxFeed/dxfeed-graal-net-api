// <copyright file="InstrumentProfile.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Collections.Generic;
using DxFeed.Graal.Net.Native.Ipf;
using DxFeed.Graal.Net.Utils;

namespace DxFeed.Graal.Net.Ipf;

/// <summary>
/// Represents basic profile information about market instrument.
/// Please see <a href="http://www.dxfeed.com/downloads/documentation/dxFeed_Instrument_Profile_Format.pdf">Instrument Profile Format documentation</a>
/// for complete description.
/// </summary>
public class InstrumentProfile
{
    private readonly InstrumentProfileHandle handle;

    /// <summary>
    /// Initializes a new instance of the <see cref="InstrumentProfile"/> class.
    /// </summary>
    public InstrumentProfile() =>
        handle = InstrumentProfileHandle.Create();

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
    public InstrumentProfile(InstrumentProfile ip) =>
        handle = InstrumentProfileHandle.Create(ip.GetHandle());

    internal InstrumentProfile(InstrumentProfileHandle handle) =>
        this.handle = handle;

    /// <summary>
    /// Gets or sets type of instrument.
    /// It takes precedence in conflict cases with other fields.
    /// It is a mandatory field. It may not be empty.
    /// </summary>
    /// <example>"STOCK", "FUTURE", "OPTION".</example>
    public string Type
    {
        get => handle.Type;
        set => handle.Type = value;
    }

    /// <summary>
    /// Gets or sets identifier of instrument,
    /// preferable an international one in Latin alphabet.
    /// It is a mandatory field. It may not be empty.
    /// </summary>
    /// <example>"GOOG", "/YGM9", ".ZYEAD".</example>
    public string Symbol
    {
        get => handle.Symbol;
        set => handle.Symbol = value;
    }

    /// <summary>
    /// Gets or sets description of instrument,
    /// preferable an international one in Latin alphabet.
    /// </summary>
    /// <example>"Google Inc.", "Mini Gold Futures,Jun-2009,ETH".</example>
    public string Description
    {
        get => handle.Description;
        set => handle.Description = value;
    }

    /// <summary>
    /// Gets or sets identifier of instrument in national language.
    /// It shall be empty if same as <see cref="Symbol"/>.
    /// </summary>
    public string LocalSymbol
    {
        get => handle.LocalSymbol;
        set => handle.LocalSymbol = value;
    }

    /// <summary>
    /// Gets or sets description of instrument in national language.
    /// It shall be empty if same as <see cref="Description"/>.
    /// </summary>
    public string LocalDescription
    {
        get => handle.LocalDescription;
        set => handle.LocalDescription = value;
    }

    /// <summary>
    /// Gets or sets country of origin (incorporation) of corresponding company or parent entity.
    /// It shall use two-letter country code from ISO 3166-1 standard.
    /// See <a href="http://en.wikipedia.org/wiki/ISO_3166-1">ISO 3166-1 on Wikipedia</a>.
    /// </summary>
    /// <example>"US", "RU".</example>
    public string Country
    {
        get => handle.Country;
        set => handle.Country = value;
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
        get => handle.OPOL;
        set => handle.OPOL = value;
    }

    /// <summary>
    /// Gets or sets exchange-specific data required to properly identify instrument when communicating with exchange.
    /// It uses exchange-specific format.
    /// </summary>
    public string ExchangeData
    {
        get => handle.ExchangeData;
        set => handle.ExchangeData = value;
    }

    /// <summary>
    /// Gets or sets list of exchanges where instrument is quoted or traded.
    /// It shall use the following format:
    /// <code>
    ///     &lt;VALUE&gt; ::= &lt;empty&gt; | &lt;LIST&gt;
    ///     &lt;IST&gt; ::= &lt;MIC&gt; | &lt;MIC&gt; &lt;semicolon&gt;
    /// </code>
    /// &lt;LIST&gt; the list shall be sorted by MIC.
    /// </summary>
    /// <example>"ARCX;CBSX;XNAS;XNYS".</example>
    public string Exchanges
    {
        get => handle.Exchanges;
        set => handle.Exchanges = value;
    }

    /// <summary>
    /// Gets or sets currency of quotation, pricing and trading.
    /// It shall use three-letter currency code from ISO 4217 standard.
    /// See <a href="http://en.wikipedia.org/wiki/ISO_4217">ISO 4217 on Wikipedia</a>.
    /// </summary>
    /// <example>"USD", "RUB".</example>
    public string Currency
    {
        get => handle.Currency;
        set => handle.Currency = value;
    }

    /// <summary>
    /// Gets or sets base currency of currency pair (FOREX instruments).
    /// It shall use three-letter currency code similarly to <see cref="Currency"/>.
    /// </summary>
    public string BaseCurrency
    {
        get => handle.BaseCurrency;
        set => handle.BaseCurrency = value;
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
        get => handle.CFI;
        set => handle.CFI = value;
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
        get => handle.ISIN;
        set => handle.ISIN = value;
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
        get => handle.SEDOL;
        set => handle.SEDOL = value;
    }

    /// <summary>
    /// Gets or sets committee on Uniform Security Identification Procedures code.
    /// It shall use nine-letter code assigned by CUSIP Services Bureau.
    /// See <a href="http://en.wikipedia.org/wiki/CUSIP">CUSIP on Wikipedia</a>.
    /// </summary>
    /// <example>"38259P508".</example>
    public string CUSIP
    {
        get => handle.CUSIP;
        set => handle.CUSIP = value;
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
        get => handle.ICB;
        set => handle.ICB = value;
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
        get => handle.SIC;
        set => handle.SIC = value;
    }

    /// <summary>
    /// Gets or sets market value multiplier.
    /// </summary>
    /// <example>"100", "33.2".</example>
    public double Multiplier
    {
        get => handle.Multiplier;
        set => handle.Multiplier = value;
    }

    /// <summary>
    /// Gets or sets product for futures and options on futures (underlying asset name).
    /// </summary>
    /// <example>"/YG".</example>
    public string Product
    {
        get => handle.Product;
        set => handle.Product = value;
    }

    /// <summary>
    /// Gets or sets primary underlying symbol for options.
    /// </summary>
    /// <example>"C", "/YGM9".</example>
    public string Underlying
    {
        get => handle.Underlying;
        set => handle.Underlying = value;
    }

    /// <summary>
    /// Gets or sets shares per contract for options.
    /// </summary>
    /// <example>"1", "100".</example>
    public double SPC
    {
        get => handle.SPC;
        set => handle.SPC = value;
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
        get => handle.AdditionalUnderlyings;
        set => handle.AdditionalUnderlyings = value;
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
        get => handle.MMY;
        set => handle.MMY = value;
    }

    /// <summary>
    /// Gets or sets day id of expiration.
    /// </summary>
    /// <example><see cref="DayUtil.GetYearMonthDayByDayId">DayUtil.GetYearMonthDayByDayId(20090117)</see>.</example>
    public int Expiration
    {
        get => handle.Expiration;
        set => handle.Expiration = value;
    }

    /// <summary>
    /// Gets or sets day id of last trading day.
    /// </summary>
    /// <example><see cref="DayUtil.GetYearMonthDayByDayId">DayUtil.GetYearMonthDayByDayId(20090117)</see>.</example>
    public int LastTrade
    {
        get => handle.LastTrade;
        set => handle.LastTrade = value;
    }

    /// <summary>
    /// Gets or sets strike price for options.
    /// </summary>
    /// <example>"80", "22.5".</example>
    public double Strike
    {
        get => handle.Strike;
        set => handle.Strike = value;
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
        get => handle.OptionType;
        set => handle.OptionType = value;
    }

    /// <summary>
    /// Gets or sets expiration cycle style, such as "Weeklys", "Quarterlys".
    /// </summary>
    public string ExpirationStyle
    {
        get => handle.ExpirationStyle;
        set => handle.ExpirationStyle = value;
    }

    /// <summary>
    /// Gets or sets settlement price determination style, such as "Open", "Close".
    /// </summary>
    public string SettlementStyle
    {
        get => handle.SettlementStyle;
        set => handle.SettlementStyle = value;
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
        get => handle.PriceIncrements;
        set => handle.PriceIncrements = value;
    }

    /// <summary>
    /// Gets or sets trading hours specification.
    /// See <see cref="Schedules.Schedule.GetInstance(string)">Schedule.GetInstance(string)</see>.
    /// </summary>
    public string TradingHours
    {
        get => handle.TradingHours;
        set => handle.TradingHours = value;
    }

    /// <summary>
    /// Gets the value of the field with the specified name.
    /// </summary>
    /// <param name="name">The name of the field.</param>
    /// <returns>The field value, or an empty string if the field does not exist.</returns>
    public string GetField(string name) =>
        handle.GetField(name);

    /// <summary>
    /// Sets the value of the field with the specified name.
    /// </summary>
    /// <param name="name">The name of the field.</param>
    /// <param name="value">The value to set for the field.</param>
    public void SetField(string name, string value) =>
        handle.SetField(name, value);

    /// <summary>
    /// Gets the numeric value of the field with the specified name.
    /// </summary>
    /// <param name="name">The name of the field.</param>
    /// <returns>The numeric field value.</returns>
    public double GetNumericField(string name) =>
        handle.GetNumericField(name);

    /// <summary>
    /// Sets the numeric value of the field with the specified name.
    /// </summary>
    /// <param name="name">The name of the field.</param>
    /// <param name="value">The numeric value to set for the field.</param>
    public void SetNumericField(string name, double value) =>
        handle.SetNumericField(name, value);

    /// <summary>
    /// Gets the day id value of the date field with the specified name.
    /// See <see cref="DayUtil.GetYearMonthDayByDayId(int)"/>.
    /// </summary>
    /// <param name="name">The name of the date field.</param>
    /// <returns>The day id field value.</returns>
    public int GetDateField(string name) =>
        handle.GetDateField(name);

    /// <summary>
    /// Sets the day id value of the date field with the specified name.
    /// See <see cref="DayUtil.GetDayIdByYearMonthDay(int)"/>.
    /// </summary>
    /// <param name="name">The name of the date field.</param>
    /// <param name="value">The day id value to set for the date field.</param>
    public void SetDateField(string name, int value) =>
        handle.SetDateField(name, value);

    /// <summary>
    /// Adds names of non-empty custom fields to the specified collection.
    /// </summary>
    /// <param name="targetFieldNames">
    /// The collection to which the names of non-empty custom fields will be added.
    /// </param>
    /// <returns>
    /// <c>true</c> if <paramref name="targetFieldNames"/> changed as a result of the call; otherwise, <c>false</c>.
    /// </returns>
    public bool AddNonEmptyCustomFieldNames(ICollection<string> targetFieldNames) =>
        handle.AddNonEmptyCustomFieldNames(targetFieldNames);

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj) =>
        obj is InstrumentProfile ip && handle.Equals(ip.GetHandle());

    /// <summary>
    /// Returns a hash code value for this object.
    /// </summary>
    /// <returns>A hash code value for this object.</returns>
    public override int GetHashCode() =>
        handle.GetHashCode();

    /// <summary>
    /// Returns a string representation of the instrument profile.
    /// </summary>
    /// <returns>The string representation.</returns>
    public override string ToString() =>
       handle.ToString();

    internal InstrumentProfileHandle GetHandle() =>
        handle;
}
