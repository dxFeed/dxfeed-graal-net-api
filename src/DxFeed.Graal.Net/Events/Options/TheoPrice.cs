// <copyright file="TheoPrice.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Globalization;
using DxFeed.Graal.Net.Events.Market;
using DxFeed.Graal.Net.Native.Events;
using DxFeed.Graal.Net.Utils;

namespace DxFeed.Graal.Net.Events.Options;

/// <summary>
/// Theo price is a snapshot of the theoretical option price computation that is
/// periodically performed by <a href="http://www.devexperts.com/en/products/price.html">dxPrice</a>
/// model-free computation.
/// It represents the most recent information that is available about the corresponding
/// values at any given moment of time.
/// The values include first and second order derivative of the price curve by price, so that
/// the real-time theoretical option price can be estimated on real-time changes of the underlying
/// price in the vicinity.
/// <br/>
/// For more details see <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/event/option/TheoPrice.html">Javadoc</a>.
/// </summary>
[EventCode(EventCodeNative.TheoPrice)]
public class TheoPrice : MarketEvent, ITimeSeriesEvent, ILastingEvent
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
    /// Initializes a new instance of the <see cref="TheoPrice"/> class.
    /// </summary>
    public TheoPrice()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TheoPrice"/> class with the specified event symbol.
    /// </summary>
    /// <param name="eventSymbol">The specified event symbol.</param>
    public TheoPrice(string? eventSymbol)
        : base(eventSymbol)
    {
    }

    /// <summary>
    /// Gets a source for this event.
    /// This method always returns <see cref="IndexedEventSource.DEFAULT"/>.
    /// </summary>
    public IndexedEventSource EventSource =>
        IndexedEventSource.DEFAULT;

    /// <inheritdoc/>
    public int EventFlags { get; set; }

    /// <summary>
    /// Gets or sets unique per-symbol index of this event.
    /// The index is composed of <see cref="Time"/> and <see cref="Sequence"/>,
    /// invocation of this method changes time and sequence.
    /// <b>Do not use this method directly.</b>
    /// Change <see cref="Time"/> and/or <see cref="Sequence"/>.
    /// </summary>
    public long Index { get; set; }

    /// <summary>
    /// Gets or sets timestamp of the event in milliseconds.
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
    /// Gets or sets theoretical option price.
    /// </summary>
    public double Price { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets underlying price at the time of theo price computation.
    /// </summary>
    public double UnderlyingPrice { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets delta of the theoretical price.
    /// Delta is the first derivative of an option price by an underlying price.
    /// </summary>
    public double Delta { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets gamma of the theoretical price.
    /// Gamma is the second derivative of an option price by an underlying price.
    /// </summary>
    public double Gamma { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets implied simple dividend return of the corresponding option series.
    /// </summary>
    public double Dividend { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets implied simple interest return of the corresponding option series.
    /// </summary>
    public double Interest { get; set; } = double.NaN;

    /// <summary>
    /// Returns string representation of this theo price event.
    /// </summary>
    /// <returns>The string representation.</returns>
    public override string ToString() =>
        "TheoPrice{" + StringUtil.EncodeNullableString(EventSymbol) +
        ", eventTime=" + DXTimeFormat.Default().WithMillis().Format(EventTime) +
        ", eventFlags=0x" + EventFlags.ToString("x", CultureInfo.InvariantCulture) +
        ", time=" + DXTimeFormat.Default().WithMillis().Format(Time) +
        ", sequence=" + Sequence +
        ", price=" + Price +
        ", underlyingPrice=" + UnderlyingPrice +
        ", delta=" + Delta +
        ", gamma=" + Gamma +
        ", dividend=" + Dividend +
        ", interest=" + Interest +
        "}";
}
