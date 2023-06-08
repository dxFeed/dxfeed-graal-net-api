// <copyright file="DXEndpoint.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using DxFeed.Graal.Net.Api.Osub;
using DxFeed.Graal.Net.Native.Endpoint;
using DxFeed.Graal.Net.Native.ErrorHandling;
using DxFeed.Graal.Net.Utils;

namespace DxFeed.Graal.Net.Api;

/// <summary>
/// Manages network connections to <see cref="DXFeed"/> or <see cref="DXPublisher"/>.
/// <br/>
/// Porting a Java class <c>com.dxfeed.api.DXEndpoint</c>.
/// <br/>
/// For more details see <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXEndpoint.html">Javadoc</a>.
/// <br/>
/// There are ready-to-use singleton instances that are available with
/// <see cref="GetInstance()"/> and <see cref="GetInstance(Role)"/> methods as wel as
/// factory methods <see cref="Create()"/> and <see cref="Create(Role)"/>, and a number of configuration methods.
/// <br/>
/// Advanced properties can be configured with <see cref="Builder"/> (creates with <see cref="NewBuilder"/>).
///
/// <h3>Threads and locks</h3>
///
/// This class is thread-safe and can be used concurrently from multiple threads without external synchronization.
///
/// <h3>Lifetimes</h3>
///
/// This class will not be garbage-collected and its resources will not be freed until <see cref="Dispose"/>,
/// <see cref="Close"/> or <see cref="CloseAndAwaitTermination"/> methods are called. Calling these methods
/// ensures that instance can be safely garbage-collected when all outside references to it are lost.
/// If a reference to an instance of this class is lost before calling the above methods,
/// it causes a memory/resource leak.
/// This behavior was implemented intentionally.
/// Inner instances of <see cref="DXFeed"/> and <see cref="DXPublisher"/> have the same lifetime
/// as <see cref="DXEndpoint"/>.
/// <see cref="DXFeed"/> and <see cref="DXPublisher"/> have no public resource release methods,
/// and exist as long as <see cref="DXEndpoint"/> exists.
/// </summary>
///
/// <example>
/// <code>
/// DXFeed feed = DXEndpoint.Create()
///     .User("demo").Password("demo")
///     .Connect("demo.dxfeed.com:7300")
///     .GetFeed();
/// </code>
/// </example>
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
    /// as the default property for all instances <see cref="DXEndpoint"/> with <see cref="Role.Feed"/> or
    /// or <see cref="Role.OnDemandFeed"/> role.
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
    /// Set this property to to store all <see cref="Events.ILastingEvent"/>
    /// and <see cref="Events.ILastingEvent"/> events
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
    /// A list of singleton <see cref="DXEndpoint"/> instances with different roles.
    /// </summary>
    private static readonly ConcurrentDictionary<Role, Lazy<DXEndpoint>> Instances = new();

    /// <summary>
    /// Endpoint native wrapper.
    /// </summary>
    private readonly EndpointNative _endpointNative;

    /// <summary>
    /// The endpoint role.
    /// </summary>
    private readonly Role _role;

    /// <summary>
    /// The endpoint name.
    /// </summary>
    private readonly string _name;

    /// <summary>
    /// Lazy initialization of the <see cref="DXFeed"/> instance.
    /// </summary>
    private readonly Lazy<DXFeed> _feed;

    /// <summary>
    /// Lazy initialization of the <see cref="DXPublisher"/> instance.
    /// </summary>
    private readonly Lazy<DXPublisher> _publisher;

    /// <summary>
    /// A list of state change listeners callback.
    /// </summary>
    private ImmutableList<StateChangeListenerCallback> _listeners = ImmutableList.Create<StateChangeListenerCallback>();

    /// <summary>
    /// Initializes a new instance of the <see cref="DXEndpoint"/>
    /// class with specified <see cref="EndpointNative"/>, <see cref="Role"/> and properties.
    /// </summary>
    /// <param name="endpointNative">The specified <see cref="EndpointNative"/>.</param>
    /// <param name="role">The endpoint role.</param>
    /// <param name="name">The endpoint name.</param>
    private DXEndpoint(EndpointNative endpointNative, Role role, string name)
    {
        _endpointNative = endpointNative;
        _role = role;
        _name = name;

        _feed = new Lazy<DXFeed>(() => new DXFeed(_endpointNative.GetFeed()));
        _publisher = new Lazy<DXPublisher>(() => new DXPublisher(_endpointNative.GetPublisher()));

        unsafe
        {
            // Add a listener and create a GCHandle to avoid garbage collection.
            // GCHandle will be released when the listener receives the Closed state.
            _endpointNative.AddStateChangeListener(&OnStateChanges, GCHandle.Alloc(this));
        }
    }

    /// <summary>
    /// Notifies a change in the state of this endpoint.
    /// </summary>
    /// <param name="oldState">The old state of endpoint.</param>
    /// <param name="newState">The new state of endpoint.</param>
    public delegate void StateChangeListenerCallback(State oldState, State newState);

    /// <summary>
    /// A list of endpoint roles.
    /// </summary>
    public enum Role
    {
        /// <summary>
        /// <c>Feed</c> endpoint connects to the remote data feed provider
        /// and is optimized for real-time or delayed data processing (<b>this is a default role</b>).
        /// <see cref="GetFeed"/> method returns feed object that subscribes
        /// to the remote data feed provider and receives events from it.
        /// When event processing threads cannot keep up (don't have enough CPU time), data is dynamically conflated
        /// to minimize latency between received events and their processing time.
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
        /// <see cref="GetFeed"/> method returns feed object that subscribes
        /// to the data from the opened files and receives events from them.
        /// Events from the files are not conflated, are not skipped, and are processed as fast as possible.
        /// </summary>
        StreamFeed,

        /// <summary>
        /// <c>Publisher</c> endpoint connects to the remote publisher hub (also known as multiplexor) or
        /// creates a publisher on the local host.
        /// <see cref="GetPublisher"/> method returns a publisher object
        /// that publishes events to all connected feeds.
        /// </summary>
        Publisher,

        /// <summary>
        /// <c>StreamPublisher</c> endpoint is similar to <see cref="Publisher"/>
        /// and also connects to the remote publisher hub, but is designed for bulk publishing of data.
        /// <see cref="GetPublisher"/> method returns a publisher object that publishes events
        /// to all connected feeds.
        /// Published events are not conflated, are not skipped, and are processed as fast as possible.
        /// </summary>
        StreamPublisher,

        /// <summary>
        /// <c>LocalHub</c> endpoint is a local hub without ability to establish network connections.
        /// Events that are published via <see cref="GetPublisher"/> are delivered to local
        /// <see cref="GetFeed"/> only.
        /// </summary>
        LocalHub,
    }

    /// <summary>
    /// A list of endpoint states.
    /// </summary>
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
    /// Most applications use only a single data-source and should rely on this method to get one.
    /// </summary>
    /// <returns>Returns singleton instance of <see cref="DXEndpoint"/>.</returns>
    public static DXEndpoint GetInstance() =>
        GetInstance(Role.Feed);

    /// <summary>
    /// Gets a default application-wide singleton instance of <see cref="DXEndpoint"/>
    /// with a specific <see cref="Role"/>.
    /// Most applications use only a single data-source and should rely on this method to get one.
    /// </summary>
    /// <param name="role">The <see cref="Role"/>.</param>
    /// <returns>Returns singleton instance of <see cref="DXEndpoint"/>.</returns>
    public static DXEndpoint GetInstance(Role role) =>
        Instances.GetOrAdd(role, r => new Lazy<DXEndpoint>(() => Create(r))).Value;

    /// <summary>
    /// Creates new <see cref="Builder"/> instance.
    /// Use <see cref="Builder.Build"/> to build an instance of <see cref="DXEndpoint"/> when
    /// all configuration properties were set.
    /// </summary>
    /// <returns>The created <see cref="Builder"/> instance.</returns>
    public static Builder NewBuilder() =>
        new();

    /// <summary>
    /// Creates an endpoint with a role <see cref="Role.Feed"/>.
    /// </summary>
    /// <returns>The created <see cref="DXEndpoint"/>.</returns>
    public static DXEndpoint Create() =>
        Create(Role.Feed);

    /// <summary>
    /// Creates an endpoint with a specified <see cref="Role"/>.
    /// </summary>
    /// <param name="role">The specified <see cref="Role"/>.</param>
    /// <returns>The created <see cref="DXEndpoint"/>.</returns>
    public static DXEndpoint Create(Role role) =>
        NewBuilder().WithRole(role).Build();

    /// <summary>
    /// Closes this endpoint. All network connection are terminated as with
    /// <see cref="Disconnect"/> method and no further connections can be established.
    /// The endpoint <see cref="State"/> immediately becomes <see cref="State.Closed"/>.
    /// <br/>
    /// This method ensures that <see cref="DXEndpoint"/> can be safely garbage-collected
    /// when all outside references to it are lost.
    /// </summary>
    public void Close()
    {
        _endpointNative.Close();
        CloseInner();
    }

    /// <summary>
    /// Closes this endpoint and wait until all pending data processing tasks are completed.
    /// This  method performs the same actions as close <see cref="Close"/>, but also awaits
    /// termination of all outstanding data processing tasks. It is designed to be used
    /// with <see cref="Role.StreamFeed"/> role after <see cref="AwaitNotConnected"/> method returns
    /// to make sure that file was completely processed.
    /// <br/>
    /// <b>This method is blocking.</b>
    /// <br/>
    /// This method ensures that <see cref="DXEndpoint"/> can be safely garbage-collected
    /// when all outside references to it are lost.
    /// </summary>
    public void CloseAndAwaitTermination()
    {
        _endpointNative.CloseAndAwaitTermination();
        CloseInner();
    }

    /// <summary>
    /// Gets the <see cref="Role"/> of this endpoint.
    /// </summary>
    /// <returns>The <see cref="Role"/>.</returns>
    public Role GetRole() =>
        _role;

    /// <summary>
    /// Gets the <see cref="State"/> of this endpoint.
    /// </summary>
    /// <returns>The <see cref="State"/>.</returns>
    public State GetState() =>
        EnumUtil.ValueOf((State)_endpointNative.GetState());

    /// <summary>
    /// Gets a value indicating whether if this endpoint is closed.
    /// There is a shortcut for <see cref="GetState"/> == <see cref="State.Closed"/>.
    /// </summary>
    /// <returns>Returns <c>true</c> if this endpoint is closed.</returns>
    public bool IsClosed() =>
        GetState() == State.Closed;

    /// <summary>
    /// Gets endpoint name.
    /// </summary>
    /// <returns>Returns endpoint name.</returns>
    public string GetName() =>
        _name;

    /// <summary>
    /// Changes user name for this endpoint.
    /// This method shall be called before <see cref="Connect"/> together
    /// with <see cref="Password"/> to configure service access credentials.
    /// </summary>
    /// <param name="user">The user name.</param>
    /// <returns>Returns this <see cref="DXEndpoint"/>.</returns>
    /// <exception cref="ArgumentNullException">If user is null.</exception>
    public DXEndpoint User(string user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        _endpointNative.User(user);
        return this;
    }

    /// <summary>
    /// Changes password for this endpoint.
    /// This method shall be called before <see cref="Connect"/> together
    /// with <see cref="User"/> to configure service access credentials.
    /// </summary>
    /// <param name="password">The user password.</param>
    /// <returns>Returns this <see cref="DXEndpoint"/>.</returns>
    /// <exception cref="ArgumentNullException">If password is null.</exception>
    public DXEndpoint Password(string password)
    {
        if (password == null)
        {
            throw new ArgumentNullException(nameof(password));
        }

        _endpointNative.Password(password);
        return this;
    }

    /// <summary>
    /// Connects to the specified remote address. Previously established connections are closed if
    /// the new address is different from the old one.
    /// This method does nothing if address does not change or if this endpoint is <see cref="State.Closed"/>.
    /// The endpoint <see cref="State"/> immediately becomes <see cref="State.Connecting"/> otherwise.
    /// <br/>
    /// The address string is provided with the market data vendor agreement.
    /// Use "demo.dxfeed.com:7300" for a demo quote feed.
    /// <br/>
    /// <ul>
    ///     <li> <c>host:port</c> to establish a TCP/IP connection.</li>
    ///     <li> <c>:port</c> to listen for a TCP/IP connection with a plain socket connector
    ///     (good for up to a few hundred of connections).</li>
    /// </ul>
    /// <br/>
    /// For premium services access credentials must be configured before invocation of <see cref="Connect"/> method
    /// using <see cref="User"/> and <see cref="Password"/> methods.
    /// <br/>
    /// <b>This method does not wait until connection actually gets established</b>. The actual connection establishment
    /// happens asynchronously after the invocation of this method. However, this method waits until notification
    /// about state transition from <see cref="State.NotConnected"/> to <see cref="State.Connecting"/>
    /// gets processed by all <see cref="StateChangeListenerCallback"/> that were installed via
    /// <see cref="AddStateChangeListener"/> method.
    /// </summary>
    /// <param name="address">The data source address.</param>
    /// <returns>Returns this <see cref="DXEndpoint"/>.</returns>
    /// <exception cref="ArgumentNullException">If address is null.</exception>
    /// <exception cref="JavaException">If address string is malformed.</exception>
    public DXEndpoint Connect(string address)
    {
        if (address == null)
        {
            throw new ArgumentNullException(nameof(address));
        }

        _endpointNative.Connect(address);
        return this;
    }

    /// <summary>
    /// Terminates all established network connections and initiates connecting again with the same address.
    /// <br/>
    /// The effect of the method is alike to invoking <see cref="Disconnect"/> and <see cref="Connect"/>
    /// with the current address, but internal resources used for connections may be reused by implementation.
    /// TCP connections with multiple target addresses will try switch to an alternative address, configured
    /// reconnect timeouts will apply.
    /// <br/>
    /// <b>Note:</b> The method will not connect endpoint that was not initially connected with
    /// <see cref="Connect"/> method or was disconnected with <see cref="Disconnect"/> method.
    /// <br/>
    /// The method initiates a short-path way for reconnecting, so whether observers will have a chance to see
    /// an intermediate state <see cref="State.NotConnected"/> depends on the implementation.
    /// </summary>
    public void Reconnect() =>
        _endpointNative.Reconnect();

    /// <summary>
    /// Terminates all remote network connections.
    /// This method does nothing if this endpoint is <see cref="State.Closed"/>.
    /// The endpoint <see cref="State"/> immediately becomes <see cref="State.NotConnected"/> otherwise.
    /// <br/>
    /// This method does not release all resources that are associated with this endpoint.
    /// Use <see cref="Close"/>, <see cref="Dispose"/> or <see cref="CloseAndAwaitTermination"/>
    /// methods to release all resources.
    /// </summary>
    public void Disconnect() =>
        _endpointNative.Disconnect();

    /// <summary>
    /// Terminates all remote network connections and clears stored data.
    /// This method does nothing if this endpoint is <see cref="State.Closed"/>.
    /// The endpoint <see cref="State"/> immediately becomes <see cref="State.NotConnected"/> otherwise.
    /// <br/>
    /// This method does not release all resources that are associated with this endpoint.
    /// Use <see cref="Close"/>, <see cref="Dispose"/> or <see cref="CloseAndAwaitTermination"/>
    /// methods to release all resources.
    /// </summary>
    public void DisconnectAndClear() =>
        _endpointNative.DisconnectAndClear();

    /// <summary>
    /// Waits until this endpoint stops processing data (becomes quiescent).
    /// This is important when writing data to file via "tape:..." connector to make sure that
    /// all published data was written before closing this endpoint.
    /// <br/>
    /// <b>This method is blocking.</b>
    /// </summary>
    public void AwaitProcessed() =>
        _endpointNative.AwaitProcessed();

    /// <summary>
    /// Waits while this endpoint <see cref="State"/> becomes <see cref="State.NotConnected"/> or
    /// <see cref="State.Closed"/>. It is a signal that any files that were opened with
    /// <see cref="Connect">Connect("file:...")</see> method were finished reading, but not necessary were completely
    /// processed by the corresponding subscription listeners. Use <see cref="CloseAndAwaitTermination"/> after
    /// this method returns to make sure that all processing has completed.
    /// <br/>
    /// <b>This method is blocking.</b>
    /// </summary>
    public void AwaitNotConnected() =>
        _endpointNative.AwaitNotConnected();

    /// <summary>
    /// Adds listener that is notified about changes in <see cref="GetState"/> property.
    /// <br/>
    /// Installed listener can be removed with <see cref="RemoveStateChangeListener"/> method.
    /// </summary>
    /// <param name="listener">The listener to add.</param>
    public void AddStateChangeListener(StateChangeListenerCallback listener) =>
        ImmutableInterlocked.Update(ref _listeners, (list, added) => list.Add(added), listener);

    /// <summary>
    /// Removes listener that is notified about changes in <see cref="GetState"/> property.
    /// <br/>
    /// It removes the listener that was previously installed with <see cref="AddStateChangeListener"/> method.
    /// </summary>
    /// <param name="listener">The listener to remove.</param>
    public void RemoveStateChangeListener(StateChangeListenerCallback listener) =>
        ImmutableInterlocked.Update(ref _listeners, (list, removed) => list.Remove(removed), listener);

    /// <summary>
    /// Gets <see cref="DXFeed"/> that is associated with this endpoint.
    /// </summary>
    /// <returns>The <see cref="DXFeed"/>.</returns>
    public DXFeed GetFeed() =>
        _feed.Value;

    /// <summary>
    /// Gets <see cref="DXPublisher"/> that is associated with this endpoint.
    /// </summary>
    /// <returns>The <see cref="DXPublisher"/>.</returns>
    public DXPublisher GetPublisher() =>
        _publisher.Value;

    /// <summary>
    /// Closes this endpoint and releases all resources used
    /// by the current instance of the <see cref="DXEndpoint"/> class.
    /// This is the same as <see cref="Close"/>.
    /// <br/>
    /// This method ensures that <see cref="DXEndpoint"/> can be safely garbage-collected
    /// when all outside references to it are lost.
    /// </summary>
    public void Dispose() =>
        Close();

    /// <summary>
    /// Callback function.
    /// It is called from the native code when the state of the endpoint changes.
    /// </summary>
    /// <param name="thread">The current isolate thread. <b>Ignored</b>.</param>
    /// <param name="oldState">The old state of endpoint.</param>
    /// <param name="newState">The new state of endpoint.</param>
    /// <param name="self">The endpoint handle.</param>
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static void OnStateChanges(nint thread, int oldState, int newState, nint self)
    {
        var handle = GCHandle.FromIntPtr(self);
        var endpoint = handle.Target as DXEndpoint;
        endpoint?.FireStateChanges((State)oldState, (State)newState);

        // If a closed state occurs, we can free self handle.
        if ((State)newState == State.Closed)
        {
            handle.Free();
        }
    }

    /// <summary>
    /// Notifies all listeners of a change of state.
    /// </summary>
    /// <param name="oldState">The old state of endpoint.</param>
    /// <param name="newState">The new state of endpoint.</param>
    private void FireStateChanges(State oldState, State newState)
    {
        var listeners = Volatile.Read(ref _listeners);
        foreach (var listener in listeners)
        {
            try
            {
                listener(oldState, newState);
            }
            catch (Exception e)
            {
                // ToDo Add log entry.
                Console.Error.WriteLine($"Exception in {_name} endpoint state change listener({listener.Method}): {e}");
            }
        }
    }

    /// <summary>
    /// Closes all associated resources with this <see cref="DXEndpoint"/>.
    /// </summary>
    private void CloseInner()
    {
        if (_feed.IsValueCreated)
        {
            _feed.Value.Close();
        }
    }

    /// <summary>
    /// Builder class for <see cref="DXEndpoint"/> that supports additional configuration properties.
    /// <br/>
    /// Porting a Java class <c>com.dxfeed.api.DXEndpoint.Builder</c>.
    /// <br/>
    /// For more details see <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXEndpoint.Builder.html">Javadoc</a>.
    ///
    /// <h3>Default properties file</h3>
    ///
    /// The <see cref="Build"/> method tries to load the default property file for the <see cref="Role.Feed"/>,
    /// <see cref="Role.OnDemandFeed"/> and <see cref="Role.Publisher"/> role.
    /// The default properties file is loaded only if there are no system properties (<see cref="SystemProperty"/>)
    /// or user properties (<see cref="WithProperty(string,string)"/>) set with the same key
    /// (<see cref="DXFeedPropertiesProperty"/>, <see cref="DXPublisherPropertiesProperty"/>)
    /// and the file exists and is readable.
    /// <br/>
    /// This file must be in the <a href="https://en.wikipedia.org/wiki/.properties">Java properties file format</a>.
    ///
    /// <h3>Endpoint name</h3>
    ///
    /// If no endpoint name has been specified (<see cref="WithName"/>), the default name will be used.
    /// The default name includes a counter that increments each time an endpoint is created ("qdnet", "qdnet-1", etc.).
    /// To get the name of the created endpoint, call the <see cref="GetName"/> method.
    ///
    /// <h3>Threads and locks</h3>
    ///
    /// This class is thread-safe and can be used concurrently from multiple threads without external synchronization.
    /// </summary>
    public class Builder
    {
        /// <summary>
        /// A counter that is incremented every time an endpoint is created.
        /// </summary>
        private static ulong _instancesNumerator;

        /// <summary>
        /// This lazy builder instance is used only to define supported properties.
        /// This instance is created only when the <see cref="SupportsProperty"/> method is called.
        /// When it is created, it will be correctly deleted later,
        /// using the finalizer (<see cref="SafeHandle"/>).
        /// The implementation of the definition of supported properties may change in the future.
        /// </summary>
        private readonly Lazy<BuilderNative> _builderForDefineSupportProperties = new(BuilderNative.Create);

        /// <summary>
        /// List of user-defined properties.
        /// </summary>
        private readonly ConcurrentDictionary<string, string> _props = new();

        /// <summary>
        /// Current role for implementations of <see cref="Builder"/>.
        /// </summary>
        private volatile Role _role = Role.Feed;

        /// <summary>
        /// Changes name that is used to distinguish multiple <see cref="DXEndpoint"/>
        /// in the same in logs and in other diagnostic means.
        /// This is a shortcut for
        /// <see cref="WithProperty(string,string)">WithProperty(DXEndpoint.NameProperty, name)</see>.
        /// </summary>
        /// <param name="name">The endpoint name.</param>
        /// <returns>Returns this <see cref="Builder"/>.</returns>
        /// <exception cref="ArgumentNullException">If the name is null.</exception>
        public Builder WithName(string name) =>
            WithProperty(NameProperty, name);

        /// <summary>
        /// Sets role for the created <see cref="DXEndpoint"/>.
        /// Default role is <see cref="Role.Feed"/>.
        /// </summary>
        /// <param name="role">The endpoint role.</param>
        /// <returns>Returns this <see cref="Builder"/>.</returns>
        /// <exception cref="ArgumentException"> If the role does not exist.</exception>
        public Builder WithRole(Role role)
        {
            // Verifiable assignment.
            _role = EnumUtil.ValueOf(role);

            return this;
        }

        /// <summary>
        /// Sets the specified property. Unsupported properties are ignored.
        /// </summary>
        /// <param name="key">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        /// <returns>Returns this <see cref="Builder"/>.</returns>
        /// <exception cref="ArgumentNullException">If key or value is null.</exception>
        public Builder WithProperty(string key, string value)
        {
            if (key == null || value == null)
            {
                throw new ArgumentNullException(key == null ? nameof(key) : nameof(value));
            }

            _props[key] = value;

            return this;
        }

        /// <summary>
        /// Sets the specified property. Unsupported properties are ignored.
        /// </summary>
        /// <param name="kvp">The key-value pair.</param>
        /// <returns>Returns this <see cref="Builder"/>.</returns>\
        /// <exception cref="ArgumentNullException">If key or value is null.</exception>
        public Builder WithProperty(KeyValuePair<string, string> kvp)
        {
            WithProperty(kvp.Key, kvp.Value);
            return this;
        }

        /// <summary>
        /// Sets the specified properties from the provided key-value collection. Unsupported properties are ignored.
        /// </summary>
        /// <param name="properties">The key-value collection.</param>
        /// <returns>Returns this <see cref="Builder"/>.</returns>
        /// <exception cref="ArgumentNullException">If key or value inside the properties is null.</exception>
        public Builder WithProperties(IReadOnlyDictionary<string, string> properties)
        {
            foreach (var property in properties)
            {
                WithProperty(property);
            }

            return this;
        }

        /// <summary>
        /// Checks if the corresponding property key is supported.
        /// </summary>
        /// <param name="key">The name of the property.</param>
        /// <returns>
        /// Returns <c>true</c> if the corresponding property key is supported; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">If the key is null.</exception>
        public bool SupportsProperty(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            // Use a separate instance of the builder to define supported properties.
            return _builderForDefineSupportProperties.Value.SupportsProperty(key);
        }

        /// <summary>
        /// Builds <see cref="DXEndpoint"/> instance.
        /// This method tries to load default properties file.
        /// </summary>
        /// <returns>The created <see cref="DXEndpoint"/>.</returns>
        /// <exception cref="JavaException">If the error occurred on the java side.</exception>
        public DXEndpoint Build()
        {
            using var builder = BuilderNative.Create();
            var role = _role;
            builder.WithRole((int)role);

            // Create properties snapshot.
            // This ensures that the properties will not be changed from another thread.
            var props = new Dictionary<string, string>();
            foreach (var prop in _props)
            {
                builder.WithProperty(prop.Key, prop.Value);
                props[prop.Key] = prop.Value;
            }

            var name = GetOrCreateEndpointName(props);
            builder.WithProperty(NameProperty, name);

            LoadDefaultPropertiesFileIfNeeded(builder, role, props);

            return new(builder.Build(), role, name);
        }

        /// <summary>
        /// Gets or creates an endpoint name.
        /// If there is no <see cref="NameProperty"/> in the user-defined properties,
        /// it returns a default name that includes a counter that increments each time an endpoint is created.
        /// </summary>
        /// <param name="props">The user-defined properties.</param>
        /// <returns>Returns the name of the endpoint.</returns>
        private static string GetOrCreateEndpointName(IDictionary<string, string> props)
        {
            if (props.TryGetValue(NameProperty, out var name))
            {
                return name;
            }

            // Decrement the number, because Interlocked.Increment returns incremented value.
            var number = Interlocked.Increment(ref _instancesNumerator) - 1;
            return $"qdnet{(number == 0 ? string.Empty : $"-{number}")}";
        }

        /// <summary>
        /// Tries to load a default properties file for the specified builder,
        /// with the specified role.
        /// <param name="builder">The specified builder.</param>
        /// <param name="role">The current builder role.</param>
        /// <param name="props">The user-defined properties for this builder.</param>
        /// </summary>
        private static void LoadDefaultPropertiesFileIfNeeded(
            BuilderNative builder,
            Role role,
            IReadOnlyDictionary<string, string> props)
        {
            // The default properties file is valid only for the
            // Feed, OnDemandFeed and Publisher roles.
            string propFileKey;
            switch (role)
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

            // If propFileKey has been set in the system properties,
            // don't try to load the default properties file.
            if (!string.IsNullOrEmpty(SystemProperty.GetProperty(propFileKey)))
            {
                return;
            }

            // If there is no propFileKey in the user-defined properties,
            // tries loading the default properties file from the current runtime directory if the file exists.
            if (!props.ContainsKey(propFileKey) && File.Exists(propFileKey))
            {
                // The default property file has the same value as the key.
                builder.WithProperty(propFileKey, propFileKey);
            }
        }
    }
}
