// <copyright file="DXPublisherHandle.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Collections.Generic;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Native.Events;
using DxFeed.Graal.Net.Native.Interop;
using static DxFeed.Graal.Net.Native.ErrorHandling.ErrorCheck;

namespace DxFeed.Graal.Net.Native.Publisher;

internal sealed unsafe class DXPublisherHandle : JavaHandle
{
    public void PublishEvents(IEnumerable<IEventType> events)
    {
        var eventList = EventMapper.ToNative(events);
        try
        {
            SafeCall(Import.PublishEvents(CurrentThread, this, eventList));
        }
        finally
        {
            EventMapper.Release(eventList);
        }
    }

    private static class Import
    {
        [DllImport(
            ImportInfo.DllName,
            EntryPoint = "dxfg_DXPublisher_publishEvents")]
        public static extern int PublishEvents(
            nint thread,
            DXPublisherHandle publisherHandle,
            ListNative<EventTypeNative>* events);
    }
}
