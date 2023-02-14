// <copyright file="BuilderNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using DxFeed.Graal.Net.Api;

namespace DxFeed.Graal.Net.Native.Endpoint;

/// <summary>
/// Native wrapper over the Java <c>com.dxfeed.api.DXEndpoint.Builder</c> class.
/// The location of the imported functions is in the header files <c>"dxfg_endpoint.h"</c>.
/// </summary>
internal sealed unsafe class BuilderNative : IDisposable
{
    private readonly BuilderSafeHandle _builderHandle;

    private BuilderNative(BuilderSafeHandle builderHandle) =>
        _builderHandle = builderHandle;

    public static BuilderNative Create() =>
        new(BuilderSafeHandle.Create());

    public void WithRole(DXEndpoint.Role role) =>
        _builderHandle.WithRole(role);

    public void WithProperty(string key, string value) =>
        _builderHandle.WithProperty(key, value);

    public bool SupportsProperty(string key) =>
        _builderHandle.SupportsProperty(key);

    public EndpointNative Build() =>
        new(_builderHandle.Build());

    public void Dispose() =>
        _builderHandle.Dispose();
}
