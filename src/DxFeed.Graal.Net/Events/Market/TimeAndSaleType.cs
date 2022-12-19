// <copyright file="TimeAndSaleType.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace DxFeed.Graal.Net.Events.Market;

/// <summary>
/// Type of a time and sale event.
/// <br/>
/// For more details see <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/event/market/TimeAndSaleType.html">Javadoc</a>.
/// </summary>
public enum TimeAndSaleType
{
    /// <summary>
    /// Represents new time and sale event.
    /// </summary>
    New,

    /// <summary>
    /// Represents correction time and sale event.
    /// </summary>
    Correction,

    /// <summary>
    /// Represents cancel time and sale event.
    /// </summary>
    Cancel,
}
