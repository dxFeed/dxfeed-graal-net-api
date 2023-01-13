// <copyright file="DXFeed.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Native.Feed;
using DxFeed.Graal.Net.Utils;

namespace DxFeed.Graal.Net.Api;

/// <summary>
/// Main entry class for dxFeed API.
/// This class is a wrapper for <see cref="FeedNative"/>.
/// <br/>
/// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXFeed.html"><b>Read it first Javadoc.</b></a>
/// </summary>
// ToDo All creates subscription methods must use DXFeedSubscription ctor, and adds sub to _attachedSubscription Set.
public class DXFeed
{
    /// <summary>
    /// Feed native wrapper.
    /// </summary>
    private readonly FeedNative _feedNative;

    // ToDo Wrap a ConcurrentDictionary to ConcurrentHashSet. Dotnet does not have implementation ConcurrentHashSet.
    private readonly ConcurrentSet<DXFeedSubscription> _attachedSubscription = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="DXFeed"/> class with specified feed native.
    /// </summary>
    /// <param name="feedNative">The specified feed native.</param>
    internal DXFeed(FeedNative feedNative) =>
        _feedNative = feedNative;

    /// <summary>
    /// Gets a default application-wide singleton instance of feed.
    /// Most applications use only a single data-source and should rely on this method to get one.
    /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXFeed.html#getInstance--">Javadoc.</a>
    /// </summary>
    public static DXFeed Instance =>
        DXEndpoint.GetInstance().GetFeed();

    /// <summary>
    /// Creates new subscription for a list of event types that is <i>attached</i> to this feed.
    /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXFeed.html#createSubscription-java.lang.Class-">Javadoc.</a>
    /// </summary>
    /// <param name="eventTypes">
    /// The list of event types.
    /// Events types must be implement <see cref="IEventType"/> and have <see cref="EventCodeAttribute"/>.
    /// </param>
    /// <returns>The created <see cref="DXFeedSubscription"/>.</returns>
    /// <exception cref="ArgumentException">
    /// If one on the specified <see cref="Type"/> has no <see cref="EventCodeAttribute"/>.
    /// </exception>
    public DXFeedSubscription CreateSubscription(params Type[] eventTypes)
    {
        var eventCodes = EventCodeAttribute.GetEventCodes(eventTypes);
        DXFeedSubscription sub = new(_feedNative.CreateSubscription(eventCodes), new HashSet<Type>(eventTypes));
        _attachedSubscription.Add(sub);
        return sub;
    }

    /// <summary>
    /// Creates new subscription for a list of event types that is <i>attached</i> to this feed.
    /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXFeed.html#createSubscription-java.lang.Class-">Javadoc.</a>
    /// </summary>
    /// <param name="eventTypes">
    /// The list of event types.
    /// Events types must be implement <see cref="IEventType"/> and have <see cref="EventCodeAttribute"/>.
    /// </param>
    /// <returns>The created subscription.</returns>
    /// <exception cref="ArgumentException">
    /// If one on the specified <see cref="Type"/> has no <see cref="EventCodeAttribute"/>.
    /// </exception>
    public DXFeedSubscription CreateSubscription(IEnumerable<Type> eventTypes) =>
        CreateSubscription(eventTypes.ToArray());

    /// <summary>
    /// Attaches the given subscription to this feed. This method does nothing if the
    /// corresponding subscription is already attached to this feed.
    /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXFeed.html#attachSubscription-com.dxfeed.api.DXFeedSubscription-">Javadoc.</a>
    /// </summary>
    /// <param name="subscription">The subscription.</param>
    public void AttachSubscription(DXFeedSubscription subscription)
    {
        _attachedSubscription.Add(subscription);
        _feedNative.AttachSubscription(subscription.GetNative());
    }

    /// <summary>
    /// Detaches the given subscription from this feed and clears data delivered
    /// to this subscription by publishing empty events.
    /// This method does nothing if the corresponding subscription is not attached to this feed.
    /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXFeed.html#detachSubscription-com.dxfeed.api.DXFeedSubscription-">Javadoc.</a>
    /// </summary>
    /// <param name="subscription">The subscription.</param>
    public void DetachSubscription(DXFeedSubscription subscription)
    {
        _attachedSubscription.Remove(subscription);
        _feedNative.DetachSubscription(subscription.GetNative());
    }

    /// <summary>
    /// Detaches the given subscription from this feed and clears data delivered
    /// to this subscription by publishing empty events.
    /// This method does nothing if the corresponding subscription is not attached to this feed.
    /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXFeed.html#detachSubscriptionAndClear-com.dxfeed.api.DXFeedSubscription-">Javadoc.</a>
    /// </summary>
    /// <param name="subscription">The subscription.</param>
    public void DetachSubscriptionAndClear(DXFeedSubscription subscription)
    {
        _attachedSubscription.Remove(subscription);
        _feedNative.DetachSubscriptionAndClear(subscription.GetNative());
    }

    /// <summary>
    /// Gets underlying "feed native wrapper".
    /// </summary>
    /// <returns>Returns the feed native associated with this feed.</returns>
    internal FeedNative GetNative() =>
        _feedNative;

    internal void Close() =>
        _attachedSubscription.Clear();
}
