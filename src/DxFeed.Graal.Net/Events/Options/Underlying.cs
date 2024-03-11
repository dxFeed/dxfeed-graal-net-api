// <copyright file="Underlying.cs" company="Devexperts LLC">
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
/// Underlying event is a snapshot of computed values that are available for an option underlying
/// symbol based on the option prices on the market.
/// It represents the most recent information that is available about the corresponding values on
/// the market at any given moment of time.
/// <br/>
/// For more details see <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/event/option/Underlying.html">Javadoc</a>.
/// </summary>
[EventCode(EventCodeNative.Underlying)]
public class Underlying : MarketEvent, ITimeSeriesEvent, ILastingEvent
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
    /// Initializes a new instance of the <see cref="Underlying"/> class.
    /// </summary>
    public Underlying()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Underlying"/> class with the specified event symbol.
    /// </summary>
    /// <param name="eventSymbol">The specified event symbol.</param>
    public Underlying(string? eventSymbol)
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
    /// Gets or sets day id of expiration.
    /// </summary>
    /// <seealso cref="DayUtil.GetDayIdByYearMonthDay(int)"/>.
    /// <example>
    /// <code>
    /// DayUtil.GetDayIdByYearMonthDay(20090117)
    /// </code>
    /// </example>
    public int Expiration { get; set; }

    /// <summary>
    /// Gets or sets 30-day implied volatility for this underlying based on VIX methodology.
    /// </summary>
    public double Volatility { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets front month implied volatility for this underlying based on VIX methodology.
    /// </summary>
    public double FrontVolatility { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets back month implied volatility for this underlying based on VIX methodology.
    /// </summary>
    public double BackVolatility { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets call options traded volume for a day.
    /// </summary>
    public double CallVolume { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets put options traded volume for a day.
    /// </summary>
    public double PutVolume { get; set; } = double.NaN;

    /// <summary>
    /// Gets options traded volume for a day.
    /// </summary>
    public double OptionVolume
    {
        get
        {
            if (double.IsNaN(PutVolume))
            {
                return CallVolume;
            }

            return double.IsNaN(CallVolume) ? PutVolume : PutVolume + CallVolume;
        }
    }

    /// <summary>
    /// Gets or sets ratio of put options traded volume to call options traded volume for a day.
    /// </summary>
    public double PutCallRatio { get; set; } = double.NaN;

    /// <summary>
    /// Returns string representation of this underlying event.
    /// </summary>
    /// <returns>The string representation.</returns>
    public override string ToString() =>
        "Underlying{" + BaseFieldsToString() + "}";

    /// <summary>
    /// Returns string representation of this underlying fields.
    /// </summary>
    /// <returns>The string representation.</returns>
    protected string BaseFieldsToString() =>
        StringUtil.EncodeNullableString(EventSymbol) +
        ", eventTime=" + DXTimeFormat.Default().WithMillis().Format(EventTime) +
        ", eventFlags=0x" + EventFlags.ToString("x", CultureInfo.InvariantCulture) +
        ", time=" + DXTimeFormat.Default().WithMillis().Format(Time) +
        ", sequence=" + Sequence +
        ", volatility=" + Volatility +
        ", frontVolatility=" + FrontVolatility +
        ", backVolatility=" + BackVolatility +
        ", callVolume=" + CallVolume +
        ", putVolume=" + PutVolume +
        ", putCallRatio=" + PutCallRatio;
}
