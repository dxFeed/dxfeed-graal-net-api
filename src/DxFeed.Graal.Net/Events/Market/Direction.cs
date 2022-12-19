// <copyright file="Direction.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace DxFeed.Graal.Net.Events.Market;

/// <summary>
/// Direction of the price movement. For example tick direction for last trade price.
/// <br/>
/// For more details see <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/event/market/Direction.html">Javadoc</a>.
/// </summary>
public enum Direction
{
    /// <summary>
    /// Direction is undefined, unknown or inapplicable.
    /// It includes cases with undefined price value or when direction computation was not performed.
    /// </summary>
    Undefined,

    /// <summary>
    /// Current price is lower than previous price.
    /// </summary>
    Down,

    /// <summary>
    /// Current price is the same as previous price and is lower than the last known price of different value.
    /// </summary>
    ZeroDown,

    /// <summary>
    /// Current price is equal to the only known price value suitable for price direction computation.
    /// Unlike <see cref="Undefined"/> the <see cref="Zero"/> direction implies that current price is defined and
    /// direction computation was duly performed but has failed to detect any upward or downward price movement.
    /// It is also reported for cases when price sequence was broken and direction computation was restarted anew.
    /// </summary>
    Zero,

    /// <summary>
    /// Current price is the same as previous price and is higher than the last known price of different value.
    /// </summary>
    ZeroUp,

    /// <summary>
    /// Current price is higher than previous price.
    /// </summary>
    Up,
}
