// <copyright file="AnalyticOrder.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Native.Events;
using DxFeed.Graal.Net.Utils;

namespace DxFeed.Graal.Net.Events.Market;

/// <summary>
/// Represents an extension of <see cref="Order"/> introducing analytics information,
/// e.g. adding to this order iceberg related information
/// (<see cref="IcebergPeakSize"/>, <see cref="IcebergHiddenSize"/>, <see cref="IcebergExecutedSize"/>).
/// The collection of analytic order events of a symbol represents the most recent analytic information
/// that is available about orders on the market at any given moment of time.
/// <br/>
/// For more details see <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/event/market/AnalyticOrder.html">Javadoc</a>.
/// </summary>
[EventCode(EventCodeNative.AnalyticOrder)]
public class AnalyticOrder : Order
{
    /*
     * Analytic flags property has several significant bits that are packed into an integer in the following way:
     *      31...2       1    0
     * +--------------+-------+-------+
     * |              |   IcebergType |
     * +--------------+-------+-------+
     */

    // TYPE values are taken from Type enum.
    private const int IcebergTypeMask = 3;
    private const int IcebergTypeShift = 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="AnalyticOrder"/> class.
    /// </summary>
    public AnalyticOrder()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AnalyticOrder"/> class with the specified event symbol.
    /// </summary>
    /// <param name="eventSymbol">The specified event symbol.</param>
    public AnalyticOrder(string? eventSymbol)
        : base(eventSymbol)
    {
    }

    /// <summary>
    /// Gets or sets iceberg peak size of this analytic order.
    /// </summary>
    public double IcebergPeakSize { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets iceberg hidden size of this analytic order.
    /// </summary>
    public double IcebergHiddenSize { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets iceberg executed size of this analytic order.
    /// </summary>
    public double IcebergExecutedSize { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets iceberg type of this analytic order.
    /// </summary>
    public IcebergType IcebergType
    {
        get => IcebergTypeExt.ValueOf(BitUtil.GetBits(IcebergFlags, IcebergTypeMask, IcebergTypeShift));
        set => IcebergFlags = BitUtil.SetBits(IcebergFlags, IcebergTypeMask, IcebergTypeShift, (int)value);
    }

    /// <summary>
    /// Gets or sets implementation-specific flags relevant only for iceberg related part of analytic order.
    /// <b>Do not use this property directly.</b>
    /// </summary>
    internal int IcebergFlags { get; set; }

    /// <summary>
    /// Returns string representation of this spread order event.
    /// </summary>
    /// <returns>The string representation.</returns>
    public override string ToString() =>
        "AnalyticOrder{" + BaseFieldsToString() +
        ", marketMaker='" + StringUtil.EncodeNullableString(MarketMaker) + "'" +
        ", icebergPeakSize=" + IcebergPeakSize +
        ", icebergHiddenSize=" + IcebergHiddenSize +
        ", icebergExecutedSize=" + IcebergExecutedSize +
        ", icebergType=" + IcebergType +
        "}";
}
