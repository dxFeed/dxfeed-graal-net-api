// <copyright file="DXFeed.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
    /// Requests the last event for the specified event type and symbol.
    /// This method works only for event types that implement  <see cref="ILastingEvent"/>  marker interface.
    /// This method requests the data from the the uplink data provider,
    /// creates new event of the specified event type,
    /// and <see cref="Task.Result"/> the resulting promise with this event.
    /// </summary>
    /// <param name="symbol">the symbol.</param>
    /// <param name="token">the cancellation token.</param>
    /// <typeparam name="T">the type of event.</typeparam>
    /// <returns>the promise for the result of the request.</returns>
    public async Task<T?> GetLastEventAsync<T>(object symbol, CancellationToken token = default)
    where T : ILastingEvent
    {
        if (token.IsCancellationRequested)
        {
            return default;
        }

        using var nativePromise = _feedNative.GetLastEventPromise(EventCodeAttribute.GetEventCode(typeof(T)), symbol);
        while (!nativePromise.HasResult())
        {
            try
            {
                await Task.Delay(100, token).ConfigureAwait(true);
            }
            catch (TaskCanceledException e)
            {
                return default;
            }

            if (token.IsCancellationRequested)
            {
                nativePromise.Cancel();
                return default;
            }
        }

        return (T?)nativePromise.Result();
    }


    /// <summary>
    /// Requests time series of events for the specified event type, symbol, and a range of time.
    /// This method works only for event types that implement <see cref="ITimeSeriesEvent"/> interface.
    /// his method requests the data from the the uplink data provider,
    /// creates a list of events of the specified event type,
    /// and <see cref="Task.Result"/> the resulting promise with this list.
    /// </summary>
    /// <param name="symbol">the symbol.</param>
    /// <param name="from">the time, inclusive, to request events from <see cref="ITimeSeriesEvent.Time"/>.</param>
    /// <param name="to">the time, inclusive, to request events from <see cref="ITimeSeriesEvent.Time"/>.
    /// Use long.MaxValue to retrieve events without an upper limit on time.</param>
    /// <param name="token">the cancellation token.</param>
    /// <typeparam name="T">the event type.</typeparam>
    /// <returns>the task for the result of the request.</returns>
    public async Task<IEnumerable<T>?> GetTimeSeriesEventAsync<T>(object symbol, long from, long to, CancellationToken token = default)
        where T : ITimeSeriesEvent
    {
        if (token.IsCancellationRequested)
        {
            return default;
        }

        using var nativePromise = _feedNative.GetTimeSeriesPromise(EventCodeAttribute.GetEventCode(typeof(T)), symbol, from, to);
        while (!nativePromise.HasResult())
        {
            try
            {
                await Task.Delay(100, token).ConfigureAwait(true);
            }
            catch (TaskCanceledException e)
            {
                return default;
            }

            if (token.IsCancellationRequested)
            {
                nativePromise.Cancel();
                return default;
            }
        }

        return nativePromise.Results().OfType<T>();
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
