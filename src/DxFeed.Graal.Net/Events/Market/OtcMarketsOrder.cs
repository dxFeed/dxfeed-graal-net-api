// <copyright file="OtcMarketsOrder.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using DxFeed.Graal.Net.Native.Events;
using DxFeed.Graal.Net.Utils;

namespace DxFeed.Graal.Net.Events.Market;

/// <summary>
/// Represents an extension of <see cref="Order"/> for the symbols traded on the OTC Markets. It includes the OTC Markets specific quote data
/// For more information about original fields, QAP, Quote Flags and Extended Quote Flags,
/// see <a href="https://downloads.dxfeed.com/specifications/OTC_Markets_Multicast_Data_Feeds.pdf">OTC Markets Multicast Data Feed</a>.
/// </summary>
[EventCode(EventCodeNative.OtcMarketsOrder)]
public class OtcMarketsOrder : Order
{
    /*
     * OTC Markets flags property has several significant bits that are packed into an integer in the following way:
     *   31..7          6                5             4          3       2          1          0
     * +-------+-----------------+---------------+-----------+--------+-------+-------------+------+
     * |       | NMS Conditional | AutoExecution | Saturated | OTC Price Type | Unsolicited | Open |
     * +-------+-----------------+---------------+-----------+--------+-------+-------------+------+
     * |                Extended Quote Flags                 |             Quote Flags             |
     * +-----------------------------------------------------+-------------------------------------+
     */

    private const int NmsConditional = 3;
    private const int AutoExecution = 0;
    private const int Saturated = 0;

    // OTC_PRICE_TYPE values are taken from OtcMarketsPriceType enum.
    private const int OtcPriceTypeMask = 3;
    private const int OtcPriceTypeShift = 0;

    private const int Unsolicited = 0;
    private const int Open = 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="OtcMarketsOrder"/> class.
    /// </summary>
    public OtcMarketsOrder()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OtcMarketsOrder"/> class with the specified event symbol.
    /// </summary>
    /// <param name="eventSymbol">The specified event symbol.</param>
    public OtcMarketsOrder(string? eventSymbol)
        : base(eventSymbol)
    {
    }

    /// <summary>
    /// Gets or sets Quote Access Payment (QAP) of this OTC Markets order.
    ///
    /// QAP functionality allows participants to dynamically set access fees or rebates,
    /// in real-time and on a per-security basis through OTC Dealer or OTC FIX connections.
    /// Positive integers (1 to 30) indicate a rebate, and negative integers (-1 to -30) indicate an access fee.
    /// 0 indicates no rebate or access fee.
    /// </summary>
    public int QuoteAccessPayment { get; set; }

    /// <summary>
    /// Gets or sets transactional OTC Markets flags.
    /// </summary>
    public int OtcMarketsFlags { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this event is available for business within the operating hours of the OTC Link system.
    /// All quotes will be closed at the start of the trading day and will remain closed until the traders open theirs.
    /// </summary>
    public bool IsOpen
    {
        get => (OtcMarketsFlags & Open) != 0;
        set => OtcMarketsFlags = value ? OtcMarketsFlags | Open : OtcMarketsFlags & ~Open;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this event is unsolicited.
    /// </summary>
    public bool IsUnsolicited
    {
        get => (OtcMarketsFlags & Unsolicited) != 0;
        set => OtcMarketsFlags = value ? OtcMarketsFlags | Unsolicited : OtcMarketsFlags & ~Unsolicited;
    }

    /// <summary>
    /// Gets or sets OTC Markets price type of this OTC Markets order events.
    /// </summary>
    public OtcMarketsPriceType OtcMarketsPriceType
    {
        get => OtcMarketsPriceTypeExt.ValueOf(BitUtil.GetBits(OtcMarketsFlags, OtcPriceTypeMask, OtcPriceTypeShift));
        set => OtcMarketsFlags =
            BitUtil.SetBits(OtcMarketsFlags, OtcPriceTypeMask, OtcPriceTypeShift, (int)value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether this event should NOT be considered for the inside price.
    /// </summary>
    public bool IsSaturated
    {
        get => (OtcMarketsFlags & Saturated) != 0;
        set => OtcMarketsFlags = value ? OtcMarketsFlags | Saturated : OtcMarketsFlags & ~Saturated;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this event is in 'AutoEx' mode.
    /// If this event is in 'AutoEx' mode then a response to an OTC Link trade message will be immediate.
    /// </summary>
    public bool IsAutoExecution
    {
        get => (OtcMarketsFlags & AutoExecution) != 0;
        set => OtcMarketsFlags = value ? OtcMarketsFlags | AutoExecution : OtcMarketsFlags & ~AutoExecution;
    }

    /// <summary>
    /// Gets or sets a value indicating whether event represents a NMS conditional.
    /// This flag indicates the displayed <see cref="OrderBase.Size"/>
    /// is a round lot at least two times greater than the minimum round lot size in the security
    /// and a trade message relating to the event cannot be sent or filled for less than the displayed size.
    /// </summary>
    public bool IsNmsConditional
    {
        get => (OtcMarketsFlags & NmsConditional) != 0;
        set => OtcMarketsFlags = value ? OtcMarketsFlags | NmsConditional : OtcMarketsFlags & ~NmsConditional;
    }

    /// <summary>
    /// Returns string representation of this otc markets order event.
    /// </summary>
    /// <returns>The string representation.</returns>
    public override string ToString() =>
        "OtcMarketsOrder{" + BaseFieldsToString() +
        ", QAP=" + QuoteAccessPayment +
        ", open=" + Open +
        ", unsolicited=" + Unsolicited +
        ", priceType=" + OtcMarketsPriceType +
        ", saturated=" + Saturated +
        ", autoEx=" + AutoExecution +
        ", NMS=" + NmsConditional +
        "}";
}
