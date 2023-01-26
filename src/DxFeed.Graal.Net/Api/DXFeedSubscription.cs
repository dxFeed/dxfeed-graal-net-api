// <copyright file="DXFeedSubscription.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using DxFeed.Graal.Net.Api.Osub;
using DxFeed.Graal.Net.Native.Events;
using DxFeed.Graal.Net.Native.Interop;
using DxFeed.Graal.Net.Native.Subscription;

namespace DxFeed.Graal.Net.Api;

/// <summary>
/// Subscription for a set of symbols and event types.
/// This class is a wrapper for <see cref="SubscriptionNative"/>.
/// For more details see <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXFeedSubscription.html">Javadoc</a>.
/// </summary>
// ToDO Add a state change notification.
public sealed class DXFeedSubscription : IObservableSubscription, IDisposable
{
    /// <summary>
    /// Subscription native wrapper.
    /// </summary>
    private readonly SubscriptionNative _subscriptionNative;

    /// <summary>
    /// List of event types associated with this <see cref="DXFeedSubscription"/>.
    /// </summary>
    private readonly IReadOnlySet<Type> _eventTypeSet;

    /// <summary>
    /// A delegate to pass to native code.
    /// </summary>
    private readonly EventListenerFunc _eventListenerFunc;

    /// <summary>
    /// Lock for listener list.
    /// </summary>
    private readonly object _listenersLock = new();

    /// <summary>
    /// List of event listeners.
    /// </summary>
    private volatile ImmutableList<DXFeedEventListener> _listeners = ImmutableList.Create<DXFeedEventListener>();

    /// <summary>
    /// Initializes a new instance of the <see cref="DXFeedSubscription"/> class with specified feed native
    /// for the given list of event types.
    /// </summary>
    /// <param name="subscriptionNative">The specified subscription native.</param>
    /// <param name="eventTypesSet">The list of event types.</param>
    internal unsafe DXFeedSubscription(SubscriptionNative subscriptionNative, IReadOnlySet<Type> eventTypesSet)
    {
        _subscriptionNative = subscriptionNative;
        _eventTypeSet = eventTypesSet;
        _eventListenerFunc = EventListenerFuncWrapper;
    }

    /// <inheritdoc/>
    public bool IsClosed =>
        _subscriptionNative.IsClosed();

    /// <inheritdoc/>
    public IReadOnlySet<Type> GetEventTypes() =>
        _eventTypeSet;

    /// <inheritdoc/>
    public bool IsContainsEventType(Type eventType) =>
        _eventTypeSet.Contains(eventType);

    /// <summary>
    /// Attaches subscription to the specified feed.
    /// </summary>
    /// <param name="feed">The <see cref="DXFeed"/> to attach to.</param>
    public void Attach(DXFeed feed) =>
        feed.AttachSubscription(this);

    /// <summary>
    /// Detaches subscription from the specified feed.
    /// </summary>
    /// <param name="feed">The <see cref="DXFeed"/> to detach from.</param>
    public void Detach(DXFeed feed) =>
        feed.DetachSubscription(this);

    /// <summary>
    /// Adds listener for events.
    /// Event lister can be added only when subscription is not producing any events.
    /// The subscription must be either empty (no symbols have been added)
    /// or not attached to any feed.
    /// This method does nothing if this subscription is closed.
    /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXFeedSubscription.html#addEventListener-com.dxfeed.api.DXFeedEventListener-">Javadoc.</a>
    /// </summary>
    /// <param name="listener">The event listener.</param>
    // ToDo Add method overload to pass IDXFeedEventListener.
    public void AddEventListener(DXFeedEventListener listener)
    {
        lock (_listenersLock)
        {
            if (_listeners.IsEmpty)
            {
                _subscriptionNative.SetEventListener(_eventListenerFunc);
            }

            _listeners = _listeners.Add(listener);
        }
    }

    /// <summary>
    /// Removes listener for events.
    /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXFeedSubscription.html#addEventListener-com.dxfeed.api.DXFeedEventListener-">Javadoc.</a>
    /// </summary>
    /// <param name="listener">The event listener.</param>
    // ToDo Add method overload to pass IDXFeedEventListener.
    public void RemoveEventListener(DXFeedEventListener listener)
    {
        lock (_listenersLock)
        {
            _listeners = _listeners.Remove(listener);

            if (_listeners.IsEmpty)
            {
                _subscriptionNative.ClearEventListener();
            }
        }
    }

    /// <summary>
    /// Adds the specified collection of symbols to the set of subscribed symbols.
    /// All registered event listeners will receive update on the last events for all
    /// newly added symbols.
    /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXFeedSubscription.html#addSymbols-java.util.Collection-">Javadoc.</a>
    /// </summary>
    /// <param name="symbols">The collection of symbols.</param>
    public void AddSymbols(params object[] symbols) =>
        _subscriptionNative.AddSymbols(symbols);

    /// <summary>
    /// Adds the specified collection of symbols to the set of subscribed symbols.
    /// All registered event listeners will receive update on the last events for all
    /// newly added symbols.
    /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXFeedSubscription.html#addSymbols-java.util.Collection-">Javadoc.</a>
    /// </summary>
    /// <param name="symbols">The collection of symbols.</param>
    public void AddSymbols(IEnumerable<object> symbols) =>
        AddSymbols(symbols.ToArray());

    /// <summary>
    /// Removes the specified collection of symbols from the set of subscribed symbols.
    /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXFeedSubscription.html#removeSymbols-java.util.Collection-">Javadoc.</a>
    /// </summary>
    /// <param name="symbols">The collection of symbols.</param>
    public void RemoveSymbols(params object[] symbols) =>
        _subscriptionNative.RemoveSymbol(symbols);

    /// <summary>
    /// Removes the specified collection of symbols from the set of subscribed symbols.
    /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXFeedSubscription.html#removeSymbols-java.util.Collection-">Javadoc.</a>
    /// </summary>
    /// <param name="symbols">The collection of symbols.</param>
    public void RemoveSymbols(IEnumerable<object> symbols) =>
        RemoveSymbols(symbols.ToArray());

    /// <summary>
    /// Clears the set of subscribed symbols.
    /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXFeedSubscription.html#clear--">Javadoc.</a>
    /// </summary>
    public void Clear() =>
        _subscriptionNative.Clear();

    /// <summary>
    /// Closes this subscription and makes it <i>permanently</i> detached.
    /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXFeedSubscription.html#close--">Javadoc.</a>
    /// </summary>
    // ToDo Should notify the close. If it is attached to the feed, the feed should detach it.
    public void Close() =>
        _subscriptionNative.Close();

    /// <summary>
    /// Releases all resources used by the current instance of the <see cref="DXFeedSubscription"/> class.
    /// </summary>
    // ToDo Must use Close().
    public void Dispose() =>
        _subscriptionNative.Dispose();

    internal SubscriptionNative GetNative() =>
        _subscriptionNative;

    /// <summary>
    /// Wrapper function over native event listener calls.
    /// </summary>
    /// <param name="thread">The current isolate thread. <b>Ignored</b>.</param>
    /// <param name="events">The pointer-to-pointer events (array of pointers to events).</param>
    /// <param name="userData">The pointer to user data.</param>
    private unsafe void EventListenerFuncWrapper(nint thread, ListNative<EventTypeNative>* events, nint userData)
    {
        try
        {
            var eventList = EventMapper.FromNative(events);
            var listeners = _listeners;
            foreach (var listener in listeners)
            {
                listener(eventList);
            }
        }
        catch (Exception e)
        {
            // ToDo Add log entry.
            Console.Error.WriteLine($"Exception in {nameof(GetType)} event listener: {e}");
        }
    }
}
