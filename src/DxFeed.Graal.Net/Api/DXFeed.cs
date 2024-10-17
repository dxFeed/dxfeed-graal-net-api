// <copyright file="DXFeed.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DxFeed.Graal.Net.Api.Osub;
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
public class DXFeed
{
    /// <summary>
    /// Feed native wrapper.
    /// </summary>
    private readonly FeedNative _feedNative;

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
    /// </summary>
    /// <returns>The <see cref="DXFeed"/>.</returns>
    public static DXFeed GetInstance() =>
        DXEndpoint.GetInstance().GetFeed();

    /// <summary>
    /// Creates new subscription for a list of event types that is <i>attached</i> to this feed.
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
    /// Returns the last event for the specified event instance.
    /// This method works only for event types that implement <see cref="ILastingEvent"/> marker interface.
    /// This method <b>does not</b> make any remote calls to the uplink data provider.
    /// It just retrieves last received event from the local cache of this feed.
    /// The events are stored in the cache only if there is some
    /// attached <see cref="DXFeedSubscription"/> that is subscribed to the corresponding symbol and event type.
    /// <see cref="WildcardSymbol.All"/> subscription does not count for that purpose.
    ///
    /// <p>Use <see cref="GetLastEventAsync{T}"/> method
    /// if an event needs to be requested in the absence of subscription.</p>
    ///
    /// <p>This method fills in the values for the last event into the <c>event</c> argument.
    /// If the last event is not available for any reason (no subscription, no connection to uplink, etc).
    /// then the event object is not changed.
    /// This method always returns the same <c>event</c> instance that is passed to it as an argument.</p>
    ///
    /// <p>This method provides no way to distinguish a case when there is no subscription from the case when
    /// there is a subscription, but the event data have not arrived yet. It is recommended to use
    /// <see cref="GetLasEventIfSubscribed{TE}"/> method
    /// instead of this <c>GetLastEvent</c> method to fail-fast in case when the subscription was supposed to be
    /// set by the logic of the code, since <see cref="GetLasEventIfSubscribed{TE}"/>
    /// method returns null when there is no subscription.</p>
    ///
    /// <p>Note, that this method does not work when <see cref="DXEndpoint"/> was created with
    /// <see cref="DXEndpoint.Role.StreamFeed"/> role (never fills in the event).</p>
    ///
    /// </summary>
    /// <param name="e">The event.</param>
    /// <typeparam name="TE">The type of event.</typeparam>
    /// <returns>The same event.</returns>
    public TE GetLastEvent<TE>(TE e)
        where TE : ILastingEvent =>
        _feedNative.GetLastEvent(e);

    /// <summary>
    /// Returns the last events for the specified list of event instances.
    /// This is a bulk version of <see cref="GetLastEvent{TE}"/> method.
    ///
    /// <p>Note, that this method does not work when <see cref="DXEndpoint"/> was created with
    /// <see cref="DXEndpoint.Role.StreamFeed"/> role (never fills in the event).</p>
    ///
    /// </summary>
    /// <param name="events">The collection of events.</param>
    /// <typeparam name="TE">The type of event.</typeparam>
    /// <returns>The same collection of events.</returns>
    public IList<TE> GetLastEvents<TE>(IList<TE> events)
        where TE : ILastingEvent =>
        _feedNative.GetLastEvents(events);

    /// <summary>
    /// Returns the last event for the specified event type and symbol if there is a subscription for it.
    /// This method works only for event types that implement <see cref="ILastingEvent"/> marker interface.
    /// This method <b>does not</b> make any remote calls to the uplink data provider.
    /// It just retrieves last received event from the local cache of this feed.
    /// The events are stored in the cache only if there is some
    /// attached <see cref="DXFeedSubscription"/> that is subscribed to the corresponding event type and symbol.
    /// The subscription can also be permanently defined using <see cref="DXEndpoint"/> properties.
    /// <see cref="WildcardSymbol.All"/> subscription does not count for that purpose.
    /// If there is no subscription, then this method returns null.
    ///
    /// <p>If there is a subscription, but the event has not arrived from the uplink data provider,
    /// this method returns an non-initialized event object: its <see cref="GetLastEvent{TE}"/>
    /// property is set to the requested symbol, but all the other properties have their default values.</p>
    ///
    /// <p>Use <see cref="GetLastEventAsync{T}"/> method
    /// if an event needs to be requested in the absence of subscription.</p>
    ///
    /// <p>Note, that this method does not work when <see cref="DXEndpoint"/> was created with
    /// <see cref="DXEndpoint.Role.StreamFeed"/> role (never fills in the event).</p>
    ///
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <typeparam name="TE">The event type.</typeparam>
    /// <returns>The event or null if there is no subscription for the specified event type and symbol.</returns>
    public TE? GetLasEventIfSubscribed<TE>(object symbol)
        where TE : ILastingEvent =>
        _feedNative.GetLastEventIfSubscribed<TE>(symbol);

    /// <summary>
    /// Requests the last event for the specified event type and symbol.
    /// This method works only for event types that implement  <see cref="ILastingEvent"/>  marker interface.
    /// This method requests the data from the uplink data provider,
    /// creates new event of the specified event type,
    /// and <see cref="Task{TResult}.Result"/> the resulting task with this event.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="token">The cancellation token.</param>
    /// <typeparam name="T">The type of event.</typeparam>
    /// <returns>The task for the result of the request.</returns>
    public async Task<T> GetLastEventAsync<T>(object symbol, CancellationToken token = default)
        where T : ILastingEvent
    {
        token.ThrowIfCancellationRequested();
        using var nativePromise = _feedNative.GetLastEventPromise(EventCodeAttribute.GetEventCode(typeof(T)), symbol);
        try
        {
            while (!nativePromise.IsDone())
            {
                await Task.Delay(100, token);
            }

            nativePromise.ThrowIfJavaExceptionExists();
            return (T)nativePromise.Result();
        }
        catch (OperationCanceledException)
        {
            nativePromise.Cancel();
            throw;
        }
    }

    /// <summary>
    /// Requests time series of events for the specified event type, symbol, and a range of time.
    /// This method works only for event types that implement <see cref="ITimeSeriesEvent"/> interface.
    /// his method requests the data from the the uplink data provider,
    /// creates a list of events of the specified event type,
    /// and <see cref="Task{TResult}.Result"/> the resulting task with this list.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="from">The time, inclusive, to request events from <see cref="ITimeSeriesEvent.Time"/>.</param>
    /// <param name="to">The time, inclusive, to request events from <see cref="ITimeSeriesEvent.Time"/>.
    /// Use long.MaxValue to retrieve events without an upper limit on time.</param>
    /// <param name="token">The cancellation token.</param>
    /// <typeparam name="T">The event type.</typeparam>
    /// <returns>The task for the result of the request.</returns>
    public async Task<IEnumerable<T>> GetTimeSeriesAsync<T>(object symbol, long from, long to, CancellationToken token = default)
        where T : ITimeSeriesEvent
    {
        token.ThrowIfCancellationRequested();
        using var nativePromise =
            _feedNative.GetTimeSeriesPromise(EventCodeAttribute.GetEventCode(typeof(T)), symbol, from, to);
        try
        {
            while (!nativePromise.IsDone())
            {
                await Task.Delay(100, token);
            }

            nativePromise.ThrowIfJavaExceptionExists();
            return nativePromise.Results().OfType<T>();
        }
        catch (OperationCanceledException)
        {
            nativePromise.Cancel();
            throw;
        }
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
