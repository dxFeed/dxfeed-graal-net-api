// <copyright file="Candle.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Globalization;
using DxFeed.Graal.Net.Native.Events;
using DxFeed.Graal.Net.Utils;

namespace DxFeed.Graal.Net.Events.Candles;

/// <summary>
/// Candle event with open, high, low, close prices and other information for a specific period.
/// Candles are build with a specified <see cref="CandlePeriod"/> using a specified <see cref="CandlePrice"/> type
/// with a data taken from the specified <see cref="CandleExchange"/> from the specified <see cref="CandleSession"/>
/// with further details of aggregation provided by <see cref="CandleAlignment"/>.
/// </summary>
[EventCode(EventCodeNative.Candle)]
public class Candle : ITimeSeriesEvent, ILastingEvent
{
    /// <summary>
    /// Maximum allowed sequence value.
    /// <seealso cref="Sequence"/>
    /// </summary>
    public const int MaxSequence = (1 << 22) - 1;

    /*
     * EventFlags property has several significant bits that are packed into an integer in the following way:
     *    31..7    6    5    4    3    2    1    0
     * +---------+----+----+----+----+----+----+----+
     * |         | SM |    | SS | SE | SB | RE | TX |
     * +---------+----+----+----+----+----+----+----+
     */

    /// <summary>
    /// Initializes a new instance of the <see cref="Candle"/> class.
    /// </summary>
    public Candle()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Candle"/> class with the specified event symbol.
    /// </summary>
    /// <param name="eventSymbol">The specified event symbol.</param>
    public Candle(CandleSymbol eventSymbol) =>
        CandleSymbol = eventSymbol;

    /// <inheritdoc/>
    public string? EventSymbol
    {
        get => CandleSymbol?.ToString();
        set => CandleSymbol = CandleSymbol.ValueOf(value);
    }

    /// <summary>
    /// Gets or sets candle symbol object.
    /// </summary>
    public CandleSymbol? CandleSymbol { get; set; }

    /// <inheritdoc/>
    public long EventTime { get; set; }

    /// <inheritdoc cref="ITimeSeriesEvent.EventSource" />
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
    /// Gets or sets total number of original trade (or quote) events in this candle.
    /// </summary>
    public long Count { get; set; }

    /// <summary>
    /// Gets or sets the first (open) price of this candle.
    /// </summary>
    public double Open { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets the maximal (high) price of this candle.
    /// </summary>
    public double High { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets the minimal (low) price of this candle.
    /// </summary>
    public double Low { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets the last (close) price of this candle.
    /// </summary>
    public double Close { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets total volume in this candle as floating number with fractions.
    /// Total turnover in this candle can be computed with <c>VWAP * Volume</c>.
    /// </summary>
    public double Volume { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets volume-weighted average price (VWAP) in this candle.
    /// Total turnover in this candle can be computed with <c>VWAP * Volume</c>.
    /// </summary>
    public double VWAP { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets bid volume in this candle as floating number with fractions.
    /// </summary>
    public double BidVolume { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets ask volume in this candle as floating number with fractions.
    /// </summary>
    public double AskVolume { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets implied volatility.
    /// </summary>
    public double ImpVolatility { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets open interest as floating number with fractions.
    /// </summary>
    public double OpenInterest { get; set; } = double.NaN;

    /// <summary>
    /// Returns string representation of this candle event.
    /// </summary>
    /// <returns>The string representation.</returns>
    public override string ToString() =>
        "Candle{" + BaseFieldsToString() + "}";

    /// <summary>
    /// Returns string representation of this candle fields.
    /// </summary>
    /// <returns>The string representation.</returns>
    protected string BaseFieldsToString() =>
        StringUtil.EncodeNullableString(EventSymbol) +
        ", eventTime=" + TimeFormat.Local.WithMillis().WithTimeZone().FormatFromMillis(EventTime) +
        ", eventFlags=0x" + EventFlags.ToString("x", CultureInfo.InvariantCulture) +
        ", time=" + TimeFormat.Local.WithMillis().WithTimeZone().FormatFromMillis(Time) +
        ", sequence=" + Sequence +
        ", count=" + Count +
        ", open=" + Open +
        ", high=" + High +
        ", low=" + Low +
        ", close=" + Close +
        ", volume=" + Volume +
        ", vwap=" + VWAP +
        ", bidVolume=" + BidVolume +
        ", askVolume=" + AskVolume +
        ", impVolatility=" + ImpVolatility +
        ", openInterest=" + OpenInterest;
}
