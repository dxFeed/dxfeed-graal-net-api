// <copyright file="PublisherNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Collections.Generic;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Native.ErrorHandling;
using DxFeed.Graal.Net.Native.Events;
using DxFeed.Graal.Net.Native.Graal;
using DxFeed.Graal.Net.Native.Interop;

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

    public void PublishEvents(IEnumerable<IEventType> events)
    {
        var eventList = EventMapper.ToNative(events);
        try
        {
            PublisherImport.PublishEvents(GetCurrentThread(), _publisherHandle, eventList);
        }
        finally
        {
            EventMapper.Release(eventList);
        }
    }

    private static nint GetCurrentThread() =>
        Isolate.Instance.IsolateThread;

    /// <summary>
    /// Contains imported functions from native code.
    /// </summary>
    private static class PublisherImport
    {
        public static void PublishEvents(
            nint thread,
            PublisherHandle* publisherHandle,
            ListNative<EventTypeNative>* events) =>
            ErrorCheck.NativeCall(thread, NativePublishEvents(thread, publisherHandle, events));

        [DllImport(
            ImportInfo.DllName,
            EntryPoint = "dxfg_DXPublisher_publishEvents")]
        private static extern int NativePublishEvents(
            nint thread,
            PublisherHandle* publisherHandle,
            ListNative<EventTypeNative>* events);
    }
}
