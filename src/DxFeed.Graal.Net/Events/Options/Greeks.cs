// <copyright file="Greeks.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Globalization;
using DxFeed.Graal.Net.Events.Market;
using DxFeed.Graal.Net.Native.Events;
using DxFeed.Graal.Net.Utils;
using DxFeed.Graal.Net.Utils.Time;

namespace DxFeed.Graal.Net.Events.Options;

/// <summary>
/// Greeks event is a snapshot of the option price, Black-Scholes volatility and greeks.
/// It represents the most recent information that is available about the corresponding values on
/// the market at any given moment of time.
/// <br/>
/// For more details see <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/event/option/Greeks.html">Javadoc</a>.
/// </summary>
[EventCode(EventCodeNative.Greeks)]
public class Greeks : MarketEvent, ITimeSeriesEvent, ILastingEvent
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
    /// Initializes a new instance of the <see cref="Greeks"/> class.
    /// </summary>
    public Greeks()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Greeks"/> class with the specified event symbol.
    /// </summary>
    /// <param name="eventSymbol">The specified event symbol.</param>
    public Greeks(string? eventSymbol)
        : base(eventSymbol)
    {
    }

    /// <inheritdoc cref="ITimeSeriesEvent.EventSource" />
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
    /// Gets or sets option market price.
    /// </summary>
    public double Price { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets Black-Scholes implied volatility of the option.
    /// </summary>
    public double Volatility { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets option delta.
    /// Delta is the first derivative of an option price by an underlying price.
    /// </summary>
    public double Delta { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets option gamma.
    /// Gamma is the second derivative of an option price by an underlying price.
    /// </summary>
    public double Gamma { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets option theta.
    /// Theta is the first derivative of an option price by a number of days to expiration.
    /// </summary>
    public double Theta { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets option rho.
    /// Rho is the first derivative of an option price by percentage interest rate.
    /// </summary>
    public double Rho { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets vega.
    /// Vega is the first derivative of an option price by percentage volatility.
    /// </summary>
    public double Vega { get; set; } = double.NaN;

    /// <summary>
    /// Returns string representation of this greeks event.
    /// </summary>
    /// <returns>The string representation.</returns>
    public override string ToString() =>
        "Greeks{" + StringUtil.EncodeNullableString(EventSymbol) +
        ", eventTime=" + TimeFormat.Default.WithMillis().Format(EventTime) +
        ", eventFlags=0x" + EventFlags.ToString("x", CultureInfo.InvariantCulture) +
        ", time=" + TimeFormat.Default.WithMillis().Format(Time) +
        ", sequence=" + Sequence +
        ", price=" + Price +
        ", volatility=" + Volatility +
        ", delta=" + Delta +
        ", gamma=" + Gamma +
        ", theta=" + Theta +
        ", rho=" + Rho +
        ", vega=" + Vega +
        "}";
}
