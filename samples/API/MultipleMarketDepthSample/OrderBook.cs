// <copyright file="OrderBook.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Collections.Generic;
using DxFeed.Graal.Net.Events.Market;

namespace DxFeed.Graal.Net.Samples;

public class OrderBook<T>
    where T : OrderBase
{
    public List<T> Buy { get; set; } = new();
    public List<T> Sell { get; set; } = new();
}
