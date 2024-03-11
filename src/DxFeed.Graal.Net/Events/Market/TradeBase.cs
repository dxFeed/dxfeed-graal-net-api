// <copyright file="TradeBase.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using DxFeed.Graal.Net.Utils;
using DxFeed.Graal.Net.Utils.Time;

namespace DxFeed.Graal.Net.Events.Market;

/// <summary>
/// Base class for common fields of <see cref="Trade"/> and <see cref="TradeETH"/> events.
/// Trade events represent the most recent information that is available about the last trade on the market
/// at any given moment of time.
/// <br/>
/// For more details see <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/event/market/TradeBase.html">Javadoc</a>.
/// </summary>
public abstract class TradeBase : MarketEvent, ILastingEvent
{
    /// <summary>
    /// Maximum allowed sequence value.
    /// <seealso cref="Sequence"/>
    /// </summary>
    public const int MaxSequence = (1 << 22) - 1;

    /*
     * Flags property has several significant bits that are packed into an integer in the following way:
     *   31..4     3    2    1    0
     * +--------+----+----+----+----+
     * |        |  Direction   | ETH|
     * +--------+----+----+----+----+
     */

    // DIRECTION values are taken from Direction enum.
    private const int DirectionMask = 7;
    private const int DirectionShift = 1;

    // ETH mask.
    private const int Eth = 1;

    /// <summary>
    /// Initializes a new instance of the <see cref="TradeBase"/> class.
    /// </summary>
    protected TradeBase()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TradeBase"/> class with the specified event symbol.
    /// </summary>
    /// <param name="eventSymbol">The specified event symbol.</param>
    protected TradeBase(string? eventSymbol)
        : base(eventSymbol)
    {
    }

    /// <summary>
    /// Gets or sets time and sequence of last trade packaged into single long value.
    /// <b>Do not set this property directly.</b>
    /// Sets <see cref="Time"/> and/or <see cref="Sequence"/>.
    /// </summary>
    public long TimeSequence { get; set; }

    /// <summary>
    /// Gets or sets time of the last trade.
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
    /// Gets or sets microseconds and nanoseconds time part of the last trade.
    /// </summary>
    public int TimeNanoPart { get; set; }

    /// <summary>
    /// Gets or sets sequence number of the last trade to distinguish trades that have the same <see cref="Time"/>.
    /// This sequence number does not have to be unique and
    /// does not need to be sequential. Sequence can range from 0 to <see cref="MaxSequence"/>.
    /// </summary>
    /// <exception cref="ArgumentException">If sequence out of range.</exception>
    public int Sequence
    {
        get => (int)(TimeSequence & MaxSequence);
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
    /// Gets or sets exchange code of the last trade.
    /// </summary>
    public char ExchangeCode { get; set; }

    /// <summary>
    /// Gets or sets price of the last trade.
    /// </summary>
    public double Price { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets size of this last trade event as floating number with fractions.
    /// </summary>
    public double Size { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets identifier of the current trading day.
    /// Identifier of the day is the number of days passed since January 1, 1970.
    /// </summary>
    public int DayId { get; set; }

    /// <summary>
    /// Gets or sets total volume traded for a day as floating number with fractions.
    /// </summary>
    public double DayVolume { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets total turnover traded for a day.
    /// Day VWAP can be computed with  <see cref="DayTurnover"/> / <see cref="DayVolume"/>.
    /// </summary>
    public double DayTurnover { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets tick direction of the last trade.
    /// </summary>
    public Direction TickDirection
    {
        get => DirectionExt.ValueOf(BitUtil.GetBits(Flags, DirectionMask, DirectionShift));
        set => Flags = BitUtil.SetBits(Flags, DirectionMask, DirectionShift, (int)value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether last trade was in extended trading hours.
    /// </summary>
    public bool IsExtendedTradingHours
    {
        get => (Flags & Eth) != 0;
        set => Flags = value ? Flags | Eth : Flags & ~Eth;
    }

    /// <summary>
    /// Gets or sets change of the last trade.
    /// </summary>
    public double Change { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets implementation-specific flags.
    /// <b>Do not use this method directly.</b>
    /// </summary>
    internal int Flags { get; set; }

    /// <summary>
    /// Returns string representation of this base trade event's.
    /// </summary>
    /// <returns>The string representation.</returns>
    public override string ToString() =>
        GetType().Name + "{" + BaseFieldsToString() + "}";

    /// <summary>
    /// Returns string representation of this trade fields.
    /// </summary>
    /// <returns>The string representation.</returns>
    protected string BaseFieldsToString() =>
        StringUtil.EncodeNullableString(EventSymbol) +
        ", eventTime=" + DXTimeFormat.Default().WithMillis().Format(EventTime) +
        ", time=" + DXTimeFormat.Default().WithMillis().Format(Time) +
        ", timeNanoPart=" + TimeNanoPart +
        ", sequence=" + Sequence +
        ", exchange=" + StringUtil.EncodeChar(ExchangeCode) +
        ", price=" + Price +
        ", change=" + Change +
        ", size=" + Size +
        ", day=" + DayUtil.GetYearMonthDayByDayId(DayId) +
        ", dayVolume=" + DayVolume +
        ", dayTurnover=" + DayTurnover +
        ", direction=" + TickDirection +
        ", ETH=" + IsExtendedTradingHours;
}
