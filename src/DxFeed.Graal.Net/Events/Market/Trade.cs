// <copyright file="Trade.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Native.Events;

namespace DxFeed.Graal.Net.Events.Market;

/// <summary>
/// Trade event is a snapshot of the price and size of the last trade during regular trading hours
/// and an overall day volume and day turnover.
/// It represents the most recent information that is available about the regular last trade on the market
/// at any given moment of time.
/// <br/>
/// For more details see <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/event/market/Trade.html">Javadoc</a>.
/// </summary>
[EventCode(EventCodeNative.Trade)]
public class Trade : TradeBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Trade"/> class.
    /// </summary>
    public Trade()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Trade"/> class with the specified event symbol.
    /// </summary>
    /// <param name="eventSymbol">The specified event symbol.</param>
    public Trade(string? eventSymbol)
        : base(eventSymbol)
    {
    }

    /// <summary>
    /// Returns string representation of this trade event.
    /// </summary>
    /// <returns>The string representation.</returns>
    public override string ToString() =>
        "Trade{" + BaseFieldsToString() +
        "}";
}
