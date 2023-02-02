// <copyright file="EventCodeNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Events;

namespace DxFeed.Graal.Net.Native.Events;

/// <summary>
/// A list of all events, that can be passed to/from native code, represented as a numeric code.
/// In a particular case, these are implementations <see cref="IEventType"/>.
/// </summary>
public enum EventCodeNative
{
    /// <summary>
    /// <see cref="Quote"/>.
    /// </summary>
    Quote,

    /// <summary>
    /// <see cref="Profile"/>.
    /// </summary>
    Profile,

    /// <summary>
    /// <see cref="Summary"/>.
    /// </summary>
    Summary,

    /// <summary>
    /// <see cref="Greeks"/>.
    /// </summary>
    Greeks,

    /// <summary>
    /// <see cref="Candle"/>.
    /// </summary>
    Candle,

    /// <summary>
    /// <b>Deprecated. Doesn't need to be implemented.</b>
    /// </summary>
    DailyCandle,

    /// <summary>
    /// <see cref="Underlying"/>.
    /// </summary>
    Underlying,

    /// <summary>
    /// <see cref="TheoPrice"/>.
    /// </summary>
    TheoPrice,

    /// <summary>
    /// <see cref="Trade"/>.
    /// </summary>
    Trade,

    /// <summary>
    /// <see cref="TradeETH"/>.
    /// </summary>
    TradeETH,

    /// <summary>
    /// <b>Not implemented.</b>
    /// </summary>
    Configuration,

    /// <summary>
    /// <b>Not implemented.</b>
    /// </summary>
    Message,

    /// <summary>
    /// <see cref="TimeAndSale"/>.
    /// </summary>
    TimeAndSale,

    /// <summary>
    /// <see cref="OrderBase"/>.
    /// <b>Doesn't need to be implemented. Abstract class.</b>
    /// </summary>
    OrderBase,

    /// <summary>
    /// <see cref="Order"/>.
    /// </summary>
    Order,

    /// <summary>
    /// <see cref="AnalyticOrder"/>.
    /// </summary>
    AnalyticOrder,

    /// <summary>
    /// <see cref="SpreadOrder"/>.
    /// </summary>
    SpreadOrder,

    /// <summary>
    /// <see cref="Series"/>.
    /// </summary>
    Series,
}
