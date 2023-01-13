// <copyright file="Quote.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using DxFeed.Graal.Net.Native.Events;
using DxFeed.Graal.Net.Utils;

namespace DxFeed.Graal.Net.Events.Market;

/// <summary>
/// Quote event is a snapshot of the best bid and ask prices, and other fields that change with each quote.
/// It represents the most recent information that is available about the best quote on the market
/// at any given moment of time.
/// <br/>
/// For more details see <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/event/market/Quote.html">Javadoc</a>.
/// </summary>
[EventCode(EventCodeNative.Quote)]
public class Quote : MarketEvent, ILastingEvent
{
    /// <summary>
    /// Maximum allowed sequence value.
    /// <seealso cref="Sequence"/>
    /// </summary>
    public const int MaxSequence = (1 << 22) - 1;

    private long _bidTime;
    private long _askTime;

    /// <summary>
    /// Initializes a new instance of the <see cref="Quote"/> class.
    /// </summary>
    public Quote()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Quote"/> class with the specified event symbol.
    /// </summary>
    /// <param name="eventSymbol">The specified event symbol.</param>
    public Quote(string? eventSymbol)
        : base(eventSymbol)
    {
    }

    /// <summary>
    /// Gets or sets sequence number of this quote to distinguish quotes that have the same <see cref="Time"/>.
    /// This sequence number does not have to be unique and
    /// does not need to be sequential. Sequence can range from 0 to <see cref="MaxSequence"/>.
    /// </summary>
    /// <exception cref="ArgumentException">If sequence out of range.</exception>
    public int Sequence
    {
        get => TimeMillisSequence & MaxSequence;
        set
        {
            if (value is < 0 or > MaxSequence)
            {
                throw new ArgumentException($"Sequence({value}) is < 0 or > MaxSequence({MaxSequence})", nameof(value));
            }

            TimeMillisSequence = (TimeMillisSequence & ~MaxSequence) | value;
        }
    }

    /// <summary>
    /// Gets time of the last bid or ask change.
    /// Time is measured in milliseconds between the current time and midnight, January 1, 1970 UTC.
    /// </summary>
    public long Time =>
        (MathUtil.FloorDiv(Math.Max(_bidTime, _askTime), 1000) * 1000) + ((uint)TimeMillisSequence >> 22);

    /// <summary>
    /// Gets time of the last bid or ask change in nanoseconds.
    /// Time is measured in nanoseconds between the current time and midnight, January 1, 1970 UTC.
    /// </summary>
    public long TimeNanos =>
        TimeNanosUtil.GetNanosFromMillisAndNanoPart(Time, TimeNanoPart);

    /// <summary>
    /// Gets or sets microseconds and nanoseconds part of time of the last bid or ask change.
    /// <b>This method changes <see cref="TimeNanos"/> result.</b>
    /// </summary>
    public int TimeNanoPart { get; set; }

    /// <summary>
    /// Gets or sets time of the last bid change.
    /// Time is measured in milliseconds between the current time and midnight, January 1, 1970 UTC.
    /// <b>This time is always transmitted with seconds precision, so the result of this method is
    /// usually a multiple of 1000.</b>
    /// <br/>
    /// You can set the actual millisecond-precision time here to publish event and
    /// the millisecond part will make the time of this quote even precise up to a millisecond.
    /// </summary>
    public long BidTime
    {
        get => _bidTime;
        set
        {
            _bidTime = value;
            RecomputeTimeMillisPart();
        }
    }

    /// <summary>
    /// Gets or sets bid exchange code.
    /// </summary>
    public char BidExchangeCode { get; set; }

    /// <summary>
    /// Gets or sets bid price.
    /// </summary>
    public double BidPrice { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets bid size.
    /// </summary>
    public double BidSize { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets time of the last ask change.
    /// Time is measured in milliseconds between the current time and midnight, January 1, 1970 UTC.
    /// <b>This time is always transmitted with seconds precision, so the result of this method is
    /// usually a multiple of 1000.</b>
    /// <br/>
    /// You can set the actual millisecond-precision time here to publish event and
    /// the millisecond part will make the time of this quote even precise up to a millisecond.
    /// </summary>
    public long AskTime
    {
        get => _askTime;
        set
        {
            _askTime = value;
            RecomputeTimeMillisPart();
        }
    }

    /// <summary>
    /// Gets or sets ask exchange code.
    /// </summary>
    public char AskExchangeCode { get; set; }

    /// <summary>
    /// Gets or sets ask price.
    /// </summary>
    public double AskPrice { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets ask size.
    /// </summary>
    public double AskSize { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets time millis sequence.
    /// <b>Do not sets this value directly.</b>
    /// Change <see cref="Sequence"/> and/or <see cref="Time"/>.
    /// </summary>
    internal int TimeMillisSequence { get; set; }

    /// <summary>
    /// Returns string representation of this quote event.
    /// </summary>
    /// <returns>The string representation.</returns>
    public override string ToString() =>
        "Quote{" + StringUtil.EncodeNullableString(EventSymbol) +
        ", eventTime=" + TimeFormat.LocalTime.WithMillis().FromMillis(EventTime) +
        ", time=" + TimeFormat.LocalTime.WithMillis().FromMillis(Time) +
        ", timeNanoPart=" + TimeNanoPart +
        ", sequence=" + Sequence +
        ", bidTime=" + TimeFormat.LocalTime.FromMillis(BidTime) +
        ", bidExchange=" + StringUtil.EncodeChar(BidExchangeCode) +
        ", bidPrice=" + BidPrice +
        ", bidSize=" + BidSize +
        ", askTime=" + TimeFormat.LocalTime.FromMillis(AskTime) +
        ", askExchange=" + StringUtil.EncodeChar(AskExchangeCode) +
        ", askPrice=" + AskPrice +
        ", askSize=" + AskSize +
        '}';

    private void RecomputeTimeMillisPart()
        => TimeMillisSequence = (TimeUtil.GetMillisFromTime(Math.Max(_askTime, _bidTime)) << 22) | Sequence;
}
