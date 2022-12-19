// <copyright file="MarketEvent.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace DxFeed.Graal.Net.Events.Market;

/// <summary>
/// Abstract base class for all market events. All market events are objects that
/// extend this class. Market event classes are simple beans with setter and getter methods for their
/// properties and minimal business logic. All market events have <see cref="EventSymbol"/>
/// property that is defined by this class.
/// <br/>
/// For more details see <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/event/market/MarketEvent.html">Javadoc</a>.
/// </summary>
public abstract class MarketEvent : IEventType<string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MarketEvent"/> class.
    /// </summary>
    protected MarketEvent()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MarketEvent"/> class with event symbol.
    /// </summary>
    /// <param name="eventSymbol">The event symbol.</param>
    protected MarketEvent(string? eventSymbol) =>
        EventSymbol = eventSymbol;

    /// <inheritdoc cref="IEventType{T}.EventSymbol"/>
    public string? EventSymbol { get; set; }

    /// <inheritdoc/>
    public long EventTime { get; set; }
}
