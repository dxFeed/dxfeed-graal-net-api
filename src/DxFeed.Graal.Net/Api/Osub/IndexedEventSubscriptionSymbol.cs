// <copyright file="IndexedEventSubscriptionSymbol.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using DxFeed.Graal.Net.Events;

namespace DxFeed.Graal.Net.Api.Osub;

// File May Only Contain A Single Type. It's more readable to put the generic and non-generic versions in the same file.
#pragma warning disable SA1402

/// <summary>
/// Represents subscription to a specific source of indexed events.
/// Instances of this class can be used with <see cref="DXFeedSubscription"/>
/// to specify subscription to a particular source of indexed events.
/// By default, when subscribing to indexed events by their event symbol object,
/// the subscription is performed to all supported sources.
/// <br/>
/// For more details see <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/osub/IndexedEventSubscriptionSymbol.html">Javadoc</a>.
/// </summary>
/// <typeparam name="T">The type of event symbol.</typeparam>
public class IndexedEventSubscriptionSymbol<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IndexedEventSubscriptionSymbol{T}"/> class
    /// with a specified event symbol and source.
    /// </summary>
    /// <param name="eventSymbol">The event symbol.</param>
    /// <param name="source">The event source.</param>
    /// <exception cref="ArgumentNullException">If eventSymbol or source are null.</exception>
    public IndexedEventSubscriptionSymbol(T eventSymbol, IndexedEventSource source)
    {
        EventSymbol = eventSymbol ?? throw new ArgumentNullException(nameof(eventSymbol));
        Source = source ?? throw new ArgumentNullException(nameof(source));
    }

    /// <summary>
    /// Gets event symbol.
    /// </summary>
    public T EventSymbol { get; }

    /// <summary>
    /// Gets indexed event source.
    /// </summary>
    public IndexedEventSource Source { get; }

    /// <inheritdoc/>
    public override string ToString() =>
        $"{EventSymbol}{{source={Source}}}";

    /// <summary>
    /// Indicates whether some other indexed event subscription symbol
    /// has the same <see cref="EventSymbol"/> and <see cref="Source"/>.
    /// </summary>
    /// <param name="obj"> The object to compare with the current object.</param>
    /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj)
    {
        if (this == obj)
        {
            return true;
        }

        if (obj is not IndexedEventSubscriptionSymbol<T> that)
        {
            return false;
        }

        return EventSymbol!.Equals(that.EventSymbol) && Source.Equals(that.Source);
    }

    /// <summary>
    /// Returns a hash code value for this object.
    /// </summary>
    /// <returns>A hash code value for this object.</returns>
    public override int GetHashCode() =>
        EventSymbol!.GetHashCode() + (Source.GetHashCode() * 31);
}

/// Non-generic version, for erasing a generic type.
/// <inheritdoc/>
public class IndexedEventSubscriptionSymbol : IndexedEventSubscriptionSymbol<object>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IndexedEventSubscriptionSymbol"/> class
    /// with a specified event symbol and source.
    /// </summary>
    /// <param name="eventSymbol">The event symbol.</param>
    /// <param name="source">The event source.</param>
    /// <exception cref="ArgumentNullException">If eventSymbol or source are null.</exception>
    public IndexedEventSubscriptionSymbol(object eventSymbol, IndexedEventSource source)
        : base(eventSymbol, source)
    {
    }
}
