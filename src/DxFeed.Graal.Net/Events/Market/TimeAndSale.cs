// <copyright file="TimeAndSale.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Globalization;
using DxFeed.Graal.Net.Native.Events;
using DxFeed.Graal.Net.Utils;

namespace DxFeed.Graal.Net.Events.Market;

/// <summary>
/// Time and Sale represents a trade or other market event with price, like market open/close price, etc.
/// Time and Sales are intended to provide information about trades <b>in a continuous time slice</b>
/// (unlike <see cref="Trade"/> events which are supposed to provide snapshot about the <b>current last</b> trade).
/// <br/> Time and Sale events have unique <see cref="Index"/>
/// which can be used for later correction/cancellation processing.
/// <br/>
/// For more details see <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/event/market/TimeAndSale.html">Javadoc</a>.
/// </summary>
[EventCode(EventCodeNative.TimeAndSale)]
public class TimeAndSale : MarketEvent, ITimeSeriesEvent<string>
{
    /// <summary>
    /// Maximum allowed sequence value.
    /// <seealso cref="Sequence"/>
    /// </summary>
    public const int MaxSequence = (1 << 22) - 1;

    /*
     * Flags property has several significant bits that are packed into an integer in the following way:
     *   31..16   15...8    7    6    5    4    3    2    1    0
     * +--------+--------+----+----+----+----+----+----+----+----+
     * |        |   TTE  |    |  Side   | SL | ETH| VT |  Type   |
     * +--------+--------+----+----+----+----+----+----+----+----+
     */

    // TTE (TradeThroughExempt) values are ASCII chars in [0, 255].
    private const int TteMask = 0xff;
    private const int TteShift = 8;

    // SIDE values are taken from Side enum.
    private const int SideMask = 3;
    private const int SideShift = 5;

    private const int SpreadLeg = 1 << 4;
    private const int Eth = 1 << 3;
    private const int ValidTick = 1 << 2;

    // TYPE values are taken from TimeAndSaleType enum.
    private const int TypeMask = 3;
    private const int TypeShift = 0;

    /*
     * EventFlags property has several significant bits that are packed into an integer in the following way:
     *    31..7    6    5    4    3    2    1    0
     * +--------+----+----+----+----+----+----+----+
     * |        | SM |    | SS | SE | SB | RE | TX |
     * +--------+----+----+----+----+----+----+----+
    */

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeAndSale"/> class.
    /// </summary>
    public TimeAndSale()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeAndSale"/> class with the specified event symbol.
    /// </summary>
    /// <param name="eventSymbol">The specified event symbol.</param>
    public TimeAndSale(string? eventSymbol)
        : base(eventSymbol)
    {
    }

    /// <inheritdoc/>
    public IndexedEventSource EventSource =>
        IndexedEventSource.DEFAULT;

    /// <inheritdoc/>
    public int EventFlags { get; set; }

    /// <summary>
    /// Gets or sets unique per-symbol index of this time and sale event.
    /// Time and sale index is composed of <see cref="Time"/> and <see cref="Sequence"/>.
    /// Changing either time or sequence changes event index.
    /// <b>Do not sets this value directly.</b>
    /// Change <see cref="Time"/> and/or <see cref="Sequence"/>.
    /// </summary>
    public long Index { get; set; }

    /// <summary>
    /// Gets or sets timestamp of the original event.
    /// Time is measured in milliseconds between the current time and midnight, January 1, 1970 UTC.
    /// </summary>
    public long Time
    {
        get => ((Index >> 32) * 1000) + ((Index >> 22) & 0x3ff);
        set => Index = ((long)TimeUtil.GetSecondsFromTime(value) << 32) |
                       ((long)TimeUtil.GetMillisFromTime(value) << 22) | (uint)Sequence;
    }

    /// <summary>
    /// Gets or sets timestamp of the original event in nanoseconds.
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
    /// Gets or sets sequence number of this event to distinguish events that have the same <see cref="Time"/>.
    /// This sequence number does not have to be unique and
    /// does not need to be sequential. Sequence can range from 0 to <see cref="MaxSequence"/>.
    /// </summary>
    /// <exception cref="ArgumentException">If sequence out of range.</exception>
    public int Sequence
    {
        get => (int)Index & MaxSequence;
        set
        {
            if (value is < 0 or > MaxSequence)
            {
                throw new ArgumentException($"Sequence({value}) is < 0 or > MaxSequence({MaxSequence})", nameof(value));
            }

            Index = (Index & ~MaxSequence) | (uint)value;
        }
    }

    /// <summary>
    /// Gets or sets exchange code of this time and sale event.
    /// </summary>
    public char ExchangeCode { get; set; }

    /// <summary>
    /// Gets or sets price of this time and sale event.
    /// </summary>
    public double Price { get; set; }

    /// <summary>
    /// Gets or sets size of this time and sale event as floating number with fractions.
    /// </summary>
    public double Size { get; set; }

    /// <summary>
    /// Gets or sets the current bid price on the market when this time and sale event had occurred.
    /// </summary>
    public double BidPrice { get; set; }

    /// <summary>
    /// Gets or sets the current ask price on the market when this time and sale event had occurred.
    /// </summary>
    public double AskPrice { get; set; }

    /// <summary>
    /// Gets or sets sale conditions provided for this event by data feed.
    /// This field format is specific for every particular data feed.
    /// </summary>
    public string? ExchangeSaleConditions { get; set; }

    /// <summary>
    /// Gets or sets TradeThroughExempt flag of this time and sale event.
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
    /// Gets or sets aggressor side of this time and sale event.
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
    /// Gets or sets type of this time and sale event.
    /// </summary>
    public TimeAndSaleType Type
    {
        get => TimeAndSaleTypeExt.ValueOf(BitUtil.GetBits(Flags, TypeMask, TypeShift));
        set => Flags = BitUtil.SetBits(Flags, TypeMask, TypeShift, (int)value);
    }

    /// <summary>
    /// Gets a value indicating whether this is a new event (not cancellation or correction).
    /// It is <c>true</c> for newly created time and sale event.
    /// </summary>
    public bool IsNew =>
        Type == TimeAndSaleType.New;

    /// <summary>
    /// Gets a value indicating whether this is a correction of a previous event.
    /// It is <c>false</c> for newly created time and sale event.
    /// <c>true</c> if this is a correction of a previous event.
    /// </summary>
    public bool IsCorrection =>
        Type == TimeAndSaleType.Correction;

    /// <summary>
    /// Gets a value indicating whether this is a cancellation of a previous event.
    /// It is <c>false</c> for newly created time and sale event.
    /// <c>true</c> if this is a cancellation of a previous event.
    /// </summary>
    public bool IsCancel =>
        Type == TimeAndSaleType.Cancel;

    /// <summary>
    /// Gets or sets buyer of this time and sale event.
    /// </summary>
    public string? Buyer { get; set; }

    /// <summary>
    /// Gets or sets seller of this time and sale event.
    /// </summary>
    public string? Seller { get; set; }

    /// <summary>
    /// Gets or sets implementation-specific flags.
    /// <b>Do not use this method directly.</b>
    /// </summary>
    internal int Flags { get; set; }

    /// <summary>
    /// Returns string representation of this time and sale event.
    /// </summary>
    /// <returns>The string representation.</returns>
    public override string ToString() =>
        "TimeAndSale{" + StringUtil.EncodeNullableString(EventSymbol) +
        ", eventTime=" + TimeFormat.LocalTime.FromMillis(EventTime) +
        ", eventFlags=0x" + EventFlags.ToString("x", CultureInfo.InvariantCulture) +
        ", time=" + TimeFormat.LocalTime.WithMillis().FromMillis(Time) +
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
        (Buyer == null ? string.Empty : ", buyer='" + Buyer + "'") +
        (Seller == null ? string.Empty : ", seller='" + Seller + "'") +
        "}";
}
