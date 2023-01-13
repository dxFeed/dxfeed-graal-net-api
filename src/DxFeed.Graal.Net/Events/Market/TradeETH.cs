// <copyright file="TradeETH.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Native.Events;

// Disable pascal case naming rules.
#pragma warning disable S101

namespace DxFeed.Graal.Net.Events.Market;

/// <summary>
/// TradeETH event is a snapshot of the price and size of the last trade during
/// extended trading hours and the extended trading hours day volume and day turnover.
/// This event is defined only for symbols (typically stocks and ETFs) with a designated
/// <b>extended trading hours</b> (ETH, pre market and post market trading sessions).
/// It represents the most recent information that is available about
/// ETH last trade on the market at any given moment of time.
/// <br/>
/// For more details see <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/event/market/TradeETH.html">Javadoc</a>.
/// </summary>
[EventCode(EventCodeNative.TradeETH)]
public class TradeETH : TradeBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TradeETH"/> class.
    /// </summary>
    public TradeETH()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TradeETH"/> class with the specified event symbol.
    /// </summary>
    /// <param name="eventSymbol">The specified event symbol.</param>
    public TradeETH(string? eventSymbol)
        : base(eventSymbol)
    {
    }

    /// <summary>
    /// Returns string representation of this trade event.
    /// </summary>
    /// <returns>The string representation.</returns>
    public override string ToString() =>
        "TradeETH{" + BaseFieldsToString() +
        "}";
}
