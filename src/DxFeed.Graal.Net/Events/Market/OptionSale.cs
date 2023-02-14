// <copyright file="OptionSale.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Globalization;
using DxFeed.Graal.Net.Native.Events;
using DxFeed.Graal.Net.Utils;
using static DxFeed.Graal.Net.Events.Market.TimeAndSale;

namespace DxFeed.Graal.Net.Events.Market;

/// <summary>
/// Option Sale event represents a trade or another market event with the price
/// (for example, market open/close price, etc.) for each option symbol listed under the specified Underlying.
/// Option Sales are intended to provide information about option trades <b>in a continuous time slice</b> with
/// the additional metrics, like Option Volatility, Option Delta, and Underlying Price.
/// <br/> Option Sale events have unique <see cref="Index"/>
/// which can be used for later correction/cancellation processing.
/// <br/>
/// For more details see <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/event/market/OptionSale.html">Javadoc</a>.
/// </summary>
[EventCode(EventCodeNative.OptionSale)]
public class OptionSale : MarketEvent, IIndexedEvent
{
    /// <summary>
    /// Maximum allowed sequence value.
    /// <seealso cref="Sequence"/>
    /// </summary>
    public const int MaxSequence = (1 << 22) - 1;

    /*
     * EventFlags property has several significant bits that are packed into an integer in the following way:
     *    31..7    6    5    4    3    2    1    0
     * +--------+----+----+----+----+----+----+----+
     * |        | SM |    | SS | SE | SB | RE | TX |
     * +--------+----+----+----+----+----+----+----+
    */

    /// <summary>
    /// Initializes a new instance of the <see cref="OptionSale"/> class.
    /// </summary>
    public OptionSale()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OptionSale"/> class with the specified event symbol.
    /// </summary>
    /// <param name="eventSymbol">The specified event symbol.</param>
    public OptionSale(string? eventSymbol)
        : base(eventSymbol)
    {
    }

    /// <inheritdoc cref="ITimeSeriesEvent.EventSource" />
    public IndexedEventSource EventSource =>
        IndexedEventSource.DEFAULT;

    /// <inheritdoc/>
    public int EventFlags { get; set; }

    /// <summary>
    /// Gets or sets unique per-symbol index of this option sale event.
    /// </summary>
    public long Index { get; set; }

    /// <summary>
    /// Gets or sets time and sequence of of this event.
    /// <b>Do not set this property directly.</b>
    /// Sets <see cref="Time"/> and/or <see cref="Sequence"/>.
    /// </summary>
    public long TimeSequence { get; set; }

    /// <summary>
    /// Gets or sets timestamp of the original event.
    /// Time is measured in milliseconds between the current time and midnight, January 1, 1970 UTC.
    /// </summary>
    public long Time
    {
        get => ((TimeSequence >> 32) * 1000) + ((TimeSequence >> 22) & 0x3ff);
        set => TimeSequence = ((long)TimeUtil.GetSecondsFromTime(value) << 32) |
                              ((long)TimeUtil.GetMillisFromTime(value) << 22) | (uint)Sequence;
    }

    /// <summary>
    /// Gets or sets time of the last trade in nanoseconds.
    /// Time is measured in nanoseconds between the current time and midnight, January 1, 1970 UTC.
    /// </summary>
    public long TimeNanos
    {
        get => TimeNanosUtil.GetNanosFromMillisAndNanoPart(Time, TimeNanoPart);
        set
        {
            Time = TimeNanosUtil.GetNanoPartFromNanos(value);
            TimeNanoPart = TimeNanosUtil.GetNanoPartFromNanos(value);
        }
    }

    /// <summary>
    /// Gets or sets microseconds and nanoseconds time part of event.
    /// </summary>
    public int TimeNanoPart { get; set; }

    /// <summary>
    /// Gets or sets sequence number sequence number of this order to distinguish orders that have the same
    /// <see cref="Time"/>. This sequence number does not have to be unique and
    /// does not need to be sequential. Sequence can range from 0 to <see cref="MaxSequence"/>.
    /// </summary>
    /// <exception cref="ArgumentException">If sequence out of range.</exception>
    public int Sequence
    {
        get => (int)TimeSequence & MaxSequence;
        set
        {
            if (value is < 0 or > MaxSequence)
            {
                throw new ArgumentException($"Sequence({value}) is < 0 or > MaxSequence({MaxSequence})", nameof(value));
            }

            TimeSequence = (TimeSequence & ~MaxSequence) | (uint)value;
        }
    }

    /// <summary>
    /// Gets or sets exchange code of this option sale event.
    /// </summary>
    public char ExchangeCode { get; set; }

    /// <summary>
    /// Gets or sets price of this option sale event.
    /// </summary>
    public double Price { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets size this option sale event as floating number with fractions.
    /// </summary>
    public double Size { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets the current bid price on the market when this option sale event had occurred.
    /// </summary>
    public double BidPrice { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets the current ask price on the market when this option sale event had occurred.
    /// </summary>
    public double AskPrice { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets sale conditions provided for this event by data feed.
    /// This field format is specific for every particular data feed.
    /// </summary>
    public string? ExchangeSaleConditions { get; set; }

    /// <summary>
    /// Gets or sets TradeThroughExempt flag of this option sale event.
    /// </summary>
    public char TradeThroughExempt
    {
        get => (char)BitUtil.GetBits(Flags, TteMask, TteShift);
        set
        {
            StringUtil.CheckChar(value, TteMask, "tradeThroughExempt");
            Flags = BitUtil.SetBits(Flags, TteMask, TteShift, value);
        }
    }


    /// <summary>
    /// Gets or sets aggressor side of this option sale event.
    /// </summary>
    public Side AggressorSide
    {
        get => SideExt.ValueOf(BitUtil.GetBits(Flags, SideMask, SideShift));
        set => Flags = BitUtil.SetBits(Flags, SideMask, SideShift, (int)value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether this event represents a spread leg.
    /// </summary>
    public bool IsSpreadLeg
    {
        get => (Flags & SpreadLeg) != 0;
        set => Flags = value ? Flags | SpreadLeg : Flags & ~SpreadLeg;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this event represents an extended trading hours sale.
    /// </summary>
    public bool IsExtendedTradingHours
    {
        get => (Flags & Eth) != 0;
        set => Flags = value ? Flags | Eth : Flags & ~Eth;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this event represents a valid intraday tick.
    /// Note, that a correction for a previously distributed valid tick represents a new valid tick itself,
    /// but a cancellation of a previous valid tick does not.
    /// </summary>
    public bool IsValidTick
    {
        get => (Flags & ValidTick) != 0;
        set => Flags = value ? Flags | ValidTick : Flags & ~ValidTick;
    }

    /// <summary>
    /// Gets or sets type of this option sale event.
    /// </summary>
    public TimeAndSaleType Type
    {
        get => TimeAndSaleTypeExt.ValueOf(BitUtil.GetBits(Flags, TypeMask, TypeShift));
        set => Flags = BitUtil.SetBits(Flags, TypeMask, TypeShift, (int)value);
    }

    /// <summary>
    /// Gets a value indicating whether this is a new event (not cancellation or correction).
    /// It is <c>true</c> for newly created option sale event.
    /// </summary>
    public bool IsNew =>
        Type == TimeAndSaleType.New;

    /// <summary>
    /// Gets a value indicating whether this is a correction of a previous event.
    /// It is <c>false</c> for newly created optionsale event.
    /// <c>true</c> if this is a correction of a previous event.
    /// </summary>
    public bool IsCorrection =>
        Type == TimeAndSaleType.Correction;

    /// <summary>
    /// Gets a value indicating whether this is a cancellation of a previous event.
    /// It is <c>false</c> for newly created option sale event.
    /// <c>true</c> if this is a cancellation of a previous event.
    /// </summary>
    public bool IsCancel =>
        Type == TimeAndSaleType.Cancel;

    /// <summary>
    /// Gets or sets underlying price at the time of this option sale event.
    /// </summary>
    public double UnderlyingPrice { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets Black-Scholes implied volatility of the option at the time of this option sale event.
    /// </summary>
    public double Volatility { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets option delta at the time of this option sale event.
    /// Delta is the first derivative of an option price by an underlying price.
    /// </summary>
    public double Delta { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets option symbol of this event.
    /// </summary>
    public string? OptionSymbol { get; set; }

    /// <summary>
    /// Gets or sets implementation-specific flags.
    /// <b>Do not use this method directly.</b>
    /// </summary>
    internal int Flags { get; set; }

    /// <summary>
    /// Returns string representation of this option sale event.
    /// </summary>
    /// <returns>The string representation.</returns>
    public override string ToString() =>
        "OptionSale{" + StringUtil.EncodeNullableString(EventSymbol) +
        ", eventTime=" + TimeFormat.Local.WithMillis().WithTimeZone().FormatFromMillis(EventTime) +
        ", eventFlags=0x" + EventFlags.ToString("x", CultureInfo.InvariantCulture) +
        ", index=0x" + Index.ToString("x", CultureInfo.InvariantCulture) +
        ", time=" + TimeFormat.Local.WithMillis().WithTimeZone().FormatFromMillis(Time) +
        ", timeNanoPart=" + TimeNanoPart +
        ", sequence=" + Sequence +
        ", exchange=" + StringUtil.EncodeChar(ExchangeCode) +
        ", price=" + Price +
        ", size=" + Size +
        ", bid=" + BidPrice +
        ", ask=" + AskPrice +
        ", ESC='" + StringUtil.EncodeNullableString(ExchangeSaleConditions) + "'" +
        ", TTE=" + StringUtil.EncodeChar(TradeThroughExempt) +
        ", side=" + AggressorSide +
        ", spread=" + IsSpreadLeg +
        ", ETH=" + IsExtendedTradingHours +
        ", validTick=" + IsValidTick +
        ", type=" + Type +
        ", underlyingPrice=" + UnderlyingPrice +
        ", volatility=" + Volatility +
        ", delta=" + Delta +
        ", optionSymbol='" + StringUtil.EncodeNullableString(OptionSymbol) + "'" +
        "}";
}
