// <copyright file="EndpointNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.Endpoint.Handles;
using DxFeed.Graal.Net.Native.Feed;
using DxFeed.Graal.Net.Native.Publisher;

namespace DxFeed.Graal.Net.Native.Endpoint;

/// <summary>
/// Native wrapper over the Java <c>com.dxfeed.api.DXEndpoint</c> class.
/// </summary>
internal sealed unsafe class EndpointNative : IDisposable
{
    private readonly EndpointSafeHandle _endpointHandle;
    private readonly Lazy<FeedNative> _feedNative;
    private readonly Lazy<PublisherNative> _publisherNative;

    internal EndpointNative(EndpointSafeHandle endpointHandle)
    {
        _endpointHandle = endpointHandle;
        _feedNative = new Lazy<FeedNative>(() => new FeedNative(_endpointHandle.GetFeed()));
        _publisherNative = new Lazy<PublisherNative>(() => new PublisherNative(_endpointHandle.GetPublisher()));
    }

    public void Close() =>
        _endpointHandle.Close();

    public void CloseAndAwaitTermination() =>
        _endpointHandle.CloseAndAwaitTermination();

    public void User(string user) =>
        _endpointHandle.SetUser(user);

    public void Password(string password) =>
        _endpointHandle.SetPassword(password);

    public void Connect(string address) =>
        _endpointHandle.Connect(address);

    public void Reconnect() =>
        _endpointHandle.Reconnect();

    public void Disconnect() =>
        _endpointHandle.Disconnect();

    public void DisconnectAndClear() =>
        _endpointHandle.DisconnectAndClear();

    public void AwaitProcessed() =>
        _endpointHandle.AwaitProcessed();

    public void AwaitNotConnected() =>
        _endpointHandle.AwaitNotConnected();

    public int GetState() =>
        _endpointHandle.GetState();

    public void AddStateChangeListener(
        delegate* unmanaged[Cdecl]<nint, int, int, nint, void> listener,
        GCHandle userData)
    {
        using var stateChangeListenerHandle = StateChangeListenerSafeHandle.Create(listener, userData);
        _endpointHandle.AddStateChangeListener(stateChangeListenerHandle);
    }

    public FeedNative GetFeed() =>
        _feedNative.Value;

    public PublisherNative GetPublisher() =>
        _publisherNative.Value;

    public void Dispose() =>
        _endpointHandle.Dispose();
}
