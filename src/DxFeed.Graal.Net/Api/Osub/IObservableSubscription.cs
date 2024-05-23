// <copyright file="IObservableSubscription.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;

namespace DxFeed.Graal.Net.Api.Osub;

// ToDo Add ObservableSubscriptionChangeListener.

/// <summary>
/// Observable set of subscription symbols for the specific event type.
/// <br/>
/// For more details see <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/osub/ObservableSubscription.html">Javadoc</a>.
/// </summary>
public interface IObservableSubscription
{
    /// <summary>
    /// Gets a value indicating whether if this subscription is closed.
    /// <a hreh="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/osub/ObservableSubscription.html#isClosed--">Javadoc</a>.
    /// </summary>
    bool IsClosed { get; }

    /// <summary>
    /// Gets a set of subscribed event types. The resulting set cannot be modified.
    /// <a hreh="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/osub/ObservableSubscription.html#getEventTypes--">Javadoc</a>.
    /// </summary>
    /// <returns>Returns a set of subscribed event types.</returns>
    ISet<Type> GetEventTypes();

    /// <summary>
    /// Gets a value indicating whether if this subscription contains the corresponding event type.
    /// <a hreh="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/osub/ObservableSubscription.html#containsEventType-java.lang.Class-">Javadoc</a>.
    /// </summary>
    /// <param name="eventType">The event type.</param>
    /// <returns>Returns <c>true</c> if this subscription contains the corresponding event type.</returns>
    bool IsContainsEventType(Type eventType);

    // ToDo Add methods for add/remove ObservableSubscriptionChangeListener.
}
