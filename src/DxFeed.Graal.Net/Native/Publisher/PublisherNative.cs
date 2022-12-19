// <copyright file="PublisherNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace DxFeed.Graal.Net.Native.Publisher;

/// <summary>
/// Native wrapper over the Java <c>com.dxfeed.api.DXPublisher</c> class.
/// The location of the imported functions is in the header files <c>"dxfg_publisher.h"</c>.
/// </summary>
internal sealed unsafe class PublisherNative
{
    private readonly PublisherHandle* _publisherHandle;

    public PublisherNative(PublisherHandle* publisherHandle) =>
        _publisherHandle = publisherHandle;
}
