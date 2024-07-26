// <copyright file="MarketDepthModelListener.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Collections.Generic;
using DxFeed.Graal.Net.Events.Market;

namespace DxFeed.Graal.Net.Models;

/// <summary>
/// Invoked when the order book is changed.
/// <p>The <see cref="MarketDepthModelListener{TE}"/> delegate is used to handle notifications
/// of changes to the market depth, including updates to the buy and sell orders.
/// Implement this delegate to process or react to changes in the market order book.</p>
/// </summary>
/// <param name="buy">The collection of buy orders.</param>
/// <param name="sell">The collection of sell orders.</param>
/// <typeparam name="TE">The type of order derived from <see cref="OrderBase"/>.</typeparam>
public delegate void MarketDepthModelListener<TE>(List<TE> buy, List<TE> sell)
    where TE : OrderBase;
