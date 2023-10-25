// <copyright file="OrderBase.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Globalization;
using DxFeed.Graal.Net.Utils;

namespace DxFeed.Graal.Net.Events.Market;

/// <summary>
/// Base class for common fields of <see cref="Order"/>, <see cref="AnalyticOrder"/>
/// and <see cref="SpreadOrder"/> events.
/// Order events represent a snapshot for a full available market depth for a symbol.
/// The collection of order events of a symbol represents the most recent information that is
/// available about orders on the market at any given moment of time.
/// <br/>
/// <see cref="Order"/> event represents market depth for a <b>specific symbol</b>.
/// <br/>
/// <see cref="AnalyticOrder"/> event represents market depth for a <b>specific symbol</b>
/// extended with an analytic information, for example, whether particular order represent an iceberg or not.
/// <br/>
/// <see cref="SpreadOrder"/> event represents market depth for <b>all spreads on a given underlying symbol</b>.
/// <br/>
/// For more details see <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/event/market/OrderBase.html">Javadoc</a>.
/// </summary>
public abstract class OrderBase : MarketEvent, IIndexedEvent
{
    /// <summary>
    /// Maximum allowed sequence value.
    /// <seealso cref="Sequence"/>
    /// </summary>
    public const int MaxSequence = (1 << 22) - 1;

    /*
     * Flags property has several significant bits that are packed into an integer in the following way:
     *   31..15   14..11    10..4    3    2    1    0
     * +--------+--------+--------+----+----+----+----+
     * |        | Action |Exchange|  Side   |  Scope  |
     * +--------+--------+--------+----+----+----+----+
     */

    // ACTION values are taken from OrderAction enum.
    private const int ActionMask = 0x0f;
    private const int ActionShift = 11;

    // EXCHANGE values are ASCII chars in [0, 127].
    private const int ExchangeMask = 0x7f;
    private const int ExchangeShift = 4;

    // SIDE values are taken from Side enum.
    private const int SideMask = 3;
    private const int SideShift = 2;

    // SCOPE values are taken from Scope enum.
    private const int ScopeMask = 3;
    private const int ScopeShift = 0;

    /*
     * Index field contains source identifier, optional exchange code and low-end index (virtual id or MMID).
     * Index field has 2 formats depending on whether source is "special" (see OrderSource.IsSpecialSourceId()).
     * Note: both formats are IMPLEMENTATION DETAILS, they are subject to change without notice.
     *   63..48   47..32   31..16   15..0
     * +--------+--------+--------+--------+
     * | Source |Exchange|      Index      |  <- "special" order sources (non-printable id with exchange)
     * +--------+--------+--------+--------+
     *   63..48   47..32   31..16   15..0
     * +--------+--------+--------+--------+
     * |     Source      |      Index      |  <- generic order sources (alphanumeric id without exchange)
     * +--------+--------+--------+--------+
     * Note: when modifying formats track usages of getIndex/setIndex, getSource/setSource and isSpecialSourceId
     * methods in order to find and modify all code dependent on current formats.
     */

    /*
     * EventFlags property has several significant bits that are packed into an integer in the following way:
     *    31..7    6    5    4    3    2    1    0
     * +---------+----+----+----+----+----+----+----+
     * |         | SM |    | SS | SE | SB | RE | TX |
     * +---------+----+----+----+----+----+----+----+
     */

    private long _index;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderBase"/> class.
    /// </summary>
    protected OrderBase()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderBase"/> class with specified event symbol.
    /// </summary>
    /// <param name="eventSymbol">The event symbol.</param>
    protected OrderBase(string? eventSymbol)
        : base(eventSymbol)
    {
    }

    /// <inheritdoc/>
    IndexedEventSource IIndexedEvent.EventSource => EventSource;

    /// <summary>
    /// Gets or sets order source of this event.
    /// The source is stored in the highest bits of the <see cref="Index"/> of this event.
    /// </summary>
    public OrderSource EventSource
    {
        get
        {
            var sourceId = (int)(Index >> 48);
            if (!OrderSource.IsSpecialSourceId(sourceId))
            {
                sourceId = (int)(Index >> 32);
            }

            return OrderSource.ValueOf(sourceId);
        }

        set
        {
            var shift = OrderSource.IsSpecialSourceId(value.Id) ? 48 : 32;
            var mask = OrderSource.IsSpecialSourceId((int)(Index >> 48)) ? ~(-1L << 48) : ~(-1L << 32);
            Index = ((long)value.Id << shift) | (Index & mask);
        }
    }

    /// <inheritdoc/>
    public int EventFlags { get; set; }

    /// <summary>
    /// Gets or sets unique per-symbol index of this order. Index is non-negative.
    /// Note, that this method also changes <see cref="EventSource"/>, whose id occupies highest bits of index.
    /// Use <see cref="EventSource"/> after invocation of this method to set the desired value of source.
    /// </summary>
    /// <exception cref="ArgumentException">When index is negative.</exception>
    public long Index
    {
        get => _index;
        set
        {
            if (value < 0)
            {
                throw new ArgumentException($"Negative index: {value}", nameof(value));
            }

            _index = value;
        }
    }

    /// <summary>
    /// Gets or sets time and sequence of this order packaged into single long value
    /// This method is intended for efficient order time priority comparison.
    /// <b>Do not set their property directly.</b>
    /// Change <see cref="Time"/> and/or <see cref="Sequence"/>.
    /// </summary>
    public long TimeSequence { get; set; }

    /// <summary>
    /// Gets or sets time of this order.
    /// Time is measured in milliseconds between the current time and midnight, January 1, 1970 UTC.
    /// </summary>
    public long Time
    {
        get => ((TimeSequence >> 32) * 1000) + ((TimeSequence >> 22) & 0x3ff);
        set => TimeSequence = ((long)TimeUtil.GetSecondsFromTime(value) << 32) |
                              ((long)TimeUtil.GetMillisFromTime(value) << 22) |
                              (uint)Sequence;
    }

    /// <summary>
    /// Gets or sets microseconds and nanoseconds time part of this order.
    /// </summary>
    public int TimeNanoPart { get; set; }

    /// <summary>
    /// Gets or sets sequence number sequence number of this order to distinguish orders that have the same
    /// <see cref="Time"/>. This sequence number does not have to be unique and
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
    /// Gets or sets action of this order.
    /// Returns order action if available, otherwise - <see cref="OrderAction.Undefined"/>.
    /// </summary>
    public OrderAction Action
    {
        get => OrderActionExt.ValueOf(BitUtil.GetBits(Flags, ActionMask, ActionShift));
        set => Flags = BitUtil.SetBits(Flags, ActionMask, ActionShift, (int)value);
    }

    /// <summary>
    /// Gets or sets time of the last <see cref="Action"/>.
    /// </summary>
    public long ActionTime { get; set; }

    /// <summary>
    /// Gets or sets order ID if available.
    /// Some actions <see cref="OrderAction.Trade"/>, <see cref="OrderAction.Bust"/>
    /// have no order ID since they are not related to any order in Order book.
    /// </summary>
    public long OrderId { get; set; }

    /// <summary>
    /// Gets or sets order ID if available.
    /// Returns auxiliary order ID if available:.
    /// <ul>
    /// <li>in <see cref="OrderAction.New"/> - ID of the order replaced by this new order.</li>
    /// <li>in <see cref="OrderAction.Delete"/> - ID of the order that replaces this deleted order.</li>
    /// <li>in <see cref="OrderAction.Partial"/> - ID of the aggressor order.</li>
    /// <li>in <see cref="OrderAction.Execute"/> - ID of the aggressor order.</li>
    /// </ul>
    /// </summary>
    public long AuxOrderId { get; set; }

    /// <summary>
    /// Gets or sets price of this order event.
    /// </summary>
    public double Price { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets size of this order event as floating number with fractions.
    /// </summary>
    public double Size { get; set; } = double.NaN;

    /// <summary>
    /// Gets a value indicating whether this order has some size
    /// (<see cref="Size"/> is neither <c>0</c> nor <see cref="double.NaN"/>).
    /// </summary>
    public bool HasSize => Size != 0 && !double.IsNaN(Size);

    /// <summary>
    /// Gets or sets executed size of this order.
    /// </summary>
    public double ExecutedSize { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets number of individual orders in this aggregate order.
    /// </summary>
    public long Count { get; set; }

    /// <summary>
    /// Gets or sets trade (order execution) ID for events containing trade-related action.
    /// Returns <c>0</c> if trade ID not available.
    /// </summary>
    public long TradeId { get; set; }

    /// <summary>
    /// Gets or sets trade price for events containing trade-related action.
    /// </summary>
    public double TradePrice { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets trade size for events containing trade-related action.
    /// </summary>
    public double TradeSize { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets exchange code of this order.
    /// </summary>
    /// <exception cref="ArgumentException">If exchange code is greater than 127.</exception>
    public char ExchangeCode
    {
        get => (char)BitUtil.GetBits(Flags, ExchangeMask, ExchangeShift);
        set
        {
            StringUtil.CheckChar(value, ExchangeMask, "exchangeCode");
            Flags = BitUtil.SetBits(Flags, ExchangeMask, ExchangeShift, value);
        }
    }

    /// <summary>
    /// Gets or sets side of this order.
    /// </summary>
    public Side OrderSide
    {
        get => SideExt.ValueOf(BitUtil.GetBits(Flags, SideMask, SideShift));
        set => Flags = BitUtil.SetBits(Flags, SideMask, SideShift, (int)value);
    }

    /// <summary>
    /// Gets or sets scope of this order.
    /// </summary>
    public Scope Scope
    {
        get => ScopeExt.ValueOf(BitUtil.GetBits(Flags, ScopeMask, ScopeShift));
        set => Flags = BitUtil.SetBits(Flags, ScopeMask, ScopeShift, (int)value);
    }

    /// <summary>
    /// Gets or sets implementation-specific flags.
    /// <b>Do not use this property directly.</b>
    /// </summary>
    internal int Flags { get; set; }

    /// <summary>
    /// Returns string representation of this base order event's.
    /// </summary>
    /// <returns>The string representation.</returns>
    public override string ToString() =>
        GetType().Name + "{" + BaseFieldsToString() + "}";

    /// <summary>
    /// Returns string representation of this order fields.
    /// </summary>
    /// <returns>The string representation.</returns>
    protected string BaseFieldsToString() =>
        StringUtil.EncodeNullableString(EventSymbol) +
        ", eventTime=" + TimeFormat.Local.WithMillis().WithTimeZone().FormatFromMillis(EventTime) +
        ", source=" + EventSource +
        ", eventFlags=0x" + EventFlags.ToString("x", CultureInfo.InvariantCulture) +
        ", index=0x" + Index.ToString("x", CultureInfo.InvariantCulture) +
        ", time=" + TimeFormat.Local.WithMillis().WithTimeZone().FormatFromMillis(Time) +
        ", sequence=" + Sequence +
        ", timeNanoPart=" + TimeNanoPart +
        ", action=" + Action +
        ", actionTime=" + TimeFormat.Local.WithMillis().WithTimeZone().FormatFromMillis(ActionTime) +
        ", orderId=" + OrderId +
        ", auxOrderId=" + AuxOrderId +
        ", price=" + Price +
        ", size=" + Size +
        ", executedSize=" + ExecutedSize +
        ", count=" + Count +
        ", exchange=" + StringUtil.EncodeChar(ExchangeCode) +
        ", side=" + OrderSide +
        ", scope=" + Scope +
        ", tradeId=" + TradeId +
        ", tradePrice=" + TradePrice +
        ", tradeSize=" + TradeSize;
}
