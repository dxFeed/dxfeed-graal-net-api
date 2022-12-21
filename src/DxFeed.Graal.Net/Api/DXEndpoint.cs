// <copyright file="DXEndpoint.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using DxFeed.Graal.Net.Api.Osub;
using DxFeed.Graal.Net.Native.Endpoint;
using DxFeed.Graal.Net.Utils;

namespace DxFeed.Graal.Net.Api;

/// <summary>
/// Manages network connections to <see cref="DXFeed"/> or <see cref="DXPublisher"/>.
/// This class is a wrapper for <see cref="EndpointNative"/>.
/// <br/>
/// For more details see <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXEndpoint.html">Javadoc</a>.
/// </summary>
public sealed class DXEndpoint : IDisposable
{
    /// <summary>
    /// Defines property for endpoint name that is used to distinguish multiple endpoints
    /// in the same JVM in logs and in other diagnostic means.
    /// Use <see cref="Builder.WithProperty(string,string)"/> method.
    /// This property is also changed by <see cref="Builder.WithName"/> method.
    /// </summary>
    public const string NameProperty = "name";

    /// <summary>
    /// Defines path to a file with properties for an endpoint with role
    /// <see cref="Role.Feed"/> or <see cref="Role.OnDemandFeed"/>.
    /// <br/>
    /// This file must be in the <a href="https://en.wikipedia.org/wiki/.properties">Java properties file format</a>.
    /// <br/>
    /// This property can also be set using <see cref="SystemProperty.SetProperty"/>,
    /// as the default property for all instances <see cref="DXEndpoint"/> with <see cref="Role.Feed"/> role.
    /// <br/>
    /// When the path to this properties file not provided (<see cref="SystemProperty.SetProperty"/>
    /// and <see cref="Builder.WithProperty(string,string)"/>),
    /// the file "dxfeed.properties" loaded from current runtime directory.
    /// It means that the corresponding file can be placed into the current directory with any need
    /// to specify additional properties.
    /// </summary>
    /// <seealso cref="Builder.WithProperty(string,string)"/>
    public const string DXFeedPropertiesProperty = "dxfeed.properties";

    /// <summary>
    /// Defines default connection address for an endpoint with role
    /// <see cref="Role.Feed"/> or <see cref="Role.OnDemandFeed"/>.
    /// Connection is established to this address by role <see cref="Role.Feed"/> as soon as endpoint is created.
    /// <br/>
    /// By default, without this property, connection is not established until <see cref="Connect"/> is invoked.
    /// <br/>
    /// Credentials for access to premium services may be configured with
    /// <see cref="DXFeedUserProperty"/> and <see cref="DXFeedPasswordProperty"/>.
    /// </summary>
    /// <seealso cref="Builder.WithProperty(string,string)"/>
    public const string DXFeedAddressProperty = "dxfeed.address";

    /// <summary>
    /// Defines default user name for an endpoint with role
    /// <see cref="Role.Feed"/> or <see cref="Role.OnDemandFeed"/>.
    /// </summary>
    /// <seealso cref="User"/>
    public const string DXFeedUserProperty = "dxfeed.user";

    /// <summary>
    /// Defines default password for an endpoint with role
    /// <see cref="Role.Feed"/> or <see cref="Role.OnDemandFeed"/>.
    /// </summary>
    /// <seealso cref="Password"/>
    public const string DXFeedPasswordProperty = "dxfeed.password";

    /// <summary>
    /// Defines thread pool size for an endpoint with role <see cref="Role.Feed"/>.
    /// By default, the thread pool size is equal to the number of available processors.
    /// </summary>
    /// <seealso cref="Builder.WithProperty(string,string)"/>
    public const string DXFeedThreadPoolSizeProperty = "dxfeed.threadPoolSize";

    /// <summary>
    /// Defines data aggregation period an endpoint with role <see cref="Role.Feed"/> that
    /// limits the rate of data notifications. For example, setting the value of this property
    /// to "0.1s" limits notification to once every 100ms (at most 10 per second).
    /// </summary>
    /// <seealso cref="Builder.WithProperty(string,string)"/>
    public const string DXFeedAggregationPeriodProperty = "dxfeed.aggregationPeriod";

    /// <summary>
    /// Set this property to <c>true</c> to turns on wildcard support.
    /// By default, the endpoint does not support wildcards. This property is needed for
    /// <see cref="WildcardSymbol"/> support and for the use of "tape:..." address in <see cref="DXPublisher"/>.
    /// </summary>
    /// <seealso cref="Builder.WithProperty(string,string)"/>
    public const string DXFeedWildcardEnableProperty = "dxfeed.wildcard.enable";

    /// <summary>
    /// Defines path to a file with properties for an endpoint with role <see cref="Role.Publisher"/>.
    /// <br/>
    /// This file must be in the <a href="https://en.wikipedia.org/wiki/.properties">Java properties file format</a>.
    /// <br/>
    /// This property can also be set using <see cref="SystemProperty.SetProperty"/>,
    /// as the default property for all instances <see cref="DXEndpoint"/> with <see cref="Role.Publisher"/> role.
    /// <br/>
    /// When the path to this properties file not provided (<see cref="SystemProperty.SetProperty"/>
    /// and <see cref="Builder.WithProperty(string,string)"/>),
    /// the file "dxpublisher.properties" loaded from current runtime directory.
    /// It means that the corresponding file can be placed into the current directory with any need
    /// to specify additional properties.
    /// </summary>
    /// <seealso cref="Builder.WithProperty(string,string)"/>
    public const string DXPublisherPropertiesProperty = "dxpublisher.properties";

    /// <summary>
    /// Defines default connection address for an endpoint with role <see cref="Role.Publisher"/>.
    /// Connection is established to this address as soon as endpoint is created.
    /// By default, connection is not established until <see cref="Connect"/> is invoked.
    /// </summary>
    /// <seealso cref="Builder.WithProperty(string,string)"/>
    public const string DXPublisherAddressProperty = "dxpublisher.address";

    /// <summary>
    /// Defines thread pool size for an endpoint with role <see cref="Role.Publisher"/>.
    /// By default, the thread pool size is equal to the number of available processors.
    /// </summary>
    /// <seealso cref="Builder.WithProperty(string,string)"/>
    public const string DXPublisherThreadPoolSizeProperty = "dxpublisher.threadPoolSize";

    /// <summary>
    /// Set this property to true to enable <see cref="Events.IEventType.EventTime"/> support.
    /// By default, the endpoint does not support event time.
    /// <br/>
    /// The event time is available only when the corresponding <see cref="DXEndpoint"/>
    /// is created with this property and the data source has embedded event times.
    /// This is typically <c>true</c> only for data events that are read from historical tape files.
    /// Events that are coming from a network connections do not have an embedded event time
    /// information and event time is not available for them anyway.
    /// </summary>
    public const string DXEndpointEventTimeProperty = "dxendpoint.eventTime";

    /// <summary>
    /// Set this property to to store all <see cref="Events.ILastingEvent{T}"/>
    /// and <see cref="Events.ILastingEvent{T}"/> events
    /// even when there is no subscription on them. By default, the endpoint stores only events from subscriptions.
    /// It works in the same way both for <see cref="DXFeed"/> and <see cref="DXPublisher"/>.
    /// <br/>
    /// Use this property with extreme care, since API does not currently provide any means to remove those events
    /// from the storage and there might be an effective memory leak
    /// if the spaces of symbols on which events are published grows without bound.
    /// </summary>
    public const string DXEndpointStoreEverythingProperty = "dxendpoint.storeEverything";

    /// <summary>
    /// Set this property to <c>true</c> to turn on nanoseconds precision business time.
    /// By default, this feature is turned off.
    /// Business time in most events is available with millisecond precision by default,
    /// while <see cref="Events.Market.Quote"/> events business <see cref="Events.Market.Quote.Time"/>
    /// is available with seconds precision.
    /// <br/>
    /// This method provides a higher-level control than turning on individual properties that are responsible
    /// for nano-time via <see cref="DXSchemeEnabledPropertyPrefix"/>.
    /// The later can be used to override of fine-time nano-time support for individual fields.
    /// Setting this property to <c>true</c> is essentially equivalent to setting:
    /// <code>
    /// dxscheme.enabled.Sequence=*
    /// dxscheme.enabled.TimeNanoPart=*
    /// </code>
    /// </summary>
    public const string DXSchemeNanoTimeProperty = "dxscheme.nanoTime";

    /// <summary>
    /// Defines whether a specified field from the scheme should be enabled instead of it's default behaviour.
    /// <br/>
    /// Use it according to following format:
    /// <br/>
    /// <c>dxscheme.enabled.field_property_name=event_name_mask_glob</c>
    /// <br/>
    /// For example, <b>dxscheme.enabled.TimeNanoPart=Trade</b> enables <c>NanoTimePart</c>
    /// internal field only in Trade events.
    /// There is a shortcut for turning on nano-time support using <see cref="DXSchemeNanoTimeProperty"/>.
    /// </summary>
    public const string DXSchemeEnabledPropertyPrefix = "dxscheme.enabled.";

    /// <summary>
    /// List of singleton instances.
    /// </summary>
    private static readonly Dictionary<Role, DXEndpoint> Instances = new();

    /// <summary>
    /// Endpoint native wrapper.
    /// </summary>
    private readonly EndpointNative _endpointNative;

    /// <summary>
    /// A delegate to to the endpoint state change listener.
    /// </summary>
    private readonly StateChangeListenerFunc _stateChangeListenerFunc;

    /// <summary>
    /// Lazy initialization of the <see cref="DXFeed"/> instance.
    /// </summary>
    private readonly Lazy<DXFeed> _feed;

    /// <summary>
    /// Lazy initialization of the <see cref="DXPublisher"/> instance.
    /// </summary>
    private readonly Lazy<DXPublisher> _publisher;

    /// <summary>
    /// Lock for listener list.
    /// </summary>
    private readonly object _listenersLock = new();

    /// <summary>
    /// List of state change listeners.
    /// </summary>
    private volatile ImmutableList<OnStateChangeListener> _listeners = ImmutableList.Create<OnStateChangeListener>();

    /// <summary>
    /// Initializes a new instance of the <see cref="DXEndpoint"/> class with specified endpoint native.
    /// </summary>
    /// <param name="endpointNative">The specified endpoint native.</param>
    private DXEndpoint(EndpointNative endpointNative)
    {
        _endpointNative = endpointNative;
        _stateChangeListenerFunc = StateChangeListenerFuncWrapper;
        _feed = new Lazy<DXFeed>(() => new DXFeed(_endpointNative.GetFeed()));
        _publisher = new Lazy<DXPublisher>(() => new DXPublisher(_endpointNative.GetPublisher()));
    }

    /// <summary>
    /// Notifies of a change in the state of this endpoint.
    /// </summary>
    /// <param name="oldState">The old state of endpoint.</param>
    /// <param name="newState">The new state of endpoint.</param>
    public delegate void OnStateChangeListener(State oldState, State newState);

    /// <summary>
    /// List of endpoint roles.
    /// </summary>
    /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXEndpoint.Role.html">Javadoc.</a>
    public enum Role
    {
        /// <summary>
        /// <c>Feed</c> endpoint connects to the remote data feed provider and is optimized for real-time or
        /// delayed data processing (<b>this is a default role</b>).
        /// </summary>
        Feed,

        /// <summary>
        /// <c>OnDemandFeed</c> endpoint is similar to <see cref="Feed"/>, but it is designed to be used with
        /// <c>OnDemandService</c> for historical data replay only.
        /// </summary>
        OnDemandFeed,

        /// <summary>
        /// <c>StreamFeed</c> endpoint is similar to <see cref="Feed"/>
        /// and also connects to the remote data feed provider, is designed for bulk parsing of data from files.
        /// </summary>
        StreamFeed,

        /// <summary>
        /// <c>Publisher</c> endpoint connects to the remote publisher hub (also known as multiplexor) or
        /// creates a publisher on the local host.
        /// </summary>
        Publisher,

        /// <summary>
        /// <c>StreamPublisher</c> endpoint is similar to <see cref="Publisher"/>
        /// and also connects to the remote publisher hub, but is designed for bulk publishing of data.
        /// </summary>
        StreamPublisher,

        /// <summary>
        /// <c>LocalHub</c> endpoint is a local hub without ability to establish network connections.
        /// </summary>
        LocalHub,
    }

    /// <summary>
    /// List of endpoint states.
    /// </summary>
    /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXEndpoint.State.html">Javadoc.</a>
    public enum State
    {
        /// <summary>
        /// Endpoint was created by is not connected to remote endpoints.
        /// </summary>
        NotConnected,

        /// <summary>
        /// The <see cref="Connect"/> method was called to establish connection to remove endpoint,
        /// but connection is not actually established yet or was lost.
        /// </summary>
        Connecting,

        /// <summary>
        /// The connection to remote endpoint is established.
        /// </summary>
        Connected,

        /// <summary>
        /// Endpoint was <see cref="Close"/>.
        /// </summary>
        Closed,
    }

    /// <summary>
    /// Gets a default application-wide singleton instance of DXEndpoint with a <see cref="Role.Feed"/> role.
    /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXEndpoint.html#getInstance--">Javadoc.</a>
    /// </summary>
    /// <returns>Returns singleton instance of <see cref="DXEndpoint"/>.</returns>
    public static DXEndpoint Instance =>
        GetInstance(Role.Feed);

    /// <summary>
    /// Gets a default application-wide singleton instance of DXEndpoint with a specific <see cref="Role"/>.
    /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXEndpoint.html#getInstance-com.dxfeed.api.DXEndpoint.Role-">Javadoc.</a>
    /// </summary>
    /// <param name="role">The <see cref="Role"/>.</param>
    /// <returns>Returns singleton instance of <see cref="DXEndpoint"/>.</returns>
    public static DXEndpoint GetInstance(Role role)
    {
        lock (Instances)
        {
            if (!Instances.TryGetValue(role, out var instance))
            {
                using var builder = NewBuilder().WithRole(role);
                instance = builder.Build();
                Instances[role] = instance;
            }

            return instance;
        }
    }

    /// <summary>
    /// Creates new <see cref="Builder"/> instance.
    /// Use <see cref="Builder.Build"/> to build an instance of <see cref="DXEndpoint"/> when
    /// all configuration properties were set.
    /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXEndpoint.html#newBuilder--">Javadoc.</a>
    /// </summary>
    /// <returns>The new <see cref="Builder"/> instance.</returns>
    public static Builder NewBuilder() =>
        new();

    /// <summary>
    /// Creates an endpoint with a role <see cref="Role.Feed"/>.
    /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXEndpoint.html#create--">Javadoc.</a>
    /// </summary>
    /// <returns>The created <see cref="DXEndpoint"/>.</returns>
    public static DXEndpoint Create()
    {
        using var builder = NewBuilder();
        return builder.WithRole(Role.Feed).Build();
    }

    /// <summary>
    /// Creates an endpoint with a specified <see cref="Role"/>.
    /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXEndpoint.html#create-com.dxfeed.api.DXEndpoint.Role-">Javadoc.</a>
    /// </summary>
    /// <param name="role">The specified <see cref="Role"/>.</param>
    /// <returns>The created <see cref="DXEndpoint"/>.</returns>
    public static DXEndpoint Create(Role role)
    {
        using var builder = NewBuilder();
        return builder.WithRole(role).Build();
    }

    /// <summary>
    /// Closes this endpoint. All network connection are terminated as with
    /// <see cref="Disconnect"/> method and no further connections
    /// can be established.
    /// The endpoint <see cref="State"/> immediately becomes <see cref="State.Closed"/>.
    /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXEndpoint.html#close--">Javadoc.</a>
    /// </summary>
    // ToDo Implement feed and publisher closure.
    public void Close() =>
        _endpointNative.Close();

    /// <summary>
    /// Closes this endpoint and wait until all pending data processing tasks are completed.
    /// This  method performs the same actions as close <see cref="Close"/>, but also awaits
    /// termination of all outstanding data processing tasks. It is designed to be used
    /// with <see cref="Role.StreamFeed"/> role after <see cref="AwaitNotConnected"/> method returns
    /// to make sure that file was completely processed.
    /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXEndpoint.html#closeAndAwaitTermination--">Javadoc.</a>
    /// </summary>
    public void CloseAndAwaitTermination() =>
        _endpointNative.CloseAndAwaitTermination();

    /// <summary>
    /// Gets the <see cref="Role"/> of this endpoint.
    /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXEndpoint.html#getRole--">Javadoc.</a>
    /// </summary>
    /// <returns>The <see cref="Role"/>.</returns>
    /// ToDo Returns role from current instance, without native code.
    public Role GetRole() =>
        _endpointNative.GetRole();

    /// <summary>
    /// Changes user name for this endpoint.
    /// This method shall be called before <see cref="Connect"/> together
    /// with <see cref="Password"/> to configure service access credentials.
    /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXEndpoint.html#user-java.lang.String-">Javadoc.</a>
    /// </summary>
    /// <param name="user">The user name.</param>
    /// <returns>Returns this <see cref="DXEndpoint"/>.</returns>
    public DXEndpoint User(string user)
    {
        _endpointNative.User(user);
        return this;
    }

    /// <summary>
    /// Changes password for this endpoint.
    /// This method shall be called before <see cref="Connect"/> together
    /// with <see cref="User"/> to configure service access credentials.
    /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXEndpoint.html#password-java.lang.String-">Javadoc.</a>
    /// </summary>
    /// <param name="password">The user password.</param>
    /// <returns>Returns this <see cref="DXEndpoint"/>.</returns>
    public DXEndpoint Password(string password)
    {
        _endpointNative.Password(password);
        return this;
    }

    /// <summary>
    /// Connects to the specified remote address. Previously established connections are closed if
    /// the new address is different from the old one.
    /// This method does nothing if address does not change or if this endpoint is <see cref="State.Closed"/>.
    /// The endpoint <see cref="State"/> immediately becomes <see cref="State.Connecting"/>  otherwise.
    /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXEndpoint.html#connect-java.lang.String-">Javadoc.</a>
    /// </summary>
    /// <param name="address">The data source address.</param>
    /// <returns>Returns this <see cref="DXEndpoint"/>.</returns>
    public DXEndpoint Connect(string address)
    {
        _endpointNative.Connect(address);
        return this;
    }

    /// <summary>
    /// Terminates all established network connections and initiates connecting again with the same address.
    /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXEndpoint.html#reconnect--">Javadoc.</a>
    /// </summary>
    public void Reconnect() =>
        _endpointNative.Reconnect();

    /// <summary>
    /// Terminates all remote network connections.
    /// This method does nothing if this endpoint is <see cref="State.Closed"/>.
    /// The endpoint <see cref="State"/> immediately becomes <see cref="State.NotConnected"/> otherwise.
    /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXEndpoint.html#disconnect--">Javadoc.</a>
    /// </summary>
    public void Disconnect() =>
        _endpointNative.Disconnect();

    /// <summary>
    /// Terminates all remote network connections and clears stored data.
    /// This method does nothing if this endpoint is <see cref="State.Closed"/>.
    /// The endpoint <see cref="State"/> immediately becomes <see cref="State.NotConnected"/> otherwise.
    /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXEndpoint.html#disconnectAndClear--">Javadoc.</a>
    /// </summary>
    public void DisconnectAndClear() =>
        _endpointNative.DisconnectAndClear();

    /// <summary>
    /// Waits until this endpoint stops processing data (becomes quiescent).
    /// This is important when writing data to file via "tape:..." connector to make sure that
    /// all published data was written before closing this endpoint.
    /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXEndpoint.html#awaitProcessed--">Javadoc.</a>
    /// </summary>
    public void AwaitProcessed() =>
        _endpointNative.AwaitProcessed();

    /// <summary>
    /// Waits while this endpoint <see cref="State"/> becomes <see cref="State.NotConnected"/> or
    /// <see cref="State.Closed"/>. It is a signal that any files that were opened with
    /// <see cref="Connect">Connect("file:...")</see> method were finished reading, but not necessary were completely
    /// processed by the corresponding subscription listeners. Use <see cref="CloseAndAwaitTermination"/> after
    /// this method returns to make sure that all processing has completed.
    /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXEndpoint.html#awaitNotConnected--">Javadoc.</a>
    /// </summary>
    public void AwaitNotConnected() =>
        _endpointNative.AwaitNotConnected();

    /// <summary>
    /// Gets the <see cref="State"/> of this endpoint.
    /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXEndpoint.html#getState--">Javadoc.</a>
    /// </summary>
    /// <returns>The <see cref="State"/>.</returns>
    public State GetState() =>
        EnumUtil.ValueOf<State>(_endpointNative.GetState());

    /// <summary>
    /// Adds listener that is notified about changes in <see cref="GetState"/> property.
    /// It removes the listener that was previously installed with
    /// <see cref="RemoveStateChangeListener"/> method.
    /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXEndpoint.html#addStateChangeListener-java.beans.PropertyChangeListener-">Javadoc.</a>
    /// </summary>
    /// <param name="listener">The listener to add.</param>
    public void AddStateChangeListener(OnStateChangeListener listener)
    {
        lock (_listenersLock)
        {
            if (_listeners.IsEmpty)
            {
                _endpointNative.SetStateChangeListener(_stateChangeListenerFunc);
            }

            _listeners = _listeners.Add(listener);
        }
    }

    /// <summary>
    /// Removes listener that is notified about changes in <see cref="GetState"/> property.
    /// It removes the listener that was previously installed with
    /// <see cref="AddStateChangeListener"/> method.
    /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXEndpoint.html#removeStateChangeListener-java.beans.PropertyChangeListener-">Javadoc.</a>
    /// </summary>
    /// <param name="listener">The listener to remove.</param>
    public void RemoveStateChangeListener(OnStateChangeListener listener)
    {
        lock (_listenersLock)
        {
            _listeners = _listeners.Remove(listener);

            if (_listeners.IsEmpty)
            {
                _endpointNative.ClearStateChangeListener();
            }
        }
    }

    /// <summary>
    /// Gets <see cref="DXFeed"/> that is associated with this endpoint.
    /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXEndpoint.html#getFeed--">Javadoc.</a>
    /// </summary>
    /// <returns>The <see cref="DXFeed"/>.</returns>
    public DXFeed GetFeed() =>
        _feed.Value;

    /// <summary>
    /// Gets <see cref="DXPublisher"/> that is associated with this endpoint.
    /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXEndpoint.html#getPublisher--">Javadoc.</a>
    /// </summary>
    /// <returns>The <see cref="DXPublisher"/>.</returns>
    public DXPublisher GetPublisher() =>
        _publisher.Value;

    /// <summary>
    /// Releases all resources used by the current instance of the <see cref="DXEndpoint"/> class.
    /// </summary>
    /// ToDo Must use Close().
    public void Dispose() =>
        _endpointNative.Dispose();

    /// <summary>
    /// Wrapper function over native change state listener calls.
    /// </summary>
    /// <param name="thread">The current isolate thread. <b>Ignored</b>.</param>
    /// <param name="oldState">The old state of endpoint.</param>
    /// <param name="newState">The new state of endpoint.</param>
    /// <param name="userData">The pointer to user data. <b>Ignored</b>.</param>
    private void StateChangeListenerFuncWrapper(nint thread, State oldState, State newState, nint userData)
    {
        var listeners = _listeners;
        foreach (var listener in listeners)
        {
            try
            {
                listener(oldState, newState);
            }
            catch (Exception e)
            {
                // ToDo Add log entry.
                Console.Error.WriteLine($"Exception in {nameof(GetType)} state change listener: {e}");
            }
        }
    }

    /// <summary>
    /// Builder class for <see cref="DXEndpoint"/> that supports additional configuration properties.
    /// This class is a wrapper for <see cref="BuilderNative"/>.
    /// <br/>
    /// For more details see <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXEndpoint.Builder.html">Javadoc</a>.
    /// </summary>
    public sealed class Builder : IDisposable
    {
        /// <summary>
        /// Endpoint builder native wrapper.
        /// </summary>
        private readonly BuilderNative _builderNative;

        // List of set properties.
        private readonly Dictionary<string, string> _props = new();

        // The current role.
        private Role _role = Role.Feed;

        /// <summary>
        /// Initializes a new instance of the <see cref="Builder"/> class.
        /// </summary>
        public Builder() =>
            _builderNative = BuilderNative.Create();

        /// <summary>
        /// Sets role for the created <see cref="DXEndpoint"/>.
        /// Default role is <see cref="Role.Feed"/>.
        /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXEndpoint.Builder.html#withRole-com.dxfeed.api.DXEndpoint.Role-">Javadoc.</a>
        /// </summary>
        /// <param name="role">The endpoint role.</param>
        /// <returns>Returns this <see cref="Builder"/>.</returns>
        public Builder WithRole(Role role)
        {
            _role = role;
            _builderNative.WithRole(role);
            return this;
        }

        /// <summary>
        /// Changes name that is used to distinguish multiple <see cref="DXEndpoint"/>
        /// in the same in logs and in other diagnostic means.
        /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXEndpoint.Builder.html#withName-java.lang.String-">Javadoc.</a>
        /// </summary>
        /// <param name="name">The endpoint name.</param>
        /// <returns>Returns this <see cref="Builder"/>.</returns>
        public Builder WithName(string name) =>
            WithProperty(NameProperty, name);

        /// <summary>
        /// Sets the specified property. Unsupported properties are ignored.
        /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXEndpoint.Builder.html#withProperty-java.lang.String-java.lang.String-">Javadoc.</a>
        /// </summary>
        /// <param name="key">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        /// <returns>Returns this <see cref="Builder"/>.</returns>
        public Builder WithProperty(string key, string value)
        {
            _props[key] = value;
            _builderNative.WithProperty(key, value);
            return this;
        }

        /// <summary>
        /// Sets the specified property. Unsupported properties are ignored.
        /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXEndpoint.Builder.html#withProperty-java.lang.String-java.lang.String-">Javadoc.</a>
        /// </summary>
        /// <param name="kvp">The key-value pair.</param>
        /// <returns>Returns this <see cref="Builder"/>.</returns>
        public Builder WithProperty(KeyValuePair<string, string> kvp)
        {
            WithProperty(kvp.Key, kvp.Value);
            return this;
        }

        /// <summary>
        /// Sets the specified properties from the provided key-value collection.
        /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXEndpoint.Builder.html#withProperties-java.util.Properties-">Javadoc.</a>
        /// </summary>
        /// <param name="properties">The key-value collection.</param>
        /// <returns>Returns this <see cref="Builder"/>.</returns>
        public Builder WithProperties(IReadOnlyDictionary<string, string> properties)
        {
            foreach (var property in properties)
            {
                WithProperty(property.Key, property.Value);
            }

            return this;
        }

        // ToDo Add method for parsing properties file in *.properties format (like in Java).

        /// <summary>
        /// Checks if the corresponding property key is supported.
        /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXEndpoint.Builder.html#supportsProperty-java.lang.String-">Javadoc.</a>
        /// </summary>
        /// <param name="key">The name of the property.</param>
        /// <returns>Returns <c>true</c> if the corresponding property key is supported.</returns>
        public bool SupportsProperty(string key) =>
            _builderNative.SupportsProperty(key);

        /// <summary>
        /// Builds <see cref="DXEndpoint"/> instance.
        /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXEndpoint.Builder.html#build--">Javadoc.</a>
        /// </summary>
        /// <returns>The created <see cref="DXEndpoint"/>.</returns>
        public DXEndpoint Build()
        {
            LoadDefaultPropertiesFileIfExist();
            return new(_builderNative.Build());
        }

        /// <summary>
        /// Releases all resources used by the current instance of the <see cref="Builder"/> class.
        /// </summary>
        public void Dispose() =>
            _builderNative.Dispose();

        /// <summary>
        /// Tries to load default properties file for <see cref="Role.Feed"/>, <see cref="Role.OnDemandFeed"/>
        /// and <see cref="Role.Publisher"/> role.
        /// The default properties file is loaded only if there are no system properties
        /// or user properties set with the same key.
        /// </summary>
        private void LoadDefaultPropertiesFileIfExist()
        {
            // The default properties file is only valid for the Feed, OnDemandFeed and Publisher roles.
            string propFileKey;
            switch (_role)
            {
                case Role.Feed:
                case Role.OnDemandFeed:
                    propFileKey = DXFeedPropertiesProperty;
                    break;
                case Role.Publisher:
                    propFileKey = DXPublisherPropertiesProperty;
                    break;
                default:
                    return;
            }

            // If propFileKey was set in system properties, don't try to load the default properties file.
            if (SystemProperty.GetProperty(propFileKey) != null)
            {
                return;
            }

            // If there is no propFileKey in user-set properties,
            // try load default properties file from current runtime directory.
            if (!_props.ContainsKey(propFileKey) && File.Exists(propFileKey))
            {
                // The default property file has the same value as the key.
                _builderNative.WithProperty(propFileKey, propFileKey);
            }
        }
    }
}
