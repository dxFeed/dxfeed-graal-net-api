// <copyright file="DXPublisher.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Native.Publisher;

namespace DxFeed.Graal.Net.Api;

/// <summary>
/// Provides API for publishing of events to local or remote <see cref="DXFeed"/>.
/// This class is a wrapper for <see cref="PublisherNative"/>.
/// <br/>
/// For more details see <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXPublisher.html">Javadoc</a>.
/// </summary>
public class DXPublisher
{
    /// <summary>
    /// Publisher native wrapper.
    /// </summary>
    private readonly PublisherNative _publisherNative;

    /// <summary>
    /// Initializes a new instance of the <see cref="DXPublisher"/> class with specified publisher native.
    /// </summary>
    /// <param name="publisherNative">The specified publisher native.</param>
    internal DXPublisher(PublisherNative publisherNative) =>
        _publisherNative = publisherNative;

    /// <summary>
    /// Gets a default application-wide singleton instance of <see cref="DXPublisher"/>.
    /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXPublisher.html#getInstance--">Javadoc.</a>
    /// </summary>
    public static DXPublisher Instance =>
        DXEndpoint.GetInstance(DXEndpoint.Role.Publisher).GetPublisher();

    /// <summary>
    /// Publishes events to the corresponding feed. If the <see cref="DXEndpoint"/> of this publisher has
    /// <see cref="DXEndpoint.Role.Publisher"/> role and it is connected, the
    /// published events will be delivered to the remote endpoints. Local <see cref="DXEndpoint.GetFeed"/> will
    /// always receive published events.
    /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXPublisher.html#publishEvents-java.util.Collection-">Javadoc.</a>
    /// </summary>
    /// <param name="events">The list of events to publish.</param>
    // ToDo Implement method.
    public void PublishEvents(params IEventType[] events) =>
        throw new NotImplementedException();

    /// <summary>
    /// Publishes events to the corresponding feed. If the <see cref="DXEndpoint"/> of this publisher has
    /// <see cref="DXEndpoint.Role.Publisher"/> role and it is connected, the
    /// published events will be delivered to the remote endpoints. Local <see cref="DXEndpoint.GetFeed"/> will
    /// always receive published events.
    /// <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/api/DXPublisher.html#publishEvents-java.util.Collection-">Javadoc.</a>
    /// </summary>
    /// <param name="events">The collection of events to publish.</param>
    // ToDo Implement method.
    public void PublishEvents(IEnumerable<IEventType> events) =>
        PublishEvents(events.ToArray());
}
