// <copyright file="DXEndpointWrapper.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using DxFeed.Graal.Net.Native.Feed;
using DxFeed.Graal.Net.Native.Interop;
using DxFeed.Graal.Net.Native.Publisher;
using static DxFeed.Graal.Net.Api.DXEndpoint;

namespace DxFeed.Graal.Net.Native.Endpoint;

internal sealed unsafe class DXEndpointWrapper : IDisposable
{
    private readonly DXEndpointHandle endpoint;
    private readonly Lazy<FeedNative> feed;
    private readonly Lazy<PublisherNative> publisher;
    private readonly HandleMap<StateChangeListener, StateChangeListenerHandle> listeners;

    private DXEndpointWrapper(DXEndpointHandle endpoint)
    {
        this.endpoint = endpoint;
        feed = new(() => new FeedNative(endpoint.GetFeed()));
        publisher = new(() => new PublisherNative(endpoint.GetPublisher()));
        listeners = new(StateChangeListenerHandle.Create);
    }

    public void Close() =>
        endpoint.Close();

    public void CloseAndAwaitTermination() =>
        endpoint.CloseAndAwaitTermination();

    public void User(string user) =>
        endpoint.SetUser(user);

    public void Password(string password) =>
        endpoint.SetPassword(password);

    public void Connect(string address) =>
        endpoint.Connect(address);

    public void Reconnect() =>
        endpoint.Reconnect();

    public void Disconnect() =>
        endpoint.Disconnect();

    public void DisconnectAndClear() =>
        endpoint.DisconnectAndClear();

    public void AwaitProcessed() =>
        endpoint.AwaitProcessed();

    public void AwaitNotConnected() =>
        endpoint.AwaitNotConnected();

    public int GetState() =>
        endpoint.GetState();

    public void AddStateChangeListener(StateChangeListener listener) =>
        endpoint.AddStateChangeListener(listeners.Add(listener));

    public void RemoveStateChangeListener(StateChangeListener listener)
    {
        if (listeners.TryRemove(listener, out var handle))
        {
            using (handle)
            {
                endpoint.RemoveStateChangeListener(handle);
            }
        }
    }

    public FeedNative GetFeed() =>
        feed.Value;

    public PublisherNative GetPublisher() =>
        publisher.Value;

    public void Dispose() =>
        Close();

    public sealed class BuilderWrapper : IDisposable
    {
        private readonly BuilderHandle builder = BuilderHandle.Create();

        public void WithRole(Role role) =>
            builder.WithRole(role);

        public void WithProperty(string key, string value) =>
            builder.WithProperty(key, value);

        public bool SupportsProperty(string key) =>
            builder.SupportsProperty(key);

        public DXEndpointWrapper Build() =>
            new(builder.Build());

        public void Dispose() =>
            builder.Dispose();
    }
}
