// <copyright file="Profile.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Native.Events;
using DxFeed.Graal.Net.Utils;

namespace DxFeed.Graal.Net.Events.Market;

/// <summary>
/// Profile information snapshot that contains security instrument description.
/// It represents the most recent information that is available about the traded security
/// on the market at any given moment of time.
/// <br/>
/// For more details see <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/event/market/Profile.html">Javadoc</a>.
/// </summary>
[EventCode(EventCodeNative.Profile)]
public class Profile : MarketEvent, ILastingEvent
{
    /*
     * Flags property has several significant bits that are packed into an integer in the following way:
     *   31..4     3    2    1    0
     * +--------+----+----+----+----+
     * |        |   SSR   |  Status |
     * +--------+----+----+----+----+
     */

    // SSR values are taken from ShortSaleRestriction enum.
    private const int SsrMask = 3;
    private const int SsrShift = 2;

    // STATUS values are taken from TradingStatus enum.
    private const int StatusMask = 3;
    private const int StatusShift = 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="Profile"/> class.
    /// </summary>
    public Profile()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Profile"/> class with the specified event symbol.
    /// </summary>
    /// <param name="eventSymbol">The specified event symbol.</param>
    public Profile(string? eventSymbol)
        : base(eventSymbol)
    {
    }

    /// <summary>
    /// Gets or sets description of the security instrument.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets short sale restriction of the security instrument.
    /// </summary>
    public ShortSaleRestriction ShortSaleRestriction
    {
        get => ShortSaleRestrictionExt.ValueOf(BitUtil.GetBits(Flags, SsrMask, SsrShift));
        set => Flags = BitUtil.SetBits(Flags, SsrMask, SsrShift, (int)value);
    }

    /// <summary>
    /// Gets a value indicating whether short sale of the security instrument is restricted.
    /// </summary>
    public bool IsShortSaleRestricted =>
        ShortSaleRestriction == ShortSaleRestriction.Active;

    /// <summary>
    /// Gets or sets trading status of the security instrument.
    /// </summary>
    public TradingStatus TradingStatus
    {
        get => TradingStatusExt.ValueOf(BitUtil.GetBits(Flags, StatusMask, StatusShift));
        set => Flags = BitUtil.SetBits(Flags, StatusMask, StatusShift, (int)value);
    }

    /// <summary>
    /// Gets a value indicating whether trading of the security instrument is halted.
    /// </summary>
    public bool IsTradingHalted =>
        TradingStatus == TradingStatus.Halted;

    /// <summary>
    /// Gets or sets description of the reason that trading was halted.
    /// </summary>
    public string? StatusReason { get; set; }

    /// <summary>
    /// Gets or sets starting time of the trading halt interval.
    /// Time is measured in milliseconds between the current time and midnight, January 1, 1970 UTC.
    /// </summary>
    public long HaltStartTime { get; set; }

    /// <summary>
    /// Gets or sets ending time of the trading halt interval.
    /// Time is measured in milliseconds between the current time and midnight, January 1, 1970 UTC.
    /// </summary>
    public long HaltEndTime { get; set; }

    /// <summary>
    /// Gets or sets the maximal (high) allowed price.
    /// </summary>
    public double HighLimitPrice { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets the minimal (low) allowed price.
    /// </summary>
    public double LowLimitPrice { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets the maximal (high) price in last 52 weeks.
    /// </summary>
    public double High52WeekPrice { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets the minimal (low) price in last 52 weeks.
    /// </summary>
    public double Low52WeekPrice { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets the correlation coefficient of the instrument to the S&amp;P500 index.
    /// </summary>
    public double Beta { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets the earnings per share (the company’s profits divided by the number of shares).
    /// </summary>
    public double EarningsPerShare { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets the frequency of cash dividends payments per year (calculated).
    /// </summary>
    public double DividendFrequency { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets the amount of the last paid dividend.
    /// </summary>
    public double ExDividendAmount { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets the identifier of the day of the last dividend payment (ex-dividend date).
    /// Identifier of the day is the number of days passed since January 1, 1970.
    /// </summary>
    public int ExDividendDayId { get; set; }

    /// <summary>
    /// Gets or sets the the number of shares outstanding.
    /// </summary>
    public double Shares { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets the free-float - the number of shares outstanding that are available to the public for trade.
    /// </summary>
    public double FreeFloat { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets implementation-specific flags.
    /// <b>Do not use this method directly.</b>
    /// </summary>
    internal int Flags { get; set; }

    /// <summary>
    /// Returns string representation of this profile event.
    /// </summary>
    /// <returns>The string representation.</returns>
    public override string ToString() =>
        "Profile{" + BaseFieldsToString() + "}";

    /// <summary>
    /// Returns string representation of this order fields.
    /// </summary>
    /// <returns>The string representation.</returns>
    protected string BaseFieldsToString() =>
        StringUtil.EncodeNullableString(EventSymbol) +
        ", eventTime=" + DXTimeFormat.DefaultWithMillis().Format(EventTime) +
        ", description='" + StringUtil.EncodeNullableString(Description) + "'" +
        ", SSR=" + ShortSaleRestriction +
        ", status=" + TradingStatus +
        ", statusReason='" + StringUtil.EncodeNullableString(StatusReason) + "'" +
        ", haltStartTime=" + DXTimeFormat.DefaultWithMillis().Format(HaltStartTime) +
        ", haltEndTime=" + DXTimeFormat.DefaultWithMillis().Format(HaltEndTime) +
        ", highLimitPrice=" + HighLimitPrice +
        ", lowLimitPrice=" + LowLimitPrice +
        ", high52WeekPrice=" + High52WeekPrice +
        ", low52WeekPrice=" + Low52WeekPrice +
        ", beta=" + Beta +
        ", earningsPerShare=" + EarningsPerShare +
        ", dividendFrequency=" + DividendFrequency +
        ", exDividendAmount=" + ExDividendAmount +
        ", exDividendDay=" + DayUtil.GetYearMonthDayByDayId(ExDividendDayId) +
        ", shares=" + Shares +
        ", freeFloat=" + FreeFloat;
}
