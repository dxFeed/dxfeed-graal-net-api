// <copyright file="WildcardSymbol.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using static System.StringComparison;

namespace DxFeed.Graal.Net.Api.Osub;

/// <summary>
/// Represents [wildcard] subscription to all events of the specific event type.
/// The <see cref="All"/> constant can be added to any
/// <see cref="DXFeedSubscription"/> instance with <see cref="DXFeedSubscription.AddSymbols(object[])"/> method
/// to the effect of subscribing to all possible event symbols. The corresponding subscription will start
/// receiving all published events of the corresponding types.
/// <br/>
/// For more details see <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/osub/WildcardSymbol.html">Javadoc</a>.
/// </summary>
public class WildcardSymbol
{
    /// <summary>
    /// Symbol prefix that is reserved for wildcard subscriptions.
    /// Any subscription starting with "*" is ignored with the exception of <see cref="WildcardSymbol"/> subscription.
    /// </summary>
    public const string ReservedPrefix = "*";

    /// <summary>
    /// Represents [wildcard] subscription to all events of the specific event type.
    /// <br/>
    /// <b>NOTE:</b>
    /// <br/>
    /// Wildcard subscription can create extremely high network and CPU load for certain kinds of
    /// high-frequency events like quotes. It requires a special arrangement on the side of upstream data provider and
    /// is disabled by default in upstream feed configuration.
    /// Make that sure you have adequate resources and understand the impact before using it.
    /// It can be used for low-frequency events only (like Forex quotes), because each instance
    /// of <see cref="DXFeedSubscription"/> processes events in a single thread
    /// and there is no provision to load-balance wildcard
    /// subscription amongst multiple threads.
    /// </summary>
    public static readonly WildcardSymbol All = new(ReservedPrefix);

    private readonly string _symbol;

    /// <summary>
    /// Initializes a new instance of the <see cref="WildcardSymbol"/> class.
    /// </summary>
    /// <param name="symbol">The wildcard symbol.</param>
    private WildcardSymbol(string symbol) =>
        _symbol = symbol;

    /// <inheritdoc/>
    public override string ToString() =>
        _symbol;

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj) =>
        this == obj || (obj is WildcardSymbol symbol && _symbol.Equals(symbol._symbol, Ordinal));

    /// <summary>
    /// Returns a hash code value for this object.
    /// </summary>
    /// <returns>A hash code value for this object.</returns>
    public override int GetHashCode() =>
        _symbol.GetHashCode();
}
