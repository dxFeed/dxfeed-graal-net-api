// <copyright file="OrderAction.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Utils;
using static DxFeed.Graal.Net.Events.Market.OrderAction;

namespace DxFeed.Graal.Net.Events.Market;

// Documentation Text Must End With A Period. False positive.
#pragma warning disable SA1629

/// <summary>
/// Action enum for the Full Order Book (FOB) Orders. Action describes business meaning of the
/// <see cref="Order"/> event: whether order was added or replaced, partially or fully executed, etc.
/// </summary>
public enum OrderAction
{
    /// <summary>
    /// Default enum value for orders that do not support "Full Order Book" and for backward compatibility -
    /// action must be derived from other <see cref="Order"/> fields.
    /// <br/>All Full Order Book related fields for this action will be empty.
    /// </summary>
    Undefined,

    /// <summary>
    /// New Order is added to Order Book.
    /// <br/>
    /// <para>
    /// Full Order Book fields:
    /// <ul>
    /// <li><see cref="OrderBase.OrderId"/> - always present.</li>
    /// <li><see cref="OrderBase.AuxOrderId"/> - ID of the order replaced by this new order - if available.</li>
    /// <li>Trade fields will be empty</li>
    /// </ul>
    /// </para>
    /// </summary>
    New,

    /// <summary>
    /// Order is modified and price-time-priority is not maintained (i.e. order has re-entered Order Book).
    /// Order <see cref="MarketEvent.EventSymbol"/> and <see cref="OrderBase.OrderSide"/> will remain the same.
    /// <br/>
    /// <para>
    /// Full Order Book fields:
    /// <ul>
    /// <li><see cref="OrderBase.OrderId"/> - always present.</li>
    /// <li>Trade fields will be empty.</li>
    /// </ul>
    /// </para>
    /// </summary>
    Replace,

    /// <summary>
    /// Order is modified without changing its price-time-priority (usually due to partial cancel by user).
    /// Order's <see cref="OrderBase.Size"/> will contain new updated size.
    /// <br/>
    /// <para>
    /// Full Order Book fields:
    /// <ul>
    /// <li><see cref="OrderBase.OrderId"/> - always present.</li>
    /// <li>Trade fields will be empty.</li>
    /// </ul>
    /// </para>
    /// </summary>
    Modify,

    /// <summary>
    /// Order is fully canceled and removed from Order Book.
    /// Order's <see cref="OrderBase.Size"/> will be equal to 0.
    /// <br/>
    /// <para>
    /// Full Order Book fields:
    /// <ul>
    /// <li><see cref="OrderBase.OrderId"/> - always present.</li>
    /// <li><see cref="OrderBase.AuxOrderId"/> - ID of the new order replacing this order - if available.</li>
    /// <li>Trade fields will be empty.</li>
    /// </ul>
    /// </para>
    /// </summary>
    Delete,

    /// <summary>
    /// Size is changed (usually reduced) due to partial order execution.
    /// Order's <see cref="OrderBase.Size"/> will be updated to show current outstanding size.
    /// <br/>
    /// <para>
    /// Full Order Book fields:
    /// <ul>
    /// <li><see cref="OrderBase.OrderId"/> - always present.</li>
    /// <li><see cref="OrderBase.AuxOrderId"/> - aggressor order ID, if available.</li>
    /// <li><see cref="OrderBase.TradeId"/> - if available.</li>
    /// <li><see cref="OrderBase.TradeSize"/> and <see cref="OrderBase.TradePrice"/> - contain size and price
    /// of this execution.</li>
    /// </ul>
    /// </para>
    /// </summary>
    Partial,

    /// <summary>
    /// Order is fully executed and removed from Order Book.
    /// Order's <see cref="OrderBase.Size"/> will be equals to 0.
    /// <br/>
    /// <para>
    /// Full Order Book fields:
    /// <ul>
    /// <li><see cref="OrderBase.OrderId"/> - always present.</li>
    /// <li><see cref="OrderBase.AuxOrderId"/> - aggressor order ID, if available.</li>
    /// <li><see cref="OrderBase.TradeId"/> - if available.</li>
    /// <li><see cref="OrderBase.TradeSize"/> and <see cref="OrderBase.TradePrice"/> - contain size and price
    /// of this execution - always present.</li>
    /// </ul>
    /// </para>
    /// </summary>
    Execute,

    /// <summary>
    /// Non-Book Trade - this Trade not refers to any entry in Order Book.
    /// Order's <see cref="OrderBase.Size"/> and <see cref="OrderBase.Price"/> will be equals to 0.
    /// <br/>
    /// <para>
    /// Full Order Book fields:
    /// <ul>
    /// <li><see cref="OrderBase.OrderId"/> - always empty.</li>
    /// <li><see cref="OrderBase.TradeId"/> - if available.</li>
    /// <li><see cref="OrderBase.TradeSize"/> and <see cref="OrderBase.TradePrice"/> - contain size and price
    /// of this trade - always present.</li>
    /// </ul>
    /// </para>
    /// </summary>
    Trade,

    /// <summary>
    /// Prior Trade/Order Execution bust.
    /// Order's <see cref="OrderBase.Size"/> and <see cref="OrderBase.Price"/> will be equals to 0.
    /// <br/>
    /// <para>
    /// Full Order Book fields:
    /// <ul>
    /// <li><see cref="OrderBase.OrderId"/> - always empty.</li>
    /// <li><see cref="OrderBase.TradeId"/> - always present.</li>
    /// <li><see cref="OrderBase.TradeSize"/> and <see cref="OrderBase.TradePrice"/> - always empty.</li>
    /// </ul>
    /// </para>
    /// </summary>
    Bust,
}

/// <summary>
/// Class extension for <see cref="OrderAction"/> enum.
/// </summary>
internal static class OrderActionExt
{
    private static readonly OrderAction[] Values = EnumUtil.BuildEnumBitMaskArrayByValue(Undefined);

    /// <summary>
    /// Returns an enum constant of the <see cref="OrderAction"/> by integer code bit pattern.
    /// </summary>
    /// <param name="value">The specified value.</param>
    /// <returns>The enum constant of the specified enum type with the specified value.</returns>
    public static OrderAction ValueOf(int value) =>
        Values[value];
}
