// <copyright file="Summary.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Native.Events;
using DxFeed.Graal.Net.Utils;

namespace DxFeed.Graal.Net.Events.Market;

/// <summary>
/// Summary information snapshot about the trading session including session highs, lows, etc.
/// It represents the most recent information that is available about the trading session in
/// the market at any given moment of time.
/// <br/>
/// For more details see <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/event/market/Summary.html">Javadoc</a>.
/// </summary>
[EventCode(EventCodeNative.Summary)]
public class Summary : MarketEvent, ILastingEvent
{
    /*
     * Flags property has several significant bits that are packed into an integer in the following way:
     *   31..4     3    2    1    0
     * +--------+----+----+----+----+
     * |        |  Close  |PrevClose|
     * +--------+----+----+----+----+
     */

    // PRICE_TYPE values are taken from PriceType enum.
    private const int DayClosePriceTypeMask = 3;
    private const int DayClosePriceTypeShift = 2;

    // PRICE_TYPE values are taken from PriceType enum.
    private const int PrevDayClosePriceTypeMask = 3;
    private const int PrevDayClosePriceTypeShift = 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="Summary"/> class.
    /// </summary>
    public Summary()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Summary"/> class with the specified event symbol.
    /// </summary>
    /// <param name="eventSymbol">The specified event symbol.</param>
    public Summary(string? eventSymbol)
        : base(eventSymbol)
    {
    }

    /// <summary>
    /// Gets or sets identifier of the day that this summary represents.
    /// Identifier of the day is the number of days passed since January 1, 1970.
    /// </summary>
    public int DayId { get; set; }

    /// <summary>
    /// Gets or sets the first (open) price for the day.
    /// </summary>
    public double DayOpenPrice { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets the maximal (high) price for the day.
    /// </summary>
    public double DayHighPrice { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets the minimal (low) price for the day.
    /// </summary>
    public double DayLowPrice { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets the last (close) price for the day.
    /// </summary>
    public double DayClosePrice { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets the price type of the last (close) price for the day.
    /// </summary>
    public PriceType DayClosePriceType
    {
        get => PriceTypeExt.ValueOf(BitUtil.GetBits(Flags, DayClosePriceTypeMask, DayClosePriceTypeShift));
        set => Flags = BitUtil.SetBits(Flags, DayClosePriceTypeMask, DayClosePriceTypeShift, (int)value);
    }

    /// <summary>
    /// Gets or sets identifier of the previous day that this summary represents.
    /// Identifier of the day is the number of days passed since January 1, 1970.
    /// </summary>
    public int PrevDayId { get; set; }

    /// <summary>
    /// Gets or sets the last (close) price for the previous day.
    /// </summary>
    public double PrevDayClosePrice { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets the price type of the last (close) price for the previous day.
    /// </summary>
    public PriceType PrevDayClosePriceType
    {
        get => PriceTypeExt.ValueOf(BitUtil.GetBits(Flags, PrevDayClosePriceTypeMask, PrevDayClosePriceTypeShift));
        set => Flags = BitUtil.SetBits(Flags, PrevDayClosePriceTypeMask, PrevDayClosePriceTypeShift, (int)value);
    }

    /// <summary>
    /// Gets or sets total volume traded for the previous day.
    /// </summary>
    public double PrevDayVolume { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets open interest of the symbol as the number of open contracts.
    /// </summary>
    public long OpenInterest { get; set; }

    /// <summary>
    /// Gets or sets implementation-specific flags.
    /// <b>Do not use this method directly.</b>
    /// </summary>
    internal int Flags { get; set; }

    /// <summary>
    /// Returns string representation of this summary event.
    /// </summary>
    /// <returns>The string representation.</returns>
    public override string ToString() =>
        "Summary{" + StringUtil.EncodeNullableString(EventSymbol) +
        ", eventTime=" + TimeFormat.Default.WithMillis().Format(EventTime) +
        ", day=" + DayUtil.GetYearMonthDayByDayId(DayId) +
        ", dayOpen=" + DayOpenPrice +
        ", dayHigh=" + DayHighPrice +
        ", dayLow=" + DayLowPrice +
        ", dayClose=" + DayClosePrice +
        ", dayCloseType=" + DayClosePriceType +
        ", prevDay=" + DayUtil.GetYearMonthDayByDayId(PrevDayId) +
        ", prevDayClose=" + PrevDayClosePrice +
        ", prevDayCloseType=" + PrevDayClosePriceType +
        ", prevDayVolume=" + PrevDayVolume +
        ", openInterest=" + OpenInterest +
        "}";
}
