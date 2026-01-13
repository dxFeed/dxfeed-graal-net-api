// <copyright file="PriceLevelChecker.cs" company="Devexperts LLC">
// Copyright © 2026 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using DxFeed.Graal.Net.Events.Market;
using DxFeed.Graal.Net.Native.Orcs;

namespace DxFeed.Graal.Net.Orcs;

/// <summary>
/// Utility class to check consistency of <see cref="Order"/> list in terms that in each time bid price is less than ask price.
/// There is additional information may be gathered during the check:
/// <ul>
///     <li>Gap detection for subsequent events. In other words if there was a pause between orders greater than <c>timeGapBound</c></li>
///     <li>Last bid and ask change with the minimal period of 1 second</li>
///     <li>Spike in quotes</li>
/// </ul>
/// <p/>
/// Important to mention that sometimes it's a valid situation when bid greater or equal to ask.
/// </summary>
public class PriceLevelChecker
{
    /// <summary>
    /// Checks consistency of <see cref="Order"/> list.
    /// </summary>
    /// <param name="orders">The list of orders.</param>
    /// <param name="timeGapBound">The gap bound to check.</param>
    /// <param name="printQuotes">Enable a quotes logging during the check.</param>
    /// <returns><c>true</c> if orders are valid.</returns>
    public static bool Validate(List<Order> orders, TimeSpan timeGapBound, bool printQuotes) => PriceLevelCheckerNative.Validate(orders, timeGapBound, printQuotes);
}
